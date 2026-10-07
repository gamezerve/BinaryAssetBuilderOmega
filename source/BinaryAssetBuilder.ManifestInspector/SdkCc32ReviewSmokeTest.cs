namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: pin native metadata identity boundaries without requiring external game files in the default test suite.
internal static class SdkCc32ReviewSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove absence/presence, complete identity matching and ambiguous-name refusal separately from native state layout interpretation. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        var target = Asset("AIStrategicStateDefinition:AlliedCaptureTech_MEDIUM",2,7,Array.Empty<AssetId>());
        var owner = Asset("AIPersonalityDefinition:AIP_CC_32_AlliedEnemy",3,9,new[] { new AssetId(4,7) });
        if (SdkCc32Review.Observe(new[] { owner,target }).TargetReferencePresent) throw new InvalidDataException("Wrong-type same-instance reference matched the target.");
        owner = owner with { References = new[] { new AssetId(2,8) } };
        if (SdkCc32Review.Observe(new[] { owner,target }).TargetReferencePresent) throw new InvalidDataException("Same-type wrong-instance reference matched the target.");
        owner = owner with { References = new[] { new AssetId(2,7) } };
        if (!SdkCc32Review.Observe(new[] { owner,target }).TargetReferencePresent) throw new InvalidDataException("Exact native metadata identity was not matched.");
        bool duplicate = false; try { SdkCc32Review.Observe(new[] { owner,target,target }); } catch (InvalidOperationException) { duplicate = true; }
        if (!duplicate) throw new InvalidDataException("Ambiguous target name was accepted.");
        bool missing = false; try { SdkCc32Review.Observe(new[] { owner }); } catch (InvalidOperationException) { missing = true; }
        if (!missing) throw new InvalidDataException("Missing native target was mistaken for a proved absent owner reference.");
        Console.WriteLine("SDK CC32 review self-test: OK (complete type/instance metadata identity, absent/present references, ambiguous/missing target refusal; no native state layout proof)");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: build owned metadata records without manufacturing game payload or manifest-validation authority. */
    //-------------------------------------------------------------------------------------------------
    private static ManifestAsset Asset(string name,uint type,uint instance,AssetId[] references) => new(type,instance,0,0,0,references.Length,0,0,0,0,0,null,name,"owned-fixture",references);
}
