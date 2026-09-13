using System;
using System.IO;

namespace BinaryAssetBuilder.Utility
{
    public class ManifestHeader : AStructWrapper<AssetStream.ManifestHeader>
    {
#if VERSION5
        public static ushort LatestVersion => 5;
#elif VERSION6
        public static ushort LatestVersion => 6;
#elif VERSION7
        public static ushort LatestVersion => 7;
#endif

        public int SerializedSize { get; private set; } = 48;

        public unsafe bool IsBigEndian => Data->IsBigEndian;
        public unsafe bool IsLinked { get => Data->IsLinked; set => Data->IsLinked = value; }
        public unsafe ushort Version { get => Data->Version; set => Data->Version = value; }
        public unsafe uint StreamChecksum { get => Data->StreamChecksum; set => Data->StreamChecksum = value; }
        public unsafe uint AllTypesHash { get => Data->AllTypesHash; set => Data->AllTypesHash = value; }
        public unsafe uint AssetCount { get => Data->AssetCount; set => Data->AssetCount = value; }
        public unsafe uint TotalInstanceDataSize { get => Data->TotalInstanceDataSize; set => Data->TotalInstanceDataSize = value; }
        public unsafe uint MaxInstanceChunkSize { get => Data->MaxInstanceChunkSize; set => Data->MaxInstanceChunkSize = value; }
        public unsafe uint MaxRelocationChunkSize { get => Data->MaxRelocationChunkSize; set => Data->MaxRelocationChunkSize = value; }
        public unsafe uint MaxImportsChunkSize { get => Data->MaxImportsChunkSize; set => Data->MaxImportsChunkSize = value; }
        public unsafe uint AssetReferenceBufferSize { get => Data->AssetReferenceBufferSize; set => Data->AssetReferenceBufferSize = value; }
        public unsafe uint ReferenceManifestNameBufferSize { get => Data->ReferenceManifestNameBufferSize; set => Data->ReferenceManifestNameBufferSize = value; }
        public unsafe uint AssetNameBufferSize { get => Data->AssetNameBufferSize; set => Data->AssetNameBufferSize = value; }
        public unsafe uint SourceFileNameBufferSize { get => Data->SourceFileNameBufferSize; set => Data->SourceFileNameBufferSize = value; }

        public unsafe ManifestHeader() : base()
        {
            try
            {
                Data->Version = LatestVersion;
            }
            catch
            {
                Dispose();
            }
        }

        protected override unsafe void Swap()
        {
            Data->Version = Endian.BigEndian(Data->Version);
            Data->StreamChecksum = Endian.BigEndian(Data->StreamChecksum);
            Data->AllTypesHash = Endian.BigEndian(Data->AllTypesHash);
            Data->AssetCount = Endian.BigEndian(Data->AssetCount);
            Data->TotalInstanceDataSize = Endian.BigEndian(Data->TotalInstanceDataSize);
            Data->MaxInstanceChunkSize = Endian.BigEndian(Data->MaxInstanceChunkSize);
            Data->MaxRelocationChunkSize = Endian.BigEndian(Data->MaxRelocationChunkSize);
            Data->MaxImportsChunkSize = Endian.BigEndian(Data->MaxImportsChunkSize);
            Data->AssetReferenceBufferSize = Endian.BigEndian(Data->AssetReferenceBufferSize);
            Data->ReferenceManifestNameBufferSize = Endian.BigEndian(Data->ReferenceManifestNameBufferSize);
            Data->AssetNameBufferSize = Endian.BigEndian(Data->AssetNameBufferSize);
            Data->SourceFileNameBufferSize = Endian.BigEndian(Data->SourceFileNameBufferSize);
        }

        public override unsafe void SaveToStream(Stream output, bool isBigEndian)
        {
            Data->IsBigEndian = isBigEndian;
            if (Data->Version != 7)
            {
                SerializedSize = 48;
                base.SaveToStream(output, isBigEndian);
                return;
            }

            // EP1 stores a four-byte container prefix before the manifest header
            // and reorders its first four logical bytes to Version, Endian, Linked.
            using MemoryStream legacyHeader = new MemoryStream(48);
            base.SaveToStream(legacyHeader, isBigEndian);
            byte[] legacy = legacyHeader.ToArray();
            output.Write(new byte[4], 0, 4);
            output.WriteByte(legacy[2]);
            output.WriteByte(legacy[3]);
            output.WriteByte(legacy[0]);
            output.WriteByte(legacy[1]);
            output.Write(legacy, 4, legacy.Length - 4);
            SerializedSize = 52;
        }

        public override void LoadFromStream(Stream input, bool isBigEndian)
        {
            byte[] first = new byte[4];
            input.ReadExactly(first);

            if (first[0] == 0 && first[1] == 0 && first[2] == 0 && first[3] == 0)
            {
                byte[] ep1 = new byte[48];
                input.ReadExactly(ep1);
                LoadVersion7Header(ep1, isBigEndian);
                SerializedSize = 52;
                return;
            }

            byte[] header = new byte[48];
            first.CopyTo(header, 0);
            input.ReadExactly(header.AsSpan(4));
            ushort leadingVersion = isBigEndian
                ? (ushort)((header[0] << 8) | header[1])
                : (ushort)(header[0] | (header[1] << 8));
            if (leadingVersion == 7 && header[2] <= 1 && header[3] <= 1)
            {
                LoadVersion7Header(header, isBigEndian);
            }
            else
            {
                base.LoadFromBuffer(header, isBigEndian);
            }
            SerializedSize = 48;
        }

        private void LoadVersion7Header(byte[] ep1, bool isBigEndian)
        {
            ushort version = isBigEndian
                ? (ushort)((ep1[0] << 8) | ep1[1])
                : (ushort)(ep1[0] | (ep1[1] << 8));
            if (version != 7 || ep1[2] > 1 || ep1[3] > 1)
            {
                throw new InvalidDataException("Invalid Red Alert 3 Uprising manifest header.");
            }

            byte[] normalized = new byte[48];
            normalized[0] = ep1[2];
            normalized[1] = ep1[3];
            normalized[2] = ep1[0];
            normalized[3] = ep1[1];
            ep1.AsSpan(4).CopyTo(normalized.AsSpan(4));
            base.LoadFromBuffer(normalized, isBigEndian);
        }
    }
}
