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
| Same combined graph, new object ID + explicit command | 30 | 43 | 84120 | 37572 | 271 + one local command import |

All six cases passed repeat builds, complete native owner/relocation/import
coverage, semantic stock reference comparison, complete metadata ownership,
stock-bound field/type/target validation and exact tokenized-native inversion.
Thirty native and sixty metadata corruption controls were rejected. Six real
stream/BIG/SKUDef packages passed deterministic serialization, manifest/checksum/
selector readback and archive variant extraction. The stream gates rejected
3480 source/reference/format corruptions/refusals, including changes to source,
BIN, RELO and IMP after graph qualification. The new-ID case additionally passed
the two-asset GameObject/explicit UNIT_BUILD command gate and its four refusal
controls. No command is synthesized from the new object ID.

Behavior metadata is independently stock-qualified per source-selected template
and then checked at its actual combined table position. All other root/module
metadata owners retain independent stock-to-current native coordinate maps.
Every prefix byte must belong to exactly one verified metadata owner. No fixed
88824-byte prefix or fixed 275-occurrence expectation is used for changed cases.

## Live Studio source admission

<!-- Reborn: use the ordinary EA source language and the user's existing GUI entry point, not hidden template IDs or a new launcher. -->
Use `alpha/EALAModStudio.exe` as usual. No extra XML flag, special module-ID
prefix, alternate SDK directory or separate user build command is required.
`ConscriptVariableBehaviorSource` derives the template-origin map from official
schema-qualified type and complete nested payload/reference shape; the module
ID alone never selects a native template. Matching ignores only reviewed fixed
attributes and the own local ID. Typed reference spelling equivalence retains
the same actual asset identity; it is not an arbitrary target-retarget mask.

The source route admits add/remove/reorder/clone operations within `Behaviors`,
plus reviewed direct module fixed fields/local IDs and fixed root primitive/
enum/flag/weak-ID attributes. The same templates can be inherited/imported using
normal `AssetDeclaration`, `Includes`, `Defines` and `inheritFrom` source.
Other module containers keep their reviewed count/order and nested shape.
Dynamic root strings, root strong-reference presence/target edits, existing
nonmodule nested scalar edits and arbitrary nested lists/families are not yet
combinable with this variable profile. Existing fixed-topology capabilities
remain available on their established route.

`ConscriptVariableGraphVerifier` is shared by the diagnostic suite and Studio.
It verifies actual fixed-field encodings before restoring only comparison bytes
for independent stock ownership checks; actual output values are never reset.
`ConscriptVariableBehaviorBuild` separately derives source reference occurrences,
checks catalog identity collisions, repeats complete native/metadata compilation,
then uses graph-qualified stream/package production. It reads only an explicitly
authored adjacent command; no LogicCommand is synthesized from an object ID.

Seven live-driver controls passed: a combined 30-behavior/new-ID/explicit-command
build with BuildTime=1, Scale=1.25, ThreatLevel=15, MaxHealth=500 and AI
StopChaseDistance=42.5; nested DieMuxData change refusal; colliding-ID refusal;
65-behavior budget refusal; the same variable graph through DATA includes,
imported definitions and real instance inheritance; restored original developer
source compilation; and its explicit recruitment consumer. Failed inputs kept
the good package. Original user/developer source ownership was preserved.

## Publication safeguards and remaining work

<!-- Reborn: widening the serializer requires exact verified bytes, not dropping the source guard. -->
The serializer now has an explicit graph-qualified route: `ConscriptGraphQualification`
captures the normalized source, ordered semantic references, actual metadata
extent and SHA-256 of each verified buffer. Subsequent source, target ordering,
type/hash or byte changes are refused. This evidence is build-local, not a
loadable receipt or an untrusted XML flag. The existing non-graph serializer
route retains its original limits; ordinary GameObject serialization is still
closed. Package receipts verify their metadata origin against the actual BIN.

This graph-qualified route is now connected to Studio source admission, but only
through the schema/template contract above. Ordinary unqualified GameObject
serialization remains closed. The next steps are independently admitting new
nested payload shapes/families and variable topology in the other containers;
simply dropping those guards would still be unsafe.

The historical six-case diagnostic retains complete, unchanged module payload
templates; the shared verifier additionally supports the fixed-value edits
exercised by live source builds. Neither establishes arbitrary nested module
children, arbitrary new module families or arbitrary GameObjects.
The existing reviewed nonempty-container safety bounds still apply; they are
not evidence of an EA engine module-count limit. Empty/absent Behaviors,
variable Draws/AI/Body/client topology, renamed local-reference graphs and edited
nested template payloads need their own source-derived proof and integration.

## Evidence and scope

The developer-only offline batch includes
`conscript-module-graph-self-test <static.manifest> <global.manifest> <fresh-output>`.
Each fresh directory contains six authored XML variants, tokenized BIN diagnostic
buffers, `module-graph-report.json`, and `<case>-package/` directories with
independently verified BIG/config/SKUDef/source/receipt files. The new-ID case
also retains its explicit command XML. These are offline diagnostic candidates,
not published Studio outputs or game-tested mods.
The test does not edit either live mod or launch/install anything in the game.

<!-- Reborn: preserve the existing Studio authoring/output contract while adding a separate graph-qualified serializer path. -->
The final compiler build passed with zero warnings/errors. Seven variable-profile
live-driver checks passed, including combined values/structure, inheritance,
three rejection/last-good-package controls, restored developer-source compilation
and the explicit recruitment consumer. The shared six-case native/metadata/
stream/package suite also passed after verifier extraction. All 22 original XML
location checks passed; schema errors retain their original offending node rather
than being retried through the variable capability matcher.
User Reborn sources/output were unchanged; developer sources were restored.
The original developer BIG retained SHA-256
`87241958D0EC4CDB3A133FFC837E023A01B51716924F6B532A30BCBDE7EDF918`.
