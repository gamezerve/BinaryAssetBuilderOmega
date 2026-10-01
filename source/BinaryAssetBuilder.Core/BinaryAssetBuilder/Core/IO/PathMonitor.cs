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
        // Reborn: restored uncertain batches must remain uncertain until successfully handed off.
        private bool _pendingUntrusted;

        // Reborn: an opaque batch owns its detached paths and captures trust at the same instant as the queue drain.
        public sealed class ChangeBatch
        {
            internal readonly PathMonitor Owner;
            internal int State;
            public IReadOnlyList<string> Paths { get; }
            public int EventCount { get; }
            public bool IsTrustable { get; }

            //-------------------------------------------------------------------------------------------------
            /** Reborn: immutable batch contents cannot be altered by cache initialization or later watcher callbacks. */
            //-------------------------------------------------------------------------------------------------
            internal ChangeBatch(PathMonitor owner, List<string> paths, int eventCount, bool trustable)
            {
                Owner = owner;
                Paths = paths.AsReadOnly();
                EventCount = eventCount;
                IsTrustable = trustable;
            }
        }

        public bool IsResultTrustable
        {
            // Reborn: read trust state consistently with callback counters and reset operations.
            get { lock (_eventLock) return !_unrecoverableErrorOccured && !_pendingUntrusted && _numEvents < EventLimit; }
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
                // Reborn: saturating counters cannot wrap an overflowing event queue back into a trusted state.
                if (_numEvents < int.MaxValue) ++_numEvents;
                _changedFiles.Add(args.FullPath);
            }
        }

        private void OnRenamed(object source, RenamedEventArgs args)
        {
            // Reborn: renames invalidate both the disappearing source and the new destination identity.
            lock (_eventLock)
            {
                // Reborn: saturating counters retain overflow distrust even in a long-running resident monitor.
                if (_numEvents < int.MaxValue) ++_numEvents;
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
                // Reborn: explicit legacy reset discards the pending batch but cannot repair a permanent watcher error.
                _pendingUntrusted = false;
            }
        }

        public List<string> GetChangedFiles()
        {
            // Reborn: retain the legacy non-consuming snapshot API for compatibility; build handoff uses DrainChanges instead.
            WaitForNotifications();
            lock (_eventLock) return new List<string>(_changedFiles);
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: collect current notifications and trust atomically, leaving subsequent events queued for the next build. */
        //-------------------------------------------------------------------------------------------------
        public ChangeBatch DrainChanges(bool waitForNotifications = true)
        {
            bool flushSucceeded = !waitForNotifications || WaitForNotifications();
            lock (_eventLock)
            {
                ChangeBatch batch = new ChangeBatch(this, new List<string>(_changedFiles), _numEvents,
                    flushSucceeded && !_unrecoverableErrorOccured && !_pendingUntrusted && _numEvents < EventLimit);
                _changedFiles.Clear();
                _numEvents = 0;
                _pendingUntrusted = false;
                return batch;
            }
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: acknowledge one consumed batch without clearing events that arrived during cache initialization. */
        //-------------------------------------------------------------------------------------------------
        public void CompleteBatch(ChangeBatch batch)
        {
            lock (_eventLock)
            {
                ValidateActiveBatch(batch);
                batch.State = 1;
            }
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: merge a failed handoff back into pending events without losing later callbacks or trust provenance. */
        //-------------------------------------------------------------------------------------------------
        public void RestoreBatch(ChangeBatch batch)
        {
            lock (_eventLock)
            {
                ValidateActiveBatch(batch);
                foreach (string path in batch.Paths) _changedFiles.Add(path);
                _numEvents = (int)Math.Min(int.MaxValue, (long)_numEvents + batch.EventCount);
                _pendingUntrusted |= !batch.IsTrustable;
                batch.State = 2;
            }
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: reject foreign or already acknowledged/restored batches before changing the pending event queue. */
        //-------------------------------------------------------------------------------------------------
        private void ValidateActiveBatch(ChangeBatch batch)
        {
            if (batch == null || !ReferenceEquals(batch.Owner, this)) throw new ArgumentException("Change batch belongs to another monitor.", nameof(batch));
            if (batch.State != 0) throw new InvalidOperationException("Change batch was already completed or restored.");
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: bind the cache handoff to one batch, restoring consumed notifications if initialization fails. */
        //-------------------------------------------------------------------------------------------------
        public void InitializeCache(ISessionCache cache, bool useNotifications)
        {
            if (cache == null) throw new ArgumentNullException(nameof(cache));
            ChangeBatch batch = DrainChanges(useNotifications);
            try
            {
                // Reborn: incomplete batches still force their positive reports; only completeness-dependent omission checks are disabled.
                cache.InitializeCache(useNotifications ? new List<string>(batch.Paths) : new List<string>(),
                    useNotifications && batch.IsTrustable);
                CompleteBatch(batch);
            }
            catch
            {
                RestoreBatch(batch);
                throw;
            }
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: wait before the atomic snapshot and return flush uncertainty together with collected notifications. */
        //-------------------------------------------------------------------------------------------------
        private bool WaitForNotifications()
        {
            bool flushSucceeded = true;
            try
            {
                Flush();
            }
            catch (Exception ex)
            {
                // Reborn: accurately report retained positive notifications rather than claiming the watcher was disabled.
                _tracer.TraceWarning("Unable to flush a monitored disk volume; collected change notifications will be retained.\n" + ex.Message);
                // Reborn: inability to flush a volume must not discard positive change notifications already collected.
                flushSucceeded = false;
            }
            Thread.Sleep(100);
            // Reborn: native flush failures make the drained batch untrusted even when event count is below the limit.
            return flushSucceeded;
        }
    }
}
