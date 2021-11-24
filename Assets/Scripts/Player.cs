using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    public Interactable lastInteraction;
    public int selected = 0;

    float scrollLock;
    public float scrollLockDefault = 0.1f;

    public Animator animator;

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

    // Update is called once per frame
    public override void Update()
    {
        if (pickableInHand != null && Input.GetKeyDown(KeyCode.Mouse0) && !pickableInHand.isOnCooldown) {
           
            if (pickableInHand is Glyph) {
                if (((Glyph)pickableInHand).data.nature == GlyphData.Nature.SELF) animator.SetTrigger("SpellCastSelf");
                else animator.SetTrigger("SpellCast");
             }
            pickableInHand.Use(this, CreateTarget());
            ResetTriggers();
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
        }
        if (selected < 0) selected = Mathf.Max(inventory.count - 1, 0);
        if (selected >= inventory.size) selected = 0;
        if (scrollLock > 0f) scrollLock -= Time.deltaTime;

        //Debug.Log(selected);
        pickableInHand = inventory.Get(selected);
        //Debug.Log(pickableInHand);
        //Debug.Log(inventory.pickables.Count);
    }

    public void Pick(Pickable obj)
    {
        if(inventory.Add(obj))
        {
            obj.gameObject.transform.position = new Vector3(-100, -100, transform.position.z);
            obj.gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
        }
        foreach(Pickable key in inventory.inventory.Keys) Debug.Log(inventory.inventory[key]);
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

}
