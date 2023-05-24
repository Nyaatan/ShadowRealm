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
    public ParticleSystem particles;

    // Start is called before the first frame update
    void Start()
    {
        initScale = transform.localScale;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (start)
        {
            step += 0.05f;
            transform.localScale = Vector3.Lerp(initScale, initScale * range, step);
            Debug.Log(range);
            var shape = particles.shape;
            shape.radius = transform.localScale.x*0.09f;
            if (step >= 1 || (detections.Count >= maxCount && maxCount != -1))
            {
                step = 0f;
                start = false;
                callback(detections);
                var emission = particles.emission;
                emission.rateOverTime = 0;
                shape.radius = 0.1f;
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
        //collider2D.radius = 1;
        start = true;
        var emission = particles.emission;
        emission.rateOverTime = 12000;
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
