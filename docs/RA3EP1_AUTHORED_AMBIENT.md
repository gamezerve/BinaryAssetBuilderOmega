# Authored ambient events

<!-- Reborn: bridge the checked EP1 audio writer into alpha authoring while keeping retail sample payloads external. -->
Normal alpha preparation and mixed streams now admit AudioEvent declarations.
The existing checked AudioEvent writer explicitly opts into AMBIENT and
FADE_ON_START for this path; legacy diagnostic profiles retain their old
admission rules. Existing unsupported AudioEvent structures still fail clearly.

The complete original `SOV_Reactor_AmbientLoop` is independently reproduced
against retail global BIN data before authored events compile. This includes
BaseSoundEffect's inherited values, 14 semantic AudioFile imports, the AMBIENT
slider and start/kill fade flags. Source metadata is revalidated after Core
normalization and attachment; current schema types and imports remain required.

`alpha/catalogs/ra3ep1/audio.manifest` supplies local sample lookup. The current
compatibility output uses `data/audio.manifest` in the runtime reference table.
An attempted change to bare runtime names caused a match-audio regression and
was rolled back; the final runtime dependency policy is still unresolved.
Native/readback qualification does not prove audible playback.
No retail waveform, encoder or game-directory access is required for compilation.
The optional audio catalog also participates in missing-asset checks.

The reviewable [Reactor candidate](../tests/fixtures/live-alpha/ReactorAudioCandidate/README.md)
uses a deeper existing reactor loop plus six animation-synchronized spark
accents. It is kept outside user-owned Reborn after an automatic approval
review rejected writing those sources. No user mod package was published.
