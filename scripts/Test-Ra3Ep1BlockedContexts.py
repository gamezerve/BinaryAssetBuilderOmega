# Reborn: exercise original UI exports and isolated source fragments through existing production compiler routes without changing originals.
from pathlib import Path
import argparse
import csv
import hashlib
import json
import os
import shutil
import subprocess
import time
import xml.etree.ElementTree as ET
from xml.sax.saxutils import escape
import runpy
# Reborn: reuse the bounded retail metadata parser; loading this helper never runs its command-line audit.
assets = runpy.run_path(str(Path(__file__).with_name('Audit-Ra3Ep1BlockedSources.py')))['assets']


# Reborn: hash evidence files in bounded blocks and keep native/output proofs distinct from source evidence.
def sha(path):
    with path.open('rb') as stream:
        return hashlib.file_digest(stream, 'sha256').hexdigest().upper()


# Reborn: retain complete diagnostics and return timeouts as failed qualification, never as successful output.
def invoke(exe, arguments, log):
    start = time.monotonic()
    env = dict(os.environ, REBORN_DEVELOPER_BUILD_TESTS='0', REBORN_SILENT_MODE='1', TEMP=str(log.parent), TMP=str(log.parent))
    with log.open('w', encoding='utf-8') as output:
        try:
            result = subprocess.run([str(exe), *map(str, arguments)], stdout=output, stderr=subprocess.STDOUT, env=env, timeout=600, check=False, creationflags=subprocess.CREATE_NO_WINDOW)
            code = result.returncode
        except subprocess.TimeoutExpired:
            output.write('\nERROR: qualification timed out; no native pass.\n')
            code = 124
    return code, round(time.monotonic() - start, 3)


# Reborn: a source-authored legacy UI include resolves inside a reconstructed private layout; the compiler consumes that exact original export.
def ui(repo, suite, exe, source):
    results = []
    schema = repo / 'alpha/Schemas/CnC3Types.xsd'
    exports = repo / 'alpha/Original EP1 UI Source SWF'
    layout = suite / 'apt/legacy-layout'
    for wrapper in sorted((source / 'AptUI').glob('*.xml')):
        original = ET.parse(wrapper).getroot()
        if original.tag != '{uri:ea.com:eala:asset}AssetDeclaration':
            continue
        includes = original.findall('.//{uri:ea.com:eala:asset}Include')
        if len(includes) != 1:
            raise ValueError('Expected exactly one authored UI include')
        logical = includes[0].attrib['source']
        if not logical.startswith('..\\..\\..\\aptbuildxml\\'):
            raise ValueError('Unexpected legacy UI layout')
        staged_wrapper = layout / 'production/Data/SageXml/pc/AptUI' / wrapper.name
        target = (staged_wrapper.parent / logical.replace('\\', '/')).resolve()
        target.relative_to(layout.resolve())
        exported = exports / wrapper.stem / wrapper.name
        if not exported.is_file():
            raise FileNotFoundError(exported)
        staged_wrapper.parent.mkdir(parents=True, exist_ok=True)
        shutil.copyfile(wrapper, staged_wrapper)
        if target.parent.exists():
            raise FileExistsError('Retain earlier proof: ' + str(target.parent))
        shutil.copytree(exported.parent, target.parent)
        evidence = []
        for path in sorted(exported.parent.rglob('*')):
            if path.is_file():
                copied = target.parent / path.relative_to(exported.parent)
                digest = sha(path)
                if digest != sha(copied):
                    raise ValueError('Original UI payload changed during staging')
                evidence.append({'Relative': str(path.relative_to(exported.parent)), 'Sha256': digest})
        # Reborn: the existing production AptUI adapter selects <page>/<page>.xml; its parent entry is only an adapter, not a rewritten original.
        adapter = target.parent.parent / wrapper.name
        shutil.copyfile(wrapper, adapter)
        case = suite / 'apt/cases' / wrapper.stem
        case.mkdir(parents=True)
        output = case / 'compiled'
        code, seconds = invoke(exe, ['aptui-compile-xml-test', adapter, schema, output], case / 'native.log')
        proof = []
        count = 0
        if code == 0:
            for path in sorted(output.glob('diagnostic.*')):
                proof.append({'Path': str(path), 'Sha256': sha(path)})
            count = len(list(assets((output / 'diagnostic.manifest').read_bytes(), str(output))))
            if count == 0 or len(proof) != 4:
                raise ValueError('UI compiler produced no complete native stream')
        row = {'OriginalSource': str(wrapper.relative_to(source)), 'Status': 'NativeUIExportQualified' if code == 0 else 'Failed', 'EvidenceKind': 'ReconstructedLegacyLayoutWithOriginalExport', 'OriginalSourceSha256': sha(wrapper), 'ResolvedLogicalInclude': logical, 'ResolvedOriginalExportSha256': sha(exported), 'StagedInclude': str(target), 'WrapperUnchanged': sha(wrapper) == sha(staged_wrapper), 'Assets': count, 'PayloadSources': evidence, 'NativeEvidence': proof, 'ExitCode': code, 'Seconds': seconds}
        (case / 'result.json').write_text(json.dumps(row, indent=2), encoding='utf-8')
        results.append(row)
        print(f"APT {len(results)}/122 {wrapper.stem}: {row['Status']}", flush=True)
    proxy = source / 'AptUI/AptIncludeProxy_pc.xml'
    names = [x.attrib['name'].lower() for x in ET.parse(proxy).getroot().findall('.//includes')]
    tested = {Path(x['OriginalSource']).name.lower() for x in results}
    if set(names) != tested or len(names) != len(tested) or len(tested) != 122:
        raise ValueError('Build proxy and original UI wrapper inventory disagree')
    results.append({'OriginalSource': str(proxy.relative_to(source)), 'OriginalSourceSha256': sha(proxy), 'Status': 'BuildMetadataValidated', 'RuntimeAsset': False, 'ListedWrappers': len(names), 'Explanation': 'Original project/fileset is an EA build-tool manifest, not an AssetDeclaration or native asset. All 122 listed source wrappers were qualified individually.'})
    return results


# Reborn: synthetic schema owners test fragment contents without pretending an unreferenced fragment has an original authored consumer.
def fragments(repo, suite, exe, source):
    csv.field_size_limit(64 * 1024 * 1024)
    with (repo / 'alpha/.work/stock-source-coverage/coverage-failures.csv').open(encoding='utf-8-sig', newline='') as stream:
        rows = [x for x in csv.DictReader(stream) if x['Class'] == 'SourceContextRequired' and x['OriginalSource'].startswith('Includes\\')]
    results = []
    for item in rows:
        original = source / item['OriginalSource']
        root = ET.parse(original).getroot()
        kind = root.tag.split('}')[-1]
        key = original.stem
        case = suite / 'fragments' / key
        data = case / 'input/Data'
        data.mkdir(parents=True)
        xi = '<xi:include href="DATA:' + escape(item['OriginalSource'].replace('\\', '/')) + '"'
        if kind in ('CommonArmorDraws', 'ParachuteDraws', 'FormationPreviewStates'):
            xi += ' xpointer="xmlns(n=uri:ea.com:eala:asset) xpointer(/n:' + kind + '/child::*)"'
        xi += '/>'
        draws, behaviors = '', ''
        if kind in ('ScriptedModelDraw', 'CommonArmorDraws', 'ParachuteDraws', 'TracerModelDraw'):
            draws = xi
        elif kind == 'FormationPreviewStates':
            draws = '<ScriptedModelDraw id="RebornFragmentDraw"><ModelConditionState ParseCondStateType="PARSE_DEFAULT"><Model Name=""/></ModelConditionState>' + xi + '</ScriptedModelDraw>'
        else:
            behaviors = xi
        text = '<AssetDeclaration xmlns="uri:ea.com:eala:asset" xmlns:xi="http://www.w3.org/2001/XInclude"><!-- Reborn: synthetic schema owner proves the unchanged orphan fragment only; no actual source consumer is claimed. --><Includes><Include type="all" source="DATA:GlobalData/GlobalDefines.xml"/></Includes><GameObject id="RebornFragment_' + key + '"><Draws>' + draws + '</Draws><Behaviors>' + behaviors + '</Behaviors><Body><ActiveBody id="RebornFragmentBody" MaxHealth="1"/></Body></GameObject></AssetDeclaration>'
        wrapper = data / 'Static_Mod.xml'
        wrapper.write_text(text, encoding='utf-8')
        prepared, compiled = case / 'source', case / 'compiled'
        code, seconds = invoke(exe, ['conscript-source-prepare', wrapper, prepared, repo / 'alpha', source], case / 'prepare.log')
        stage, captures, proof = 'Preparation', [], []
        if code == 0:
            receipt = json.loads((prepared / 'source-input.json').read_text(encoding='utf-8-sig'))
            captures = [x for x in receipt['Sources'] if Path(x['Path']) == original]
            if not captures or captures[0]['Sha256'] != sha(original):
                raise ValueError('Original fragment was not captured unchanged')
            stage = 'Native'
            code, native_seconds = invoke(exe, ['conscript-alpha-build', source, repo / 'alpha/catalogs/ra3ep1/streams/static/static.manifest', repo / 'alpha/catalogs/ra3ep1/streams/global/global.manifest', prepared / 'game.xml', compiled], case / 'native.log')
            seconds += native_seconds
            if code == 0:
                for path in compiled.rglob('*.manifest'):
                    proof.append({'Path': str(path), 'Sha256': sha(path)})
                for path in compiled.rglob('*.big'):
                    proof.append({'Path': str(path), 'Sha256': sha(path)})
                if not proof:
                    raise ValueError('No fragment native evidence')
        message = ''
        if code:
            log = case / ('native.log' if stage == 'Native' else 'prepare.log')
            message = next((x for x in log.read_text(encoding='utf-8-sig').splitlines() if 'ERROR:' in x or 'Error:' in x), '')
        row = {'OriginalSource': item['OriginalSource'], 'OriginalSourceSha256': sha(original), 'Status': 'SyntheticOwnerQualified' if code == 0 else 'BlockedSyntheticContext', 'EvidenceKind': 'SyntheticSchemaOwnerNotOriginalConsumer', 'OriginalConsumerFound': False, 'Stage': stage, 'ExitCode': code, 'Seconds': round(seconds, 3), 'Message': message, 'NativeEvidence': proof}
        (case / 'result.json').write_text(json.dumps(row, indent=2), encoding='utf-8')
        results.append(row)
        print('FRAGMENT ' + key + ': ' + row['Status'] + ' ' + message[-180:], flush=True)
    return results


# Reborn: compiler and baseline source-plan hashes fence every additional proof; original baseline results remain unmodified.
def main():
    parser = argparse.ArgumentParser()
    parser.add_argument('--repository', required=True)
    parser.add_argument('--output', required=True)
    parser.add_argument('--mode', choices=['apt', 'fragments'], required=True)
    args = parser.parse_args()
    repo, suite = Path(args.repository).resolve(), Path(args.output).resolve()
    suite.relative_to(repo / 'alpha/.work')
    suite.mkdir(parents=True, exist_ok=True)
    source = repo / 'alpha/SageXML'
    exe = repo / 'alpha/tools/compiler/BinaryAssetBuilder.ManifestInspector.exe'
    binaries = [exe.with_name(x) for x in ['BinaryAssetBuilder.ManifestInspector.dll', 'BinaryAssetBuilder.XmlCompiler.dll', 'BinaryAssetBuilder.W3XCompiler.dll', 'BinaryAssetBuilder.Core.dll', 'SageBinaryData.dll']]
    binaries.append(repo / 'scripts/Test-Ra3Ep1StockSources.ps1')
    before = [sha(x) for x in binaries]
    generation = hashlib.sha256(':'.join(before).encode()).hexdigest().upper()
    baseline = json.loads((repo / 'alpha/.work/stock-source-coverage/coverage-current-report.json').read_text(encoding='utf-8-sig'))
    if generation != baseline['CompilerGeneration']:
        raise ValueError('Original baseline/compiler generation changed')
    results = (ui if args.mode == 'apt' else fragments)(repo, suite, exe, source)
    if before != [sha(x) for x in binaries]:
        raise ValueError('Compiler changed during qualification')
    report = {'Format': 'RebornBlockedContextQualification1', 'CompilerGeneration': generation, 'HarnessSha256': sha(Path(__file__)), 'Mode': args.mode, 'Results': results, 'Counts': dict(__import__('collections').Counter(x['Status'] for x in results)), 'OriginalBaselineUnmodified': True, 'GameExecuted': False, 'ModPublished': False}
    (suite / (args.mode + '-report.json')).write_text(json.dumps(report, indent=2), encoding='utf-8')
    print(json.dumps(report['Counts']), flush=True)


if __name__ == '__main__':
    main()
