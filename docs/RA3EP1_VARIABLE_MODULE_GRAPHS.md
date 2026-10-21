# Variable Conscript behavior graphs

<!-- Reborn: variable native/metadata evidence is a prerequisite for flexible authoring, not an alpha publication claim. -->
The complete-object compiler and metadata composer now have an independent
variable-topology diagnostic, `ConscriptModuleGraphSmokeTest.cs`. It does not
compare a changed module's new ordinal against the same stock ordinal. Authored
local identity and an explicit diagnostic template-origin map select each
independently captured stock module instead.

## Verified October 21, 2026

| Complete-object source | Behaviors | Total modules | Metadata bytes | Native bytes | Reference occurrences |
| --- | ---: | ---: | ---: | ---: | ---: |
| Original | 36 | 49 | 88824 | 39360 | 275 |
| Reversed behavior order | 36 | 49 | 88824 | 39360 | 275 |
| Every second behavior removed | 18 | 31 | 80892 | 36840 | 266 |
| Twelve behaviors cloned with new IDs | 48 | 61 | 92052 | 40092 | 280 |
| Removal, cloning and reverse order combined | 30 | 43 | 84120 | 37572 | 271 |

All five cases passed repeat builds, complete native owner/relocation/import
coverage, semantic stock reference comparison, complete metadata ownership,
stock-bound field/type/target validation and exact tokenized-native inversion.
Twenty-five native and fifty metadata corruption controls were rejected.

Behavior metadata is independently stock-qualified per source-selected template
and then checked at its actual combined table position. All other root/module
metadata owners retain independent stock-to-current native coordinate maps.
Every prefix byte must belong to exactly one verified metadata owner. No fixed
88824-byte prefix or fixed 275-occurrence expectation is used for changed cases.

## Remaining publication work

<!-- Reborn: never remove the live frontend guard while its serializer still assumes the original complete graph. -->
This diagnostic is not connected to Studio BIG publication yet. The live alpha
retains its fixed module topology. The next implementation step is to extract
this proof into a reusable source-derived graph contract and replace the
serializer's fixed prefix/native-size/relocation/reference assumptions, then
admit variable source topology through that contract.

The diagnostic uses complete, unchanged module payload templates; only their
presence/order and cloned local identity vary. It does not establish arbitrary
nested module children, arbitrary new module families or arbitrary GameObjects.
The existing reviewed nonempty-container safety bounds still apply; they are
not evidence of an EA engine module-count limit. Empty/absent Behaviors,
variable Draws/AI/Body/client topology, renamed local-reference graphs and edited
template payloads need their own source-derived proof and integration.

## Evidence and scope

The developer-only offline batch includes
`conscript-module-graph-self-test <static.manifest> <global.manifest> <fresh-output>`.
Each fresh directory contains the five authored XML variants, tokenized BIN
diagnostic buffers and `module-graph-report.json`. These BIN buffers are not
published mod assets: no matching stream/archive/launch package is claimed.
The test does not edit either live mod or launch/install anything in the game.
