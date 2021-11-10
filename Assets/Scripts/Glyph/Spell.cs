using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour
{
    public GlyphData.Nature nature = GlyphData.Nature.SINGLE_TARGET;
    public List<GlyphData.Element> elements = new List<GlyphData.Element>();

    public GameObject target;

    public float speed = 1;
    public float damage = 0;
    public float heal = 0;
    public float range = 10;
    public float width = 5;

    void Start()
    {
        
    }

    void Update()
    {
        Move();
        CreateParticles();
    }

    void Move()
    {
        
    }


    void CreateParticles()
    {

    }
}
