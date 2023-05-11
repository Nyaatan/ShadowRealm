## Copyright 2022 Marcin Swiderski
## 
## The MIT Licence (MIT)
## 
## Permission is hereby granted, free of charge, to any person obtaining a copy
## of this software and associated documentation files (the "Software"), to deal
## in the Software without restriction, including without limitation the rights
## to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
## copies of the Software, and to permit persons to whom the Software is
## furnished to do so, subject to the following conditions:
##
## The above copyright notice and this permission notice shall be included in all
## copies or substantial portions of the Software.
## THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
## IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
## FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
## AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
## LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
## OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
## SOFTWARE.

import os
import os.path
import json

from multiprocessing import Pool
from unityparser import UnityDocument
from unityparser.constants import OrderedFlowDict


sprites_dump_path = r'sprite_fileids2.dmp'

assets_root_path = r'..\Assets'
asset_exts = set(['.unity', '.asset', '.prefab', '.anim', '.playable', '.mat'])
asset_ignore_paths = [r'..\Assets\Packages', r'..\Assets\NuGet', r'..\Assets\TextMesh Pro']

ref_keys = set(['fileID', 'guid', 'type'])


def find_metas(ext='', ignore_paths=[]):
    metas_paths = []
    meta_ext = ext + '.meta'

    for root, dirs, files in os.walk(assets_root_path):
        if any(map(lambda ignore: root.startswith(ignore), ignore_paths)):
            continue

        for file in files:
            if file.endswith(meta_ext):
                metas_paths.append(os.path.join(root, file))

    return metas_paths


def find_assets(ignore_paths=[]):
    assets_paths = []

    for root, dirs, files in os.walk(assets_root_path):
        if any(map(lambda ignore: root.startswith(ignore), ignore_paths)):
            continue

        for file in files:
            file_ext = os.path.splitext(file)[1]
            if file_ext in asset_exts:
                assets_paths.append(os.path.join(root, file))

    return assets_paths


def fix_and_list_sprites(filepath):
    meta = UnityDocument.load_yaml(filepath, encoding='utf8')
    guid = meta.entry['guid']
    texture_importer = meta.entry['TextureImporter']

    try:
        sprites = texture_importer['spriteSheet']['sprites']
    except KeyError:
        sprites = []
    results = [(sprite.get('internalID', 0), sprite['name']) for sprite in sprites]

    dirty = False
    if 'internalIDToNameTable' in texture_importer:
        internalID_to_name = texture_importer['internalIDToNameTable']
        for iid, name in results:
            if iid != 0 and not any(entry['first'][213] == iid for entry in internalID_to_name):
                internalID_to_name.append(OrderedFlowDict([('first', OrderedFlowDict([(213, iid)])), ('second', name)]))
                dirty = True

    if dirty:
        meta.dump_yaml(filepath)

    # Add some built-in file IDs
    results.append((2800000, ''))
    results.append((21300000, ''))
    return (guid, results)


def fix_sprite_ref(ref, sprites, sprites_dump):
    guid = ref['guid']
    fileID = ref['fileID']
    if guid in sprites:
        if not any(fileID == sprite_fileID for sprite_fileID, _ in sprites[guid]):
            name = next((dmp_name for dmp_fileID, dmp_name in sprites_dump[guid] if fileID == dmp_fileID), None)
            fixed_fileID = next((sprite_fileID for sprite_fileID, sprite_name in sprites[guid] if name == sprite_name), None)
            if fixed_fileID is not None:
                ref['fileID'] = fixed_fileID
                return True
    return False


def fix_sprite_refs_in_value(value, sprites, sprites_dump):
    dirty = False

    if type(value) is OrderedFlowDict:
        if set(value.keys()) == ref_keys:
            dirty = fix_sprite_ref(value, sprites, sprites_dump) | dirty
        else:
            for v in value.values():
                dirty = fix_sprite_refs_in_value(v, sprites, sprites_dump) | dirty
    elif type(value) is list:
        for v in value:
            dirty = fix_sprite_refs_in_value(v, sprites, sprites_dump) | dirty

    return dirty


def fix_sprite_refs_in_asset(filepath, sprites, sprites_dump):
    asset = UnityDocument.load_yaml(filepath)
    dirty = False

    for entry in asset.entries:
        for key, value in entry.__dict__.items():
            dirty = fix_sprite_refs_in_value(value, sprites, sprites_dump) | dirty

    if dirty:
        asset.dump_yaml(filepath)


def main():
    with Pool(processes=24) as pool:

        with open(sprites_dump_path, 'r+', encoding='utf8') as dmpin:
            temp = json.load(dmpin)
            sprites_dump = {
                guid: (fname, name) for guid, fname, name in temp
            }

        sprites = dict()
        png_metas_paths = find_metas('.png')
        for guid, results in pool.imap_unordered(fix_and_list_sprites, png_metas_paths):
            for iid, name in results:
                if guid not in sprites:
                    sprites[guid] = []
                sprites[guid].append((iid, name))

        results = [pool.apply_async(fix_sprite_refs_in_asset, (asset_path, sprites, sprites_dump)) for asset_path in find_assets()]
        for result in results:
            result.get()


if __name__ == '__main__':
    main()