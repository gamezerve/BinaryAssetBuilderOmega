using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace BinaryAssetBuilder.Utility
{
    public class ReferencedFileBuffer
    {
        private readonly List<byte> _data;
        // Reborn: distinct names and normal/patch roles must never alias through a string-hash collision.
        private readonly Dictionary<(string Name, bool IsPatch), int> _positions;

        public int Length => _data.Count;

        public ReferencedFileBuffer()
        {
            _data = new List<byte>();
            // Reborn: use full ordinal string identity and the role bit for deduplication.
            _positions = new Dictionary<(string Name, bool IsPatch), int>();
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: preserve normal and patch entries separately and reject paths that truncate native strings. */
        //-------------------------------------------------------------------------------------------------
        public unsafe int AddReference(string name, bool isPatch)
        {
            if (string.IsNullOrWhiteSpace(name) || name.IndexOf('\0') >= 0)
            {
                throw new ArgumentException("Manifest reference name must be nonempty and contain no NUL.", nameof(name));
            }
            var key = (name, isPatch);
            if (!_positions.TryGetValue(key, out int result))
            {
                IntPtr hName = Marshal.StringToHGlobalAnsi(name);
                byte* pName = (byte*)hName.ToPointer();
                result = _data.Count;
                if (isPatch)
                {
                    _data.Add(2);
                }
                else
                {
                    _data.Add(1);
                }
                while (*pName != 0)
                {
                    _data.Add(*pName);
                    ++pName;
                }
                _data.Add(0);
                Marshal.FreeHGlobal(hName);
                _positions.Add(key, result);
            }
            return result;
        }

        public void SaveToStream(Stream output)
        {
            output.Write(_data.ToArray(), 0, _data.Count);
        }
    }
}
