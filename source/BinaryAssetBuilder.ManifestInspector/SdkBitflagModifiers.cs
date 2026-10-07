using System.Xml.Schema;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: prove whole-token enum-list behavior agrees with core substring operations before allowing any modifier through.
internal static class SdkBitflagModifiers
{
    // Reborn: a plan records bounded raw operands and the ordered whole-token result to compare against the unchanged core.
    internal sealed record Plan(string Before,string Modifiers,string[] Expected,int Operations);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: admit only bounded, single-space-separated signed tokens of the exact enum-list shape used by the existing core joiner. */
    //-------------------------------------------------------------------------------------------------
    internal static string[] Syntax(XmlSchemaAttribute use,string value)
    {
        // Reborn: reject overlong operands before allocating split arrays, not after tokenization.
        if (value.Length > 1024) throw new InvalidDataException("Bitflag modifier length bound exceeded.");
        var values = Enumeration(use);
        string[] operations = value.Split(' ');
        if (operations.Length is < 1 or > 64 || operations.Any(operation => operation.Length < 2 || operation[0] is not ('+' or '-') || !values.Contains(operation[1..])))
            throw new InvalidDataException("Bitflag modifiers require bounded single-space signed enum tokens.");
        return operations;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject missing/removing-absent bases and substring collisions, then predict ordered token membership without replacing core behavior. */
    //-------------------------------------------------------------------------------------------------
    internal static Plan Prove(XmlSchemaAttribute use,string before,string modifiers)
    {
        // Reborn: cap inherited raw values before splitting; the source byte bound does not replace the tighter modifier operand bound.
        if (before.Length > 4096) throw new InvalidDataException("Bitflag base length bound exceeded.");
        var values = Enumeration(use); string[] operations = Syntax(use,modifiers);
        var tokens = Tokens(before).ToList();
        if (tokens.Count > 512 || tokens.Distinct(StringComparer.Ordinal).Count() != tokens.Count || tokens.Any(token => !values.Contains(token)))
            throw new InvalidDataException("Bitflag base requires bounded unique literal enum tokens.");
        string raw = before;
        foreach (string operation in operations)
        {
            string token = operation[1..]; bool present = tokens.Contains(token,StringComparer.Ordinal);
            if (!present && raw.Contains(token,StringComparison.Ordinal)) throw new InvalidDataException("Bitflag substring identity collision remains closed.");
            if (operation[0] == '+')
            { if (!present) { tokens.Add(token); raw += " "+token; } }
            else
            {
                if (!present || tokens.Any(other => other != token && other.Contains(token,StringComparison.Ordinal))) throw new InvalidDataException("Bitflag removal requires an existing whole token without substring collisions.");
                tokens.Remove(token); raw = raw.Replace(token,string.Empty,StringComparison.Ordinal);
            }
        }
        return new(before,modifiers,tokens.ToArray(),operations.Length);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: tokenize only ASCII-space enum lists; other whitespace cannot hide core split/substring differences. */
    //-------------------------------------------------------------------------------------------------
    internal static string[] Tokens(string value) => value.Split(' ',StringSplitOptions.RemoveEmptyEntries);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require the exact compiled KindOfBitFlags direct list/enumeration shape and close arbitrary lists/restrictions. */
    //-------------------------------------------------------------------------------------------------
    private static HashSet<string> Enumeration(XmlSchemaAttribute use)
    {
        if (use.AttributeSchemaType?.QualifiedName != new System.Xml.XmlQualifiedName("KindOfBitFlags","uri:ea.com:eala:asset")
            || use.AttributeSchemaType.Content is not XmlSchemaSimpleTypeList list || list.BaseItemType?.Content is not XmlSchemaSimpleTypeRestriction restriction
            || restriction.Facets.Count is < 1 or > 1024 || restriction.Facets.OfType<XmlSchemaObject>().Any(facet => facet is not XmlSchemaEnumerationFacet))
            throw new InvalidDataException("Reviewed direct KindOfBitFlags enum-list schema required.");
        // Reborn: signed operators/whitespace are not enum token identities even if an arbitrary replacement schema declares them.
        string[] tokens = restriction.Facets.OfType<XmlSchemaEnumerationFacet>().Select(facet => facet.Value ?? "").ToArray();
        if (tokens.Any(token => token.Length is < 1 or > 128 || token.Any(character => !char.IsAsciiLetterOrDigit(character) && character != '_'))
            || tokens.Distinct(StringComparer.Ordinal).Count() != tokens.Length) throw new InvalidDataException("Bitflag enum requires unique bounded identifier tokens.");
        return tokens.ToHashSet(StringComparer.Ordinal);
    }
}
