# Reborn: audit blocked originals against actual sources and bounded retail manifest metadata; never invent provider assets.
from pathlib import Path
import argparse
import collections
import csv
import hashlib
import json
import re
import struct
import xml.etree.ElementTree as ET


# Reborn: decode bounded RefPack metadata using the same packet rules as the compiler and reject invalid offsets or sizes.
def unpack(data, limit=64 * 1024 * 1024):
    if len(data) < 2 or data[1] != 0xFB or data[0] & 0x1F != 0x10:
        return data
    width = 4 if data[0] & 0x80 else 3
    pos = 2 + (width if data[0] & 1 else 0)
    size = int.from_bytes(data[pos:pos + width], 'big')
    pos += width
    if size > limit:
        raise ValueError('Oversized metadata')
    out = bytearray()
    while pos < len(data):
        control = data[pos]
        pos += 1
        if control >= 0xFC:
            literals, count, offset = control & 3, 0, 0
        elif control >= 0xE0:
            literals, count, offset = ((control & 0x1F) << 2) + 4, 0, 0
        elif control >= 0xC0:
            a, b, c = data[pos:pos + 3]
            pos += 3
            literals, count, offset = control & 3, ((control & 12) << 6) + c + 5, ((control & 16) << 12) + (a << 8) + b + 1
        elif control >= 0x80:
            a, b = data[pos:pos + 2]
            pos += 2
            literals, count, offset = a >> 6, (control & 63) + 4, ((a & 63) << 8) + b + 1
        else:
            a = data[pos]
            pos += 1
            literals, count, offset = control & 3, ((control & 28) >> 2) + 3, ((control & 96) << 3) + a + 1
        if pos + literals > len(data) or len(out) + literals + count > size:
            raise ValueError('Invalid RefPack metadata range')
        out.extend(data[pos:pos + literals])
        pos += literals
        if count:
            if offset > len(out):
                raise ValueError('Invalid RefPack back-reference')
            for _ in range(count):
                out.append(out[-offset])
        if control >= 0xFC:
            break
    if len(out) != size:
        raise ValueError('RefPack metadata size mismatch')
    return bytes(out)


# Reborn: validate directory and entry ranges while reading only linked manifest entries from each retail archive.
def big_manifests(path):
    length = path.stat().st_size
    with path.open('rb') as stream:
        head = stream.read(16)
        if head[:4] not in (b'BIG4', b'BIGF'):
            raise ValueError('Not a retail BIG')
        count, end = struct.unpack_from('>2I', head, 8)
        if count > 1000000 or not 16 <= end <= min(length, 64 * 1024 * 1024):
            raise ValueError('Invalid BIG directory')
        directory, pos = stream.read(end - 16), 0
        selected = []
        for _ in range(count):
            offset, size = struct.unpack_from('>2I', directory, pos)
            pos += 8
            stop = directory.index(b'\0', pos)
            name = directory[pos:stop].decode('ascii')
            pos = stop + 1
            if offset < end or offset + size > length:
                raise ValueError('Invalid BIG entry')
            if name.lower().endswith('.manifest'):
                if size > 64 * 1024 * 1024:
                    raise ValueError('Oversized manifest entry')
                selected.append((offset, size, name))
        for offset, size, name in selected:
            stream.seek(offset)
            yield name, unpack(stream.read(size))


# Reborn: read actual manifest names, type words, native lengths and provider identities without loading any BIN payload.
def assets(data, provider):
    data = unpack(data)
    if len(data) < 52:
        raise ValueError('Truncated manifest')
    words = struct.unpack_from('<12I', data, 4)
    count = words[3]
    name_start = 52 + count * 48 + words[8] + words[9]
    if count > 1000000 or name_start + words[10] + words[11] != len(data):
        raise ValueError('Manifest table bounds mismatch')
    for i in range(count):
        row = struct.unpack_from('<12I', data, 52 + i * 48)
        start = name_start + row[6]
        end = data.index(b'\0', start, name_start + words[10])
        name = data[start:end].decode('ascii')
        yield {'Name': name, 'Provider': provider, 'Bytes': row[8], 'TypeId': f'{row[0]:08X}', 'InstanceId': f'{row[1]:08X}'}


# Reborn: preserve exact original hashes and distinguish matching source definitions from actually available typed native providers.
def main():
    parser = argparse.ArgumentParser()
    parser.add_argument('--repository', required=True)
    parser.add_argument('--game-data', required=True)
    parser.add_argument('--output', required=True)
    args = parser.parse_args()
    repo, game, output = map(lambda x: Path(x).resolve(), (args.repository, args.game_data, args.output))
    sdk, source = repo / 'alpha', repo / 'alpha/SageXML'
    output.relative_to(sdk / '.work')
    output.mkdir(parents=True, exist_ok=True)
    csv.field_size_limit(64 * 1024 * 1024)
    with (sdk / '.work/stock-source-coverage/coverage-failures.csv').open(encoding='utf-8-sig', newline='') as f:
        failures = list(csv.DictReader(f))
    targets = {}
    for row in failures:
        match = re.search(r'strong target missing/ambiguous: ([^\r\n]+)', row['Message'])
        if match:
            required, name = match[1].split(':', 1)
            targets.setdefault(name.lower(), {'Name': name, 'RequiredTypes': set(), 'Users': []})
            targets[name.lower()]['RequiredTypes'].add(required)
            targets[name.lower()]['Users'].append(row['OriginalSource'])
    providers, provider_hashes = collections.defaultdict(list), {}
    for path in sorted((sdk / 'catalogs/ra3ep1').rglob('*.manifest')):
        data = path.read_bytes()
        provider = str(path.relative_to(repo))
        provider_hashes[provider] = hashlib.sha256(data).hexdigest().upper()
        for asset in assets(data, provider):
            name = asset['Name'].split(':', 1)[-1].lower()
            if name in targets:
                providers[name].append(asset)
    for path in sorted(game.glob('Maps*.big')):
        for entry, data in big_manifests(path):
            provider = str(path) + '::' + entry
            provider_hashes[provider] = hashlib.sha256(data).hexdigest().upper()
            for asset in assets(data, provider):
                name = asset['Name'].split(':', 1)[-1].lower()
                if name in targets:
                    providers[name].append(asset)
    definitions, parse_errors, source_hashes = collections.defaultdict(list), [], {}
    source_docs = {}
    for path in sorted(source.rglob('*.xml')):
        relative, data = str(path.relative_to(source)), path.read_bytes()
        source_hashes[relative] = hashlib.sha256(data).hexdigest().upper()
        try:
            root = ET.fromstring(data)
        except ET.ParseError as error:
            parse_errors.append({'Source': relative, 'Diagnostic': str(error)})
            continue
        source_docs[relative] = root
        if root.tag == '{uri:ea.com:eala:asset}AssetDeclaration':
            for element in root:
                if 'id' in element.attrib:
                    name = element.attrib['id'].lower()
                    if name in targets:
                        definitions[name].append({'Source': relative, 'Type': element.tag.split('}')[-1], 'Id': element.attrib['id']})
    for key, target in targets.items():
        target['RequiredTypes'] = sorted(target['RequiredTypes'])
        target['RetailMatches'] = providers[key]
        target['SourceMatches'] = definitions[key]
    # Reborn: equal names of different asset types cannot satisfy an abstract render/OCL/AI reference.
    bases = {}
    for schema in (sdk / 'Schemas').rglob('*.xsd'):
        xml = ET.parse(schema).getroot()
        for node in xml.findall('{http://www.w3.org/2001/XMLSchema}complexType'):
            name = node.attrib.get('name')
            base = next((x.attrib['base'].split(':')[-1] for x in node.iter() if x.tag in ('{http://www.w3.org/2001/XMLSchema}extension', '{http://www.w3.org/2001/XMLSchema}restriction') and 'base' in x.attrib), None)
            if name and base:
                bases[name] = base
    # Reborn: follow actual XSD inheritance with cycle detection; no hand-written type substitutions are admitted.
    def derives(actual, required):
        visited = set()
        while actual not in visited:
            if actual == required:
                return True
            visited.add(actual)
            actual = bases.get(actual)
            if actual is None:
                break
        return False
    for target in targets.values():
        target['CompatibleRetailMatches'] = [x for x in target['RetailMatches'] if any(derives(x['Name'].split(':', 1)[0], required) for required in target['RequiredTypes'])]
        target['CompatibleSourceMatches'] = [x for x in target['SourceMatches'] if any(derives(x['Type'], required) for required in target['RequiredTypes'])]
    (output / 'dependency-targets.json').write_text(json.dumps(list(targets.values()), indent=2), encoding='utf-8')
    (output / 'source-hashes.json').write_text(json.dumps(source_hashes, indent=2), encoding='utf-8')
    (output / 'provider-hashes.json').write_text(json.dumps(provider_hashes, indent=2), encoding='utf-8')
    (output / 'parse-errors.json').write_text(json.dumps(parse_errors, indent=2), encoding='utf-8')
    summary = {'BlockedFiles': len(failures), 'UniqueStrongNames': len(targets), 'NamesFoundInRetail': sum(bool(x['RetailMatches']) for x in targets.values()), 'NamesFoundInSources': sum(bool(x['SourceMatches']) for x in targets.values()), 'CompatibleRetailTargets': sum(bool(x['CompatibleRetailMatches']) for x in targets.values()), 'CompatibleSourceTargets': sum(bool(x['CompatibleSourceMatches']) for x in targets.values()), 'ManifestsInspected': len(provider_hashes), 'OriginalSourcesHashed': len(source_hashes), 'GameExecuted': False, 'OriginalsModified': False}
    (output / 'audit-summary.json').write_text(json.dumps(summary, indent=2), encoding='utf-8')
    print(json.dumps(summary))


if __name__ == '__main__':
    main()
