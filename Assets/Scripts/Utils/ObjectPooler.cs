using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    #region Singleton
    public static ObjectPooler Instance;

    private void Awake()
    {
        Instance = this;
    }
    #endregion

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;
    public List<GameObject> usedQueue = new List<GameObject>();
    public Dictionary<string, Queue<GameObject>> poolDict;
    // Start is called before the first frame update
    void Start()
    {
        poolDict = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for(int i = 0; i < pool.size; ++i)
            {
                AddObject(pool.prefab, objectPool, pool.tag);
            }

            poolDict.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool (string tag, Vector2 position, float scale)
    {
        if (!poolDict.ContainsKey(tag))
        {
            Debug.LogWarning("No pool with tag" + tag);
            return null;
        }

        Queue<GameObject> objPool = poolDict[tag];

        if (objPool.Count <= 1)
        {
            Pool pool = GetPoolByTag(tag);
            while (objPool.Count < pool.size * 0.1) AddObject(pool.prefab, objPool, tag);
        }

        GameObject obj = objPool.Dequeue();

        obj.SetActive(true);
        obj.transform.position = new Vector3(position.x, position.y, GameManager.Instance.player.transform.position.z);
        obj.transform.localScale = new Vector3(scale, scale, scale);

        usedQueue.Add(obj);

        return obj;
    }

    public void ResetPool()
    {
        List<GameObject> temp = new List<GameObject>();
        foreach (GameObject gameObject in usedQueue)
        {
            if (gameObject != null)
            {
                gameObject.SetActive(false);
                gameObject.transform.localScale = new Vector3(1, 1, 1);
                gameObject.transform.rotation = Quaternion.identity;
                if (poolDict.ContainsKey(gameObject.name)) poolDict[gameObject.name].Enqueue(gameObject);
                else
                {
                    temp.Add(gameObject);
                    Debug.LogWarning("ObjectPooler: No pool named " + gameObject.name);
                }
            }
        }
        usedQueue = temp;
    }

    public GameObject Create(GameObject gameObject)
    {
        GameObject obj = Instantiate(gameObject);
        obj.SetActive(false);
        usedQueue.Add(obj);
        return obj;
    }

    private void AddObject(GameObject prefab, Queue<GameObject> objectPool, string tag)
    {
        GameObject obj = Instantiate(prefab);
        obj.layer = LayerMask.NameToLayer("Terrain");
        obj.SetActive(false);
        obj.name = tag;
        objectPool.Enqueue(obj);
    }

    private Pool GetPoolByTag(string tag)
    {
        foreach(Pool pool in pools)
        {
            if (pool.tag == tag) return pool;
        }
        return null;
    }

}
