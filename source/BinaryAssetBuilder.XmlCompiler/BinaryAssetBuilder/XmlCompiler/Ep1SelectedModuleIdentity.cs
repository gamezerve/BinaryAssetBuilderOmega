using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;

namespace BinaryAssetBuilder.XmlCompiler;

// Reborn: the selected partial-object profile owns one local ModuleData scope across Behaviors, Body, common AI and scripted/tracer Draws, not a general EA Poid resolver.
public static class Ep1SelectedModuleIdentity
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: collect all selected source module IDs before checking forward/backward AssociatedModule links or allocating native buffers. */
    //-------------------------------------------------------------------------------------------------
    public static void ValidateSource(XmlElement root)=>ValidateSource(root,false);
    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve the original public entry point while explicitly opting into the stock-qualified wider behavior family set. */
    //-------------------------------------------------------------------------------------------------
    public static void ValidateSource(XmlElement root,bool reviewedBehaviors)
    {
        const string ns="uri:ea.com:eala:asset";
        if(root.LocalName!="GameObject" || root.NamespaceURI!=ns)throw new InvalidDataException("Selected module identity requires an EP1 GameObject root.");
        var behaviors=root.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="Behaviors" && e.NamespaceURI==ns).ToArray();
        var bodies=root.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="Body" && e.NamespaceURI==ns).ToArray();
        if(behaviors.Length!=1 || bodies.Length!=1)throw new InvalidDataException("Selected module identity requires exactly one Behaviors and Body container.");
        var modules=behaviors[0].ChildNodes.OfType<XmlElement>().ToArray();var body=bodies[0].ChildNodes.OfType<XmlElement>().ToArray();
        // Reborn: the wider behavior path is explicit; the existing three-class/default profile remains unchanged.
        // Reborn: the complete behavior compiler uses actual allocation bounds, not the former 64-module trial quota.
        if(modules.Length<1 || (!reviewedBehaviors&&modules.Length>3) || body.Length!=1 || body[0].LocalName!="ActiveBody" || body[0].NamespaceURI!=ns || modules.Any(e=>e.NamespaceURI!=ns || (!reviewedBehaviors && e.LocalName is not ("BuildingDestructionBehavior" or "WeaponSetUpdate" or "ObjectSMCHelper"))))throw new InvalidDataException("Unimplemented selected identity container/class.");
        if(reviewedBehaviors)foreach(var module in modules)Ep1BehaviorMetadataTokenizer.ReviewedWireName(module.LocalName);
        // Reborn: selected common AI shares the same local identity scope; client/other concrete AI families remain unadmitted.
        var aiContainers=root.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="AI" && e.NamespaceURI==ns).ToArray();
        if(aiContainers.Length>1)throw new InvalidDataException("Selected identity permits at most one AI container.");var ai=aiContainers.SelectMany(e=>e.ChildNodes.OfType<XmlElement>()).ToArray();
        if(ai.Length>2 || ai.Any(e=>e.NamespaceURI!=ns || e.LocalName!="AIUpdate"))throw new InvalidDataException("Unreviewed selected AI identity class/count.");
        // Reborn: selected complete scripted/tracer Draws share the same local scope, including cross-class Body/AI/behavior collisions and forward links.
        var drawContainers=root.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="Draws" && e.NamespaceURI==ns).ToArray();
        var draws=drawContainers.SelectMany(e=>e.ChildNodes.OfType<XmlElement>()).ToArray();
        if(drawContainers.Length>1 || draws.Length>64 || draws.Any(e=>e.NamespaceURI!=ns || e.LocalName is not ("ScriptedModelDraw" or "TracerModelDraw")))throw new InvalidDataException("Unreviewed selected Draw identity class/count.");
        // Reborn: both reviewed client containers join the same local module scope; unknown namespaces/classes, repeated containers and excessive counts fail before allocation.
        var clients=new List<XmlElement>();
        foreach(string name in new[]{"ClientUpdates","ClientBehaviors"})
        {
            var containers=root.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName==name).ToArray();var selected=containers.SelectMany(e=>e.ChildNodes.OfType<XmlElement>()).ToArray();
            if(containers.Length>1 || containers.Any(e=>e.NamespaceURI!=ns) || selected.Length>64 || selected.Any(e=>e.NamespaceURI!=ns || (name=="ClientUpdates"?e.LocalName!="LargeGroupAudioClientUpdate":e.LocalName is not ("ModelConditionAudioLoopClientBehavior" or "ModelConditionSoundSelectorClientBehavior" or "AnimationSoundClientBehavior"))))throw new InvalidDataException("Unreviewed selected client identity class/container/count.");clients.AddRange(selected);
        }
        var ids=new HashSet<uint>();
        foreach(var module in modules.Concat(body).Concat(ai).Concat(draws).Concat(clients))
        {
            string id=module.GetAttribute("id");
            if(string.IsNullOrWhiteSpace(id) || !ids.Add(FastHash.GetHashCode(id.ToLowerInvariant())))throw new InvalidDataException("Selected Body/behavior/AI/Draw module IDs must be nonempty and hash-distinct in this object.");
        }
        foreach(var module in modules.Where(e=>e.LocalName=="BuildingDestructionBehavior" && e.HasAttribute("AssociatedModule")))
        {
            string target=module.GetAttribute("AssociatedModule");
            if(string.IsNullOrWhiteSpace(target) || !ids.Contains(FastHash.GetHashCode(target.ToLowerInvariant())))throw new InvalidDataException("AssociatedModule does not resolve inside the selected partial object.");
        }
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: raw native callers cannot bypass selected scope uniqueness or retain a dangling destruction-to-module hash after source checks. */
    //-------------------------------------------------------------------------------------------------
    public static void ValidateNative(byte[] data,IReadOnlyList<int> modules,int body,IReadOnlyList<int> additionalModules=null)
    {
        var ids=new HashSet<uint>();
        foreach(int start in modules.Concat(body==0?Array.Empty<int>():new[]{body}).Concat(additionalModules??Array.Empty<int>()))if(!ids.Add(Word(start+4)))throw new InvalidDataException("Selected native Body/behavior/AI/Draw module IDs collide.");
        for(int i=0;i<modules.Count;i++)
        {
            int start=modules[i],end=i+1<modules.Count?modules[i+1]:body==0?data.Length:body;
            if(Word(start)!=0x0D96A2F4)continue;
            if(end-start<72)throw new InvalidDataException("Selected destruction owner is truncated.");
            uint associated=Word(start+52);
            if(associated!=0 && !ids.Contains(associated))throw new InvalidDataException("Selected native AssociatedModule is unresolved.");
        }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: detached native identity words must remain inside the supplied graph. */
        //-------------------------------------------------------------------------------------------------
        uint Word(int at){if(at<0 || at>data.Length-4)throw new InvalidDataException("Selected module identity bounds exceeded.");return BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(at,4));}
    }
}
