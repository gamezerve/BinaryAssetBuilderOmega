# Precompiled DDS texture input

<!-- Reborn: describe the DDS passthrough contract separately from raster conversion capabilities. -->
Use a normal EA Texture declaration in an AssetDeclaration. `File` may name a
DDS beside that XML, a confined relative path or an `ART:` alias. Use Studio's
normal build workflow. Texture-only declarations also compile without adding a
placeholder GameObject or command set.

The compiler validates the existing DDS and embeds its complete unchanged bytes
in the EP1 12-byte Texture root. Format, dimensions and mip levels come from the
file. The ordinary default raster options do not trigger a conversion. Current
DDS formats are DXT1/DXT3/DXT5, A8R8G8B8, X8R8G8B8 and R5G6B5, with existing
mip chains. Legacy complete cube maps and volume layouts are validated too.
DX10 headers, BC6/BC7, palette/luminance and other unimplemented layouts fail
explicitly. The existing TGA encoder remains A8R8G8B8 without mip generation.

If OutputFormat, GenerateMipMaps or Type is explicitly authored, it must agree
with the supplied DDS. A request requiring recompression, mip generation/removal
or resource-kind conversion is an unsupported operation and reports the original
XML field. AllowAutomaticResize does not resize a precompiled DDS. To make such
changes, prepare the desired DDS before compiling. Matching DXT1A/DXT1 uses the
same DDS block format.

Resources are captured with their hash and extension. Two XML files in different
directories can use the same relative DDS filename without sharing the wrong
payload. The primary Texture and supporting Texture declarations use the same
capture/rewrite path. Package readback uses actual generated stream lengths,
including texture streams larger than the old one-MiB command-set fixture.

Validation: 8 synthetic format/mip/kind cases and 12 malformed/unsupported
conversion controls passed. Three Russian-port DDS files (512x512 DXT1,
10 mip levels each) reproduced the original EP1 WorldBuilder Texture records
byte-for-byte. The integration test covers relative filename collisions,
texture-only builds, a stream larger than one MiB, BIG readback and conversion
errors with original XML field coordinates. No game launch is implied.

A private snapshot of the current Reborn entry point also passed source
preparation (37 source files) and mixed compilation (19 explicit assets), with
binary/relocation/import stream readback. User sources and published mod output
were left unchanged.

Developer regression: `scripts/Test-Ra3Ep1DdsTexture.ps1` copies selected sources
to private output and never edits Reborn or publishes to Documents. DDS header,
block/mip sizing and pitch handling follow Microsoft's
[DDS programming guide](https://learn.microsoft.com/en-us/windows/win32/direct3ddds/dx-graphics-dds-pguide).
