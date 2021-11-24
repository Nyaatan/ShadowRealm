using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour, TimedObject
{
    public SpellEffectHandler effectHandler;

    public GameObject target;

    public List<LayerMask> collidesWith = new List<LayerMask>();
    public List<LayerMask> hasEffectsOn = new List<LayerMask>();

    public Entity caster;

    public Vector3 direction;
    public bool isCasted = false;

    public List<GameObject> targetsHit = new List<GameObject>();

    [SerializeField]
    public SpellData data;

    public void Cast()
    {
        Vector3 startPos = caster.transform.position;
        Vector3 targetPos = target.transform.position;

        direction = targetPos - startPos;

        isCasted = true;
        if (data.nature == GlyphData.Nature.SELF) target.transform.position = caster.transform.position;
        GetComponent<TTL>().start = true;
    }

    public void Start()
    {
        GetComponent<TTL>().ttl = data.range;
        GetComponent<TTL>().parent = this;
    }

    public void Update()
    {
        if(isCasted) Move();
    }

    void Move()
    {
        transform.position += direction.normalized * data.speed * Time.deltaTime;
    }



    public void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.LogWarning(collision.gameObject);
        if (collidesWith.Contains(collision.gameObject.layer)) TriggerEffect(collision);
    }

    public void TriggerEffect(Collider2D collision) {
        
        if(data.nature == GlyphData.Nature.AOE)
        {
            effectHandler.HandleAOE();
        }
        if (collision != null)
        {
            if (data.nature == GlyphData.Nature.SINGLE_TARGET && collision.gameObject != caster)
            {
                effectHandler.HandleST();
            }
            else if (data.nature == GlyphData.Nature.CHAIN && collision.gameObject != caster)
            {
                effectHandler.HandleChain();
            }
            else if(data.nature == GlyphData.Nature.SELF && collision.gameObject == caster)
            {
                effectHandler.HandleSelf();
            }
        }
    }

    public void OnTtlEnd()
    {
        isCasted = false;
        TriggerEffect(null);
    }
}

