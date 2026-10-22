# Missing localization string checks

The normal `alpha/EALAModStudio.exe` build now checks localization keys alongside
missing assets and definitions. A use missing from **both** catalogs produces a
`Warning: MISSINGSTRING` with its key, XML owner, field, file, line and column.
The warning alone does not stop compilation or alter the authored key.

<!-- Reborn: the EA GUI parser and backend parser must both accept optional localization configuration. -->
The Studio adapter now accepts `StockStrings` during initialization. A failed
initialization is retried explicitly before a build callback so its original
configuration error cannot be masked by an uninitialized lifecycle reference.
The exact CLR4/EA CodeDOM loader regression checks initialization with the current
configuration and optional-key acceptance while still rejecting duplicates and
unknown keys. Restart an already-open Studio after this adapter update.

## Catalogs

- Stock: `StockStrings` in local `alpha/uprising-alpha.paths` selects the active
  language BIG or a CSF file. The current local installation uses
  `Data/English.big` -> `data/gamestrings.csf`.
- Mod: the selected mod's `Misc/Data/mod.str`. A missing file means no mod-added
  keys, not an error. Its existing Misc packaging remains unchanged.

Keys compare case-insensitively. A label with a translation, including an empty
quoted STR translation, satisfies presence. A zero-value CSF label is not evidence
of a usable translation. The checker reads CSF label/value records and decompresses
RefPack entries in memory; it does not dump the archive. `gamestrings_temp.csf`
and unrelated CSFs are deliberately not unioned into the active table.

STR parsing handles comments, escaped quotes, multiline quoted values and common
BOM encodings. Commented-out or incomplete blocks do not count as valid keys.
Malformed or unconfigured catalogs report `STRINGCATALOG` and incomplete coverage;
they do not produce a flood of unsubstantiated missing-key warnings. No original
translations or extracted stock catalog are added to the repository.

## XML scope

The check covers DisplayName/DisplayNameTransformed, Description and
TypeDescription attributes including transformed variants, explicit string-label
fields, and whole key-shaped schema strings using a prefix present in either
catalog. It does not guess that every arbitrary XML string is localized text.
Schema-typed asset/weak references, identities, paths, namespace declarations,
code bodies and empty optional values are excluded.

For the supported GameObject/command-set source frontend, checks use effective
emitted XML after actual includes, definitions and inheritance processing.
Instance-only bases are not independently treated as compiled objects; replaced
inherited strings therefore do not generate stale warnings. Exact source mapping
is used when available; ambiguous/unavailable origins are explicitly marked as
source context. If source preparation fails because the input is already invalid
or unsupported, the checker falls back to raw all-emitted declarations and reports
`PreparedSourceUsed=false`. That fallback is not full inherited/dynamic lookup
coverage. Runtime-constructed labels and arbitrary unrecognized key namespaces
outside known localization fields are not proven by this check.

The dependency JSON adds a `Strings` summary: catalog paths, stock/mod key counts,
number of checked uses, catalog availability and effective-source status. Reports
contain keys/locations, not translated values. The existing asset counts keep
their original meaning.

## Implementation and verification

- `SdkStringCatalog.cs`: bounded CSF/STR key readers, including both CSF value tags.
- `SdkMissingStringAudit.cs`: effective source scanning and mod/stock union lookup.
- `SdkMissingDependencyAudit.cs`: combined warnings and report integration.

The targeted regression covers CSF counts/STRW/zero-value labels, malformed data,
STR comments/escaping/multiline/UTF-16/empty values, stock/mod resolution, missing
keys, definition expansion, actual inherited Replace semantics, source positions
and incomplete-catalog behavior. A read-only check of the current Reborn source
read 8,776 stock keys and five mod STR keys; its three localization uses resolved.
Neither mod source nor installed game data was changed or built by that audit.
