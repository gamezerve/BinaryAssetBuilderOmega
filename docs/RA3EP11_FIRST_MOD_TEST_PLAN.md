# First Uprising mod-test plan

## Timing: a target, not a promise

We do not need a complete SDK to begin a narrow controlled loading test.
The planning target is **2–4 focused work sessions** to prepare the first
config/stock-loading smoke test. A session here means a substantive milestone
with validation, not each short continuation message. This is a manual estimate
from current evidence, not a measured completion date or commitment. Launcher
behavior, installation completeness or unexpected runtime failures may extend it.

A newly authored XML asset that demonstrably changes the game is a separate
test. No defensible date is available for that yet: the selected type's native
serialization/tokenization, dependency closure and valid runtime hash/manifest
contract still need proof. We will not wait for all 1,390 complex types, but
will not replace missing hashes with zero or relax production guards either.

## Phase A: config and unchanged-stock loading

Goal: prove the selected 1.1 launch path actually consumes the isolated config
and reaches existing valid stock assets. This is not an authored-asset test.

Remaining preparation:

1. Locate and inspect the actual `filesystem.cfg` and relevant config candidates
   with bounded reads. Reconcile later startup updates with the intended test
   config; do not infer effective precedence from SKU order.
2. Verify launcher-to-1.1 argument forwarding and define a reproducible baseline
   launch plus isolated config launch. Keep test artifacts in a new workspace
   directory; do not patch executables, change registry settings or overwrite
   installed configs/archives.
3. Resolve the stock-profile discrepancy before treating a launch failure as a
   mod failure. Configured `Data\MapsCampaign.big` is absent; a Disabled-named
   file exists. Do not rename it without the user's decision. A missing-campaign
   test profile must be explicitly documented if chosen, not silently aliased.
4. Choose an observable read/load signal. A main menu screenshot alone cannot
   prove `-modconfig` was consumed: require a supported log, attributable file
   access observation or another independently validated signal.

Run baseline and isolated-config cases separately and record exact executable,
SKU/config identities, arguments, observable result and failure symptoms.
Successful entry to a menu is startup evidence only. Keep rollback trivial:
the isolated configuration is not the installation's active stock config.
No game launch has been performed by the current audit sequence.

## Phase B: first narrowly authored asset

Prefer a simple already-characterized type rather than starting with music.
[ArmorTemplate tokenized PoC](RA3EP1_ARMOR_TOKEN_POC.md) is a candidate because
controlled serializer output has matched two bounded stock samples. That is
not a current game-load result or automatic authorization to publish its
experimental output as a compatible mod.

For the selected type, require:

- Fresh source/schema validation and the selected serializer/tokenizer tests.
- Stock-correlated type identity and a valid selected runtime hash/flag path;
  no fabricated compiler hash, zero-hash bypass or tokenized-flag relabeling.
- Correct manifest/BIN/RELO/IMP checksums, lengths, references and dependencies.
- A package readback independent of the writer and a deterministic, reversible
  in-game change that can distinguish this asset from unchanged stock.
- A baseline control and repeat load, with crash/load errors recorded rather
  than inferred away from static evidence.

If the selected tokenized type has a valid independently proved runtime path,
it need not wait for a universal ProcessingHash generator. Proving that path
is required; the existence of a conditional native branch alone is insufficient.
Music remains a later target because EP1 AUDIO inputs and authentic PathMusic
compiler identity are unresolved. Existing diagnostic music packages with
synthetic hashes are not playable-mod candidates.

## Reporting progress

Overall SDK effort remains approximately **52% / 48%**. These audits reduce
uncertainty in startup/loading but do not earn a successful mod-load percentage.
At the first runtime test, report Phase A independently of SDK readiness; at
the first authored change, report the selected type and exact tested scope.
The next priority is Phase A preparation, rather than indefinite broad reverse
engineering before attempting any controlled loading experiment.
