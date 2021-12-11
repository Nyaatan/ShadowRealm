using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class SpellData
{
    public GlyphData.Nature nature = GlyphData.Nature.SINGLE_TARGET;
    public List<GlyphData.Element> elements = new List<GlyphData.Element>();
    public float speed = 0f;
    public float damage = 0;
    public float heal = 0;
    public float range = 0;
    public float width = 0;
    public int hitCount = -1;
    public float subsequentHitDelay = 0f;
    public float powerIncreasePerTierModifier = 0.5f;

    [SerializeField]
    public List<ElementColourPair> colours = new List<ElementColourPair>();

    public static SpellData Combine(List<SpellData> datas, int tier)
    {
        SpellData data = new SpellData();
        data.speed = datas.Select(x => x.speed).Average();
        data.damage = datas.Select(x => x.damage).Average() * tier * data.powerIncreasePerTierModifier;
        data.heal = datas.Select(x => x.heal).Average() * tier * data.powerIncreasePerTierModifier;
        data.range = datas.Select(x => x.range).Average();
        data.width = datas.Select(x => x.width).Average();
        data.subsequentHitDelay = datas.Select(x => x.subsequentHitDelay).Average();

        float count = 0;
        float r_main = 0;
        float g_main = 0;
        float b_main = 0;
        float a_main = 0;

        float r_sec = 0;
        float g_sec = 0;
        float b_sec = 0;
        float a_sec = 0;

        foreach (SpellData sdata in datas)
        {
            data.elements.Add(sdata.elements[0]);
            data.hitCount = Mathf.Max(data.hitCount, sdata.hitCount);

            r_main += sdata.colours[0].colour.r;
            g_main += sdata.colours[0].colour.g;
            b_main += sdata.colours[0].colour.b;
            a_main += sdata.colours[0].colour.a;

            r_sec += sdata.colours[1].colour.r;
            g_sec += sdata.colours[1].colour.g;
            b_sec += sdata.colours[1].colour.b;
            a_sec += sdata.colours[1].colour.a;

            ++count;
        }

        r_main /= count;
        g_main /= count;
        b_main /= count;
        a_main /= count;

        r_sec /= count;
        g_sec /= count;
        b_sec /= count;
        a_sec /= count;

        ElementColourPair main = new ElementColourPair();
        main.element = GlyphData.Element.WATER;
        main.colour = new Color(r_main, g_main, b_main, a_main);
        ElementColourPair sec = new ElementColourPair();
        sec.element = GlyphData.Element.WATER;
        sec.colour = new Color(r_sec, g_sec, b_sec, a_sec);

        data.colours.Add(main);
        data.colours.Add(sec);

        data.nature = datas[0].nature;

        return data;
    }
}