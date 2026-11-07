<!-- Reborn: developer handoff for the paused sequential original-source qualification; no completion claim. -->
# Orijinal Uprising SageXML derleme taraması — mola notu

Son kayıt (UTC): `2026-11-07T17:24:10.884584+00:00`  
Proje: `D:\TEMP\BinaryAssetBuilderOmega2-uprising`  
Durum: **Kullanıcının isteğiyle mola verildi. Tam kapsama çalışması bitmedi.**

## Amaç ve çalışma sınırları

`alpha/SageXML` altındaki orijinal XML dosyalarını sıralı şekilde gerçek kaynak hazırlama ve native derleme yolundan geçirmek; eksik alanları şema ve orijinal oyun verileriyle karşılaştırarak eklemek.

- Orijinal SageXML ve şemalar değiştirilmedi.
- Kullanıcının mod kaynaklarına, özellikle `alpha/mods/Reborn` içine dokunulmadı.
- Oyun başlatılmadı, Documents içine test paketi yayımlanmadı, commit atılmadı.
- Denemeler `alpha/.work` altında tutuldu.
- Eksik kaynaklar veya geçersiz orijinal dosyalar geçerli sayılmadı; destek engellerinden ayrı kaydedildi.

## Sayısal durum

| Ölçüm | Sayı |
|---|---:|
| Envanterdeki orijinal XML | 7113 |
| En az bir kez denenmiş ayrı dosya | 4180 |
| Henüz denenmemiş dosya | 2933 |
| Dosya başına son tarihsel sonucu Passed | 3682 |
| Dosya başına son tarihsel sonucu Failed | 498 |
| Son alpha derleyici sürümüyle yeniden denenmiş dosya | 44 |
| Bu sürümde Passed | 37 |
| Bu sürümde Failed | 7 |

**Tarihsel Passed sayısı, bütün dosyaların son derleyiciyle geçtiği anlamına gelmez.** Derleyici, test yöntemi veya kaynak kapanışı değişince önceki geçişler nihai kanıt olarak kullanılmıyor. Kalan alanlar tamamlandıktan sonra bütün 7.113 dosya güncel sürümle yeniden taranmalı.

Kaynak bağlamı envanteri: 255 kaynak parçası için gerçek tüketici bulundu; 15 parça için tüketici bulunamadı. Bir parça tüketicisi üzerinden geçtiyse `ContextQualified`, bağımsız native derleme geçtiyse `NativeQualified` kaydediliyor.

## Mola öncesi bitirilen son adım

- `AIPersonalityDefinition`: orijinal 520 baytlık global başlık, kişilik alanları, listeler, state bağlantıları ve heuristic seçenekleri eklendi. Temel/varsayılan kişilikler ve Commander's Challenge kişilikleri test edildi. Rakamla başlayan geçerli kişilik kimlikleri kabul ediliyor.
- `AIMicroManagerData`: orijinal 176 baytlık başlık; 49 ayar, hedef tercihleri, özel güç gecikme listesi, retreat kriteri ve hedef filtresi eklendi. Ana orijinal mikro yönetici kitaplığı test edildi. Boş olmayan üç özel güç gecikme kaydı ayrıca orijinal global native veriyle doğrulanıyor.
- `FXParticleDrawLightSource`: 8 baytlık gerçek EP1 ışık kaydı ve `MaximumIntensityRadiusRatio` desteği eklendi. Boş ve dolu orijinal örnekler native veriye karşı doğrulanıyor.
- `Wind.Motion`: orijinal `PING_PONG=2` eşlemesi için bağımsız doğrulamadaki yanlış INVALID=-1 varsayımı düzeltildi.
- GPU `GeometryType`: `TWO_QUADS=4` tamamlandı.
- Parçacık dönüş modu: EP1'e ait `ROTATE_AROUND_VELOCITY=2` ve `ROTATE_IN_WORLD_SPACE=3` kullanılıyor.
- `Wavefront`: 68 baytlık hız kaydı, beş inline rastgele değişken, `WAVE_PARTICLE=9` ve şekil eşlemeleri daha önce eklenip orijinal örneklerle doğrulandı; son odak testinde de tekrar kontrol edildi.

Bu alanların bağımsız orijinal native kontrolleri hızlı derleme modunda da zorunlu. Uyarı denetimlerinin kapalı olması native/source/metadata/stream doğrulamalarını kapatmıyor. Derleme testleri oyun içi davranış testi yerine geçmez.

## Bu taramada daha önce tamamlanan başlıca gruplar

- AI bütçe, yapı ve stratejik state kökleri; kaynak/native eşleşmesi bulunan 19 heuristic türü; tactic ve team kayıtları.
- UI giriş kökleri: hotkey, mappable key, mouse cursor; quick chat, EVA, formation, font ve renk tanımları.
- `MiscAudio`, `EmotionTemplate`, `RegisteredObjectSet`, `PlayerTechRank`, `RadiusCursor`, shader override ve ek global/static kökler.
- Crate collide, `DamageSphereUpdate`, `EncloseBehavior`, `ArmorUpgrade`, `LineSegmentDamageUpdate`, `SpawnedSlaveUpdate` ve önceki modül eksikleri.
- DynamicsDraw hacim/ömür alanları ve sadece bağımsız orijinal kanıt için kullanılan WorldBuilder katalog çözümlemesi.

Detaylı doğrulama kayıtları sonuç günlüğünde ve ilgili source/native sözleşmelerinde bulunuyor. Bu liste tüm SageXML desteğinin bittiği iddiası değildir.

## Son sürümde kalan odak test engelleri

- `EP1\SkirmishAI\AIMicroManagerLibrary_EP1.xml` — ERROR: D:\TEMP\BinaryAssetBuilderOmega2-uprising\alpha\.work\stock-source-coverage\cases\03893\work-2b9313a315244c2ab42e7d97dd688b35\source\game.xml(1,2) [source context; exact field origin unavailable]: Unsupported mixed asset type or invalid identity: TerrainTextureAtlas:360_DLC_TestMap
- `SkirmishAI\Personalities\AIPersonalityLibrary.xml` — ERROR: D:\TEMP\BinaryAssetBuilderOmega2-uprising\alpha\SageXML\SkirmishAI\Personalities\AIPersonalityLibrary.xml(89,25): Source graph: IncludePath DATA:maps\official\CAMP_S06_Iceland_Bass\AIP_S06_SovietKrukov.xml.: Unsafe scoped relative dependency path.
- `SkirmishAI\Personalities\AlliedFirstAssault.xml` — ERROR: D:\TEMP\BinaryAssetBuilderOmega2-uprising\alpha\.work\stock-source-coverage\cases\06764\work-450124492cc740edb0891c081d4ab217\source\game.xml(1,2) [source context; exact field origin unavailable]: Behavior strong target missing/ambiguous: AIStrategicStateDefinition:FirstAssault_TankRush
- `SkirmishAI\Personalities\AlliedPCGDemo.xml` — ERROR: D:\TEMP\BinaryAssetBuilderOmega2-uprising\alpha\.work\stock-source-coverage\cases\06765\work-32bfcdfd96824b4aa1d47d3b39365baf\source\game.xml(1,2) [source context; exact field origin unavailable]: Behavior strong target missing/ambiguous: AIStrategicStateDefinition:PCGDemo_ReactiveDefense
- `SkirmishAI\Personalities\JapanAmbushDivision.xml` — ERROR: D:\TEMP\BinaryAssetBuilderOmega2-uprising\alpha\.work\stock-source-coverage\cases\06773\work-6e76928488ae45afb92c6ebd8a7c3d25\source\game.xml(1,2) [source context; exact field origin unavailable]: Behavior strong target missing/ambiguous: AIStrategicStateDefinition:AmbushDivision_AmphibiousVehicleAttack
- `SkirmishAI\Personalities\SovietHeavyArmor.xml` — ERROR: D:\TEMP\BinaryAssetBuilderOmega2-uprising\alpha\.work\stock-source-coverage\cases\06788\work-27f1cc76a76a412f9162ea3a6cb36370\source\game.xml(1,2) [source context; exact field origin unavailable]: Behavior strong target missing/ambiguous: AIStrategicStateDefinition:HeavyArmor_GrowingAttacks_Land
- `SkirmishAI\Personalities\TestPersonalities.xml` — ERROR: D:\TEMP\BinaryAssetBuilderOmega2-uprising\alpha\SageXML\SkirmishAI\Personalities\TestPersonalities.xml(15,5): Instance overlay exceeds chain merge scope: Invalid GameObject base identity.

Özellikle `EP1/SkirmishAI/AIMicroManagerLibrary_EP1.xml` artık mikro yönetici yazıcısında durmuyor. Kapanışında gelen `TerrainTextureAtlas:360_DLC_TestMap` kimliğinin rakamla başlaması nedeniyle duruyor. Bu sonraki ayrı destek/kimlik adımıdır; mola sırasında değiştirilmedi.

Ana kişilik kitaplığında `AIP_S06_SovietKrukov.xml.` gibi sonda noktalı include yolları, PCG demo kişiliğinde ise stock katalogda bulunmayan hedefler var. Bunlar orijinal kaynaklara dokunulmadan hata olarak korunuyor.

Diğer tarihsel başarısızlıklar güncel sürümle yeniden kontrol edilmeden hâlâ açık/çözülmüş kabul edilmemeli. Örneğin eksik art kaynakları, eski APT geliştirici dosyaları, tüketicisi olmayan parçalar ve bazı geçersiz orijinal XML'ler bulunuyor.

Tarihsel son başarısızlık sınıfları (eski derleyici sonuçları içerebilir):

- `CompilerSupport`: 74
- `Dependency`: 403
- `NativeOrHarness`: 16
- `SourceContextRequired`: 3
- `SourceOrSchema`: 2

## Alpha güncellemesi

- Son Release/x86 derlemesi başarılı: **0 hata**.
- SDK otomatik eşitlemesi tamamlandı.
- `alpha/toolchain.json` içindeki 12 derleyici/yardımcı dosyanın SHA-256 değerleri yerindeki dosyalarla karşılaştırıldı: **uyumsuzluk yok**.
- Eşitleme zamanı: `2026-11-07T17:21:24.6176449Z`.
- Son test derleyici nesli: `9AC8E1790C6C4306878F72CB0146031BB1E7737E63695F28A19B591EABE6D098`.
- Kullanıcı arayüzü girişi aynı: `alpha/EALAModStudio.exe`.

## Devam noktası ve kayıtlar

**Ana sıralı taramanın son dosyası:** `FXParticleSystems\AlliedBomb_Trail02Mushroom.xml`.

Mola öncesi son odak testleri, bu noktadan sonraki AI dosyalarını da denediği için `progress.json/Current` ana sıralı devam noktası değildir. Sıralı taramaya devam ederken yukarıdaki sınır kullanılmalı.

1. `.work/stock-source-coverage/stop.request` mola işaretini kaldır.
2. Ana sıralı taramayı yukarıdaki FX dosyasından sonra sürdür; her dosyanın gerçek include kapanışını koru.
3. Eksik alanları orijinal kaynak ve native kayıtlarla doğrulayarak tamamla; kaynak eksiklerini gizleme.
4. Sonunda başlangıçtan bütün envanteri son derleyiciyle yeniden tara. Eski nesil geçişleri son kapsam raporuna taşıma.

Bunlar geliştiriciye ait devam notlarıdır; kullanıcı için alternatif bir derleme akışı değildir.

- Test sürücüsü: `scripts/Test-Ra3Ep1StockSources.ps1`
- Envanter: `alpha/.work/stock-source-coverage/inventory.json`
- Parça/tüketici planı: `alpha/.work/stock-source-coverage/source-plan.json`
- Bütün denemeler: `alpha/.work/stock-source-coverage/results.jsonl`
- Son durum: `alpha/.work/stock-source-coverage/progress.json`
- Dosya bazlı hata/native günlükleri: `alpha/.work/stock-source-coverage/cases/NNNNN/`
- Açık devam bilgisi: `alpha/.work/stock-source-coverage/pause-checkpoint.json`

Mola sırasında hiçbir test veya derleme arka planda bırakılmadı.
