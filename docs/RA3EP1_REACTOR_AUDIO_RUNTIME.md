# Reactor audio runtime paths — October 25, 2026

<!-- Reborn: user gameplay feedback overrides the earlier path-based diagnosis; withdraw the experiment without discarding its evidence. -->
## Current status: keep mod AudioEvents in the static build

<!-- Reborn: record the user's actual recovery instead of treating the path rollback as a successful audio fix. -->
Rollback alone did not restore sound. The user then moved AudioEvents back to
`Static_Mod.xml` and reported that sound returned. Although retail AudioEvents
live in the global stream, mod AudioEvents must currently be declared in
`Static_Mod.xml` or files included by it, not `AdditionalMaps/MapMetaData_*.xml`.
Global builds will be tested again with other asset types later. The exact
runtime cause of the global-audio failure is not established.

## Earlier investigation

After commit `3d1591ea`, the user reported that all match sounds disappeared,
while menu music remained. The game's saved Ambient, SFX, Voice, Music and
Movie volume settings were all 100. This is a regression relative to the first
test, where ordinary damage-effect sounds still played.

The compiler now restores the exact pre-experiment reference spelling on mixed,
standalone GameObject and recruitment routes. The global-data checkbox and
subsequent log/color changes remain. No user Reborn source, published Reborn BIG,
game installation or audio setting was modified during rollback. The subsequent
user test showed that rollback alone was insufficient.

The earlier claim that removing `data/` fixed the audio was premature. The
observed factory prefix and matching offline metadata did not prove the runtime
loading, cache, registration or ownership behavior. Those remain investigation
areas, not established causes. The user's working placement is the static build.

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

The attempted path change, now withdrawn, was:

| Reference | Pre-experiment output (restored) | Attempted output (regressed match audio) |
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

The reviewed concatenation suggested `Data\data\audio.manifest` and
`Data\data\additionalmaps\mapmetadata_global.manifest`. This was a static path
projection, not an observed successful/failed live load. The installed audio BIG
contains `data\audio.manifest`; it has no `data\data\audio.manifest` entry.
Retail global/static metadata and the user's EA-compiled RA3 mod also store
bare `audio.manifest`, `global.manifest` and `static.manifest` references.

The mixed compiler, standalone GameObject stream and explicit recruitment
composition now restore their pre-experiment `data/` names. The BIG family names
and SKUDef contract remain unchanged. Offline checks pin this compatibility
baseline and stream integrity; they do not certify runtime audio behavior.

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

Compile the AudioEvents through `Static_Mod.xml`, excluding them from global
entry points, and rebuild in EALAModStudio. The user has confirmed restored
sound with this placement. Offline output alone remains insufficient proof
of audible playback for future global-stream experiments.
