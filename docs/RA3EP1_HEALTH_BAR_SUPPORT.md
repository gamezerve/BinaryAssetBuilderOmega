<!-- Reborn: document independently qualified EP1 health-bar serialization and its remaining runtime boundary. -->
# Health-bar compiler support

`HealthBarTemplate` and `HealthBarSettings` now use the original EP1 record layout:

- Template: 8 bytes, `Length` at +4.
- Settings: 16-byte header, strong `DefaultHealthBar` import at +4, mapping list at +8.
- Mapping: 124 bytes, strong `HealthBar` import at +0, inline `ObjectFilter` at +4.

The previous implementation reversed the settings header fields and wrote weak instance hashes instead of strong import selectors. Consequently the settings had references but no IMP records. Matching manifest type hashes cannot detect either defect; a runtime reader can interpret the malformed fields as pointers.

Every authored health-bar asset now passes the source/schema/native contract before stream publication, including nested filter storage and the complete RELO/IMP inventory. These checks also run in fast builds.

Validation on 2026-11-01:

- Six retail templates and the retail settings reconstructed from bounded static-stream slices: 21 byte-identical BIN/RELO/IMP comparisons and matching reference occurrence tables. Retail settings: 828/36/28 bytes, six imports (including the repeated Medium reference).
- A private snapshot of the user's seven health-bar assets compiled through the early global route: settings 952/36/32 bytes, seven resolved local reference occurrences, stream readback verified.

The original health-bar assets reside in the retail **static** stream. The private global build proves serialization/linking, not in-game singleton loading or the absence of unrelated crash causes. Rebuild the mod in EALAModStudio and test a fresh match before claiming the crash fixed. User XML, published mod outputs and game installation were not modified by these checks.

Developer regression: `healthbar-self-test <static.manifest> <global.manifest> <fresh-absolute-output-directory>`; requires the repository-local WorldBuilder catalog to recover names used by retail weak object filters.
