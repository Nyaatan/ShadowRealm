using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glyph : Item
{
    // public GlyphData data;

    public GlyphData data;
    public Spell spellPrototype;

    public void setVector(Vector2 vector)
    {
        data.vector = vector;
    }

    public void calculateDataFromVector()
    {
        calculateElementFromVector();
        calculateNatureFromVector();
        spellPrototype.elements = data.element;
        spellPrototype.nature = data.nature;
    }

    public void calculateElementFromVector()
    {
        short n = 1;
        float x = data.vector.x;
        float y = data.vector.y;
        data.element = new List<GlyphData.Element>();
        do
        {
            if (x >= 0)
            {
                if (y >= 0) data.element.Add(GlyphData.Element.AIR);
                
                else data.element.Add(GlyphData.Element.FIRE);
            }
            else
            {
                if (y >= 0) data.element.Add(GlyphData.Element.WATER);
                
                else data.element.Add(GlyphData.Element.EARTH);
            }
            ++n;
            x = -Mathf.Abs(x) / x + 2 * x;
            y = -Mathf.Abs(y) / y + 2 * y;
        } while (n <= data.tier);
    }

    public void calculateNatureFromVector()
    {

        float x = data.vector.x;
        float y = data.vector.y;

        if (y >= Mathf.Abs(x)) data.nature = GlyphData.Nature.AOE;
        else if (y <= -Mathf.Abs(x)) data.nature = GlyphData.Nature.SINGLE_TARGET;
        else if (x >= 0) data.nature = GlyphData.Nature.CHAIN;
        else data.nature = GlyphData.Nature.SELF;
    }

    public void use(GameObject user)
    {
        Spell spell = Instantiate(spellPrototype);
        spell.transform.position = user.transform.position;
    }

    public override void Start()
    {
        calculateDataFromVector();
        Debug.Log(data.vector);
        Debug.Log(data.nature);
        foreach (GlyphData.Element elem in data.element) {
            Debug.Log(elem);
        }
        use(gameObject);
        use(gameObject);
        use(gameObject);
    }
}
