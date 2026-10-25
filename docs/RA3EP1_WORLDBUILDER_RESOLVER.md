# WorldBuilder asset resolution

<!-- Reborn: record the production selection contract and verification evidence without changing editable mod sources. -->
Studio resolves WorldBuilder references after native compilation and before final
package publication. The resolver reads `alpha/catalogs/ra3ep1/worldbuilder`
`.manifest`, `.bin`, `.relo` and `.imp` as one matching linked EP1 generation.

Validation reports pending references as informational progress, not warnings.
After resolution and final stream assembly, the package and static/global/audio
catalogs are checked together. Only still-unresolved WorldBuilder use sites
produce warnings, retaining their original owner, XML path and line/column.

Selection starts from emitted strong references and schema-typed GameObject weak
references in prepared authored XML, including OCL `CreateObject`. It recursively
collects manifest dependencies. Mod definitions and loaded static/global/audio
or earlier mod streams take precedence. Arbitrary weak IDs inside copied binary
objects are not scanned; manifest dependency closure does not prove every dynamic
spawn or script-selected asset is available.

Native payloads, asset fingerprints, per-asset reference order, relocation slots
and import selectors are preserved. Complete chunks are ordered dependency-first;
metadata tables and stream checksums are regenerated. All three packaged variants
receive the resolved data. External providers introduced by copied assets are
added to runtime manifest references only when needed.

Hash-named custom resources are copied from `catalogs/ra3ep1/cdata` into each
stream family's custom-data directory. Missing external Texture/AudioFile
resources produce an error. Embedded model and texture payloads need no extra
file. Unknown custom-resource encodings in other native families still require
their own validation.

Offline evidence: intact `VL_RussianPortBuilding` plus its collapse GameObject
selected 20 retail assets / 2,456,368 native bytes. A separate production-driver
fixture compiled two authored containers and added six retail dependencies.
Tests cover exact native and BIG readback, all variants, local/loaded precedence,
cycles, weak spawn seeds, custom-resource absence/preservation and corrupt
headers/pointers/import selectors. These checks do not establish gameplay
visibility; verify the rebuilt mod in a new match.
