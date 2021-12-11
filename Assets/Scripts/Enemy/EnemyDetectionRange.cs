using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetectionRange : MonoBehaviour
{
    public GameObject parent;

    CircleCollider2D col;

    public float range = 15;

    void Start()
    {
        col = GetComponent<CircleCollider2D>();
        col.radius = range;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name == "Player") parent.GetComponent<Enemy>().target = collision.gameObject.transform;
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player") parent.GetComponent<Enemy>().target = null;
    }

}
