using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core.Hashing;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.XmlCompiler;

// Reborn: add only sixteen independently reviewed empty/null root fields; complete named metadata is not complete original root semantics or populated child support.
public static class Ep1GameObjectEmptyRootProjection
{
    // Reborn: literal root wire signatures are independently observed stock type identities, not per-asset offsets or metadata templates.
    public readonly record struct Field(string Name,int Slot,uint Wire,bool List);
    public static IReadOnlyList<Field> Fields { get; }=Array.AsReadOnly(new[]{
        // Reborn: GameDependency's optional null/populated declaration is now owned by the complete root-record composer.
        new Field("Browser",44,0x6D66AE18,true),
        new Field("BuildFadeInOnCreateList",444,0x6D66AE18,false),new Field("BuildVariations",448,0x6D66AE18,false),new Field("EquivalentTo",452,0x148ED75D,true),
        new Field("ReplaceModule",504,0x57B786DD,false),new Field("InheritableModule",508,0xE1B75970,false),new Field("AutoResolveArmor",512,0x75987CAA,false),new Field("AutoResolveWeapon",516,0x8EA82C97,false),
        // Reborn: AudioArraySound and ProjectedBuildabilityInfo are now complete root owners, including their empty declarations.
        new Field("FormationPreviewDecal",520,0xD753F0C3,false),new Field("FormationPreviewItemDecal",524,0xD753F0C3,false),new Field("UpgradeCameo",544,0x1A2933AA,true),
        new Field("AutoResolveInfo",556,0x8546AC4F,false),new Field("DisplayUpgrade",576,0x26FACFC7,true)});
    //-------------------------------------------------------------------------------------------------
    /** Reborn: explicitly refuse source variants whose populated ABI has not been reviewed; absent direct children are the only admitted source profile (Browser text is a separately proved list). */
    //-------------------------------------------------------------------------------------------------
    public static void ValidateSource(XmlElement root)
    {
        if(root==null || root.LocalName!="GameObject" || root.NamespaceURI!="uri:ea.com:eala:asset")throw new InvalidDataException("Empty root admission requires a GameObject source.");
        // Reborn: populated Browser is admitted; its tokens are validated by the source contract and the native list is proved separately.
        foreach(var field in Fields)if(root.ChildNodes.OfType<XmlElement>().Any(e=>e.LocalName==field.Name))throw new InvalidDataException("Present empty-profile child is not admitted: "+field.Name);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve the full selected native graph and prior 110-field route; emit five count-marker tables and eleven null targets without inventing child allocations. */
    //-------------------------------------------------------------------------------------------------
    public static Chunk Compose(Chunk native,XmlSchemaSet schemas)
        =>ComposeCore(native,schemas,false);
    //-------------------------------------------------------------------------------------------------
    /** Reborn: original objects retain absent name declarations; populated name headers are already emitted by the reviewed pointed-root producer. */
    //-------------------------------------------------------------------------------------------------
    public static Chunk ComposeOriginal(Chunk native,XmlSchemaSet schemas)
        =>ComposeCore(native,schemas,true);
    //-------------------------------------------------------------------------------------------------
    /** Reborn: one insertion pass emits reviewed empty contracts, copying the native graph once without stock templates or captured asset coordinates. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk ComposeCore(Chunk native,XmlSchemaSet schemas,bool original)
    {
        if(native?.InstanceBuffer==null || native.InstanceBuffer.Length<600)throw new InvalidDataException("Bounded empty-root native input required.");
        // Reborn: a populated Browser list is detached from the lower composers and restored with item rows below (stock WorldBuilder layout).
        var browser=Ep1GameObjectBrowser.Inspect(native.InstanceBuffer,Ep1GameObjectBrowser.Slots(native.RelocationBuffer));native=Ep1GameObjectBrowser.Detach(native,browser);
        // Reborn: emit null metadata for omitted names only, never reject or duplicate a populated name already owned by the lower producer.
        var absentNames=new[]{new Field("DisplayName",404,0x1A2933AA,false),new Field("DisplayNameTransformed",408,0x1A2933AA,false)}.Where(f=>Word(native.InstanceBuffer,f.Slot)==0);
        // Reborn: an omitted ArmorSet still has a zero-count list declaration in a complete original-object metadata table.
        // Reborn: omitted VisionInfo keeps the original retail null root declaration and no native allocation.
        var absentVision=Word(native.InstanceBuffer,560)==0?new[]{new Field("VisionInfo",560,0x649F97D1,false)}:Array.Empty<Field>();
        var absentArmor=Word(native.InstanceBuffer,420)==0?new[]{new Field("ArmorSet",420,0x5E5AF575,true)}:Array.Empty<Field>();
        // Reborn: omitted movement rows retain the same complete zero-count metadata declaration as other original-object lists.
        var absentLocomotor=Word(native.InstanceBuffer,428)==0?new[]{new Field("LocomotorSet",428,0x6E6BF482,true)}:Array.Empty<Field>();
        // Reborn: complete original-object metadata keeps null declarations for omitted pointer-list containers instead of dropping root fields.
        var absentContainers=new[]{new Field("Draws",472,0xEFBBA791,false),new Field("ClientUpdates",492,FastHash.GetHashCode("ClientUpdateModules"),false),new Field("ClientBehaviors",496,FastHash.GetHashCode("ClientBehaviorModules"),false)}.Where(f=>Word(native.InstanceBuffer,f.Slot)==0);
        // Reborn: an object without AI keeps the inline zero-count AI declaration (count +480, items +484), as on stock SovietCrane.
        var absentAi=Word(native.InstanceBuffer,480)==0&&Word(native.InstanceBuffer,484)==0?new[]{new Field("AI",480,0x5FD8691D,true)}:Array.Empty<Field>();
        var fields=original?Fields.Concat(absentNames).Concat(absentVision).Concat(absentAi).Concat(absentArmor).Concat(absentLocomotor).Concat(absentContainers).Concat(new[]{new Field("Flammability",464,0x10444D6C,false),new Field("LiveCameraOffset",528,0x29368077,false)}).ToArray():Fields.ToArray();
        foreach(var field in fields)for(int at=field.Slot;at<field.Slot+(field.List?8:4);at+=4)if(Word(native.InstanceBuffer,at)!=0)throw new InvalidDataException("Populated empty-profile native storage: "+field.Name);
        var old=Ep1GameObjectRootOwnersProjection.Compose(native,schemas);int oldPrefix=old.InstanceBuffer.Length-native.InstanceBuffer.Length,insert=0;
        for(;insert<oldPrefix;insert+=12)if(Word(old.InstanceBuffer,insert)==0 && Word(old.InstanceBuffer,insert+4)==0 && Word(old.InstanceBuffer,insert+8)==0)break;
        if(insert>=oldPrefix)throw new InvalidDataException("Empty-root base sentinel absent.");int extra=fields.Length*12,prefix=oldPrefix+extra+fields.Count(f=>f.List)*12+browser.Count*12;var result=new Chunk{InstanceBuffer=new byte[prefix+native.InstanceBuffer.Length]};
        old.InstanceBuffer.AsSpan(0,insert).CopyTo(result.InstanceBuffer);old.InstanceBuffer.AsSpan(insert,oldPrefix-insert).CopyTo(result.InstanceBuffer.AsSpan(insert+extra));
        for(int at=8;at<oldPrefix;at+=12){uint target=Word(old.InstanceBuffer,at);if(target!=0)Put(result.InstanceBuffer,at+(at<insert?0:extra),target<oldPrefix?target+(uint)(target>=insert?extra:0):target-(uint)oldPrefix+(uint)prefix);}
        int table=oldPrefix+extra;
        for(int i=0;i<fields.Length;i++)
        {
            var field=fields[i];int row=insert+i*12;Put(result.InstanceBuffer,row,FastHash.GetHashCode(field.Name));Put(result.InstanceBuffer,row+4,field.Wire);
            if(field.List){Put(result.InstanceBuffer,row+8,(uint)table);Put(result.InstanceBuffer,table,0x6C827073);Put(result.InstanceBuffer,table+4,0x5D448FD8);Put(result.InstanceBuffer,table+8,(uint)(prefix+field.Slot));table+=12;
                // Reborn: each populated Browser item row targets its 8-byte native string record, as in the stock WorldBuilder table.
                if(field.Name=="Browser")for(int item=0;item<browser.Count;item++){Put(result.InstanceBuffer,table,0xB151A0EB);Put(result.InstanceBuffer,table+4,0);Put(result.InstanceBuffer,table+8,(uint)(prefix+browser.Items+item*8));table+=12;}}
        }
        // Reborn: the prior producer already proves exact native/auxiliary ownership; reuse its transformed tail once and shift its native pointer words by only the additional prefix delta.
        old.InstanceBuffer.AsSpan(oldPrefix).CopyTo(result.InstanceBuffer.AsSpan(prefix));int delta=prefix-oldPrefix;
        result.RelocationBuffer=Shift(old.RelocationBuffer,true);result.ImportsBuffer=Shift(old.ImportsBuffer,false);Ep1GameObjectBrowser.Restore(result,prefix,browser);return result;
        //-------------------------------------------------------------------------------------------------
        /** Reborn: metadata insertion changes pointer addresses and auxiliary sources, never occurrence values or slot ordering. */
        //-------------------------------------------------------------------------------------------------
        byte[] Shift(byte[] source,bool relocate){if(source.Length==0)return Array.Empty<byte>();var output=source.ToArray();for(int at=0;at<source.Length-4;at+=4){uint slot=Word(source,at);Put(output,at,slot+(uint)delta);if(relocate)Put(result.InstanceBuffer,(int)slot+delta,Word(result.InstanceBuffer,(int)slot+delta)+(uint)delta);}return output;}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: native and generated table word reads stay inside the admitted arrays. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] b,int at){if(at<0 || at>b.Length-4)throw new InvalidDataException("Empty root projection word bounds.");return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: only newly owned output arrays receive little-endian coordinate patches. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] b,int at,uint value)=>BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at,4),value);
}
