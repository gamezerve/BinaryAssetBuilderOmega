using System.Text;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: exercise the independently guarded diagnostic header model without invoking the legacy mixed-mode reference or admitting music compilation.
internal static class PathMusicHeaderSemanticsSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: pin lexical quirks, first match/zero warning, strict literal boundaries and unreviewed reference refusal as owned in-memory fixtures. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        const string name = "MenuTrack",valid = "#define PATH_EVENT_MenuTrack 0x01B9D554";
        var line = PathMusicHeaderSemantics.ReviewLine(valid,name);
        Require(line.LegacyShapeMatch && line.CanonicalLiteral && line.Value == 0x01B9D554,"Canonical header value differs.");
        foreach (string wrong in new[] { valid.Replace("#define","#DEFINE"),valid.Replace("MenuTrack","menutrack"),valid.Replace("0x","0X"),
            "#define PATH_EVENT_MenuTrackExtra 0x01B9D554","#define PATH_EVENT_MenuTrack\t0x01B9D554","#define PATH_EVENT_MenuTrack 28955988" })
            Require(!PathMusicHeaderSemantics.ReviewLine(wrong,name).LegacyShapeMatch,"Unproved lexical broadening.");
        line = PathMusicHeaderSemantics.ReviewLine("// "+valid,name);
        Require(line.LegacyShapeMatch && line.Value == 0x01B9D554 && !line.CanonicalLiteral && line.UnsupportedReason != null,"Comment substring hazard hidden.");
        line = PathMusicHeaderSemantics.ReviewLine("0x2 // "+valid,name);
        Require(line.Value == 2 && !line.CanonicalLiteral,"Legacy first-hex selection was silently corrected.");
        line = PathMusicHeaderSemantics.ReviewLine(valid+"suffix",name);
        Require(line.Value == 0x01B9D554 && !line.CanonicalLiteral,"Partial numeric suffix silently admitted.");
        foreach (string number in new[] { "0xZZ","0x80000000","0x100000000" })
        {
            line = PathMusicHeaderSemantics.ReviewLine("#define PATH_EVENT_MenuTrack "+number,name);
            Require(line.LegacyShapeMatch && line.Value == null && !line.CanonicalLiteral && line.UnsupportedReason != null,"Unknown CRT numeric behavior admitted.");
        }
        var scan = PathMusicHeaderSemantics.Scan(Encoding.ASCII.GetBytes("#define PATH_EVENT_MenuTrack 0x0\r\n"+valid),name);
        Require(scan.MatchedLine == 1 && scan.Value == 0 && scan.LegacyWarningExpected == true && scan.CanonicalLiteral,"First explicit zero was replaced by later nonzero definition.");
        scan = PathMusicHeaderSemantics.Scan(Encoding.ASCII.GetBytes(valid+"\n#define PATH_EVENT_MenuTrack 0x2"),name);
        Require(scan.MatchedLine == 1 && scan.Value == 0x01B9D554 && scan.LegacyWarningExpected == false,"First nonzero selection differs.");
        scan = PathMusicHeaderSemantics.Scan(Encoding.ASCII.GetBytes("#define PATH_EVENT_Other 0x1"),name);
        Require(scan.MatchedLine == null && scan.Value == 0 && scan.LegacyWarningExpected == true && !scan.CanonicalLiteral,"No match was reported as verified zero constant.");
        // Reborn: unsupported native numeric behavior must leave warning prediction unknown, never assert successful verification.
        scan = PathMusicHeaderSemantics.Scan(Encoding.ASCII.GetBytes("#define PATH_EVENT_MenuTrack 0xZZ"),name);
        Require(scan.Value == null && scan.LegacyWarningExpected == null && !scan.CanonicalLiteral,"Unknown CRT result was presented as a non-warning success.");
        Refuses(() => PathMusicHeaderSemantics.VerifyReference(new byte[128]));
        Refuses(() => PathMusicHeaderSemantics.ReviewLine(new string('a',511),name));
        Refuses(() => PathMusicHeaderSemantics.ReviewLine(valid+"\0",name));
        Refuses(() => PathMusicHeaderSemantics.ReviewLine(valid+"é",name));
        Refuses(() => PathMusicHeaderSemantics.Scan(new byte[1048577],name));
        Refuses(() => PathMusicHeaderSemantics.Scan(Encoding.ASCII.GetBytes(new string('\n',8193)),name));
        Refuses(() => PathMusicHeaderSemantics.Scan(Encoding.ASCII.GetBytes(valid),"../MenuTrack"));
        Console.WriteLine("PathMusic header semantics self-test: OK (bounded canonical hex, ordinal prefix/name/space, comments/first-hex/suffix hazards, unknown CRT refusal, first match/explicit zero/missing warning, ASCII/budget/event/reference guards; no legacy execution or processor admission)");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: unsafe or unreviewed diagnostic inputs must refuse rather than gain production authority. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuses(Action action)
    {
        try { action(); } catch (InvalidDataException) { return; }
        throw new InvalidDataException("Unsupported header evidence accepted.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve distinctions between lexical observations, literal admission and genuine recovered input. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
