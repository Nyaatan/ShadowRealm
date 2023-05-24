using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField]
    public Inventory inventory;
    public float health = 100;
    public float mana = 100;
    public float maxHealth = 100;
    public float maxMana = 100;
    public Affinity affinity;

    public Color floatColor = Color.yellow;

    public bool invunerable = false;

    public Pickable pickableInHand = null;

    public GameObject floatDamage;
    // Start is called before the first frame update
    public virtual void Start()
    {
        
    }

    // Update is called once per frame
    public virtual void Update()
    {
        
    }

    public virtual void ReceiveDamage(float value, GlyphData.Element[] element, bool mpSignal = false, ushort sourceID=0)
    {
        if (floatColor.Equals(new Color(0, 0, 0, 0))) floatColor = Color.yellow;
        floatDamage = Instantiate(GameManager.Instance.floatDamage, transform.position + new Vector3(0, 3, 0), Quaternion.identity);
        TTL ttl = floatDamage.AddComponent<TTL>();
        ttl.ttl = 0.5f;
        ttl.start = true;
        if (!invunerable)
        {
            health = Mathf.Max(0, health - value);
            Debug.Log(this.gameObject + " received " + value + " damage");
            if (health == 0) OnDeath();
            floatDamage.GetComponent<TextMesh>().text = value.ToString();
        } else floatDamage.GetComponent<TextMesh>().text = "0";
        floatDamage.GetComponent<TextMesh>().color = floatColor;
        onReceiveDamage();
    }


    public virtual void Heal(float value, bool mpSignal=false)
    {
        health = Mathf.Min(maxHealth, health + value);
        Debug.Log(this.gameObject + " received " + value + " healing");
        OnHeal();
    }

    public virtual void OnDeath() { }

    public virtual void OnHeal() { }

    public virtual void onReceiveDamage() { }
}
