using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingComparator : MonoBehaviour
{
    public GameObject obj;
    public List<GameObject> objCreated;
    public int count = 1000;
    public bool dest = false;
    void Start()
    {
        System.DateTime start = System.DateTime.Now;
        for(int i = 0; i<count;++i) objCreated.Add(Instantiate(obj));
        Debug.Log((System.DateTime.Now - start).TotalMilliseconds);

        start = System.DateTime.Now;
        foreach (GameObject objc in objCreated) objc.transform.position = new Vector3(100, 100, 100);
        Debug.Log((System.DateTime.Now - start).TotalMilliseconds);
        Destroy(gameObject);
        
    }
}
