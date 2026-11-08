<!-- Reborn: blocked-source follow-up preserves the unchanged baseline and explicitly separates real, synthetic and non-runtime contexts. -->
## 341 engelli dosyanın bağımlılık ve bağlam incelemesi

- Ayrı devam raporu: `alpha/.work/stock-source-coverage/blocked-followup-20261108/followup-report.json`.
- Özgün 7.113 XML ve alpha compiler nesli değişmedi; önceki bağımsız tarama raporu korunuyor.
- **147 dosya ek bağlamda native doğrulandı:** 122 AptUI özgün exportu, 21 dosya gerçek Yuriko haritası tüketicisiyle, 4 AI kişiliği gerçek state kaynaklarıyla.
- **6 sahipsiz parça** açıkça sentetik şema sahibinde derlendi; gerçek kaynak tüketicisi bulunduğu veya bağımsız özgün dosya olarak geçtiği iddia edilmez.
- `AptIncludeProxy_pc.xml`: 122 menüyü listeleyen EA build metadata dosyası doğrulandı; runtime asset değildir.
- **187 dosya hâlâ engelli:** gerçek eksik/yanlış türde bağımlılıklar, bozuk özgün kaynaklar ve kullanılmayan/geçersiz parçalar. Hatalar warning/pass olarak gizlenmedi; dummy eklenmedi.
- 97 özgün katalog (oyunun MapsCampaign/MapsMultiplayer/MapsTutorial BIG içerikleri dahil) ve tüm kaynaklar tarandı. İlk strong engellerdeki 161 isimden yalnızca 17'si doğru türde retail sağlayıcı, 4'ü doğru türde özgün kaynakla eşleşti. Aynı adlı GameObject veya Texture, model/OCL yerine kabul edilmedi.
- Yeniden çalıştırma araçları: `scripts/Audit-Ra3Ep1BlockedSources.py`, `scripts/Test-Ra3Ep1BlockedContexts.py`; orijinal map-consumer testleri mevcut `Test-Ra3Ep1StockSources.ps1` ile özel çıktı klasöründe çalıştı.
- Okunabilir tam sonuç: `docs/UPRISING_SAGEXML_BLOCKED_FOLLOWUP.txt`; kalanların listesi: `remaining-blocked-files.csv`. Compiler davranışı değiştirilmedi; oyun başlatılmadı, kullanıcı modları ve Documents paketleri değiştirilmedi, commit atılmadı.

---

<!-- Reborn: completed exact-generation corpus qualification; unresolved original dependencies remain explicit failures. -->
# Uprising SageXML kapsam taraması — tamamlanan sonuç

**7.113 özgün XML'in tamamı aynı son derleyici, harness ve source-plan nesliyle kontrol edildi. Kaydedilmiş derleyici destek / native doğrulama / test aracı engeli: 0.**

Rapor zamanı (UTC): `2026-11-08T10:52:37.6666864Z`
Proje: `D:\TEMP\BinaryAssetBuilderOmega2-uprising`
Son nesil: `191FCF2ACA1BD9CE35E2B184612A46DEAB839BB4AC76C25E6C22A549A571E42B`

| Sonuç | Dosya |
|---|---:|
| Denenen özgün XML | 7113 |
| Başarılı toplam | 6772 |
| Bağımsız native derleme | 6502 |
| Gerçek kaynak tüketicisi üzerinden doğrulanan parça | 250 |
| Boş kaynak kapanışı, native asset emisyonu yok | 20 |
| Eksik bağımlılık / kaynak yolu | 324 |
| Özgün XML veya şema hatası | 5 |
| Tüketici bağlamı bulunmayan kaynak parçası | 12 |
| Güncel nesilde denenmemiş dosya | 0 |
| Derleyici destek / native / harness / zaman aşımı engeli | 0 |

### Son moladan sonraki 433 dosyanın sonucu

- Kalan **433 dosyanın tamamı** denendi: **400 geçti**, **33** gerçek bağımlılık veya tüketici bağlamı sorununda durdu.
- Bu devamda yeni compiler destek / native doğrulama / harness engeli çıkmadı; compiler veya harness değişikliği gerekmedi. Aynı nesil korundu.
- MasterAudioEvents, SoundEffects, Voice ve Voice_RetailOnly dahil kalan ses dosyaları; StaticGameObjects; Sovyet/system/UI/video son grupları aynı nesille yeniden doğrulandı.
- `Write-Ra3Ep1StockCoverageReport.ps1 -RequireComplete` başarıyla tamamlandı. Alpha ve source build kopyalarının 12 receipt hash'i tekrar doğrulandı.

Eksik bağımlılık, geçersiz XML veya tüketici bağlamı nedeniyle **341 dosya derlenemedi**; bunlar başarıya çevrilmedi. Dosya bazında bütün açıklamalar `alpha/.work/stock-source-coverage/coverage-failures.csv` ve `coverage-current-report.json` içindedir.

### Bu devamda tamamlanan destek

- Sayısal TerrainTextureAtlas kimlikleri, türe göre kalıtım/AI state kimlikleri, standalone LogicCommand, ImageSequence/StanceTemplate; DynamicsDraw dolu animasyon metadata örneği; düşük LOD particle Priority değerleri.
- 13 PathMusic geçiş/durum/koşul kökü; geniş özgün sistem ve UI ayar kökleri; GiftManagerSettings/ResourceSettings ve EP1 HeightDieUpdate.
- **205 açık kök ailesi bağımsız son kontrolde geçti:** 198 gerçek özgün asset verisi; 7 ayrı etiketlenmiş OriginalRuntimeSchemaAbi (BootupDisplaySequence, TheVersion, üç ConquerCardDefinition türevi, SiegeEvaEvent, DLContent).
- Gerçek global/locale akışından FontSubstitution 20 bayt/5 native örnek; işlenmiş AmbientStream/MusicTrack/DialogEvent 132 bayt; AudioSettings ve bütün UIConfigList grafı (71 concrete component).
- UI component class word'leri yalnızca özel writer kopyasına ekleniyor; GameTimeSetting.MinGameTimeValue, Movie Archive başlangıç sözcüğü, AptMediator eşittir literali, DefaultValues EP1 alanları, tam imzalı enum/list marshal seçimi düzeltildi.
- **32 ek davranış ailesi** bağımsız kontrolde geçti: 31 gerçek özgün native kayıt; DistributeEnergyToAlliesBehavior ise özgün PC ABI + gerçek Capture tabanı. Sandıklar, bombardıman, unified capture, enerji/vision/squeeze-fling/gather-eject/stored-draw aileleri tamamlandı.
- Havana StructureOrHarvesterUnderAttackHeuristic PC ABI'siyle eklendi; diğer 19 heuristic gerçek global native örneklerini korur.
- HealthBarTemplate/HealthBarSettings instance import yolu; standalone ObjectCreationList ve expression/Flags içeren gerçek özgün OCL tanığı; ayarlara ait polymorphic FX shader constant word'leri tamamlandı.
- GameObject içindeki şemaca boş ToppleUpdate benzeri düğümlerde yalnızca girinti normalizasyonu; gerçek yazı ve geçersiz içerik korunup reddediliyor. 2 pozitif + 2 negatif formatting denemesi geçti.
- AudioEvent normal yolda SEQUENTIAL/ALLOW_KILL_MID_FILE/FADE_ON_START ve tekrarlanan ağırlıklı dosya seçimlerini koruyor. Normal yolda 32 dosyalık eski tanı sınırı kaldırıldı; gerçek Impact_BulletVsGeneric 121 kayıt içeriyor. Dört tam özgün global native tanık hızlı modda da zorunlu. Eski izole tanı profillerinin kısıtları korunuyor.
- Sounds/MasterAudioEvents.xml, SoundEffects.xml, Voice.xml, Voice_RetailOnly.xml odak testlerinde geçti. Video/Videos.xml'in mevcut desteği için eksik instance/kalıtım yolu tamamlandı ve dosya geçti.
- SiegeEvaEvents tam kaynak testi geçti. ConquerMode artık destek hatasında değil, Uprising'de bulunmayan GameObject:GDIRifleSoldierSquad bağımlılığında duruyor. DLContent paketinde eksik MapMetaData.xml include'u var; örtülmedi.
- Production direct-instance görünürlüğü kaynak sırasındaki son tanımı seçer; aynı fiziksel kaynakta gerçek duplicate reddi korunur. Source plan precompiled reference kenarlarını tüketici saymaz.
- Gerçek boş kapanışlar ayrı SourceOnlyQualified yolundan geçer; native asset üretildiği iddia edilmez.




### Son sürümle bağımsız kontroller

- 205/205 kök ailesi: **198 gerçek özgün native asset verisi**, **7 ayrı OriginalRuntimeSchemaAbi**. Motor ABI kanıtı özgün asset-byte roundtrip diye gösterilmiyor.
- 32/32 ek davranış ailesi: 31 gerçek özgün native kayıt; enerji paylaşımı PC ABI + gerçek Capture tabanı.
- Yeni eski-tür yazıcıları için sıfır olmayan sayılar, liste stride'ları, strong import index+1, string relocation, zaman dönüşümü, default ve DLC patch byte denemeleri geçti.
- Girinti normalizasyonu için 2 pozitif + 2 negatif deneme geçti; gerçek metin korunur.
- Eski izole ses profili ve native ses regresyonları geçti; izole 32/33 sınırı korunurken normal authored kaynaklarda uzun/ağırlıklı/tekrarlı listeler destekleniyor.
- 122/122 özgün UI exportu ayrı kopyalarda derlendi. Bu sonuç, SDK'da olmayan eski aptbuildxml yollarının sağlandığı iddiası değildir ve ana native geçiş sayısına eklenmez.
- Alpha compiler toolchain'i güncellendi. Commit atılmadı; oyun başlatılmadı; kullanıcı modları, Documents paketleri, oyun/profil verileri, özgün SageXML ve şemalar değiştirilmedi.

### Kanıt ve yeniden çalıştırma

- `scripts/Test-Ra3Ep1StockSources.ps1 -ReuseProcess`: dosyaları sırayla gerçek production kaynak/native yolunda dener. Eski compiler/harness/source-plan sonuçlarını güncel geçişe katmaz.
- `scripts/Write-Ra3Ep1StockCoverageReport.ps1 -RequireComplete`: bütün 7.113 dosya ve sıfır destek/harness engeli koşulunu doğrular.
- Ana log: `alpha/.work/stock-source-coverage/final-stable-corpus-20261108.log`.
- Son 433 dosyanın devam logu: `D:\TEMP\BinaryAssetBuilderOmega2-uprising\alpha\.work\stock-source-coverage\resume-final-stable-corpus-after-pause-20261108-101613.log`.
- Son rapor: `alpha/.work/stock-source-coverage/coverage-current-report.json`.
- Doğrudan kaynak kökleri matrisi: `coverage-root-matrix.csv` (255 özgün kök türü); tek dosya birden fazla tür içerebilir.
- Ayrıntılı denemeler: `results.jsonl`; her dosyanın `cases/NNNNN` günlükleri ve sonuç/hash kaydı.
- Bağımsız kanıtlar: `root-native-proof-final-stable-20261108.json`, `behavior-native-proof-final-stable-20261108.json`, `independent-proof-final-stable-fence.json`.
- UI exportları: `apt-mirror-native-20261108/report.json`.
- PC ABI kanıt kaynağı: `docs/evidence/uprising-pc-schema-abi.json`; özgün executable SHA256 + gerçek code segmentleri/field hedefleri/type registry konumları.

### Kapsamın sınırı

Bu çalışma özgün SageXML dosyalarının derleme taramasıdır. Eksik model/audio/header/aptbuildxml dosyaları, sonda noktalı include yolları, bozuk özgün XML'ler ve uninstantiated Templates/Squad.xml gibi doldurulmamış/geçersiz iskeletler ayrıca listelenmiştir. Şemadaki henüz kullanılmamış her kombinasyonun veya oyun içi davranışların tamamının doğrulandığı iddia edilmez. Özellikle geçersiz Squad iskeletindeki HordeContain/HordeAIUpdate tam native geçiş sayılmadı.

---
## Tarihsel mola notu — aşağıdaki sayılar son sonuç değildir

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
