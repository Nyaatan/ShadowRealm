using Riptide;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Player : EntityMP, TimedObject

{
    public Interactable lastInteraction;
    public int selected = 0;

    float scrollLock;
    public float scrollLockDefault = 0.1f;

    public Animator animator;

    public HealthBar healthBar;
    public InventoryUI inventoryUI;


    #region Utils
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

    public override void Heal(float value, bool mpSignal=false)
    {
        if (!EntityMP.inSession || ((Player.List[1]) as Player).isLocal || mpSignal)
        {
            base.Heal(value);
            if (!mpSignal) SendHeal(value);
            healthBar.SetHealth(health);
        }
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
    #endregion
    
    #region Items
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
    #endregion

    // Update is called once per frame
    public override void Update()
    {
        if (pickableInHand != null && GetComponent<PlayerMovement>().enabled) {
            if (Input.GetKeyDown(KeyCode.Mouse0) && !pickableInHand.isOnCooldown) { 
                if (pickableInHand is Glyph) {
                    if (((Glyph)pickableInHand).data.nature == GlyphData.Nature.SELF) animator.SetTrigger("SpellCastSelf");
                    else animator.SetTrigger("SpellCast");
                }
                GameObject target = CreateTarget();
                //if(!inSession || (List[1] as Player).isLocal){
                //    Spell spell = pickableInHand.Use(this, target);
                //    ResearchManager.Instance.AssignSpellID(spell);
                //    if(inSession) SendAttack(new Vector2(target.transform.position.x, target.transform.position.y), ((Glyph)pickableInHand).data, spell.id);
                //}
                //else if(inSession && !(List[1] as Player).isLocal) {
                //    pickableInHand.isOnCooldown = true;
                //    SendAttack(new Vector2(target.transform.position.x, target.transform.position.y), ((Glyph)pickableInHand).data, ResearchManager.Instance.CreateSpellID());
                //}
                Spell spell = pickableInHand.Use(this, target);
                ResearchManager.Instance.AssignSpellID(spell);
                if (inSession) SendAttack(new Vector2(target.transform.position.x, target.transform.position.y), ((Glyph)pickableInHand).data, spell.id);

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

        if (shouldLerp)
        {
            transform.position = Vector3.Lerp(transform.position, lerpDest, lerpStep);
            if (Vector2.Distance((Vector2)transform.position, lerpDest) < 0.5f) shouldLerp = false;
        }
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

    #region Messages
    public override void ReceiveDamage(float value, GlyphData.Element[] element, bool mpSignal=false, ushort sourceID=0)
    {
        if (!EntityMP.inSession || ((Player.List[1]) as Player).isLocal || mpSignal)
        {
            base.ReceiveDamage(value, element, mpSignal, sourceID);
            if (!mpSignal) SendReceiveDamage(value, element, sourceID);
            invunerable = true;
            Color color = GetComponent<SpriteRenderer>().color;
            color.a = 0.3f;
            GetComponent<SpriteRenderer>().color = color;
            TTL ttl = gameObject.AddComponent<TTL>();
            ttl.ttl = 1f;
            ttl.parent = this;
            ttl.start = true;
            if (isLocal) healthBar.SetHealth(health);
        }
    }

    public void SendAttack(Vector2 target, GlyphData glyphData, ushort spellID)
    {
        Message message = Message.Create(MessageSendMode.Unreliable, MessageId.PlayerAttack);
        message.AddUShort(id);
        message.AddUShort(spellID);
        message.AddVector2(target);
        message.AddVector2(glyphData.vector);
        message.AddShort(glyphData.tier);
        NetworkManager.Singleton.Client.Send(message);
    }

    public void SendReceiveDamage(float value, GlyphData.Element[] element, ushort spellID)
    {
        
        Message message = Message.Create(MessageSendMode.Unreliable, MessageId.ServerSpellHit);
        message.AddUShort(id);
        message.AddUShort(spellID);
        message.AddFloat(value);
        message.AddUShort((ushort)element.Length);
        foreach (GlyphData.Element e in element) message.AddUShort((ushort)e);
        foreach (Player player in List.Values) if(player.id != 1)
            NetworkManager.Singleton.Server.Send(message, player.id);
    }

    public void SendHeal(float value)
    {
        Message message = Message.Create(MessageSendMode.Unreliable, MessageId.PlayerHeal);
        message.AddUShort(id);
        message.AddFloat(value);
        foreach (Player player in List.Values) if (player.id != 1)
                NetworkManager.Singleton.Server.Send(message, player.id);
    }

    public void Reset()
    {
        health = maxHealth;
        if (!isLocal) gameObject.SetActive(false);
            else{GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            GetComponent<PlayerMovement>().enabled = true;
        }
        EntityMP.inSession = false;
    }

    #endregion

}
