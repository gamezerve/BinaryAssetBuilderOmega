# Reborn: reconcile new contextual evidence with the immutable 7,113-file baseline without relabeling missing originals or synthetic consumers as native passes.
from pathlib import Path
import argparse
import collections
import csv
import hashlib
import json
import re
import xml.etree.ElementTree as ET


# Reborn: every evidence reference records the exact bytes checked, not merely the existence of an output filename.
def sha(path):
    with path.open('rb') as stream:
        return hashlib.file_digest(stream, 'sha256').hexdigest().upper()


# Reborn: bounded CSV fields retain full original dependency diagnostics; the summary never truncates the stored evidence.
def load_json(path):
    return json.loads(path.read_text(encoding='utf-8-sig'))


# Reborn: validate current sources, native proofs and real-consumer origins before publishing a separate blocked-source follow-up.
def main():
    parser = argparse.ArgumentParser()
    parser.add_argument('--repository', required=True)
    parser.add_argument('--suite', required=True)
    args = parser.parse_args()
    repo, suite = Path(args.repository).resolve(), Path(args.suite).resolve()
    sdk = repo / 'alpha'
    source = sdk / 'SageXML'
    base = sdk / '.work/stock-source-coverage'
    suite.relative_to(base)
    csv.field_size_limit(64 * 1024 * 1024)
    with (base / 'coverage-failures.csv').open(encoding='utf-8-sig', newline='') as stream:
        originals = list(csv.DictReader(stream))
    baseline = load_json(base / 'coverage-current-report.json')
    if len(originals) != 341 or baseline['Failed'] != 341:
        raise ValueError('Unexpected baseline failure inventory')
    inventory = load_json(base / 'inventory.json')
    if len(inventory) != 7113:
        raise ValueError('Unexpected original corpus')
    for item in inventory:
        if sha(source / item['Relative']) != item['Sha256']:
            raise ValueError('Original source changed: ' + item['Relative'])
    compiler = sdk / 'tools/compiler'
    fingerprints = [sha(compiler / name) for name in ['BinaryAssetBuilder.ManifestInspector.dll', 'BinaryAssetBuilder.XmlCompiler.dll', 'BinaryAssetBuilder.W3XCompiler.dll', 'BinaryAssetBuilder.Core.dll', 'SageBinaryData.dll']]
    fingerprints.append(sha(repo / 'scripts/Test-Ra3Ep1StockSources.ps1'))
    generation = hashlib.sha256(':'.join(fingerprints).encode()).hexdigest().upper()
    if generation != baseline['CompilerGeneration']:
        raise ValueError('Compiler no longer matches the baseline generation')
    receipt = load_json(sdk / 'toolchain.json')
    # Reborn: preserve the portable alpha toolchain's established receipt shape across SDK versions.
    file_rows = receipt.get('Files', receipt.get('files', receipt.get('CompilerFiles', [])))
    if isinstance(file_rows, dict):
        file_rows = [{'Name': k, 'Sha256': v} for k, v in file_rows.items()]
    if not file_rows:
        raise ValueError('Toolchain receipt has no verified file inventory')
    for item in file_rows:
        name = item.get('Name', item.get('name'))
        digest = item.get('Sha256', item.get('sha256'))
        if sha(compiler / name) != digest:
            raise ValueError('Alpha compiler receipt changed: ' + name)
    results = {row['OriginalSource']: {'OriginalSource': row['OriginalSource'], 'OriginalSha256': sha(source / row['OriginalSource']), 'BaselineClass': row['Class'], 'BaselineStage': row['Stage'], 'Status': 'BlockedDependency', 'Message': row['Message'].strip(), 'Evidence': []} for row in originals}
    targets = load_json(suite / 'dependency-targets.json')
    for target in targets:
        for name in target['Users']:
            results[name]['RequiredTypes'] = target['RequiredTypes']
            results[name]['Target'] = target['Name']
            results[name]['CompatibleRetailMatches'] = target['CompatibleRetailMatches']
            results[name]['CompatibleSourceMatches'] = target['CompatibleSourceMatches']
            results[name]['WrongTypeRetailMatches'] = [x for x in target['RetailMatches'] if x not in target['CompatibleRetailMatches']]
            results[name]['WrongTypeSourceMatches'] = [x for x in target['SourceMatches'] if x not in target['CompatibleSourceMatches']]
    apt = load_json(suite / 'apt-report.json')
    if apt['CompilerGeneration'] != generation or apt['Counts'] != {'NativeUIExportQualified': 122, 'BuildMetadataValidated': 1}:
        raise ValueError('Original UI qualification incomplete')
    for item in apt['Results']:
        row = results[item['OriginalSource']]
        if item['OriginalSourceSha256'] != row['OriginalSha256']:
            raise ValueError('Original UI wrapper changed')
        for output in item.get('NativeEvidence', []):
            if sha(Path(output['Path'])) != output['Sha256']:
                raise ValueError('Native UI evidence changed')
        row['Status'] = item['Status']
        row['Message'] = 'Unchanged original export compiled after reconstructing the authored legacy aptbuildxml include layout.' if item['Status'] == 'NativeUIExportQualified' else item['Explanation']
        row['Evidence'] = [str(suite / 'apt-report.json')]
    fragments = load_json(suite / 'fragments-report.json')
    if fragments['CompilerGeneration'] != generation or len(fragments['Results']) != 10:
        raise ValueError('Fragment qualification incomplete')
    for item in fragments['Results']:
        row = results[item['OriginalSource']]
        if item['OriginalSourceSha256'] != row['OriginalSha256']:
            raise ValueError('Original fragment changed')
        for output in item['NativeEvidence']:
            if sha(Path(output['Path'])) != output['Sha256']:
                raise ValueError('Fragment proof changed')
        row['Status'] = item['Status']
        row['Message'] = item['Message'] or 'Contents compiled in an explicitly synthetic schema owner; no real original consumer exists.'
        row['Evidence'] = [str(suite / 'fragments-report.json')]
    ai = []
    for name in ['AlliedFirstAssault', 'AlliedPCGDemo', 'JapanAmbushDivision', 'SovietHeavyArmor']:
        case = suite / 'ai-contexts' / name
        input_receipt = load_json(case / 'source/source-input.json')
        original = 'SkirmishAI\\Personalities\\' + name + '.xml'
        captures = {str(Path(x['Path']).relative_to(source)).lower(): x['Sha256'] for x in input_receipt['Sources'] if Path(x['Path']).is_relative_to(source)}
        if captures.get(original.lower()) != results[original]['OriginalSha256']:
            raise ValueError('AI personality source not captured unchanged')
        outputs = list((case / 'compiled').rglob('diagnostic.manifest'))
        if len(outputs) != 1 or 'Linking binary, relocation and import data verified by readback' not in (case / 'native.log').read_text(encoding='utf-8-sig'):
            raise ValueError('AI source/native qualification incomplete')
        state = 'SkirmishAI\\States\\' + name + 'States.xml'
        if captures.get(state.lower()) != sha(source / state):
            raise ValueError('Explicit original AI state library missing')
        proof = {'OriginalSource': original, 'StateSource': state, 'NativeManifest': str(outputs[0]), 'NativeManifestSha256': sha(outputs[0]), 'InputReceiptSha256': sha(case / 'source/source-input.json')}
        ai.append(proof)
        results[original].update(Status='NativeSourceIntegrationQualified', Message='Original personality and its real authored state library compiled together, retaining strong-reference and linked-stream proof.', Evidence=[str(case / 'source/source-input.json'), str(case / 'native.log'), str(outputs[0])])
    map_results = [json.loads(line) for line in (suite / 'real-map-consumers/results.jsonl').read_text(encoding='utf-8-sig').splitlines() if line]
    maps = {}
    for item in map_results:
        maps[item['Relative']] = item
    if len(maps) != 3 or any(x['Status'] != 'Passed' or x['CompilerGeneration'] != generation or not x['NativeEvidenceSha256'] for x in maps.values()):
        raise ValueError('Three real original map consumers have not passed')
    for item in maps.values():
        captured = {x['Relative'].lower(): x['Sha256'] for x in item['ContextSources']}
        emitted = {str(Path(x['Path']).relative_to(source)).lower() for x in item['EmittedOrigins'] if Path(x['Path']).is_relative_to(source)}
        for name, row in results.items():
            if row['Status'] != 'BlockedDependency' or name.lower() not in captured:
                continue
            # Reborn: an include-only library has no owner of its own; qualify it only when every authored all-child is actually emitted.
            include_closure = False
            if name.lower() not in emitted:
                xml = ET.parse(source / name).getroot()
                local_owners = [x for x in xml if 'id' in x.attrib]
                includes = xml.findall('.//{uri:ea.com:eala:asset}Include')
                all_children = [x.attrib['source'][5:].replace('/', '\\').lower() for x in includes if x.attrib.get('type') == 'all' and x.attrib.get('source', '').upper().startswith('DATA:')]
                include_closure = not local_owners and bool(all_children) and all(x in emitted and x in captured for x in all_children)
                if not include_closure:
                    continue
            if captured.get(name.lower()) != row['OriginalSha256']:
                raise ValueError('Emitted original source hash not captured')
            row.update(Status='NativeOriginalConsumerQualified', Message='Original source captured unchanged and emitted through the actual authored ' + item['Relative'] + ' closure; exact-map retail art resolved by production linking.', Evidence=[str(Path(item['LogDirectory']) / 'result.json'), str(Path(item['LogDirectory']) / 'native.log')])
            if include_closure:
                row['Message'] = 'Include-only original library captured unchanged; all its authored all-Includes were emitted and native-qualified through ' + item['Relative'] + '. The library itself is not a native asset.'
    for row in results.values():
        if row['BaselineClass'] == 'SourceOrSchema' or row['OriginalSource'] in ['Misc\\RallyFlag.xml', 'Templates\\Structure.xml']:
            row['Status'] = 'BlockedInvalidOriginalSource'
        if row['OriginalSource'] == 'Shell\\UIComponentMultiplayerMatchLobby.xml':
            row.update(Status='BlockedUnreferencedLegacyUIComponent', Message='Unused UIBaseComponent fragment: no authored source consumer, no top-level native asset identity and no original native component witness. It is not a standalone emitted asset.')
    statuses = collections.Counter(x['Status'] for x in results.values())
    native = sum(statuses[name] for name in ['NativeUIExportQualified', 'NativeOriginalConsumerQualified', 'NativeSourceIntegrationQualified'])
    summary = {'Format': 'RebornBlockedSourceFollowup1', 'CompilerGeneration': generation, 'BaselineReportSha256': sha(base / 'coverage-current-report.json'), 'OriginalFilesUnchanged': 7113, 'ExaminedBlockedSources': len(results), 'AdditionalNativeOrOriginalContextQualified': native, 'SyntheticOwnerOnlyQualified': statuses['SyntheticOwnerQualified'], 'BuildMetadataValidated': statuses['BuildMetadataValidated'], 'StillBlocked': len(results) - native - statuses['SyntheticOwnerQualified'] - statuses['BuildMetadataValidated'], 'Counts': dict(statuses), 'CompilerModified': False, 'GameExecuted': False, 'UserModPublished': False, 'AiContexts': ai, 'Results': list(results.values())}
    if native != 147 or statuses['SyntheticOwnerQualified'] != 6 or statuses['BuildMetadataValidated'] != 1 or summary['StillBlocked'] != 187:
        raise ValueError('Unexpected reconciliation; inspect exact source evidence before publishing counts: ' + str(dict(statuses)))
    (suite / 'followup-report.json').write_text(json.dumps(summary, indent=2), encoding='utf-8')
    fields = ['OriginalSource', 'Status', 'BaselineClass', 'Target', 'Message', 'Evidence']
    for filename, selected in [('followup-files.csv', results.values()), ('remaining-blocked-files.csv', [x for x in results.values() if x['Status'].startswith('Blocked')])]:
        with (suite / filename).open('w', encoding='utf-8-sig', newline='') as stream:
            writer = csv.DictWriter(stream, fieldnames=fields, extrasaction='ignore')
            writer.writeheader()
            for row in selected:
                writer.writerow(dict(row, Evidence='; '.join(row['Evidence'])))
    lines = ['UPRISING SAGEXML - BLOCKED SOURCE FOLLOW-UP', '', 'Compiler generation: ' + generation, 'Original 341-file baseline remains unchanged; this report records additional contexts separately.', '', 'Additional native/original-context qualification: ' + str(native), 'Synthetic schema-owner qualification only: 6', 'Validated EA build metadata (not a runtime asset): 1', 'Still blocked: 187', '', 'COUNTS']
    lines += [f'{key}: {value}' for key, value in sorted(statuses.items())]
    for row in results.values():
        lines += ['', row['OriginalSource'], '  ' + row['Status'], '  ' + row['Message'][:600]]
    (repo / 'docs/UPRISING_SAGEXML_BLOCKED_FOLLOWUP.txt').write_text('\n'.join(lines) + '\n', encoding='utf-8-sig')
    checkpoint = repo / 'docs/UPRISING_SAGEXML_COVERAGE_CHECKPOINT.md'
    text = checkpoint.read_text(encoding='utf-8-sig')
    marker = '<!-- Reborn: blocked-source follow-up preserves the unchanged baseline and explicitly separates real, synthetic and non-runtime contexts. -->'
    if marker not in text:
        section = marker + '\n## 341 engelli dosyanın bağımlılık ve bağlam incelemesi\n\n'
        section += '- Ayrı devam raporu: `alpha/.work/stock-source-coverage/blocked-followup-20261108/followup-report.json`.\n'
        section += '- Özgün 7.113 XML ve alpha compiler nesli değişmedi; önceki bağımsız tarama raporu korunuyor.\n'
        section += '- **147 dosya ek bağlamda native doğrulandı:** 122 AptUI özgün exportu, 21 dosya gerçek Yuriko haritası tüketicisiyle, 4 AI kişiliği gerçek state kaynaklarıyla.\n'
        section += '- **6 sahipsiz parça** açıkça sentetik şema sahibinde derlendi; gerçek kaynak tüketicisi bulunduğu veya bağımsız özgün dosya olarak geçtiği iddia edilmez.\n'
        section += '- `AptIncludeProxy_pc.xml`: 122 menüyü listeleyen EA build metadata dosyası doğrulandı; runtime asset değildir.\n'
        section += '- **187 dosya hâlâ engelli:** gerçek eksik/yanlış türde bağımlılıklar, bozuk özgün kaynaklar ve kullanılmayan/geçersiz parçalar. Hatalar warning/pass olarak gizlenmedi; dummy eklenmedi.\n'
        section += '- 97 özgün katalog (oyunun MapsCampaign/MapsMultiplayer/MapsTutorial BIG içerikleri dahil) ve tüm kaynaklar tarandı. İlk strong engellerdeki 161 isimden yalnızca 17\'si doğru türde retail sağlayıcı, 4\'ü doğru türde özgün kaynakla eşleşti. Aynı adlı GameObject veya Texture, model/OCL yerine kabul edilmedi.\n'
        section += '- Yeniden çalıştırma araçları: `scripts/Audit-Ra3Ep1BlockedSources.py`, `scripts/Test-Ra3Ep1BlockedContexts.py`; orijinal map-consumer testleri mevcut `Test-Ra3Ep1StockSources.ps1` ile özel çıktı klasöründe çalıştı.\n'
        section += '- Okunabilir tam sonuç: `docs/UPRISING_SAGEXML_BLOCKED_FOLLOWUP.txt`; kalanların listesi: `remaining-blocked-files.csv`. Compiler davranışı değiştirilmedi; oyun başlatılmadı, kullanıcı modları ve Documents paketleri değiştirilmedi, commit atılmadı.\n\n---\n'
        checkpoint.write_text(section + '\n' + text, encoding='utf-8')
    print(json.dumps({key: value for key, value in summary.items() if key not in ['Results', 'AiContexts']}, ensure_ascii=False))


if __name__ == '__main__':
    main()
