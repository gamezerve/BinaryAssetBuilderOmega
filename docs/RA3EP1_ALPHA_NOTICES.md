# Candidate notice preservation — October 11, 2026

## Outcome

Overlay packaging now requires an explicit `-DotNetNoticeDirectory` containing
nonempty LICENSE.txt and ThirdPartyNotices.txt, each at most 1 MiB and with no
redirected ancestry. Missing notice files refuse before the overlay output or
ZIP is created. Notice bytes are copied unchanged to notices/dotnet, pinned in
the relative manifest and verified alongside the compiler/input inventory.

The upstream project LICENSE remains unchanged. `alpha/THIRD_PARTY_NOTICES.md`
adds the observed Qibbi/Jana Mohn/source lineage, project compiler inventory,
external EA/schema boundary and unresolved host/fixture/generated-code review.
This is an attribution inventory, not a new license or legal clearance.

The currently supplied notices come from the installed dotnet root. Their
presence does not prove exact notice coverage for the host pack that supplied
the apphost. The manifest explicitly keeps ExactHostNoticeCoverageReviewed and
PublicRedistributionReviewed false. No broad .NET runtime, PowerShell or EA
component is bundled by this step; the native apphost was already part of the
compiler allowlist. No downloaded license text or substituted summary is used.

## Evidence

New candidate: `UprisingOverlay-20261011-d` (26 payload files, eight external
requirements, plus relative manifest). ZIP SHA256:
`D55B35EA2FD3EA83D25196E50CB50B49262EC692CF5E33EB88279BAC4CE8889F`.

Source/copy SHA256 equality:

- Installed .NET LICENSE.txt:
  `7F6839A61CE892B79C6549E2DC5A81FDBD240A0B260F8881216B45B7FDA8B45D`.
- Installed ThirdPartyNotices.txt:
  `E8DDD95AC905467E4E519B446CA023FA00C31732BEBD9B66E65101C712092EE7`.

Fresh setup `UprisingOverlay-test-d` passes notice inventory hashes, external-file
exclusion, path-free manifest, mismatched dependency refusal, occupied refusal,
GUN-5000/GUN-250 builds and DTD/critical-marker refusal. Missing notices refused
before creating `UprisingOverlay-notice-refusal` or its archive. Relocation to
`UprisingOverlay-moved-e` passes exact backup, original-config/output preservation,
missing/duplicate-key refusals and repeated offline builds with unchanged BIG
digests. No new GUI/game run or full compiler-suite claim is made.

The candidate remains local and unuploaded. The source scripts, attribution
summary and documentation are published only to the user's repository; installed
vendor notice copies are not added to Git by this step. Local-use alpha testing
can continue independently of the final public release review.
