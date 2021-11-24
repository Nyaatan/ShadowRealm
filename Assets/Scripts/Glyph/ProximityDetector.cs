using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityDetector : MonoBehaviour
{
    float range;
    Vector3 initScale;
    List<GameObject> detections = new List<GameObject>();
    System.Action<List<GameObject>> callback;
    bool start = false;
    float step = 0f;
    int maxCount = -1;
    private List<LayerMask> layers;

    // Start is called before the first frame update
    void Start()
    {
        initScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (start)
        {
            step += 0.1f;
            transform.localScale = Vector3.Lerp(initScale, initScale * range, step);
            if (step >= 1 || (detections.Count >= maxCount && maxCount != -1))
            {
                step = 0f;
                start = false;
                callback(detections);
            }
        }
    }

    public void Init(float range, int maxCount, System.Action<List<GameObject>> callback, List<LayerMask> layers)
    {
        this.range = range;
        this.callback = callback;
        this.maxCount = maxCount;
        this.layers = layers;
        GetComponent<Collider2D>().enabled = true;
        start = true;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject);
        if (start)
            if (layers.Contains(collision.gameObject.layer))
            {
                Debug.Log(collision.gameObject);
                detections.Add(collision.gameObject);
            }
    }
}
