using System.Text;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: read localization identities only; translations and original game archives are never rewritten or redistributed.
internal static class SdkStringCatalog
{
    internal const int MaximumBytes=32*1048576;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: read the active gamestrings table only, not gamestrings_temp or unrelated CSFs that could hide missing runtime keys. */
    //-------------------------------------------------------------------------------------------------
    internal static HashSet<string> Stock(string path)
        =>Csf(StockBytes(path));

    //-------------------------------------------------------------------------------------------------
    /** Reborn: materialize only the validated active CSF for the SDK's local catalog, never overwrite a differing user file. */
    //-------------------------------------------------------------------------------------------------
    internal static void CopyStock(string source,string destination)
    {
        source=SdkEnvironmentPreflight.Absolute(source);destination=SdkEnvironmentPreflight.Absolute(destination);SdkEnvironmentPreflight.CheckPath(destination);SdkEnvironmentPreflight.DirectoryPath(Path.GetDirectoryName(destination)!);
        byte[] bytes=StockBytes(source);int keys=Csf(bytes).Count;
        if(File.Exists(destination)){if(!SdkEnvironmentPreflight.Read(destination,MaximumBytes).SequenceEqual(bytes))throw new InvalidDataException("Different existing string catalog preserved: "+destination);}
        else{using var file=new FileStream(destination,FileMode.CreateNew,FileAccess.Write,FileShare.None);file.Write(bytes);}
        Console.WriteLine("Reborn: local string catalog: "+destination+"; "+keys+" keys; source="+source+".");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: share bounded extraction of the one active CSF; translations are never dumped into diagnostics. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] StockBytes(string path)
    {
        SdkEnvironmentPreflight.CheckPath(path);byte[] bytes;
        if(Path.GetExtension(path).Equals(".big",StringComparison.OrdinalIgnoreCase)){
            using var archive=BigArchive.Open(path,100000,16*1048576);
            var entries=archive.Entries.Where(e=>e.Name.Replace('\\','/').Equals("data/gamestrings.csf",StringComparison.OrdinalIgnoreCase)).ToArray();if(entries.Length!=1)throw new InvalidDataException("Stock archive requires one unambiguous data/gamestrings.csf entry.");
            bytes=archive.ReadEntry(entries[0],MaximumBytes);
        }else if(Path.GetExtension(path).Equals(".csf",StringComparison.OrdinalIgnoreCase))bytes=SdkEnvironmentPreflight.Read(path,MaximumBytes);
        else throw new InvalidDataException("StockStrings must name a CSF or its language BIG archive.");
        return RefPack.IsCompressed(bytes)?RefPack.Decompress(bytes,MaximumBytes):bytes;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: CSF records carry explicit label/value counts; skip encoded UTF-16 translations by their lengths instead of guessing keys from binary strings. */
    //-------------------------------------------------------------------------------------------------
    internal static HashSet<string> Csf(byte[] bytes)
    {
        if(bytes.Length>MaximumBytes)throw new InvalidDataException("CSF exceeds the localization read bound.");
        using var stream=new MemoryStream(bytes,false);using var reader=new BinaryReader(stream,Encoding.Latin1);
        try{
            if(Tag()!=" FSC")throw new InvalidDataException("Invalid CSF signature.");uint version=reader.ReadUInt32();if(version is not (2 or 3))throw new InvalidDataException("Unsupported CSF version: "+version);
            uint labels=reader.ReadUInt32(),strings=reader.ReadUInt32();reader.ReadUInt32();reader.ReadUInt32();
            if(labels>1000000||strings>1000000)throw new InvalidDataException("Invalid CSF record counts.");
            var result=new HashSet<string>(StringComparer.OrdinalIgnoreCase);uint values=0;
            for(uint index=0;index<labels;index++){
                if(Tag()!=" LBL")throw new InvalidDataException("Invalid CSF label record.");uint count=reader.ReadUInt32();if(count>strings-values)throw new InvalidDataException("Invalid CSF label value count.");
                string key=Encoding.Latin1.GetString(Read(reader.ReadUInt32()));if(key.Length==0||key.Any(char.IsControl))throw new InvalidDataException("Invalid CSF key.");
                for(uint value=0;value<count;value++){
                    string tag=Tag();if(tag is not (" RTS" or "WRTS"))throw new InvalidDataException("Invalid CSF string record.");Skip(checked(reader.ReadUInt32()*2));if(tag=="WRTS")Skip(reader.ReadUInt32());values++;
                }
                // Reborn: an untranslated label with no value does not prove that the runtime can display the string.
                if(count>0)result.Add(key);
            }
            if(values!=strings||stream.Position!=stream.Length)throw new InvalidDataException("CSF counts/trailing bytes differ.");return result;
        }catch(Exception error)when(error is EndOfStreamException or OverflowException){throw new InvalidDataException("Truncated or invalid CSF lengths.",error);}

        // Reborn: every count is checked against the actual remaining allocation before reading or skipping it.
        byte[] Read(uint length){if(length>int.MaxValue||length>stream.Length-stream.Position)throw new InvalidDataException("CSF record exceeds its buffer.");return reader.ReadBytes((int)length);}
        // Reborn: translations are not needed to audit key existence.
        void Skip(uint length){if(length>stream.Length-stream.Position)throw new InvalidDataException("CSF value exceeds its buffer.");stream.Position+=length;}
        // Reborn: on-disk CSF tags use their literal little-endian byte spelling.
        string Tag()=>Encoding.ASCII.GetString(Read(4));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: parse complete STR key/quoted-value/END blocks, ignoring comments and quoted fake labels rather than accepting line-shaped false positives. */
    //-------------------------------------------------------------------------------------------------
    internal static HashSet<string> Str(byte[] bytes)
    {
        if(bytes.Length>MaximumBytes)throw new InvalidDataException("STR exceeds the localization read bound.");
        using var stream=new MemoryStream(bytes,false);using var reader=new StreamReader(stream,new UTF8Encoding(false,false),true);string text=reader.ReadToEnd();int at=0,currentLine=1;
        var keys=new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        while(Next() is { } key){
            if(key.Quoted||key.Text.Equals("END",StringComparison.OrdinalIgnoreCase))throw new InvalidDataException("STR expected a key at line "+key.Line+".");
            int values=0;bool ended=false;while(Next() is { } value){if(!value.Quoted){if(value.Text.Equals("END",StringComparison.OrdinalIgnoreCase)){ended=true;break;}throw new InvalidDataException("STR expected a quoted value or END at line "+value.Line+".");}values++;}
            if(!ended||values==0)throw new InvalidDataException("Incomplete STR block for "+key.Text+" at line "+key.Line+".");keys.Add(key.Text);
        }
        return keys;

        // Reborn: comments are only recognized outside quoted text; escaped quotes and multiline translations remain one value token.
        (string Text,bool Quoted,int Line)? Next(){
            while(at<text.Length){if(char.IsWhiteSpace(text[at])){if(text[at]=='\n')currentLine++;at++;continue;}if(at+1<text.Length&&text[at]=='/'&&text[at+1]=='/'){while(at<text.Length&&text[at]!='\n')at++;continue;}if(at+1<text.Length&&text[at]=='/'&&text[at+1]=='*'){int end=text.IndexOf("*/",at+2,StringComparison.Ordinal);if(end<0)throw new InvalidDataException("Unterminated STR comment.");currentLine+=text.AsSpan(at,end+2-at).Count('\n');at=end+2;continue;}break;}
            if(at==text.Length)return null;int line=currentLine;
            if(text[at]=='"'){at++;int start=at;while(at<text.Length){if(text[at]=='\\'){if(at+1<text.Length&&text[at+1]=='\n')currentLine++;at+=2;continue;}if(text[at]=='"'){string value=text[start..at];at++;return(value,true,line);}if(text[at]=='\n')currentLine++;at++;}throw new InvalidDataException("Unterminated STR quoted text at line "+line+".");}
            int begin=at;while(at<text.Length&&!char.IsWhiteSpace(text[at])&&text[at]!='"'&&!(at+1<text.Length&&text[at]=='/'&&text[at+1] is '/' or '*'))at++;
            if(begin==at)throw new InvalidDataException("Invalid STR token at line "+line+".");return(text[begin..at],false,line);
        }
    }
}
