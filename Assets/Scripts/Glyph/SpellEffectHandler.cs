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
    }

    public void MakeVisualEffect()
    {
        switch (spell.data.nature)
        {
            case GlyphData.Nature.AOE:
                break;
            case GlyphData.Nature.SINGLE_TARGET:
                break;
            case GlyphData.Nature.CHAIN:
                break;
            case GlyphData.Nature.SELF:
                break;
        }
    }

    public void HandleAOE()
    {
        proximityHandler.GetComponent<ProximityDetector>().Init(spell.data.width, -1, HandleAOEDetections, spell.hasEffectsOn);
        MakeVisualEffect();
    }

    public void HandleAOEDetections(List<GameObject> detections)
    {
        Debug.Log(detections.Count);
        foreach (GameObject detection in detections) detection.GetComponent<Entity>().ReceiveDamage(spell.data.damage);
        Destroy(spell.gameObject);
    }

    internal void HandleST()
    {
        MakeVisualEffect();
        Destroy(spell.gameObject);
    }

    internal void HandleChain()
    {
        MakeVisualEffect();
        Destroy(spell.gameObject);
    }

    internal void HandleSelf()
    {
        MakeVisualEffect();
        Destroy(spell.gameObject);
    }
}
