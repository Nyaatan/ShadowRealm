using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Schema_", menuName = "DungeonSchema")]
[SerializeField]
public class Schema : ScriptableObject
{
    public Environment environment = Environment.MEADOW;
    public RoomData RoomData;
    public Vector2 minRoomSize = new Vector2(256, 256);
    public Vector2 maxRoomSize = new Vector2(1024, 256);
    public int minFloors = 1;
    public int maxFloors = 1;
    public Dictionary<Schema, float> sublocationChances = new Dictionary<Schema, float>();
    public int minRooms = 10;
    public int maxRooms = 20;
    public bool noRoofOnTop = true;
    public GeneratorData GeneratorData;
    public GraphData GraphData;
    public RoomData bossRoomData;
    public RoomData treasureRoomData;
    public RoomData challengeRoomData;
}