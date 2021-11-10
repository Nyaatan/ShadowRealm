using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlyphTable : MonoBehaviour
{
    public List<Glyph> glyphs = new List<Glyph>();
    public Vector2 currentVector = new Vector2(0, 0);
    public Glyph resultGlyph;

    // Start is called before the first frame update
    void Start()
    {
        List<Glyph> glyphsTemp = glyphs;
        glyphs = new List<Glyph>();
        Glyph res = resultGlyph;
        resultGlyph = null;
        AddEmptyGlyph(res);
        foreach (Glyph g in glyphsTemp) AddGlyph(g);
        createGlyph();
        spawnGlyph();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddGlyph(Glyph glyph)
    {
        if (resultGlyph != null)
        {
            glyphs.Add(glyph);
            currentVector += glyph.data.vector;
        }
        else throw new GlyphCreationException("Empty glyph needed.");
    }

    public void AddEmptyGlyph(Glyph glyph)
    {
        if (!glyph.data.empty) throw new GlyphCreationException("Glyph is not empty.");
        else
        {
            resultGlyph = glyph;
        }
    }

    public void createGlyph()
    {
        if (resultGlyph != null)
        {
            resultGlyph.data.vector = currentVector;
            resultGlyph.calculateDataFromVector();
            
            glyphs.Clear();
        }
        else throw new GlyphCreationException("Empty glyph needed.");
    }

    public void spawnGlyph()
    {
        Glyph glyphInstance = Instantiate(resultGlyph);
        glyphInstance.transform.position = gameObject.transform.position;
        
        
        //TODO
    }

    public class GlyphCreationException: Exception {
        public GlyphCreationException() { }
        public GlyphCreationException(string message) : base(message) { }
        public GlyphCreationException(string message, Exception inner) : base(message, inner) { }
    }
}
