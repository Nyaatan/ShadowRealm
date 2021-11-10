using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Schema : ScriptableObject
{
    public Environment environment = Environment.MEADOW;
    public int minHeight = 10;
    public int maxHeight = 20;
    public Vector2 minRoomSize = new Vector2(256, 256);
    public Vector2 maxRoomSize = new Vector2(1024, 256);
    public int minFloors = 1;
    public int maxFloors = 1;
    public Dictionary<Schema, float> sublocationChances = new Dictionary<Schema, float>();
}
