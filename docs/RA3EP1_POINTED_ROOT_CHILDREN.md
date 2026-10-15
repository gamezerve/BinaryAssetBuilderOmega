# Pointed GameObject root children

October 15, 2026. Four optional root children now join the same selected
Core/native/token graph as armor, movement, Body, destruction, weapon, SMC and
Geometry. These are allocated records, unlike the inline enum/color words from
the preceding milestone. The native root remains 600 bytes.

| Child | Root pointer slot | Root token type | Native pointee |
| --- | ---: | --- | --- |
| DisplayName | 404 | 1A2933AA | Eight-byte ASCII length/pointer header |
| DisplayNameTransformed | 408 | 1A2933AA | Eight-byte ASCII length/pointer header |
| Flammability | 464 | 10444D6C | Four unsigned 32-bit words |
| LiveCameraOffset | 528 | 29368077 | Three finite floats |

Name and camera tokens target the native pointee directly. Flammability instead
targets its generated 60-byte field table: Fuel, MaxBurnRate, Decay and Resistance,
each using unsigned-32 primitive token 763177A4, followed by a zero
terminator. The four optional fields raise the largest selected root from 94 to
**98 fields**; this is still not the complete 126-field selected stock inventory.

## Evidence and limits

Five independent stock objects establish all four root type tags and positive
name/header/string relationships. Flammability and camera are null in those five
examples. Their nonnull records therefore have reviewed schema/native primitive
contracts and authored composition evidence, **not positive stock or gameplay
parity**. Finding and comparing independent nonnull stock examples is a remaining
validation step. Names are localization-key text, not compiled localization tables;
Unicode/STR production and arbitrary string assets are not enabled here.

Follow-up review corrected the initial inferred Flammability primitive tag:
23E650B8 is the signed/TypeId primitive, whereas stock CampnessValue and
VisionBonusTestSegments independently establish 763177A4 for unsigned values.
The test now checks that independent stock anchor before authored verification;
this correction still does not replace nonnull Flammability stock evidence.
The corrected standalone test passes all 288 combinations and 20 input/18
metadata refusals, including explicit rejection of the old signed tag. Its fresh
build succeeds with 111 existing warnings and no errors. The 50-group result
below records the initial checkpoint, before this primitive-tag correction.

`gameobject-pointer-root-self-test <static> <global>` runs 288 actual-Core/native
cases: all 16 presence masks, three text/default/scalar profiles and six orders of
the three behaviors. Profiles include absent versus allocated-empty strings,
escaped/long ASCII, uint.MaxValue, default-zero Flammability and signed fractional
camera values. Raw adapted source validates against the reviewed official schema
candidate before focused Core normalization. Compilation repeats byte-for-byte.

`gameobject-authored-pointer-root-self-test <fresh-output> <static> <global>`
repeats these combinations using a genuinely newly built ArmorTemplate target;
removing that target refuses the consumer. This publishes the armor diagnostic
target, not a complete GameObject or mod package.

The composed native ownership gate checks alignment, pre-behavior bounds,
disjoint record/list/string storage, bounded ASCII and finite vectors. Exact shared
text spans may alias; partial overlap and record aliases refuse. The six possible
new root/nested relocation slots are checked exactly; no new import is admitted.
Native pointers and relocation coordinates rebase once while existing asset
reference occurrence indices remain unchanged.

The stock-target variant rejects 20 bad inputs and 17 detached metadata faults;
the authored-target variant adds the missing-target refusal. Existing independent
geometry, body, module, movement and import checks remain active in these combined
tests. This does not claim complete pre-behavior byte coverage or generic recursive
source/dependency closure.

## Files and remaining gates

`Ep1GameObjectPointerRoots.cs` owns the bounded native record gate;
`Ep1GameObjectStringTokens.cs` and `Ep1GameObjectDestructionProjection.cs` integrate
it with root metadata. `GameObjectPointerRootSmokeTest.cs` and
`tests/fixtures/RootPointerPipeline.xsd` exercise actual Core normalization and
independent native/token assertions. Existing combat adaptation and geometry
verification are reused without replacing their owner/reference checks.

Remaining gates include independent nonnull Flammability/camera stock evidence,
the other root containers/children, general module closure and complete mixed
GameObject stream publication. The Studio alpha admission boundary is unchanged.
No game execution, installation changes or new gameplay proof are claimed.

## Final regression

`artifacts/GameObjectPointerRoots-a/batch-report.json`: **50/50 Focused groups
pass in 309.755 seconds**. Stock/authored pointer groups take 20.651/13.238
seconds. All eight runtime hashes remain stable. The batch incremental build
succeeds in 1.43 seconds with zero warnings/errors; the preceding fresh build
succeeds with two warnings and no errors. GameExecuted, InstalledFilesModified
and AlphaCapabilitiesExpanded are false. No Full-suite run is claimed.
