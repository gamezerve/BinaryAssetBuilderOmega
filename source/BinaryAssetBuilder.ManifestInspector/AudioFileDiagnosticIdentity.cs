namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: constrain authored names to literal reference-safe ASCII tokens, not paths or selector expressions.
internal static class AudioFileDiagnosticIdentity
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: permit bounded caller identities without allowing XML punctuation, reference separators or whitespace. */
    //-------------------------------------------------------------------------------------------------
    internal static void Validate(string name)
    {
        if (name.Length is < 1 or > 128 || name.Any(c => !(c is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or >= '0' and <= '9' or '_' or '-')))
            throw new InvalidDataException("Audio diagnostic identity must be 1..128 ASCII letters, digits, underscores or hyphens.");
    }
}
