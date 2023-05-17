using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GlyphData", menuName = "GlyphData")]
public class GlyphData: ScriptableObject
{
    public List<Element> element;
    public Nature nature;
    public short tier;
    public Vector2 vector;
    public bool empty = false;

    public enum Nature
    {
        AOE, SINGLE_TARGET, CHAIN, SELF
    }

    public enum Element: ushort
    {
        FIRE, EARTH, WATER, AIR
    }

    public Vector2 getDefaultElementVector(Element element)
    {
        switch (element)
        {
            case Element.FIRE:
                return new Vector2(1, 1);
            case Element.AIR:
                return new Vector2(-1, 1);
            case Element.WATER:
                return new Vector2(-1, -1);
            case Element.EARTH:
                return new Vector2(1, -1);
            default:
                return new Vector2(0, 0);
        }
    }

    public Vector2 getDefaultNatureVector(Nature nature)
    {
        switch (nature)
        {
            case Nature.AOE:
                return new Vector2(0, 1);
            case Nature.CHAIN:
                return new Vector2(1, 0);
            case Nature.SINGLE_TARGET:
                return new Vector2(0, -1);
            case Nature.SELF:
                return new Vector2(-1, 0);
            default:
                return new Vector2(0, 0);
        }
    }
}
