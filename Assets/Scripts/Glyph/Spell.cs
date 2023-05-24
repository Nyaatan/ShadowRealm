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

    public ushort id;

    public void Cast()
    {
        Vector3 startPos = caster.transform.position;
        Vector3 targetPos = target.transform.position;

        direction = targetPos - startPos;

        isCasted = true;
        if (data.nature == GlyphData.Nature.SELF)
        {
            target.transform.position = caster.transform.position;

            GetComponent<TTL>().ttl = 0.001f;
        }
        GetComponent<TTL>().start = true;
        //Debug.Log(GetComponent<TTL>().start);
    }

    public void Start()
    {
        GetComponent<TTL>().start = isCasted;
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
        try
        {
            //Debug.LogWarning(collision.gameObject);
            if (isCasted)
                foreach (LayerMask layerMask in collidesWith)
                {
                    if (collision.gameObject.layer == (int)Mathf.Log(layerMask, 2) && collision.gameObject.layer != caster.gameObject.layer)
                    {
                        isCasted = false;
                        ResearchManager.Instance.HandleSpellCollision(this, collision.gameObject);
                        TriggerEffect(collision);
                    }
                }
        }
        catch (MissingReferenceException e)
        {
            Debug.LogWarning(e);
        }
    }

    public void TriggerEffect(Collider2D collision) {
        //if (collision != null) Debug.LogWarning(collision.gameObject);

        if (data.nature == GlyphData.Nature.AOE)
        {
            effectHandler.HandleAOE(collision);
        }
        else if (data.nature == GlyphData.Nature.SELF)
        {
            effectHandler.HandleSelf(caster);
        }
        else if (collision != null)
        {

            if (data.nature == GlyphData.Nature.SINGLE_TARGET && collision.gameObject != caster)
            {
                effectHandler.HandleST(collision);
            }
            else if (data.nature == GlyphData.Nature.CHAIN && collision.gameObject != caster)
            {
                effectHandler.HandleChain(collision);
            }
        }
        else effectHandler.HandleNone();
    }

    public void OnTtlEnd()
    {
        isCasted = false;
        TriggerEffect(null);
        Destroy(target);
    }

    public void OnDestroy(){
        ResearchManager.Instance.HandleDestroySpell(this);
    }
}

