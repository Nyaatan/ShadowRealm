using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dungeon : MonoBehaviour
{
    public List<List<Location>> rooms = new List<List<Location>>();


    public Dungeon(Schema schema, Dictionary<Schema, Dictionary<string, int>> extraSchemas)
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public interface Location { }