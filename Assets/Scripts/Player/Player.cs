using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity, TimedObject

{
    public Interactable lastInteraction;
    public int selected = 0;

    float scrollLock;
    public float scrollLockDefault = 0.1f;

    public Animator animator;

    public HealthBar healthBar;
    public InventoryUI inventoryUI;

    // Start is called before the first frame update
    public override void Start()
    {
        inventory = new Inventory();
        scrollLock = 0;
    }

    public IEnumerator ResetTriggers()
    {
        new WaitForSeconds(1f);
        animator.ResetTrigger("SpellCastSelf");
        animator.ResetTrigger("SpellCast");
        yield return null;
    }

    public void DropItem()
    {
        if (pickableInHand is Glyph) (pickableInHand as Glyph).spellPrototype.effectHandler.particles.Pause();
        pickableInHand.gameObject.transform.position = transform.position + new Vector3(transform.localScale.x / Mathf.Abs(transform.localScale.x) * GameManager.Instance.dungeon.scale, 0, 0);
        pickableInHand.gameObject.GetComponent<Collider2D>().enabled = true;
        pickableInHand.gameObject.GetComponent<Rigidbody2D>().simulated = true;
        inventory.Remove(pickableInHand);
        UpdateInventoryUI();

        if (pickableInHand is Glyph) (pickableInHand as Glyph).spellPrototype.effectHandler.particles.Play();
    }

    // Update is called once per frame
    public override void Update()
    {
        if (pickableInHand != null && GetComponent<PlayerMovement>().enabled) {
            if (Input.GetKeyDown(KeyCode.Mouse0) && !pickableInHand.isOnCooldown) { 
                if (pickableInHand is Glyph) {
                    if (((Glyph)pickableInHand).data.nature == GlyphData.Nature.SELF) animator.SetTrigger("SpellCastSelf");
                    else animator.SetTrigger("SpellCast");
                }
                pickableInHand.Use(this, CreateTarget());
                ResetTriggers();
            }
            if (Input.GetKeyDown(KeyCode.G)) DropItem();
        }


        if (scrollLock <= 0f)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                scrollLock = scrollLockDefault;
                selected += 1;
            }
            else if(Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                scrollLock = scrollLockDefault;
                selected -= 1;
            }
            if (selected < 0) selected = inventory.size - 1;
            if (selected >= inventory.size) selected = 0;
            inventoryUI.SelectSlot(selected);
            pickableInHand = inventory.Get(selected);
        }

        if (scrollLock > 0f) scrollLock -= Time.deltaTime;
        //Debug.Log(selected);
        //Debug.Log(pickableInHand);
        //Debug.Log(inventory.pickables.Count);
    }

    public void Pick(Pickable obj)
    {
        if(inventory.Add(obj))
        {
            obj.gameObject.transform.position = new Vector3(-1000, -1000, transform.position.z);
            obj.gameObject.GetComponent<Rigidbody2D>().simulated = false;
            
        }
        //foreach(Pickable key in inventory.inventory.Keys) Debug.Log(inventory.inventory[key]);
        UpdateInventoryUI();
    }

    public void UpdateInventoryUI()
    {
        int i = 0;
        foreach(Pickable pickable in inventory.pickables)
        {
            inventoryUI.SetSpriteInSlot(((Item)pickable).prefab.GetComponent<SpriteRenderer>().sprite, i);
            ++i;
        }
        for (int j=i; j < inventory.size; ++j) inventoryUI.SetSpriteInSlot(null, j);
    }

    public GameObject CreateTarget()
    {
        GameObject target = new GameObject("Target");
        target.AddComponent<TTL>();
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        Vector2 pos = Camera.main.ScreenToWorldPoint(mousePos);
        target.transform.position = pos;
        target.transform.position += new Vector3(0, 0, transform.position.z);
        return target;
    }

    public override void ReceiveDamage(float value, GlyphData.Element[] element)
    {
        base.ReceiveDamage(value, element);
        invunerable = true;
        Color color = GetComponent<SpriteRenderer>().color;
        color.a = 0.3f;
        GetComponent<SpriteRenderer>().color = color;
        TTL ttl = gameObject.AddComponent<TTL>();
        ttl.ttl = 1f;
        ttl.parent = this;
        ttl.start = true;
        healthBar.SetHealth(health);
    }
    public override void Heal(float value)
    {
        base.Heal(value);
        healthBar.SetHealth(health);
    }

    public void OnTtlEnd()
    {
        Color color = GetComponent<SpriteRenderer>().color;
        color.a = 1f;
        GetComponent<SpriteRenderer>().color = color;
        invunerable = false;
    }

    public override void OnDeath()
    {
        base.OnDeath();
        GetComponent<PlayerMovement>().enabled = false;
        animator.SetBool("Death", true);
    }

    public void Revive()
    {
        Heal(maxHealth);
        GetComponent<PlayerMovement>().enabled = true;
        animator.SetBool("Death", false);
    }
}
