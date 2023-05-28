using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TTL : MonoBehaviour
{
    public float ttl = 10f;
    public TimedObject parent;
    public bool start = false;
    void Update()
    {
        if (start)
        {
            ttl -= Time.deltaTime;
            if (ttl <= 0)
            {
                if (parent != null) parent.OnTtlEnd();
                else Destroy(gameObject);
                Destroy(this);
            }
        }
    }

    public void Start()
    {
        if (parent == null)
        {
            start = true;
        }
    }
}

public interface TimedObject
{
    public void OnTtlEnd();
}