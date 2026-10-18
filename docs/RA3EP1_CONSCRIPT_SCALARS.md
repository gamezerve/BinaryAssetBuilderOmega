# Conscript scalar authoring — grouped alpha step

The Studio route now admits six numeric attributes of the supplied complete
`SovietAntiInfantryInfantry` XML. It is not an arbitrary GameObject compiler.
Use only `alpha/EALAModStudio.exe`; select **RebornTest**, then **Build Mod**.
The user's Reborn source and outputs are not used by developer regressions.

## Source contract

| Owner | Attribute | Stock | Grouped trial | Range |
| --- | --- | --- | --- | --- |
| GameObject | BuildTime | 4 | 1 | 0.1–600 |
| ObjectResourceInfo/BuildCost | Amount | 100 | 25 | integer 0–100000 |
| Body/ActiveBody | MaxHealth | 100 | 500 | 1–100000 |
| VisionInfo | VisionRange | 200 | 350 | 1–5000 |
| VisionInfo | ShroudClearingRange | 500 | 650 | 1–5000 |
| LocomotorSet | Speed | 50 | 90 | 1–500 |

These are practical alpha admission ranges, not engine-wide limits. Cost uses
unsigned decimal digits; other fields accept finite invariant floating-point
numbers. NaN, infinity, missing/duplicate owners, new IDs, changed references,
additional nodes and unreviewed attributes are rejected. Preserve supplied XML
whitespace/structure outside these values. Includes, inheritance and definitions
are not enabled for this complete GameObject route yet.

## Implementation and offline proof

- `Invoke-Ra3Ep1ConscriptAlphaBuild.ps1`: restores only the six values in a copy
  before comparing the existing complete-source SHA256 fingerprint. Original
  source hash remains in the receipt. No full-source whitelist bypass.
- `ConscriptEditableScalars.cs`: shared C# source/range policy and native field
  coordinates. Root BuildTime is +140; body, vision, cost list and locomotor use
  anchored native parent pointers. Cost is uint, other values are float.
- `GameObjectConscriptTrialSmokeTest.cs`: real normalization/allocation twice,
  exact complete native equality after restoring the six stock words, complete
  metadata equality with exactly those words edited. Unchanged relocation/import
  order and 275 dependencies are mandatory. Nine extra profiles compile each
  field separately, all fields together, and all lower/upper bounds together
  (nine profiles total). Source/value/native corruptions fail.
- `BoundedDiagnosticBuild.SerializeConscriptTrial`: retains fixed graph/type/
  identity/count admission and rechecks all six source/native values before
  stream serialization. Generic tokenized GameObject admission remains closed.
- Package receipts include `EditableValues` and actual `BuildTime`; they do not
  claim game execution. BIG stream variant/readback/refusal controls remain.
- `Test-Ra3Ep1DevSdk.ps1`: preserves BuildTime=1 golden archive, adds combined
  scalar publication and malformed cost/health/vision/reference rejection,
  verifies rejected builds retain last-good output, restores developer sources
  and rebuilds them. Reborn sources/receipt remain unchanged.

## One grouped gameplay check (not performed automatically)

Build **RebornTest** in Studio, then choose its current SKUDef in the Uprising
launcher. Start a **new Soviet skirmish**, not a saved game:

1. Conscript production should remain about one second; its cost should show 25.
2. Compare movement across the same distance with stock (90 versus 50).
3. Check larger fog clearing (650 versus 500); targeting vision (350 versus 200)
   is distinct from weapon firing range, which this package does not change.
4. Under comparable fire a fresh Conscript should survive longer (500 max health
   versus 100); weapon damage, armor and model remain stock.

Menu loading alone does not prove scalar behavior. The user subsequently reported
the grouped trial **worked**. This is qualitative gameplay evidence for this
specific six-scalar package, not six independent numeric measurements or proof
of arbitrary values. New custom identities/recruitment and arbitrary modules
remain separate work from this original-ID trial.

## Local verification — October 18, 2026

- Alpha regression: **20/20** cases passed. The old BuildTime=1-only fixture
  retained BIG SHA256 `3FCDFFB9909B39E9C3A6B75AC50791A19D3EFBAE5A83D6169C174197C50C8FB2`.
- Final compiler build: zero warnings/errors (incremental Release/x86 build).
- Final refreshed RebornTest build: all nine native/metadata scalar profiles,
  36 source/native refusal controls, original complete ownership, both stream
  readback suites and fresh package publication passed.
- Confirmed original-ID grouped BIG SHA256:
  `739B9B8B544940E99DF83770AB12DE2F02D302FCF8D1B785C2A8224D9B513A36`.
- Exact-reference CLR4 Studio adapter loader/config/options check passed without
  showing Studio or starting a build callback/game.
- `alpha/.work/dev-sdk-test.json`, `scalar-regression.log`,
  `scalar-final-compiler.log` and `scalar-final-build.log` retain local evidence.
- No game execution, installation modification or Reborn build took place.
