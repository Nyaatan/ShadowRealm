using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Glyph : Item
{
    // public GlyphData data;

    public GlyphData data;
    public Spell spellPrototype;

    public GameObject prefabInstance;

    public bool isOnTable = false;

    public void setVector(Vector2 vector)
    {
        data.vector = vector;
    }

    internal static GameObject GetRandom(Vector2 minMaxTier, Vector2 minVectorBoundary, Vector2 maxVectorBoundary, bool canBeEmpty)
    {
        GameObject instance = Instantiate(GameManager.Instance.glyphPrototype);
        Glyph glyph = instance.GetComponent<Glyph>(); 
        glyph.data = Instantiate(glyph.data);
        glyph.data.vector = new Vector2(UnityEngine.Random.Range(minVectorBoundary.x, maxVectorBoundary.x), UnityEngine.Random.Range(minVectorBoundary.y, maxVectorBoundary.y));
        glyph.data.tier = (short) UnityEngine.Random.Range((int) minMaxTier.x, (int) minMaxTier.y + 1);
        glyph.calculateDataFromVector();
        glyph.spellPrototype.data = glyph.CalculateSpellData();
        instance.GetComponent<Rigidbody2D>().simulated = true;
        return instance;
    }
    internal static GameObject GetFromValues(Vector2 vector, short tier)
    {
        GameObject instance = Instantiate(GameManager.Instance.glyphPrototype);
        Glyph glyph = instance.GetComponent<Glyph>();
        glyph.data = Instantiate(glyph.data);
        glyph.data.vector = vector;
        glyph.data.tier = tier;
        glyph.calculateDataFromVector();
        glyph.spellPrototype.data = glyph.CalculateSpellData();
        instance.GetComponent<Rigidbody2D>().simulated = true;
        return instance;
    }
    public void calculateDataFromVector()
    {
        calculateElementFromVector();
        calculateNatureFromVector();
        Debug.Log(data.element[0]);
        Debug.Log(data.nature);
        spellPrototype.data.elements = data.element;
        spellPrototype.data.nature = data.nature;
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

    public override Spell Use(Entity user, GameObject target)
    {
        Spell spell = Instantiate(spellPrototype);
        spell.transform.position = user.gameObject.transform.position;
        spell.caster = user;
        spell.target = target;
        
        spell.Cast();
        isOnCooldown = true;
        return spell;
    }

    public void Start()
    {
        prefabInstance = Instantiate(prefab);
        prefabInstance.transform.position = transform.position;
        prefabInstance.transform.parent = this.transform;
        data = Instantiate(data);
        data.element = data.element.Take(data.tier).ToList();
        if (!data.empty) spellPrototype.data = CalculateSpellData();
        else spellPrototype.gameObject.SetActive(false);


        //calculateDataFromVector();
        //Debug.Log(data.vector);
        //Debug.Log(data.nature);
        //foreach (GlyphData.Element elem in data.element) {
        //    Debug.Log(elem);
        //}
    }

    public SpellData CalculateSpellData()
    {
        SpellData data = new SpellData();

        List<SpellData> defaults = new List<SpellData>();
        foreach (GlyphData.Element element in this.data.element)
        {
            defaults.Add(GameManager.Instance.getDefaultSpellData(this.data.nature, element));
        }

        data = SpellData.Combine(defaults, this.data.tier);
        return data;
    }

    public override void Update()
    {
        base.Update();
        prefabInstance.transform.position = transform.position;
        spellPrototype.transform.position = transform.position;
    }

    public override string GetTooltipString()
    {
        string elems = data.empty ? "empty" : string.Join(" ", data.element);
        string nature = data.empty ? "" : data.nature.ToString();
        string damage = data.empty ? "" : spellPrototype.data.damage.ToString();
        string heal = data.empty ? "" : spellPrototype.data.heal.ToString();
        return $"Tier: {data.tier}\n" +
            $"Elements:  {elems}\n" +
            $"Nature: {nature}\n" +
            $"Damage: {damage}\n" +
            $"Heal: {heal}";
    }
}
