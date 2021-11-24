using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickable : MonoBehaviour
{
    public bool isOnCooldown = false;
    public float cooldown = 1f;
    float timeLeft = 0f;
    

    // Update is called once per frame
    public virtual void Update()
    {
        if (isOnCooldown)
        {
            if (timeLeft <= 0)
            {
                isOnCooldown = false;
                timeLeft = cooldown;
            }
            else timeLeft -= Time.deltaTime;
        }
    }

    public virtual void Use(Entity user, GameObject target) { }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name == "Player") collision.gameObject.GetComponent<Player>().Pick(this);
    }
}
