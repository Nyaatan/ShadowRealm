/*
 * Copyright 2022 Marcin Swiderski
 * 
 * The MIT Licence (MIT)
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public class FixSpriteReferencesMenuItems : MonoBehaviour
{
    [MenuItem("Scripts/Dump Sprite File IDs")]
    public static void DumpSpriteFileIDs()
    {
        using FileStream dumpFile = File.Open($"{Application.dataPath}/../Scripts/sprite_fileids.dmp", FileMode.Create);
        using StreamWriter writer = new StreamWriter(dumpFile, Encoding.Unicode); 
        writer.Write('[');
        
        IEnumerable<string> spritePaths = AssetDatabase.FindAssets("t:sprite").Select(AssetDatabase.GUIDToAssetPath);
        foreach (string spritePath in spritePaths) {
            foreach (Object obj in AssetDatabase.LoadAllAssetsAtPath(spritePath)) {
                Sprite sprite = obj as Sprite;
                if (sprite != null) {
                    AssetDatabase.TryGetGUIDAndLocalFileIdentifier(sprite, out string guid, out long fileID);
                    writer.Write($"('{guid}',{fileID},'{sprite.name}'),");
                }
            }
        }
        
        writer.Write(']');
        Debug.Log("Dump Sprite File IDs done");
    }

}