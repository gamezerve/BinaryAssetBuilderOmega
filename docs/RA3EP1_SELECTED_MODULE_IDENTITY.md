# Selected GameObject module-local identity scope

October 15, 2026. `Ep1SelectedModuleIdentity` closes a selected-graph integrity gap:
behavior-only ID uniqueness did not detect a collision with ActiveBody, and a
destruction AssociatedModule hash could refer to no module in that partial graph.

## Implemented contract

The explicit selected source path collects the one-to-three reviewed behavior
IDs and the single ActiveBody ID before detaching their containers or allocating
native memory. IDs must be nonempty and distinct after lowercase engine hashing.
Destruction AssociatedModule is optional; when authored it must resolve inside
this selected scope, independently of declaration order or letter case.

The native owner decoder separately checks Body/behavior hash uniqueness and
nonzero AssociatedModule closure. A raw caller or detached corruption cannot
bypass those constraints by avoiding source validation. Owner bounds and concrete
token writers remain separate checks; this resolver does not accept extra classes.

This is an intentionally strict **partial-object profile policy**, not proof that
every EA local-reference namespace has the same rules. A real stock destruction
module can link a Draw module; the current selected profile has no Draw container
and must not pretend to resolve that link. Wider container/reference support must
expand the collector and its tests explicitly. General production dispatch and
the alpha are not enabled by these diagnostic checks.

Root asset IDs and pipeline-only list-record IDs are not collected as ModuleData
declarations. Local links emit no asset imports. The resolver is per object, with
no persistent ID registry leaking across compilations. It is not a general Poid,
inheritObject, replace/remove-module or tokenized-source graph resolver.

## Evidence

`selected-module-identity-self-test` validates source against the reviewed official
candidate and runs actual selected native allocation/token composition:

- All six behavior orders and four local target identities, with case-folded
  links: 24 cases; one absent-link case makes **25**.
- Native weak hashes and absence of strong imports are independently checked;
  repeated allocation must preserve bytes without mutating source/global scope.
- Root/nonmodule pipeline IDs intentionally reuse module names, demonstrating
  that arbitrary XML `id` attributes are not treated as module declarations.
- **10 source refusals**: cross-container and within-container collisions,
  case-folded collisions, missing/empty/whitespace IDs and unresolved/empty links.
- **4 raw native refusals**: Body/behavior aliases and a dangling local target.

The existing stock/module/movement/combat regression gates remain necessary. No
new package, gameplay execution or full object publication is claimed here.

Files: `Ep1SelectedModuleIdentity.cs`, `Ep1GameObjectStringTokens.cs`,
`GameObjectBodyProjectionSmokeTest.cs`, `SelectedModuleIdentitySmokeTest.cs`,
the inspector command registration and the Focused batch recipe.

## Final regression

`artifacts/SelectedModuleIdentity-a/batch-report.json`: **43/43 Focused groups pass
in 118.130 seconds**, with eight unchanged runtime hashes and no game/installation
changes. The identity group takes 3.528 seconds. The batch build succeeds in 1.17
seconds with zero warnings/errors. No Full-suite or gameplay run is claimed.
