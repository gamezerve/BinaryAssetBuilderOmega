# GameObject display names and root text

<!-- Reborn: remove stock-key equality without conflating binary text headers with localization-table compilation. -->
Use the existing `alpha/EALAModStudio.exe` build workflow. Within a currently
supported GameObject graph, these optional EA child elements are source-driven:

```xml
<!-- Reborn: author new localization keys using the standard EA XML shape. -->
<DisplayName>Name:RebornConscript</DisplayName>
<DisplayNameTransformed>Name:RebornConscriptTransformed</DisplayNameTransformed>
```

Either element may be added, changed, omitted or explicitly empty. Keep the
official schema sequence, before ObjectResourceInfo/Draws/Behaviors. Root
Description/TypeDescription and their transformed attributes remain editable;
these changes now compose with variable Behaviors rather than selecting mutually
exclusive trial profiles. XML escapes are decoded normally.

The original restriction was not a localization-key lookup failure. The source
comparison required stock child text, while the original-object metadata route
also required DisplayNameTransformed to be null. The lower native marshaler and
pointed-name metadata support already existed. The new source-bound contract
checks actual key bytes and header/text ownership; null metadata is emitted only
for omitted names, without duplicating populated name rows.

Keys in these native wire fields are non-NUL ASCII. Translated UI text belongs
in a separate localization table; this change does not implement its compiler,
invent translations or prove the UI appearance of a newly named key in-game.
No fixed old key spelling or old allocation length is required. Native bounds,
reference resolution, schema errors and actual unimplemented structures remain
checked; this does not enable arbitrary new module families or art/audio.
The former 4096-byte inline root-string quota is also removed. A 4097-character
description now passes actual fixed-route native and metadata rebasing rather
than serving as a negative admission fixture; overall source/graph bounds remain.

Regression cases cover both names with longer XML-escaped keys, both explicit
empty values and both omitted, combined with inline root-string edits. They
compile the complete GameObject into manifest/BIN/RELO/IMP streams and BIGs;
wrong content, lengths, pointers and metadata targets are rejected independently.
All three complete stream/BIG cases pass, rejecting 27 native and 36 metadata
corruptions, with 584 stream/dependency/profile refusals per case. The unchanged
source-diagnostics suite also passes all 22 checks; its former valid-key rejection
fixture is replaced by an actual unsupported wire-encoding input.
The developer-slot suite additionally combines changed keys with a new object
ID, explicit recruitment command, retargeted weapon, changed Behavior count and
DATA includes/definitions/inheritance. User Reborn files are not edited and no
game is launched by these checks.
