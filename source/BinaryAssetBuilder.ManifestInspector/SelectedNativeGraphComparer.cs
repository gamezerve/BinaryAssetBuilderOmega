using System.Buffers.Binary;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using Chunk = Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: independently walk typed native records against stock bytes; shared stock strings may have different addresses, but no field or referenced identity is ignored.
internal sealed class SelectedNativeGraphComparer
{
    private readonly byte[] stock;
    private readonly AssetId[] stockReferences;
    private readonly Chunk compiled;
    private readonly InstanceDeclaration instance;
    private readonly HashSet<uint> relos = new(), imports = new();
    private readonly HashSet<(Type, int, int)> active = new();
    private readonly bool[] covered;
    private readonly bool allowScriptCrLfEquivalent;
    // Reborn: shared-container callers verify full occurrence consumption across all leaves, not separately for each sibling.
    private readonly bool requireAllReferences;
    // Reborn: native-only surveys may compare independently allocated copies of shared stock records without claiming one-to-one metadata coordinates; all existing token gates keep the strict default.
    private readonly bool captureNativeCoordinates;
    // Reborn: field-context records preserve shared-stock aliases without weakening the existing one-to-one metadata-coordinate default.
    private readonly bool captureNativePaths;
    // Reborn: only explicitly source-validated UpgradeModuleData fields may change list counts or optional pointer presence.
    private readonly ConscriptUpgradeStructure.Plan? upgrade;
    // Reborn: only source-bound root localization fields may differ from stock string contents or presence.
    private readonly IReadOnlyDictionary<string,string?>? rootText;
    // Reborn: source-checked nested primitive values and optional root imports are semantic expectations, not skipped bytes or frozen stock presence.
    private readonly IReadOnlyDictionary<int,uint>? authoredValues;
    // Reborn: plain-record attributes include packed booleans and flags, so preserve exact authored byte widths rather than assuming four-byte words.
    internal IReadOnlyDictionary<int,byte[]>? AuthoredRecordValues { get; init; }
    private readonly IReadOnlyDictionary<string,AssetId?>? rootImports;
    internal Dictionary<string,(int Stock,int Compiled,int Size)> NativeRecords { get; } = new(StringComparer.Ordinal);
    private int scriptLineEndingEquivalences;
    private int records, strings;
    // Reborn: only the explicitly detached root-selection entry point may walk zeroed polymorphic headers; ordinary complete graph comparisons retain their old generic admission.
    private bool rootSelection;
    internal sealed record RootSelection(bool[] Covered,uint[] Relocations,uint[] Imports);
    // Reborn: metadata targets address native records/headers, not interned character arrays; expose only the independently compared native byte-coordinate map.
    internal Dictionary<int, int> NativeOffsets { get; } = new();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stock and compiled graphs keep separate origins and semantic reference occurrence tables. */
    //-------------------------------------------------------------------------------------------------
    internal SelectedNativeGraphComparer(byte[] stock, AssetId[] references, Chunk compiled, InstanceDeclaration instance, bool allowScriptCrLfEquivalent = false, bool requireAllReferences = true, bool captureNativeCoordinates = true, bool captureNativePaths = false,ConscriptUpgradeStructure.Plan? upgrade=null,IReadOnlyDictionary<string,string?>? rootText=null,IReadOnlyDictionary<int,uint>? authoredValues=null,IReadOnlyDictionary<string,AssetId?>? rootImports=null)
    { this.stock = stock; stockReferences = references; this.compiled = compiled; this.instance = instance; this.allowScriptCrLfEquivalent = allowScriptCrLfEquivalent; this.requireAllReferences = requireAllReferences; this.captureNativeCoordinates = captureNativeCoordinates; this.captureNativePaths=captureNativePaths;this.upgrade=upgrade;this.rootText=rootText;this.authoredValues=authoredValues;this.rootImports=rootImports; covered = new bool[compiled.InstanceBuffer.Length]; }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: complete selected graph equality requires every native field, exact auxiliary ownership and no hidden nonzero/orphan heap allocations. */
    //-------------------------------------------------------------------------------------------------
    internal (int Records, int Strings, int Imports, int ScriptLineEndingEquivalences) Compare(Type root, int original)
    {
        if (compiled.InstanceBuffer.Length > 2 * 1048576) throw new InvalidDataException("Selected native graph exceeds diagnostic bound.");
        Visit(root, original, 0, root.Name, 0);
        if (!Slots(compiled.RelocationBuffer).Order().SequenceEqual(relos.Order()) || !Slots(compiled.ImportsBuffer).Order().SequenceEqual(imports.Order()) || requireAllReferences && imports.Count != instance.ReferencedInstances.Count) throw new InvalidDataException("Selected graph auxiliary ownership or Core reference consumption differs.");
        int gap = 0;
        for (int i = 0; i < covered.Length; i++) { if (covered[i]) { gap = 0; continue; } if (compiled.InstanceBuffer[i] != 0 || ++gap > 3) throw new InvalidDataException("Selected graph has unowned native storage at " + i); }
        return (records, strings, imports.Count, scriptLineEndingEquivalences);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare the complete typed root with caller-detached module/EP1-owner pointers; return exact visited ownership for an enclosing whole-graph union, never claim untouched heap bytes are verified. */
    //-------------------------------------------------------------------------------------------------
    internal RootSelection CompareRootSelection(int original)
    {
        rootSelection=true;
        try
        {
            var declaredRelos=Slots(compiled.RelocationBuffer);var declaredImports=Slots(compiled.ImportsBuffer);if(declaredRelos.Concat(declaredImports).Any(s=>s>compiled.InstanceBuffer.Length-4))throw new InvalidDataException("Root selection auxiliary source bounds.");
            Visit(typeof(SageBinaryData.GameObject),original,0,"GameObject",0);
            if(!declaredRelos.Where(s=>covered[(int)s]).Order().SequenceEqual(relos.Order()) || !declaredImports.Where(s=>covered[(int)s]).Order().SequenceEqual(imports.Order()))throw new InvalidDataException("Root selection auxiliary ownership differs.");
            return new(covered.ToArray(),relos.ToArray(),imports.ToArray());
        }
        finally{rootSelection=false;}
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: pointers, lists, strings, imports and fixed masks have explicit handling; all remaining struct bytes, including padding, are compared recursively. */
    //-------------------------------------------------------------------------------------------------
    private void Visit(Type type, int a, int b, string path, int depth)
    {
        if (depth > 64 || ++records > 100000 || !active.Add((type, a, b))) throw new InvalidDataException("Selected native graph cycle/depth/count exceeded at " + path);
        try
        {
            int size = Size(type); Range(stock, a, size); Range(compiled.InstanceBuffer, b, size); Array.Fill(covered, true, b, size);
            if(captureNativePaths && !NativeRecords.TryAdd(path,(a,b,size)))throw new InvalidDataException("Duplicate contextual native field path: "+path);
            // Reborn: one stock native header cannot ambiguously map to different emitted headers; shared character payloads are not metadata targets and are deliberately excluded.
            if(captureNativeCoordinates)for (int i = 0; i < size; i++) { if (NativeOffsets.TryGetValue(a + i, out int prior) && prior != b + i) throw new InvalidDataException("Ambiguous selected native metadata coordinate at " + path); NativeOffsets[a + i] = b + i; }
            if (type.IsEnum || type.IsPrimitive) { Equal(a, b, size, path); return; }
            if (type.IsGenericType)
            {
                string generic = type.GetGenericTypeDefinition().FullName!; Type element = type.GetGenericArguments()[0];
                // Reborn: the root-only partial walk cannot infer concrete classes; enclosing callers must separately prove and restore every detached polymorphic owner.
                if(rootSelection && generic=="Relo.PolymorphicList`1"){if(Word(stock,a)!=0 || Word(stock,a+4)!=0 || Word(compiled.InstanceBuffer,b)!=0 || Word(compiled.InstanceBuffer,b+4)!=0)throw new InvalidDataException("Root selection requires detached empty polymorphic headers.");Equal(a,b,8,path);return;}
                // Reborn: typed weak IDs remain exact native hashes and never become strong imports or pointer relocations.
                if (generic == "TypedAssetId`1") { Equal(a, b, 4, path); return; }
                if (generic == "Relo.AssetReference`1" || generic == "Relo.AssetReference`2")
                {
                    uint oldIndex = Word(stock, a), newIndex = Word(compiled.InstanceBuffer, b);
                    // Reborn: optional root fields can be absent or newly populated; exact authored identity and IMP ownership still have to match.
                    if(rootImports!=null&&rootImports.TryGetValue(path,out var target)){
                        if(target==null){if(newIndex!=0)throw new InvalidDataException("Absent authored root reference has an index at "+path);}
                        else{if(newIndex==0||newIndex>instance.ReferencedInstances.Count)throw new InvalidDataException("Authored root reference index bounds at "+path);var handle=instance.ReferencedInstances[(int)newIndex-1];if(handle.TypeId!=target.TypeId||handle.InstanceId!=target.InstanceId)throw new InvalidDataException("Authored root reference identity differs at "+path);imports.Add((uint)b);}return;
                    }
                    if ((oldIndex == 0) != (newIndex == 0)) throw new InvalidDataException("Reference presence differs at " + path);
                    if (oldIndex != 0)
                    {
                        if (oldIndex > stockReferences.Length || newIndex > instance.ReferencedInstances.Count) throw new InvalidDataException("Reference index exceeds occurrence table at " + path);
                        var oldId = stockReferences[(int)oldIndex - 1]; var newId = instance.ReferencedInstances[(int)newIndex - 1];
                        if (oldId.TypeId != newId.TypeId || oldId.InstanceId != newId.InstanceId) throw new InvalidDataException("Reference identity differs at " + path + ": " + newId.Name);
                        imports.Add((uint)b);
                    }
                    return;
                }
                if (generic == "Relo.String`1")
                {
                    if (element != typeof(sbyte)) throw new InvalidDataException("Unreviewed selected string encoding.");
                    // Reborn: source text replaces equality to the captured stock key, not native bounds or exact content checking.
                    if(rootText!=null&&rootText.TryGetValue(path,out var authored)){Text(b,authored,path);return;}
                    uint oldLength = Word(stock, a), newLength = Word(compiled.InstanceBuffer, b); if (oldLength > 65536 || newLength > 65536) throw new InvalidDataException("Selected string exceeds bound.");
                    int length = (int)oldLength, compiledLength = (int)newLength;
                    var pointers = Pointer(a + 4, b + 4, path); if (pointers.A == 0) { if (length != 0 || compiledLength != 0) throw new InvalidDataException("Null selected string has length."); return; }
                    Range(stock, pointers.A, length + 1); Range(compiled.InstanceBuffer, pointers.B, compiledLength + 1);
                    if (stock[pointers.A + length] != 0 || compiled.InstanceBuffer[pointers.B + compiledLength] != 0) throw new InvalidDataException("Selected string terminator differs at " + path);
                    bool exact = length == compiledLength && stock.AsSpan(pointers.A, length).SequenceEqual(compiled.InstanceBuffer.AsSpan(pointers.B, compiledLength));
                    if (!exact && allowScriptCrLfEquivalent && path.EndsWith(".Script", StringComparison.Ordinal))
                    {
                        // Reborn: only explicitly requested Script CRLF/LF equivalence is allowed; producer/source bytes stay untouched and arbitrary whitespace/content changes still fail.
                        string oldText = System.Text.Encoding.Latin1.GetString(stock, pointers.A, length), newText = System.Text.Encoding.Latin1.GetString(compiled.InstanceBuffer, pointers.B, compiledLength);
                        if (oldText.Replace("\r\n", "\n", StringComparison.Ordinal) != newText.Replace("\r\n", "\n", StringComparison.Ordinal)) throw new InvalidDataException("Selected script content differs beyond CRLF/LF at " + path);
                        scriptLineEndingEquivalences++;
                    }
                    else { Equal(a, b, 4, path + ".Length"); Equal(pointers.A, pointers.B, length + 1, path); }
                    Array.Fill(covered, true, pointers.B, compiledLength + 1); strings++; return;
                }
                if (generic == "Relo.List`1")
                {
                    // Reborn: typed upgrade arrays are source-sized; independently check every hash and all allocation/relocation ownership.
                    if(upgrade!=null&&upgrade.Lists.TryGetValue(path,out var values)){
                        if(element!=typeof(global::TypedAssetId<SageBinaryData.UpgradeTemplate>)||Word(compiled.InstanceBuffer,b)!=values.Length)throw new InvalidDataException("Upgrade list source/native count or ABI differs at "+path);
                        uint pointer=Word(compiled.InstanceBuffer,b+4);if((values.Length==0)!=(pointer==0))throw new InvalidDataException("Upgrade list empty/pointer invariant differs at "+path);
                        if(values.Length>0){Range(compiled.InstanceBuffer,checked((int)pointer),checked(values.Length*4));if(pointer%4!=0||covered.AsSpan((int)pointer,values.Length*4).Contains(true))throw new InvalidDataException("Upgrade list allocation overlaps another owner.");relos.Add((uint)(b+4));Array.Fill(covered,true,(int)pointer,values.Length*4);}
                        uint original=Word(stock,a+4);
                        for(int i=0;i<values.Length;i++){int item=checked((int)pointer+i*4);if(Word(compiled.InstanceBuffer,item)!=values[i])throw new InvalidDataException("Upgrade list hash differs from authored source at "+path+"["+i+"]");if(captureNativePaths&&!NativeRecords.TryAdd(path+"["+i+"]",((int)original,item,4)))throw new InvalidDataException("Duplicate authored upgrade list context.");}
                        return;
                    }
                    Equal(a, b, 4, path + ".Count"); uint count = Word(stock, a); if (count > 10000) throw new InvalidDataException("Selected native list exceeds bound.");
                    var pointers = Pointer(a + 4, b + 4, path); if ((count == 0) != (pointers.A == 0)) throw new InvalidDataException("Selected list empty/storage invariant differs at " + path);
                    int stride = Size(element);
                    for (int i = 0; i < count; i++) Visit(element, checked(pointers.A + i * stride), checked(pointers.B + i * stride), path + "[" + i + "]", depth + 1);
                    return;
                }
                throw new InvalidDataException("Unreviewed selected generic native type " + type);
            }
            var used = new bool[size];
            foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                int offset = Marshal.OffsetOf(type, field.Name).ToInt32(); var fixedBuffer = field.GetCustomAttribute<FixedBufferAttribute>();
                int fieldSize = fixedBuffer != null ? Size(fixedBuffer.ElementType) * fixedBuffer.Length : field.FieldType.IsPointer ? 4 : Size(field.FieldType);
                if (offset < 0 || offset + fieldSize > size) throw new InvalidDataException("Selected native field escapes its struct."); Array.Fill(used, true, offset, fieldSize);
                string child = path + "." + field.Name;
                if (fixedBuffer != null) Equal(a + offset, b + offset, fieldSize, child);
                else if (field.FieldType.IsPointer)
                {
                    // Reborn: optional display names have an eight-byte ASCII header, independently owned text and no strong import.
                    if(rootText!=null&&rootText.TryGetValue(child,out var text)){
                        if(field.FieldType.GetElementType()!=typeof(Relo.String<sbyte>))throw new InvalidDataException("Pointed root text ABI differs.");
                        uint pointer=Word(compiled.InstanceBuffer,b+offset);if((text==null)!=(pointer==0))throw new InvalidDataException("Pointed root text presence differs: "+child);
                        if(pointer!=0){int at=checked((int)pointer);Range(compiled.InstanceBuffer,at,8);if(at%4!=0||covered.AsSpan(at,8).Contains(true))throw new InvalidDataException("Pointed root text header overlaps another owner.");Array.Fill(covered,true,at,8);relos.Add((uint)(b+offset));Text(at,text,child);uint old=Word(stock,a+offset);if(old!=0&&captureNativeCoordinates)for(int i=0;i<8;i++)NativeOffsets[checked((int)old+i)]=at+i;}
                        continue;
                    }
                    // Reborn: optional animation structs use independent source words and exact ownership, including the omitted/null case.
                    if(upgrade!=null&&upgrade.Optional.TryGetValue(child,out var expected)){
                        if(field.FieldType.GetElementType()!=typeof(SageBinaryData.AnimAndDuration))throw new InvalidDataException("Optional upgrade record ABI differs.");
                        uint pointer=Word(compiled.InstanceBuffer,b+offset);if((expected==null)!=(pointer==0))throw new InvalidDataException("Optional animation source/native presence differs.");
                        if(expected!=null){Range(compiled.InstanceBuffer,checked((int)pointer),expected.Length);if(pointer%4!=0||covered.AsSpan((int)pointer,expected.Length).Contains(true)||!compiled.InstanceBuffer.AsSpan((int)pointer,expected.Length).SequenceEqual(expected))throw new InvalidDataException("Optional animation source words/allocation differ.");Array.Fill(covered,true,(int)pointer,expected.Length);relos.Add((uint)(b+offset));if(captureNativePaths&&!NativeRecords.TryAdd(child,((int)Word(stock,a+offset),(int)pointer,expected.Length)))throw new InvalidDataException("Duplicate optional animation context.");}
                        continue;
                    }
                    var pointers = Pointer(a + offset, b + offset, child); if (pointers.A != 0) Visit(field.FieldType.GetElementType()!, pointers.A, pointers.B, child, depth + 1);
                    // Reborn: an independently proven null word can itself be a metadata reference target, as with EP1's empty voice EVA handle; nonnull paths still describe only their real pointees.
                    else if(captureNativePaths && !NativeRecords.TryAdd(child,(a+offset,b+offset,4)))throw new InvalidDataException("Duplicate contextual null native field path."); }
                else Visit(field.FieldType, a + offset, b + offset, child, depth + 1);
            }
            for (int i = 0; i < size; i++) if (!used[i]) Equal(a + i, b + i, 1, path + ".padding");
        }
        finally { active.Remove((type, a, b)); }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stock interning is allowed, but native pointer presence and exact relocation source ownership must agree. */
    //-------------------------------------------------------------------------------------------------
    private (int A, int B) Pointer(int a, int b, string path)
    {
        uint x = Word(stock, a), y = Word(compiled.InstanceBuffer, b);
        if ((x == 0) != (y == 0) || x >= stock.Length || y >= compiled.InstanceBuffer.Length) throw new InvalidDataException("Selected native pointer presence/bounds differs at " + path);
        if (y != 0) relos.Add((uint)b); return ((int)x, (int)y);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: source-authored text owns exact ASCII bytes, a terminator and its relocation; absent and explicit-empty allocations remain distinct. */
    //-------------------------------------------------------------------------------------------------
    private void Text(int header,string? expected,string path)
    {
        uint length=Word(compiled.InstanceBuffer,header),pointer=Word(compiled.InstanceBuffer,header+4);
        if(expected==null){if(length!=0||pointer!=0)throw new InvalidDataException("Absent root text has storage: "+path);return;}
        var bytes=System.Text.Encoding.ASCII.GetBytes(expected);int at=checked((int)pointer);Range(compiled.InstanceBuffer,at,checked(bytes.Length+1));
        if(pointer==0||pointer%4!=0||length!=bytes.Length||covered.AsSpan(at,bytes.Length+1).Contains(true)||!compiled.InstanceBuffer.AsSpan(at,bytes.Length).SequenceEqual(bytes)||compiled.InstanceBuffer[at+bytes.Length]!=0)throw new InvalidDataException("Root text differs from authored source: "+path);
        Array.Fill(covered,true,at,bytes.Length+1);relos.Add((uint)(header+4));strings++;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: a short path/offset diagnostic identifies differences without dumping binary payloads or scripts. */
    //-------------------------------------------------------------------------------------------------
    private void Equal(int a, int b, int length, string path)
    {
        Range(stock, a, length); Range(compiled.InstanceBuffer, b, length);
        // Reborn: a source-anchored value must occupy this exact semantic field and match its complete native representation.
        if(AuthoredRecordValues!=null&&AuthoredRecordValues.TryGetValue(b,out var value)&&value.Length==length){if(!compiled.InstanceBuffer.AsSpan(b,length).SequenceEqual(value))throw new InvalidDataException("Authored record value differs at "+path);return;}
        // Reborn: this address map is independently derived from source values and anchored native owners; every other field/padding byte retains exact stock comparison.
        if(length==4&&authoredValues!=null&&authoredValues.TryGetValue(b,out uint expected)){if(Word(compiled.InstanceBuffer,b)!=expected)throw new InvalidDataException("Authored native value differs at "+path);return;}
        for (int i = 0; i < length; i++) if (stock[a + i] != compiled.InstanceBuffer[b + i]) throw new InvalidDataException($"Selected native mismatch {path}+{i}: stock={stock[a + i]:X2}, compiled={compiled.InstanceBuffer[b + i]:X2} (native {a + i}/{b + i}).");
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: generic list/string/ref wire sizes are explicit because Marshal.SizeOf rejects closed generic types. */
    //-------------------------------------------------------------------------------------------------
    private static int Size(Type type) => type.IsPointer ? 4 : type.IsEnum ? Size(Enum.GetUnderlyingType(type)) : type.IsGenericType ? type.GetGenericTypeDefinition().FullName switch { "Relo.List`1" or "Relo.String`1" or "Relo.PolymorphicList`1" => 8, "Relo.AssetReference`1" or "Relo.AssetReference`2" or "TypedAssetId`1" => 4, _ => throw new InvalidDataException("Unknown selected generic size.") } : Marshal.SizeOf(type);
    //-------------------------------------------------------------------------------------------------
    /** Reborn: malformed auxiliary size, sentinel and duplicate entries cannot be normalized away by set comparison. */
    //-------------------------------------------------------------------------------------------------
    private static uint[] Slots(byte[] bytes)
    {
        if (bytes.Length == 0) return Array.Empty<uint>(); if (bytes.Length % 4 != 0 || Word(bytes, bytes.Length - 4) != uint.MaxValue) throw new InvalidDataException("Selected auxiliary sentinel/size differs.");
        var result = Enumerable.Range(0, bytes.Length / 4 - 1).Select(i => Word(bytes, i * 4)).ToArray(); if (result.Distinct().Count() != result.Length) throw new InvalidDataException("Selected auxiliary duplicate slot."); return result;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: every binary read is limited to an admitted asset/owned allocation, never arbitrary memory. */
    //-------------------------------------------------------------------------------------------------
    private static void Range(byte[] bytes, int offset, int length) { if (offset < 0 || length < 0 || offset > bytes.Length - length) throw new InvalidDataException("Selected native range exceeded."); }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: pointer/reference/count words stay little-endian and bounded. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] bytes, int at) { Range(bytes, at, 4); return BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(at, 4)); }
}
