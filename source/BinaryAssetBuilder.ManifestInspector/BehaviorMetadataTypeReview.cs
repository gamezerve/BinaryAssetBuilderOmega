using System.Buffers.Binary;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core.Hashing;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: review only schema-name/type-tag facts from bounded independent stock metadata, never export native offsets or per-asset token templates into the generator.
internal static class BehaviorMetadataTypeReview
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: field type signatures differ from plain concrete class hashes; require one consistent observed tag per qualified schema type across every selected original graph. */
    //-------------------------------------------------------------------------------------------------
    internal static SortedDictionary<string,uint> Collect(XmlSchemaSet schemas,byte[] stock,GameObjectModuleReview.Module[] modules,int nativeRoot,IEnumerable<(string Schema,int Table)>? records=null)
    {
        const string ns="uri:ea.com:eala:asset";var result=new SortedDictionary<string,uint>(StringComparer.Ordinal);var visited=new HashSet<(string,int)>();foreach(var module in modules)Record((XmlSchemaComplexType)schemas.GlobalTypes[new XmlQualifiedName(module.Type,ns)]!,module.Wrapper,0);if(records!=null)foreach(var record in records)Record((XmlSchemaComplexType)schemas.GlobalTypes[new XmlQualifiedName(record.Schema,ns)]!,record.Table,0);return result;
        //-------------------------------------------------------------------------------------------------
        /** Reborn: read the stock record structure, then bind its names to actual schema declarations rather than guessing type IDs from native field names. */
        //-------------------------------------------------------------------------------------------------
        void Record(XmlSchemaComplexType schema,int table,int depth)
        {
            if(depth>32 || table<0 || table>=nativeRoot || !visited.Add((schema.QualifiedName.ToString(),table)))throw new InvalidDataException("Behavior type review metadata cycle/bounds.");if(Word(table)==2)table=(int)Word(table+8);var extension=(schema.ContentModel as XmlSchemaComplexContent)?.Content as XmlSchemaComplexContentExtension;var fields=new Dictionary<uint,(XmlSchemaType Type,bool Repeated)>();
            // Reborn: the official polymorphic hardpoint also emits a synthetic native TypeId, despite having no literal XSD attribute.
            if(schema.Name is "ModuleData" or "WeaponSlot_Hardpoint")fields.Add(FastHash.GetHashCode("TypeId"),((XmlSchemaType)schemas.GlobalTypes[new XmlQualifiedName("SageInt",ns)]!,false));
            var refs=new Dictionary<uint,string>();foreach(XmlSchemaAttribute a in extension?.Attributes??schema.Attributes){uint name=FastHash.GetHashCode(a.Name!);fields.Add(name,(a.AttributeSchemaType!,false));refs[name]=Reference(a.UnhandledAttributes);}Elements(extension?.Particle??schema.Particle,fields,refs);
            for(int rows=0;;rows++,table+=12){if(rows>1024 || table>nativeRoot-12)throw new InvalidDataException("Behavior type review row bound.");uint name=Word(table),tag=Word(table+4),target=Word(table+8);if(name==0 && tag==0 && target==0)break;if(name==1){if(extension==null)throw new InvalidDataException("Behavior type review unexpected parent.");Record((XmlSchemaComplexType)schemas.GlobalTypes[extension.BaseTypeName]!,checked((int)target),depth+1);continue;}if(!fields.TryGetValue(name,out var field))throw new InvalidDataException("Stock behavior field absent from schema: "+schema.Name+"/"+name.ToString("X8"));string key=field.Type.QualifiedName.Namespace==XmlSchema.Namespace?"xs:"+field.Type.QualifiedName.Name:field.Type.QualifiedName.Name;if(key.Length==0)throw new InvalidDataException("Anonymous behavior field type.");if(refs.TryGetValue(name,out string? reference) && reference.Length!=0)key+=":"+reference;if(field.Repeated)key+="[]";if(result.TryGetValue(key,out uint prior) && prior!=tag)throw new InvalidDataException("Behavior field type tag ambiguous: "+key+" at "+schema.Name+"/"+name.ToString("X8")+": "+prior.ToString("X8")+" vs "+tag.ToString("X8"));result[key]=tag;if(target==0 || target>=nativeRoot)continue;
                bool list=field.Repeated || field.Type is XmlSchemaSimpleType simple && simple.Content is XmlSchemaSimpleTypeList;if(list){if(Word((int)target)!=0x6C827073)throw new InvalidDataException("Behavior type review list marker differs.");uint count=Word((int)Word((int)target+8));if(count>4096)throw new InvalidDataException("Behavior type review list count bound.");if(field.Type is XmlSchemaComplexType item)for(int i=0;i<count;i++)Record(item,(int)Word((int)target+20+i*12),depth+1);}else if(field.Type is XmlSchemaComplexType child)Record(child,(int)target,depth+1);
            }
        }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: only explicit sequence members supply field type identity facts. */
        //-------------------------------------------------------------------------------------------------
        static void Elements(XmlSchemaParticle? particle,Dictionary<uint,(XmlSchemaType Type,bool Repeated)> fields,Dictionary<uint,string> refs){if(particle==null)return;if(particle is XmlSchemaSequence sequence){foreach(XmlSchemaParticle child in sequence.Items)Elements(child,fields,refs);}else if(particle is XmlSchemaChoice choice && choice.Items.Count==1 && choice.Items[0] is XmlSchemaElement cost && cost.Name=="BuildCost" && cost.ElementSchemaType?.Name=="MoneyTransaction")Elements(cost,fields,refs);else if(particle is XmlSchemaElement e){uint name=FastHash.GetHashCode(e.Name!);fields.Add(name,(e.ElementSchemaType!,e.MaxOccurs>1 || e.MaxOccursString=="unbounded"));refs[name]=Reference(e.UnhandledAttributes);}else throw new InvalidDataException("Unreviewed behavior type particle.");}
        //-------------------------------------------------------------------------------------------------
        /** Reborn: field-level reference targets are part of wire signature identity, even when both declarations use Poid. */
        //-------------------------------------------------------------------------------------------------
        static string Reference(XmlAttribute[]? attributes)=>attributes?.SingleOrDefault(a=>a.LocalName=="refType" && a.NamespaceURI=="uri:ea.com:eala:asset:schema")?.Value??"";
        //-------------------------------------------------------------------------------------------------
        /** Reborn: every type-review read stays inside the already admitted stock asset slice. */
        //-------------------------------------------------------------------------------------------------
        uint Word(int at){if(at<0 || at>stock.Length-4)throw new InvalidDataException("Behavior type review word bounds.");return BinaryPrimitives.ReadUInt32LittleEndian(stock.AsSpan(at,4));}
    }
}
