# RA3 EP1 Texture and PackedTextureImage

ART-included `Texture` and `PackedTextureImage` declarations (for example a mod's packed button images) are emitted into the mod stream.

## Texture (PC)

The official RA3 SDK TextureCompiler (`EmbedPCTexture`) writes a 12-byte root followed by the complete nvDXT DDS file:

| Offset | Field |
| --- | --- |
| 0 | 0 |
| 4 | pointer to +12 (the only relocation) |
| 8 | DDS byte length |
| 12 | DDS file (`DDS ` + 124-byte header + pixels) |

Stock fingerprint: type hash `0x9F5FF8DA`, untokenized. All 1113 stock EP1 textures use this root.

Supported profile (everything else is refused with an explicit message):

- `OutputFormat="A8R8G8B8"`, `GenerateMipMaps="false"`, `Type="StandardTexture"`.
- Source: uncompressed (type 2) or RLE (type 10) 32-bit TGA with 8 alpha bits, up to 4096x4096.
- Pixels are BGRA, rows top-down (bottom-left TGA origin is flipped), DDS header identical to the 153 stock A8R8G8B8/no-mip textures.
- Dimensions are kept as authored (no power-of-two rescale). `AllowAutomaticResize` only affects the official Xenon path.

Qualification on every build that contains a Texture:

1. `Texture:DecalCloudSegment` (stock) has exactly this root, relocation and generated DDS header.
2. The SDK sample TGAs `Portrait_AlliedHarbingerGunship.tga` and `AUA_Harbinger_Cannon.tga` (Working RA3 Compiler for Reference/Art/images) decode to the same visible texels found at their stock `PackedTextureImage` coordinates in `PackedImagesPortraits_004` / `PackedImages_004` (the stock packer only bleeds colour into fully transparent texels).

Preparation admits only `Texture/@File` `.tga` resources, copies each one content-addressed to `resources/<sha256>.tga` beside the prepared source and rewrites `File` to that copy; the compiler re-checks the hash.

## PackedTextureImage

Untokenized 36-byte record (official XmlCompiler marshal): Base@0, Texture import @4, Dimensions@8, Coords@16, TextureDimensions@24, Rotated@32. The existing marshaler is proved with `SchemaNativeContract.VerifyStock` on the three SDK sample images' stock records, then every authored record is proved with `SchemaNativeContract.Verify`.
