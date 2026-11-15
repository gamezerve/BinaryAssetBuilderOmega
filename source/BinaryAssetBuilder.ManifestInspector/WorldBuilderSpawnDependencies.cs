using System.Buffers.Binary;
using System.Runtime.InteropServices;
using BinaryAssetBuilder.Core.Hashing;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: recover game-object spawn IDs absent from the retail strong-reference table.
internal sealed class WorldBuilderSpawnDependencies : IDisposable
{
    private const uint GameObjectType = 0x942FFF2D;
    private const uint DestructionType = 0x0D96A2F4;
    private readonly Dictionary<AssetId, (ManifestAsset Asset, long Offset)> records = new();
    private readonly Dictionary<AssetId, AssetId[]> cached = new();
    private readonly string binaryPath;
    private readonly uint checksum;
    private FileStream? binary;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: index native offsets once and open the WorldBuilder BIN only when a spawnable record is reached. */
    //-------------------------------------------------------------------------------------------------
    internal WorldBuilderSpawnDependencies(string manifest, ManifestDocument catalog)
    {
        binaryPath = Path.ChangeExtension(manifest, ".bin");
        checksum = catalog.Header.StreamChecksum;
        long offset = 8;
        foreach (var asset in catalog.Assets)
        {
            if (asset.TypeId is GameObjectType or ObjectCreationListCompiler.TypeId)
                records.Add(WorldBuilderAssetResolver.Id(asset), (asset, offset));
            offset += asset.InstanceDataSize;
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: return typed spawn targets from reviewed EP1 layouts and reuse them across map audits. */
    //-------------------------------------------------------------------------------------------------
    internal AssetId[] Read(AssetId id)
    {
        if (!records.TryGetValue(id, out var record))
            return Array.Empty<AssetId>();
        if (cached.TryGetValue(id, out var found))
            return found;
        try
        {
            if (binary == null)
            {
                binary = File.OpenRead(binaryPath);
                var header = new byte[8];
                binary.ReadExactly(header);
                if (Word(header, 0) != 0xBABB0000 || Word(header, 4) != checksum)
                    throw new InvalidDataException("WorldBuilder BIN does not match its manifest.");
            }
            var asset = record.Asset;
            if (asset.InstanceDataSize <= 0 || record.Offset > binary.Length - asset.InstanceDataSize)
                throw new InvalidDataException("WorldBuilder asset exceeds its BIN.");
            var bytes = new byte[asset.InstanceDataSize];
            binary.Position = record.Offset;
            binary.ReadExactly(bytes);
            var targets = new HashSet<AssetId>();
            if (id.TypeId == ObjectCreationListCompiler.TypeId && asset.Tokenized == 0 && asset.TypeHash == ObjectCreationListCompiler.TypeHash)
                ReadCreationList(bytes, targets);
            else if (id.TypeId == GameObjectType && asset.Tokenized == 1 && asset.TypeHash == 0xCEB9FE36)
                ReadDestruction(bytes, targets);
            return cached[id] = targets.ToArray();
        }
        catch (Exception error) when (error is InvalidDataException or OverflowException)
        {
            throw new InvalidDataException("Invalid WorldBuilder spawn data: " + record.Asset.Name, error);
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: collect EP1 OCL creation and fling targets using the compiler's reviewed native layout. */
    //-------------------------------------------------------------------------------------------------
    private static void ReadCreationList(byte[] bytes, HashSet<AssetId> targets)
    {
        CheckRange(bytes, 0, Marshal.SizeOf<Ep1ObjectCreationList>());
        int createList = Offset<Ep1ObjectCreationList>(nameof(Ep1ObjectCreationList.CreateObject));
        int createTargets = Offset<Ep1CreateObjectNugget>(nameof(Ep1CreateObjectNugget.CreateObject));
        foreach (int nugget in Rows(bytes, createList, Marshal.SizeOf<Ep1CreateObjectNugget>()))
            foreach (int target in Rows(bytes, nugget + createTargets, 4))
                Add(targets, Word(bytes, target));

        int flingList = Offset<Ep1ObjectCreationList>(nameof(Ep1ObjectCreationList.FlingObject));
        int flingTarget = Offset<Ep1FlingObjectNugget>(nameof(Ep1FlingObjectNugget.CreateObject));
        foreach (int nugget in Rows(bytes, flingList, Marshal.SizeOf<Ep1FlingObjectNugget>()))
            Add(targets, Word(bytes, nugget + flingTarget));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: follow the stock behavior container into BuildingDestruction items and read their weak debris IDs. */
    //-------------------------------------------------------------------------------------------------
    private static void ReadDestruction(byte[] bytes, HashSet<AssetId> targets)
    {
        var root = GameObjectStockReview.Tokens(bytes, 0);
        // Reborn: empty stock behavior token envelopes vary; use the reviewed native GameObject root field.
        if (!root.TryGetValue(FastHash.GetHashCode("KindOf"), out var kind) || kind.Type != 0x9AFC9193)
            throw new InvalidDataException("WorldBuilder GameObject native root anchor is missing.");
        int nativeRoot = kind.Offset - 4;
        int header = checked((int)Word(bytes, nativeRoot + 476));
        if (header == 0)
            return;
        foreach (int row in Rows(bytes, header, 4))
        {
            int module = checked((int)Word(bytes, row));
            if (module == 0 || Word(bytes, module) != DestructionType)
                continue;
            // Reborn: BuildingDestructionContract verifies the item list at +64 and the weak ID at item +12.
            foreach (int item in Rows(bytes, module + 64, 20))
                Add(targets, Word(bytes, item + 12));
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: bound native count/pointer pairs before yielding any list entries. */
    //-------------------------------------------------------------------------------------------------
    private static IEnumerable<int> Rows(byte[] bytes, int header, int stride)
    {
        int count = checked((int)Word(bytes, header));
        if (count == 0)
            yield break;
        int start = checked((int)Word(bytes, header + 4));
        if (start <= 0)
            throw new InvalidDataException("Nonempty WorldBuilder list has a null pointer.");
        CheckRange(bytes, start, checked(count * stride));
        for (int i = 0; i < count; i++)
            yield return start + i * stride;
    }

    // Reborn: zero is an absent weak reference.
    private static void Add(HashSet<AssetId> targets, uint instance)
    {
        if (instance != 0)
            targets.Add(new AssetId(GameObjectType, instance));
    }

    // Reborn: use the same 32-bit field layout as the retail qualification code.
    private static int Offset<T>(string field) where T : struct => Marshal.OffsetOf<T>(field).ToInt32();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject out-of-record reads while decoding the retail ABI. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckRange(byte[] bytes, int at, int length)
    {
        if (at < 0 || length < 0 || at > bytes.Length - length)
            throw new InvalidDataException("WorldBuilder spawn data pointer is out of range.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: read one bounded little-endian native word. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] bytes, int at)
    {
        CheckRange(bytes, at, 4);
        return BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(at, 4));
    }

    // Reborn: release the catalog reader at the end of resolution or a campaign audit.
    public void Dispose() => binary?.Dispose();
}
