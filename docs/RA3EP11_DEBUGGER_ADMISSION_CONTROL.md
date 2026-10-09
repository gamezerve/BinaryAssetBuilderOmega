# Reborn: initial-break helper admission

Date: October 10, 2026 (Europe/Istanbul). No game executed.

## Implemented behavior

`Invoke-Ra3Ep11DebuggerAdmissionControl.ps1` compiles the fixed, hash-reviewed
`DebuggerLifetimeControl.cs` into a fresh ignored directory and starts only
that executable under the pinned installed x86 CDB. There is no target-path,
PID, attach, server, child-follow or game argument parameter.

Unlike the lifetime control, its initial command file emits an owner PID and
wait marker **without `g` or `qd`**. The supervisor owns redirected input/output
and pumps bounded chunks without blocking on a console prompt. It retains the
helper process handle and verifies the PID, executable path, direct CDB parent
and creation window before permitting observation continuation.

At that initial break, the helper's Main marker must be absent. Its entry RVA
is uniquely mapped through the helper PE32 section table, then addressed at
the actual live module base. A read-only Windows memory read must succeed,
transfer all six requested bytes and exactly match the stage-specific helper
entry thunk. The genuine read is validated before any injected mismatch.
This is six-byte entry evidence, not a whole-memory image hash or a game
function-pin/relocation verifier.

## Runtime controls

All three modes passed, then passed again with the expanded evidence records:

| Mode | Latest helper PID | Observation `g` | Cleanup `qd` | Outcome |
| --- | --- | --- | --- | --- |
| Admit | 25276 | Yes, after ownership and entry admission | No | Natural exit 0 |
| RejectLiveBytes | 12064 | No | Yes | Private expected-byte mutation refused; natural exit 0 |
| Timeout | 34360 | No | Yes | Admission withheld for two seconds; Main marker remained absent; natural exit 0 |

Local evidence directories under `artifacts/`:

- `RebornDebuggerAdmission-d6840249fe4f49729007f0a4317b6ca1`
- `RebornDebuggerAdmission-8395936fa5124879823048be8fa17587`
- `RebornDebuggerAdmission-4490d3a4c7864728aa30dcfed0f39f3d`

Each stores `result.json`, bounded output snapshots, the reviewed generated
initial command file and compiler diagnostics. New repetitions produce fresh
identities and directories. Four detached positives and sixteen refusal cases
also pass: owner/wait completeness and order, duplicates, unknown records,
diagnostic errors, output bound, read success/count/bytes and malformed helper
PE/opcodes. Synthetic preferred/nonpreferred bases confirm RVA address mapping
without assuming thunk operand fixup. Compiler groups remain 165, not rerun.

The final genuine-read-before-injection guard also passed a further refusal
control (PID 53116) in
`RebornDebuggerAdmission-5a243169c215425f9980b29dc553ee5a`: actual six-byte read
success, private expected-byte mismatch rejected, no observation `g`, cleanup
detach and natural helper completion. The existing lifetime policy's two
detached positives and twelve refusals passed as a regression check too.

## Why the first attempts refused admission

The first two attempts expected the thunk's absolute operand to be adjusted
by the live-base delta. Actual initial-break reads returned the unchanged
disk bytes `FF 25 00 20 40 00`, even though the module base differed from
`00400000`. Both attempts refused admission and sent no observation `g`.
Their failure artifacts remain local:

- `RebornDebuggerAdmission-99910d4f50bc472e99296b10b5966783`
- `RebornDebuggerAdmission-e80eafffb8244f03a2b274eec9c7ad13`

The current policy requires **only the raw bytes** for this fixed managed
helper at this specific initial-break stage; it does not accept either variant
interchangeably. The live entry address still uses the actual module base.
This observation does not establish why CLR/loader fixups differ at that stage,
nor establish that native Uprising code has the same relocation behavior.
`GameRelocationValidated` and `GameRecipeReady` remain false.

## Rejection is not execution containment

Admission guards whether the supervisor sends observation `g`. Rejection and
the withheld-admission control deliberately detach with `qd` to avoid leaving
a suspended helper behind. **Detachment releases the helper to execute.** On
unexpected observation failure, termination of only the owned CDB with `-pd`
can also release it; the result makes no completion-success claim in that case.
This is not a policy that a rejected target never runs, nor a sandbox. The
loader already executed before CDB's initial break; absence of the Main marker
is scoped helper evidence, not proof of zero prior instructions or side effects.

Compilation, initial-wait acquisition and completion each have deadlines;
output is capped per stream. `Timeout` simulates deliberately withholding an
otherwise validated admission; it is not injection of all OS timeout/cleanup
faults. Source/debugger identity and ancestry are preflight checks, not atomic
protection against hostile concurrent filesystem replacement. No memory writes,
breakpoints, installation changes, registry changes or system-wide tracing occur.

The existing x86 debugger/source pins are shared with the
[lifetime calibration](RA3EP11_DEBUGGER_LIFETIME_CONTROL.md). Initial-break options
are documented in [CDB command-line options](https://learn.microsoft.com/en-us/windows-hardware/drivers/debugger/cdb-command-line-options);
actual memory-transfer success/count semantics are documented in
[ReadProcessMemory](https://learn.microsoft.com/en-us/windows/win32/api/memoryapi/nf-memoryapi-readprocessmemory).

## Next gate

Adapt the admission/supervision mechanism to the separately pinned native
image, with correct live code/vtable verification and relocation/stage policy,
before installing the narrow config-reader observations. A direct-native
debug launch differs from the verified launcher baseline and needs separate
explicit game-debug authorization including cleanup that may resume the game.
Do not copy this CLR thunk policy into Uprising or enable descendant debugging.
Actual config read, parser consumption and authored mod loading remain unproved.
Overall estimate remains approximately **52% complete / 48% remaining**.
