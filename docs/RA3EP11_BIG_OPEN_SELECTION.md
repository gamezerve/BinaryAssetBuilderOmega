# EP1 1.1 scoped BIG open selection

## Scope and inputs

This follows [BIG name indexing](RA3EP11_BIG_NAME_INDEX.md) and retains the
same full-image, baseline, SKU and stock directory guards. The game is never
executed and archive payloads are never read. It models only reviewed query
preparation and manager-list selection over **supplied hit booleans**; those
booleans are not an emulation or execution of native per-archive lookup.

Implementation: `scripts/Get-Ra3Ep11BigOpenSelection.ps1`.
Regression: `scripts/Test-Ra3Ep11BigOpenSelection.ps1`.

| VA | Raw offset | Complete bytes | Role |
| --- | --- | ---: | --- |
| 009956D0 | 5956D0 | 768 | Outer BIG open and stream construction |
| 00995620 | 595620 | 105 | Iterator initializer |
| 00992D70 | 592D70 | 36 | Byte-preserving null-terminated copy |

Independent complete-body SHA-256 pins and the exact `big:` plus NUL literal
at VA `00C84EAC` are checked. The inspected original import-name thunks resolve
`00BD8374` to `MSVCR80.dll!strncmp` and `00BD848C` to `MSVCR80.dll!strchr`.
The recorded thunks/symbol bytes are checked without loading a DLL.

## Open query preparation

`009956D0` compares four bytes against exact lowercase `big:` with `strncmp`.
If present it skips that prefix. It then removes **at most one** leading
forward slash or backslash. It finds the first pipe with `strchr`; text before
the pipe becomes the explicit archive selector, while text after it becomes
the per-archive name query. Later pipes remain part of that query.

The reviewed preparation does not convert interior backslashes to forward
slashes, collapse dot segments, lowercase the query or strip repeated leading
separators. `BIG:` is not the same prefix. This is a rule for this body, not
a claim that every caller passes unmodified paths.

The selector is matched exactly, case-sensitively, against node+18h's label
after its first colon, or the whole label if no colon exists. This is **not**
a recovered basename/path-extension extraction rule. In particular a drive
colon may leave an entire backslash-prefixed tail; the actual label supplied
by every mounting caller remains outside this audit's scope.

## Scoped list selection

The function starts at manager+10h, follows node+00h and retains the first
successful node. Combined with the earlier reviewed tail append, this gives
first-success order **within this manager's list**. It does not prove that
SKU order equals actual startup mount order, that all providers use this
manager, or that an authored mod archive wins against stock.

Class 2 (Viv4) routes to `00996920`, checking its boolean return. Other classes
route to `00996450`, checking the returned name pointer.

| Query/result | Reviewed continuation |
| --- | --- |
| No selector, ordinary miss | Continue to next node |
| Explicit selector differs from label | Skip node |
| Explicit selector matches, ordinary miss | Return failure immediately |
| Explicit selector matches, Viv4 miss | Continue to next node |
| Any successful lookup | Retain first success; stop further lookup |

Thus an ordinary selected miss is not a fallback search through all archives.
The synthetic test includes a later node with the same selected label and a
supplied hit to ensure that this early failure is preserved. The distinct Viv4
path is modeled as control flow, not validated as a working alternate archive
format. The previously found name-hash collision caveat remains unresolved.

After finding a node, the complete open body may read 16 payload bytes to
inspect header metadata, allocate a 64-byte stream record and publish
offset/length/handle fields. These branches are present in the pinned body,
but the diagnostic does not execute them or claim their complete lifetime,
allocation/read-result safety, codec behavior or compatibility.

## Iterator normalization is separate

`00995620` initializes iterator state, calls `00992D70` to copy its pattern,
converts backslashes to forward slashes and removes all trailing slashes.
The copy helper itself preserves bytes. It does not resolve dot segments or
change case. This normalization is not in the reviewed outer-open preparation
and must not be silently imported into an authored package validation rule.

The detached helpers reject non-printable/non-ASCII input and inputs longer
than 255 characters as a conservative diagnostic policy. This is **not** a
recovered engine length limit or proof of native buffer safety: the native
copy helper has no length bound, and the selector copy length is input-derived.

## Regression and migration boundary

Eight query fixtures cover exact prefix, single leading separator, interior
backslash preservation, dot segments, first-pipe splitting, empty selector
and empty input. Eight supplied-hit selection fixtures cover first/later/no
hits, explicit selector, selector case, selected ordinary failure, Viv4
continuation and the drive-label-tail distinction. Four iterator fixtures,
three policy rejections and sixteen private image/literal faults complete the
detached checks. Every stable nested report field matches a second JSON run.

Prerequisites still cover thirteen available archives and 17,383 entries,
reading 5,390,165 directory metadata bytes per invocation and zero archive
payload bytes. WorldBuilder's large BIN is neither dumped nor decompressed.
The missing configured MapsCampaign.big remains unresolved; its Disabled-named
neighbor is untouched.

Actual startup mount order, global filesystem/provider priority, outer callers'
path normalization, unmount, full native stream lifetime, authentic EP1
ProcessingHash and an authored mod loaded by the game remain open. No hash
bypass, production guard relaxation or installed-file mutation was introduced.
The next useful package investigation is the manager's provider registration
and the real callers supplying mount labels/order.

Effort remains **approximately 52% complete / 48% remaining**. The 165 compiler
test groups were not rerun for this PowerShell-only evidence milestone.
