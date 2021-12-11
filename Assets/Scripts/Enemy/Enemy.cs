using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Enemy : Entity
{
    public Animator animator;
    public AnimatorOverrideController[] overrideControllers;
    public Sprite[] sprites;
    public AnimationOverride overrider;

    public EnemyType type = EnemyType.GENERIC;
    public Transform target;
    public bool isActive = false;
    public Transform footTracker;
    public CharacterController2D controller;
    public Transform gfx;
    public GameObject detectionRangeObj;
    bool jump = false;
    float horizontalMove = 0f;
    CircleCollider2D detectionRange;
    public List<State> allowedStates = new List<State> { State.CLOSING_IN, State.RUNNING_AWAY, State.KEEPING_DISTANCE };
    public float stateRollInterval = 3f;
    public State state;
    public float closeDistance = 3f;
    public float optimalDistance = 8f;
    public float farDistance = 12f;
    public float targetDistance = 3f;

    public EnemyData data;

    public Dictionary<State, System.Action<Enemy>> specialBehaviours = new Dictionary<State, System.Action<Enemy>>();

    bool targetObscured = false;

    public float attackCooldown = 3f;
    public float targetObscuredCooldown = 1f;
    private bool attackOnCooldown = false;

    public int burstCount = 1;

    public float dropChance = 0.1f;

    public ParticleSystem particles;

    public List<Collider2D> ignoredCollisions = new List<Collider2D>();

    public float powerScale = 0.7f;

    public float flyHeight = 10f;

    
    public void RollState()
    {
        state = allowedStates[Random.Range(0, allowedStates.Count)];
    }

    public override void Start()
    {

        Collider2D collider2D = GetComponent<Collider2D>();
        foreach (Collider2D ignored in ignoredCollisions) Physics2D.IgnoreCollision(collider2D, ignored);

        InvokeRepeating("RollState", stateRollInterval, 1);

        GameObject obj = Glyph.GetRandom(data.minMaxGlyphTier, data.minVectorValues, data.maxVectorValues, false);
        if (obj.GetComponent<Glyph>().data.nature == GlyphData.Nature.SELF) obj.GetComponent<Glyph>().data.nature = GlyphData.Nature.SINGLE_TARGET;
        obj.transform.position = new Vector3(0, 0, -1000);
        obj.GetComponent<Rigidbody2D>().simulated = false;
        pickableInHand = obj.GetComponent<Glyph>();

        (pickableInHand as Glyph).spellPrototype.data.damage *= data.powerModifier;
        (pickableInHand as Glyph).spellPrototype.data.heal *= data.powerModifier;

        var main = particles.main;
        Color color = (pickableInHand as Glyph).CalculateSpellData().colours[0].colour;
        color.a = 255f;
        main.startColor = color;

        SetData();

        if (type == EnemyType.FLYING) GetComponent<Rigidbody2D>().gravityScale = 0f;

        overrider.SetAnimations(overrideControllers[data.animatorOverriderId]);
        GetComponent<SpriteRenderer>().sprite = sprites[data.animatorOverriderId];

        InvokeRepeating("AttackLoop", attackCooldown, burstCount);
    }

    private void SetData()
    {
        transform.localScale = data.scale;
        Debug.Log(data.scale);
        maxHealth = data.maxHealth;
        if (data.allowedTypes.Count > 0) type = data.allowedTypes[Random.Range(0, data.allowedTypes.Count)];
        else type = EnemyType.GENERIC;
        Debug.Log(data.animatorOverriderId);
        foreach (Affinity affinity in GameManager.Instance.affinities) if (affinity.element == (pickableInHand as Glyph).data.element[0]) { this.affinity = affinity; break; }
    }

    // Update is called once per frame
    public void FixedUpdate()
    {
        controller.Move(horizontalMove * Time.fixedDeltaTime, false, jump);
        Debug.Log(horizontalMove * Time.fixedDeltaTime);
        jump = false;
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

    void CheckTargetInSight()
    {
        targetObscured = Physics2D.Raycast(footTracker.position, target.position, 3, controller.m_WhatIsGround).collider != null;
    }

    private void Move()
    {
        horizontalMove = (target.position.x > transform.position.x ? 1 : -1) * data.speed;
        if (Vector2.Distance(target.position, transform.position) < targetDistance - 0.5f) horizontalMove *= -1;
        else if (Vector2.Distance(target.position, transform.position) < targetDistance + 0.5f) horizontalMove = 0;
        if (targetObscured) jump = true;

        if (transform.position.y > flyHeight) GetComponent<Rigidbody2D>().AddForce(new Vector2(0, -1f));
        else if (transform.position.y < flyHeight) GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 1f));

        if (jump) animator.SetTrigger("Jump");
        animator.SetFloat("Speed", horizontalMove);

        //Debug.LogWarning(horizontalMove);
    }

    void AttackLoop()
    {
        if (!targetObscured && isActive && target != null)
        {
            CastSpell(CreateTarget());
        }
    }

    void AILogic()
    {
        if (target != null && isActive)
        {
            HandleState();
            CheckTargetInSight();
            Move();
            animator.ResetTrigger("Jump");
        }
        else horizontalMove = 0f;
    }

    public override void Update()
    {
        base.Update();
        AILogic();
    }

    public void HandleState()
    {
        if (specialBehaviours.ContainsKey(state)) specialBehaviours[state](this);
        else
        {
            switch (state)
            {
                case State.CLOSING_IN:
                    targetDistance = closeDistance;
                    break;
                case State.KEEPING_DISTANCE:
                    targetDistance = optimalDistance;
                    break;
                case State.RUNNING_AWAY:
                    targetDistance = farDistance;
                    break;
            }
        }    
    }

    public GameObject CreateTarget()
    {
        GameObject target = new GameObject("Target");
        TTL ttl = target.AddComponent<TTL>();
        ttl.ttl = 10f;
        ttl.start = true;
        target.transform.position = this.target.transform.position;
        target.SetActive(true);
        return target;
    }

    void CastSpell(GameObject target)
    {
        animator.SetTrigger("Attack");
        pickableInHand.Use(this, target);
        animator.ResetTrigger("Attack");
    }

    public override void OnDeath()
    {
        animator.SetTrigger("Death");
        isActive = false;
        
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().simulated = false;

        if(dropChance > Random.Range(0f, 1f))
        {
            pickableInHand.gameObject.transform.position = transform.position;
            (pickableInHand as Glyph).spellPrototype.data.damage /= powerScale;
            (pickableInHand as Glyph).spellPrototype.data.heal /= powerScale;
            pickableInHand.gameObject.GetComponent<Rigidbody2D>().gravityScale = 1;
            pickableInHand.gameObject.GetComponent<Rigidbody2D>().simulated = true;

            GameManager.Instance.dungeon.roomObjectsOnScreen.Add(pickableInHand.gameObject);
        }
        else
        {
            if(pickableInHand != null) Destroy(pickableInHand.gameObject);
        }

        //gameObject.SetActive(false);
    }
}

public enum State
{
    CLOSING_IN, RUNNING_AWAY, KEEPING_DISTANCE
}