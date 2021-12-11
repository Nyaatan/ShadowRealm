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
            step += 0.05f;
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
        CircleCollider2D collider2D = GetComponent<CircleCollider2D>();
        collider2D.enabled = true;
        collider2D.radius = 1;
        start = true;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (start)
            foreach(LayerMask layer in layers) if (collision.gameObject.layer == Mathf.Log(layer, 2))
            {
                //Debug.Log(collision.gameObject);
                detections.Add(collision.gameObject);
            }
    }
}
