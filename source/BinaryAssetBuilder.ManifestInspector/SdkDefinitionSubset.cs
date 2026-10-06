using System.Globalization;
using System.Text.RegularExpressions;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: admit only the three bounded definition forms observed in GlobalDefines; never run dynamic code or claim EA evaluator equivalence.
internal static class SdkDefinitionSubset
{
    internal const string Name = "diagnostic-definition-subset-v1";
    // Reborn: resolved definitions retain original text and origin as diagnostic evidence, separately from asset substitutions.
    internal sealed record Evaluation(string Name,string SourcePath,string OriginalValue,string EvaluatedValue);
    private const string Id = "[A-Za-z_][A-Za-z0-9_]{0,127}";
    private static readonly Regex Alias = new("\\A=\\s*\\$(?<name>"+Id+")\\s*\\z",RegexOptions.CultureInvariant,TimeSpan.FromSeconds(1));
    private static readonly Regex Concat = new("\\A=\\s*\\$(?<name>"+Id+")\\s*\\+\\s*'(?<suffix>[^'\\r\\n]{0,256})'\\s*\\z",RegexOptions.CultureInvariant,TimeSpan.FromSeconds(1));
    private static readonly Regex MultiplyAdd = new("\\A=\\s*\\(\\s*\\$(?<left>"+Id+")\\s*\\*\\s*(?<factor>[0-9]{1,10})\\s*\\)\\s*\\+\\s*\\$(?<right>"+Id+")\\s*\\z",RegexOptions.CultureInvariant,TimeSpan.FromSeconds(1));

    //-------------------------------------------------------------------------------------------------
    /** Reborn: resolve backward-visible aliases, single-quoted suffix concatenation and checked nonnegative integer multiply-add only. */
    //-------------------------------------------------------------------------------------------------
    internal static string Evaluate(string expression,Func<string,string?> lookup)
    {
        if (expression.Length == 0 || expression.Length > 512 || expression[0] != '=') throw new InvalidDataException("Bounded definition expression required.");
        Match match = Alias.Match(expression);
        if (match.Success) return Value(match.Groups["name"].Value);
        match = Concat.Match(expression);
        if (match.Success)
        {
            string result = Value(match.Groups["name"].Value)+match.Groups["suffix"].Value;
            if (result.Length > 512) throw new InvalidDataException("Definition concatenation exceeds 512 characters.");
            return result;
        }
        match = MultiplyAdd.Match(expression);
        if (!match.Success) throw new InvalidDataException("Definition form is outside the reviewed diagnostic subset.");
        try
        {
            long left = Number(Value(match.Groups["left"].Value)),right = Number(Value(match.Groups["right"].Value)),factor = Number(match.Groups["factor"].Value);
            return checked(left*factor+right).ToString(CultureInfo.InvariantCulture);
        }
        catch (OverflowException) { throw new InvalidDataException("Definition integer arithmetic overflow."); }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: forbid missing/forward names and unresolved expression values instead of recursively guessing definitions. */
        //-------------------------------------------------------------------------------------------------
        string Value(string name)
        {
            string? value = lookup(name);
            if (string.IsNullOrEmpty(value) || value.Length > 512 || value[0] == '=') throw new InvalidDataException("Definition refers to a missing, forward or unresolved name.");
            return value;
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: arithmetic accepts invariant unsigned decimal text within signed 64-bit range, not units, floats, signs or locale-specific numbers. */
    //-------------------------------------------------------------------------------------------------
    private static long Number(string value)
    {
        if (value.Length == 0 || value.Length > 19 || !value.All(character => character is >= '0' and <= '9')
            || !long.TryParse(value,NumberStyles.None,CultureInfo.InvariantCulture,out long number)) throw new InvalidDataException("Definition arithmetic requires bounded nonnegative integer operands.");
        return number;
    }
}
