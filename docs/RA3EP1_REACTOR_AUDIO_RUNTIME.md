# Reactor audio runtime paths — October 25, 2026

<!-- Reborn: retain the first listening-test failure and distinguish valid native imports from correctly located runtime streams. -->
The user reported silence from the new Super Reactor ambient and spark events
after building a new structure in a new match, including with Volume=100.
Damage-effect sounds were audible. The published Reborn package and its receipt
were inspected read-only; user sources and final output were not rebuilt or changed.

## Findings

The final Streams BIG matched its receipt SHA256. Its two global AudioEvents
contained normalized Volume=1.0 and PlayPercent=1.0. Ambient had 14 nonzero
AudioFile reference indices, each with weight 1000 and normalized volume 1.0;
sparks had three. The Reactor's native imports targeted both new AudioEvent IDs:
ambient occurrence 99 at offset 49460, sparks occurrence 94 at offset 46396.
The ambient entry retained EP1 soundAmbient ordinal 6. These offsets describe
this exact user package, not reusable native layout constants.

The installed `EnglishAudio.big` manifest was read and RefPack-decoded without
reading waveform payloads. It matched `alpha/catalogs/ra3ep1/audio.manifest`
byte-for-byte after decoding. All 17 required AudioFile identities were present;
none of the Sound entries was empty or silently replaced with a null import.

The actual defect was the runtime manifest reference table:

| Reference | Broken output | Correct output |
| --- | --- | --- |
| Global event → retail samples | `data\audio.manifest` | `audio.manifest` |
| Static → global mod stream | `data\additionalmaps\mapmetadata_global.manifest` | `additionalmaps\mapmetadata_global.manifest` |
| Static → retail catalogs | `data\static.manifest`, `data\global.manifest` | `static.manifest`, `global.manifest` |

Both installed EP1 reader factories prepend their global `Data\` prefix for
relative paths. EP1 1.1 ordinary factory VA `004AAC80` copies prefix storage
`00CC46C8` and appends the reference at `004AACC1`–`004AACEF`; EP1 1.0 ordinary
factory `004AA9C0` uses prefix storage `00CBD6C8` and the corresponding append
block `004AAA01`–`004AAA2F`. Both initialized prefix strings are `Data\`.
The initial comparison literal is `:\`, distinguishing the absolute-path branch.
No native game code was executed during this review.

Consequently the broken references requested `Data\data\audio.manifest` and
`Data\data\additionalmaps\mapmetadata_global.manifest`. The installed audio BIG
contains `data\audio.manifest`; it has no `data\data\audio.manifest` entry.
Retail global/static metadata and the user's EA-compiled RA3 mod also store
bare `audio.manifest`, `global.manifest` and `static.manifest` references.

The mixed compiler, standalone GameObject stream and explicit recruitment
composition now use Data-root-relative reference names. Global archive/context
names still include `data/`; only their serialized reference-table spelling is
converted. The actual BIG family names and SKUDef contract remain unchanged.
Regression checks now inspect marker-delimited reference strings and reject
the double-prefixed form instead of expecting the old spelling.

## When to listen

- `SOV_RebornSuperReactor_AmbientLoop` is the building's soundAmbient entry.
  Listen after the Super Reactor finishes construction/unpacking, with the
  camera nearby. It is a continuous building ambient; selecting the building
  or damaging it is not its trigger. Its authored MinRange/MaxRange are 200/550.
- `SOV_RebornSuperReactor_Sparks` is an AnimationSoundClientBehavior event for
  `SBPowerPlantAdvanced_IDLA`, at frames 50, 100, 150, 200, 250 and 300. Listen
  during the normal idle animation, alongside the existing electrical sparks.
  It is intermittent, with MinRange/MaxRange 100/400, not a continuous loop.
- Both events use the AMBIENT mixer slider. With the current source, both
  volumes are 100; the compiler preserves those values.

Rebuild Reborn in EALAModStudio with Build Global Data checked before repeating
the listening test in a new match. Existing published packages retain their old
reference strings until rebuilt. Corrected offline output is not itself proof
of audible playback; in-game confirmation remains the final check.
