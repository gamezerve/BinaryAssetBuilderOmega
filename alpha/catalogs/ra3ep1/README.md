# Uprising asset lookup catalogs

<!-- Reborn: local catalogs support dependency checks and selective WorldBuilder extraction without game discovery. -->
Local copies of `static.manifest`, `global.manifest` and `worldbuilder.manifest`
are read relative to Studio. WorldBuilder resolution also uses matching
`worldbuilder.bin`, `.relo` and `.imp` here, plus `cdata/` for external resources.
<!-- Reborn: maps borrow the retail WorldBuilder patch base instead of duplicating its model/texture payloads per map. -->
Static builds selectively package referenced WorldBuilder-only assets and their
dependencies. Map builds retain zero-sized WorldBuilder reference records and
link to the original game's `worldbuilder.manifest`, as shipped retail maps do.
Only authored overrides, terrain data and genuinely map-local art own payloads
in `map_mod.*`; WorldBuilder BIN/RELO/IMP/CDATA is not copied into each map.
Static/global/audio payloads otherwise remain in their selected provider streams.
<!-- Reborn: patch metadata must obey the runtime's forward-only base lookup and logical allocation sizes. -->
Borrowed entries preserve retail base order, and patch header totals/maxima include
the inherited native sizes even though those chunks occupy no local sidecar bytes.
The resolver qualifies both rules before accepting the generated map stream.
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
