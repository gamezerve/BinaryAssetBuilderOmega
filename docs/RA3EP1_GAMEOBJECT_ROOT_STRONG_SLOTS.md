# GameObject root strong-import slot projection

October 15, 2026. Three stock-audited descriptors join the partial root projection:

| Field | Native offset | Token type |
| --- | ---: | --- |
| CrowdResponse | 180 | 9AD999F3 |
| UnitTypeIcon | 368 | A1F1A41F |
| UnitIntro | 400 | 6A76B67A |

Generated root metadata now has **87 fields including Behaviors**: 62 scalars,
15 enum/flag/weak fields, six string headers, three strong reference words and
the behavior container. This is not a publishable complete GameObject.

## Ownership change

`Ep1GameObjectStringTokens.Inspect` now requires exactly one root import slot for
each nonzero reviewed reference word, and none for a zero word. Unknown root
imports, absent-value imports, missing owners and reference words in RELO refuse.
Existing dynamic string/behavior allocation checks remain unchanged.

`Ep1GameObjectDestructionProjection.Compose` extracts only module-owned IMP slots
into the standalone fragment writer. Previously, subtracting the module start
from every IMP slot would underflow any newly admitted root import. The composed
output retains all root/module import slots, shifts their **locations** by the
token prefix and never renumbers their stored dependency **indices**. Strong
reference words are not relocation pointers.

## Evidence and boundaries

`GameObjectRootStrongSmokeTest` audits all three descriptors against the independent
Allied Construction Yard and Conscript stock root tables. Eight authored presence
combinations pass raw full-candidate schema validation and real native GameObject
dispatch. After raw validation, the test deliberately supplies **diagnostic
normalized indices** 0, 5 and 17; the destruction FX uses 31. Their native words
must be 1, 6, 18 and 32. An allocated EditorName string moves the behavior module.
Independent projection checks reverse the entire native/RELO/IMP shift.

Ten detached controls refuse three missing imports, three zero-value imports,
three erroneous relocation owners and one unknown root import. Descriptor-driven
metadata corruption controls now cover 172 type/target mutations.

These tests do **not** normalize root reference names through Core, resolve their
targets in a real catalog or compile target payloads. Sparse numbers are deliberate
ownership fixtures, not an authored XML API or evidence of full dependency closure.
UnitIntro still uses the existing BaseAssetType native placeholder. Other tests
retain their separate real-Core named destruction FX/weak-reference evidence;
that evidence is not widened to these root targets by this milestone.

Next: normalize root and module names in one actual Core reference table, check
typed target identity/uniqueness against explicit catalogs, then admit remaining
root/child graphs and the general GameObject output processor. No alpha package,
game installation or gameplay acceptance changes here.

## Final regression

`artifacts/GameObjectRootStrongSlots-a/batch-report.json`: **30/30 Focused groups
pass in 83.388 seconds**, with all eight runtime hashes unchanged. The corrected
incremental build passes with zero warnings/errors. The initial test build found
an ambiguous `List` import from Relo; a Chunk-only alias fixed the test without
changing compiler behavior. The stock-source no-root-import baseline projection
is now BIN/RELO/IMP **2188/24/8**, including a 1464-byte partial token prefix.
172 descriptor type/target corruptions and all existing refusal gates pass.
No new Full-suite run, alpha expansion or game launch is claimed.
