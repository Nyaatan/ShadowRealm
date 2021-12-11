using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlyphTable : Interactable
{
    public Glyph[] glyphs;
    public Vector2 currentVector = new Vector2(0, 0);
    public Glyph resultGlyph;

    public bool isOpen = false;

    public GlyphTableUI GlyphTableUI;

    public Vector2 minValues = new Vector2(-1, -1);
    public Vector2 maxValues = new Vector2(1, 1);

    public int Count = 0;

    public Glyph glyphInstance;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        glyphs = new Glyph[5];
        //List<Glyph> glyphsTemp = glyphs;
        //glyphs = new List<Glyph>();
        //Glyph res = resultGlyph;
        //resultGlyph = null;
        //AddEmptyGlyph(res);
        //foreach (Glyph g in glyphsTemp) AddGlyph(g);
        //createGlyph();
        //spawnGlyph();
    }

    // Update is called once per frame

    void MoveVectorPlus(Vector2 value)
    {

        currentVector += value;
        currentVector = Vector2.Max(currentVector, minValues);
        currentVector = Vector2.Min(currentVector, maxValues);
    }

    void MoveVectorMinus(Vector2 value)
    {

        currentVector -= value;
        currentVector = Vector2.Max(currentVector, minValues);
        currentVector = Vector2.Min(currentVector, maxValues);
    }

    public void AddGlyph(Glyph glyph, int slot)
    {
        
        if (glyph.isOnTable) throw new GlyphCreationException("Glyph already on table");
        if (glyph.data.empty) throw new GlyphCreationException("Glyph is empty");
        MoveVectorPlus(glyph.data.vector);
        glyph.isOnTable = true;
        glyphs[slot] = glyph;
        ++Count;
    }

    public void RemoveGlyph(int slot)
    {
        if (!glyphs[slot].isOnTable) throw new GlyphTable.GlyphCreationException("Glyph is not on table");
        MoveVectorMinus(glyphs[slot].data.vector);
        glyphs[slot].isOnTable = false;
        glyphs[slot] = null;
        --Count;
    }

    public void AddEmptyGlyph(Glyph glyph)
    {
        if (!glyph.data.empty) throw new GlyphCreationException("Glyph is not empty.");
        else if (glyph.isOnTable) throw new GlyphCreationException("Glyph already on table");
        else
        {
            resultGlyph = glyph;
            glyph.isOnTable = true;
        }
    }

    public void RemoveEmptyGlyph()
    {
        if (resultGlyph == null) throw new GlyphCreationException("No empty glyph");
        resultGlyph.isOnTable = false;
        resultGlyph = null;
    }

    public void createGlyph()
    {
        if (Count == 0) throw new GlyphCreationException("No glyphs added");
        if (resultGlyph != null)
        {
            resultGlyph.isOnTable = false;
            glyphInstance = Instantiate(resultGlyph);
            glyphInstance.data.vector = currentVector;
            glyphInstance.calculateDataFromVector();
            glyphInstance.data.empty = false;

            foreach (Glyph glyph in glyphs) if(glyph != null) GameManager.Instance.player.GetComponent<Player>().inventory.Remove(glyph);
            GameManager.Instance.player.GetComponent<Player>().inventory.Remove(resultGlyph);
            GameManager.Instance.player.GetComponent<Player>().UpdateInventoryUI();

            glyphs = new Glyph[4];
        }
        else Debug.LogWarning("Empty glyph needed.");
    }

    public void spawnGlyph()
    {
        if (resultGlyph != null)
        {
            glyphInstance.transform.position = gameObject.transform.position + new Vector3(0, 5, 0);
            glyphInstance.spellPrototype.gameObject.SetActive(true);
            glyphInstance.GetComponent<Rigidbody2D>().simulated = true;

        } else Debug.LogWarning("Empty glyph needed.");
        //TODO
    }

    public override void Interact(GameObject obj)
    {
        base.Interact(obj);
        obj.GetComponent<PlayerMovement>().horizontalMove = 0;
        obj.GetComponent<PlayerMovement>().enabled = false;
        GlyphTableUI.gameObject.SetActive(true);
    }

    public class GlyphCreationException: Exception {
        public GlyphCreationException() { }
        public GlyphCreationException(string message) : base(message) { }
        public GlyphCreationException(string message, Exception inner) : base(message, inner) { }
    }

}
