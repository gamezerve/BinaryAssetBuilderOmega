# Uprising asset lookup catalogs

<!-- Reborn: local metadata supports dependency checks without game discovery or asset payload copying. -->
Local copies of `static.manifest`, `global.manifest` and `worldbuilder.manifest`
are read relative to Studio. No BIN, RELO, IMP or CDATA payloads belong here.
`gamestrings.csf` contains the original English string table (8,776 keys), copied
from the active table in English.big, not gamestrings_temp.csf. String checks
combine it with each mod's Misc/Data/mod.str without reading the game directory.
The original manifest/CSF copies are excluded from Git pending redistribution review.
Native compiler development inputs remain separate.
