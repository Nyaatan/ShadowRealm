using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellEffectHandler : MonoBehaviour
{
    public ParticleSystem particles;
    Spell spell;
    public GameObject proximityHandler;

    public void Start()
    {
        spell = GetComponent<Spell>();
        Gradient gradient = new Gradient();


        gradient.SetKeys(new GradientColorKey[]
        {
            new GradientColorKey(spell.data.colours[0].colour, 0.0f),
            new GradientColorKey(spell.data.colours[1].colour, 0.73f)
        },
        new GradientAlphaKey[] 
        { 
            new GradientAlphaKey(180f/255f, 0f),
            new GradientAlphaKey(0f, 0f)
        });

        var col = particles.colorOverLifetime;
        col.color = gradient;

    }

    public void MakeVisualEffect(List<GameObject> hits=null)
    {
        switch (spell.data.nature)
        {
            case GlyphData.Nature.AOE:
                Finish();
                break;
            case GlyphData.Nature.SINGLE_TARGET:
                break;
            case GlyphData.Nature.CHAIN:
                foreach(GameObject obj in hits)
                {
                    spell.gameObject.transform.position = obj.transform.position;
                }
                Finish();
                break;
            case GlyphData.Nature.SELF:
                break;
        }
    }

    public void HandleAOE(Collider2D collider)
    {
        if (collider != null)
        {
            Entity entity = collider.gameObject.GetComponent<Entity>();
            if (entity != null) entity.ReceiveDamage(spell.data.damage, spell.data.elements.ToArray());
        }
        proximityHandler.GetComponentInChildren<SpriteRenderer>().enabled = true;
        // Debug.Log("DUPA");
        proximityHandler.GetComponent<ProximityDetector>().Init(spell.data.width, -1, HandleDetections, spell.hasEffectsOn);
    }

    public void HandleDetections(List<GameObject> detections)
    {
        MakeVisualEffect(detections);
        //Debug.Log(detections.Count);
        foreach (GameObject detection in detections)
        {
            Entity entity = detection.GetComponent<Entity>();
            if (entity != null) entity.ReceiveDamage(spell.data.damage, spell.data.elements.ToArray());
        }
    }

    public void HandleST(Collider2D collider)
    {
        Entity entity = collider.GetComponent<Entity>();
        if (entity != null) entity.ReceiveDamage(spell.data.damage, spell.data.elements.ToArray());
        MakeVisualEffect();
        Finish();
    }

    public void HandleChain(Collider2D collider)
    {
        if (collider != null)
        {
            Entity entity = collider.gameObject.GetComponent<Entity>();
            if (entity != null) entity.ReceiveDamage(spell.data.damage, spell.data.elements.ToArray());
        }
        proximityHandler.GetComponent<ProximityDetector>().Init(spell.data.width, spell.data.hitCount, HandleDetections, spell.hasEffectsOn);
    }

    public void HandleSelf(Entity caster)
    {
        caster.Heal(spell.data.heal);
        HandleAOE(null);
        MakeVisualEffect();
        Finish();
    }

    void Finish()
    {
        particles = GetComponentInChildren<ParticleSystem>();
        particles.gameObject.transform.parent = spell.gameObject.transform.parent;
        TTL ttl = particles.gameObject.AddComponent<TTL>();
        ttl.ttl = 1f;
        ttl.Start();

        Destroy(spell.gameObject);
    }

    internal void HandleNone()
    {
        Finish();
    }
}
