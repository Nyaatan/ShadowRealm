using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Entity
{
    public Animator animator;
    public AnimatorOverrideController[] overrideControllers;
    public Sprite[] sprites;
    public AnimationOverride overrider;

    public Inventory glyphs;
    public Glyph glyphInHand;

    public bool isFlipped = false;

    public Transform player;
    public float attackRange = 15f;

    public int animatorOverrideId;

    [SerializeField]
    public BossData data;

    public HealthBar bar;
    public float dropChance = 0.5f;

    public void LookAtPlayer()
    {
        Vector3 flipped = transform.localScale;
        flipped.z *= -1f;

        if(transform.position.x < player.position.x && isFlipped)
        {
            transform.localScale = flipped;
            transform.Rotate(0, 180, 0);
            isFlipped = false;
        } else if(transform.position.x > player.position.x && !isFlipped)
        {
            transform.localScale = flipped;
            transform.Rotate(0, 180, 0);
            isFlipped = true;
        }
    }

    internal void NextPhase()
    {
        SetData(data.nextPhaseData);
    }

    public override void Start()
    {
        base.Start();
        player = GameManager.Instance.player.transform;

        SetData(data);

        GetComponent<SpriteRenderer>().sprite = sprites[animatorOverrideId];
        animator = GetComponent<Animator>();
        SetAnimations(animatorOverrideId);

        health = data.maxHealth;
        maxHealth = data.maxHealth;
        bar.SetMaxHealth((int)data.maxHealth);
        bar.SetHealth(health);

    }

    public void SetData(BossData data)
    {
        inventory.size = data.nGlyphs;

        for (int i = inventory.count; i < inventory.size; ++i)
        {
            GameObject instance = Glyph.GetRandom(data.minMaxGlyphTier, data.minVectorValues, data.maxVectorValues, false);
            instance.transform.position = new Vector3(-1000, -1000, -1000);
            instance.GetComponent<Rigidbody2D>().simulated = false;
            if (instance.GetComponent<Glyph>().data.nature == GlyphData.Nature.SELF) instance.GetComponent<Glyph>().data.nature = GlyphData.Nature.SINGLE_TARGET;
            inventory.Add(instance.GetComponent<Glyph>());
        }

        attackRange = glyphInHand.spellPrototype.data.range * glyphInHand.spellPrototype.data.speed * 0.7f;
        glyphInHand.spellPrototype.data.damage *= data.bossPowerModifier;

        transform.localScale = data.scale;

        glyphInHand = inventory.Get(0) as Glyph;
        Color colour = glyphInHand.CalculateSpellData().colours[0].colour;
        colour.a = 255;

        animatorOverrideId = (int) data.animationSet;
        Debug.Log(data.animationSet);

        GetComponent<SpriteRenderer>().color = colour;
        foreach (Affinity affinity in GameManager.Instance.affinities) if (affinity.element == (inventory.Get(0) as Glyph).data.element[0]) { this.affinity = affinity; break; }

    }

    public void SetAnimations(int i)
    {
        overrider.SetAnimations(overrideControllers[i]);
    }

    public void Attack()
    {
        for (int i = 0; i < data.burstCount; ++i)
        {
            glyphInHand.Use(this, player.gameObject);
            SelectNextGlyph();
            new WaitForSeconds(data.burstInterval);
        }
    }

    void SelectNextGlyph()
    {
        if (data.castSequence == CastSequence.SEQUENTIAL) glyphInHand = inventory.Get(inventory.pickables.FindIndex(a => a == glyphInHand) % inventory.count) as Glyph;
        else if (data.castSequence == CastSequence.RANDOM) glyphInHand = inventory.Get(Random.Range(0, inventory.count)) as Glyph;
    }

    public void SpecialAttack()
    {
        for (int i = 0; i < data.burstCount; ++i)
        {
            if (data.specialAttack != null) data.specialAttack(this);

            else glyphInHand.Use(this, player.gameObject);
            SelectNextGlyph();
            new WaitForSeconds(data.burstInterval);
        }
    }

    public override void ReceiveDamage(float value, GlyphData.Element[] elements)
    {
        floatColor = Color.yellow;
        foreach (GlyphData.Element element in elements)
            if (element == affinity.strength)
            {
                value *= 0.5f;
                floatColor = Color.gray;
            }
            else if (element == affinity.weakness)
            {
                value *= 2f;
                floatColor = Color.red;
            }
        base.ReceiveDamage(value, elements);
    }

    public override void onReceiveDamage()
    {
        base.onReceiveDamage();
        bar.SetHealth(health);
        if(health <= data.phaseChangeThreshold * maxHealth && data.nextPhaseData != null)
        {
            animator.SetTrigger("PhaseChange");
        }
    }

    public override void OnHeal()
    {
        base.OnHeal();
        bar.SetHealth(health);
    }

    public override void OnDeath()
    {
        base.OnDeath();
        animator.SetTrigger("Death");
        foreach (Pickable glyph in inventory.pickables)
        {
            if (Random.Range(0f, 1f) < dropChance)
            {
                ((Glyph)glyph).spellPrototype.data.damage /= data.bossPowerModifier;
                ((Glyph)glyph).spellPrototype.data.heal /= data.bossPowerModifier;
                GameObject obj = glyph.gameObject;
                obj.transform.position = transform.position;
                obj.GetComponent<Rigidbody2D>().simulated = true;
            }
        }
        GameManager.Instance.dungeon.BossKilled();
    }

    public void OnEnable()
    {
        Start();
    }
}
