using BinaryAssetBuilder.Core.SageXml;
using System.Collections.Generic;

namespace BinaryAssetBuilder.Core
{
    public interface ISessionCache
    {
        string CacheFileName { get; }
        List<string> DirtyStreams { get; }
        uint AssetCompilersVersion { get; set; }

        void LoadCache(string sessionCachePath);

        void InitializeCache(List<string> knownChangedFiles);

        // Reborn: retain positive reports from incomplete watcher batches without asserting that omitted paths were unchanged.
        void InitializeCache(List<string> knownChangedFiles, bool notificationsComplete);

        void SaveCache(bool compressed);

        bool TryGetFile(string path, string configuration, TargetPlatform platform, out FileHashItem hashItem);

        bool TryGetDocument(string path, string configuration, TargetPlatform platform, bool autoCreateDocument, out AssetDeclarationDocument document);

        void SaveDocumentToCache(string path, string configuration, TargetPlatform platform, AssetDeclarationDocument document);
    }
}
