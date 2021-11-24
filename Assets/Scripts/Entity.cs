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

    public Pickable pickableInHand = null;
    // Start is called before the first frame update
    public virtual void Start()
    {
        
    }

    // Update is called once per frame
    public virtual void Update()
    {
        
    }

    public void ReceiveDamage(float value)
    {
        health = Mathf.Max(0, health - value);
        Debug.Log(this.gameObject + " received " + value + " damage");
        if (health == 0) OnDeath();

    }

    public void Heal(float value)
    {
        health = Mathf.Min(maxHealth, health + value);
    }

    public virtual void OnDeath() { }

    public virtual void OnHeal() { }

    public virtual void onReceiveDamage() { }
}
