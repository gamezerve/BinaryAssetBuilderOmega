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

`alpha/catalogs/ra3ep1/audio.manifest` supplies local sample lookup. New events
add the original `data/audio.manifest` runtime reference to the generated mod
stream. No retail waveform, encoder or game-directory access is required.
The optional audio catalog also participates in missing-asset checks.

The reviewable [Reactor candidate](../tests/fixtures/live-alpha/ReactorAudioCandidate/README.md)
uses a deeper existing reactor loop plus six animation-synchronized spark
accents. It is kept outside user-owned Reborn after an automatic approval
review rejected writing those sources. No user mod package was published.
