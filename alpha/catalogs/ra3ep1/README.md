# Uprising asset lookup catalogs

<!-- Reborn: local catalogs support dependency checks and selective WorldBuilder extraction without game discovery. -->
Local copies of `static.manifest`, `global.manifest` and `worldbuilder.manifest`
are read relative to Studio. WorldBuilder resolution also uses matching
`worldbuilder.bin`, `.relo` and `.imp` here, plus `cdata/` for external resources.
Only referenced WorldBuilder-only assets and their dependencies are packaged;
static/global/audio payloads remain in the original game streams.
`gamestrings.csf` contains the original English string table (8,776 keys), copied
from the active table in English.big, not gamestrings_temp.csf. String checks
combine it with each mod's Misc/Data/mod.str without reading the game directory.
The original manifest/CSF copies are excluded from Git pending redistribution review.
Native compiler development inputs remain separate.

<!-- Reborn: AudioEvent authoring references retail sample identities through an optional metadata-only audio catalog. -->
`audio.manifest` is an additional local copy from the unpacked English audio
stream. It enables AudioFile lookup for authored AudioEvents and is also
excluded from Git. The mod references `data/audio.manifest`; retail encoded
samples are not copied into its BIG.
