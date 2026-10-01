using BinaryAssetBuilder.Core.Diagnostics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace BinaryAssetBuilder.Core.IO
{
    public class PathMonitor
    {
        public const int EventLimit = 20;

        private static readonly Tracer _tracer = Tracer.GetTracer(nameof(PathMonitor), "Monitors paths for file changes");

        private readonly List<FileSystemWatcher> _watchers;
        // Reborn: filesystem callbacks and build-thread snapshots share a case-insensitive, synchronized event set.
        private readonly HashSet<string> _changedFiles;
        private readonly object _eventLock = new object();
        private int _numEvents;
        private bool _unrecoverableErrorOccured;

        public bool IsResultTrustable
        {
            // Reborn: read trust state consistently with callback counters and reset operations.
            get { lock (_eventLock) return !_unrecoverableErrorOccured && _numEvents < EventLimit; }
        }

        public PathMonitor(string[] pathsToMonitor)
        {
            _changedFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            _watchers = new List<FileSystemWatcher>(pathsToMonitor.Length);
            _numEvents = 0;
            _unrecoverableErrorOccured = false;
            for (int idx = 0; idx < pathsToMonitor.Length; ++idx)
            {
                FileSystemWatcher watcher = new FileSystemWatcher(pathsToMonitor[idx]);
                watcher.Changed += OnChanged;
                // Reborn: source creation and nested SDK asset edits must enter the same invalidation stream.
                watcher.Created += OnChanged;
                watcher.Deleted += OnChanged;
                watcher.Renamed += OnRenamed;
                watcher.Error += OnError;
                watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size | NotifyFilters.LastWrite;
                watcher.IncludeSubdirectories = true;
                watcher.EnableRaisingEvents = true;
                _watchers.Add(watcher);
            }
        }

        private void OnChanged(object source, FileSystemEventArgs args)
        {
            // Reborn: serialize callback updates so resident snapshots cannot race mutable event collections.
            lock (_eventLock)
            {
                ++_numEvents;
                _changedFiles.Add(args.FullPath);
            }
        }

        private void OnRenamed(object source, RenamedEventArgs args)
        {
            // Reborn: renames invalidate both the disappearing source and the new destination identity.
            lock (_eventLock)
            {
                ++_numEvents;
                _changedFiles.Add(args.OldFullPath);
                _changedFiles.Add(args.FullPath);
            }
        }

        private void OnError(object source, ErrorEventArgs args)
        {
            // Reborn: an error remains untrustworthy across resets until the monitor is replaced.
            lock (_eventLock) _unrecoverableErrorOccured = true;
        }

        private void Flush()
        {
            foreach (FileSystemWatcher watcher in _watchers)
            {
                FileSystemUtils.FlushVolume(watcher.Path[0]);
            }
        }

        public void Reset()
        {
            // Reborn: reset event sets atomically without clearing a watcher failure that has not been repaired.
            lock (_eventLock)
            {
                _numEvents = 0;
                _changedFiles.Clear();
            }
        }

        public List<string> GetChangedFiles()
        {
            try
            {
                Flush();
            }
            catch (Exception ex)
            {
                // Reborn: accurately report retained positive notifications rather than claiming the watcher was disabled.
                _tracer.TraceWarning("Unable to flush a monitored disk volume; collected change notifications will be retained.\n" + ex.Message);
                // Reborn: inability to flush a volume must not discard positive change notifications already collected.
            }
            Thread.Sleep(100);
            // Reborn: expose a stable snapshot instead of enumerating concurrently changing callback data.
            lock (_eventLock) return new List<string>(_changedFiles);
        }
    }
}
