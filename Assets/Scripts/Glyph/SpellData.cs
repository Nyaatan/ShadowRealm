using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpellData
{
    public GlyphData.Nature nature = GlyphData.Nature.SINGLE_TARGET;
    public List<GlyphData.Element> elements = new List<GlyphData.Element>();
    public float speed = 50f;
    public float damage = 0;
    public float heal = 0;
    public float range = 1;
    public float width = 5;
    public int hitCount = 1;
    public float subsequentHitDelay = 0.3f;

    [SerializeField]
    public List<ElementColourPair> colours = new List<ElementColourPair>();
}