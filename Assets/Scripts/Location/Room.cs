using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Room : Location
{
    public RoomData RoomData;
    public Dungeon parent;
    public int nFloor = 0;
    public int xStart = 0;
    public int yStart = 0;
    public Node node;

    public List<int> noiseValuesFloor;
    public List<int> noiseValuesRoof;
    public List<int> noiseValuesWallLeft;
    public List<int> noiseValuesWallRight;

    public List<Room> neighbors = new List<Room>();

    public Dictionary<Vector2, GameObject> objects = new Dictionary<Vector2, GameObject>();
    public Dictionary<Vector2, GameObject> bgObjects = new Dictionary<Vector2, GameObject>();
    public Dictionary<Vector2, GameObject> enemies = new Dictionary<Vector2, GameObject>();

    public Dictionary<Direction, Vector2> spawns;

    public static Room Create(Dungeon parent, GeneratorData generatorData, RoomData roomData, int nFloor, Node node, int xStart = 0, int yStart = 0)
    {
        Room room = new Room();
        room.spawns = new Dictionary<Direction, Vector2>();
        room.spawns.Add(Direction.NONE, parent.defaultSpawn.transform.position);
        room.parent = parent;
        room.RoomData = roomData;
        room.nFloor = nFloor;
        room.xStart = xStart;
        room.yStart = yStart;
        room.node = node;

        Debug.Log(roomData.roof);

        Random.InitState((int) System.DateTime.Now.ToFileTime());

        room.noiseValuesFloor = room.RoomData.floor ? PerlinGenerator.Generate(generatorData, room.RoomData.size, room.RoomData.maxFloorHeight, room.RoomData.minFloorHeight, 1, xStart) : null;
        room.noiseValuesRoof = room.RoomData.roof ? PerlinGenerator.Generate(generatorData, room.RoomData.size, room.RoomData.maxRoofOffset, room.RoomData.minRoofOffset, 1, xStart) : null;
        room.noiseValuesWallLeft = room.RoomData.leftWall ? PerlinGenerator.Generate(generatorData, room.RoomData.size, room.RoomData.maxWallOffset, room.RoomData.minWallOffset, 2, xStart) : null;
        room.noiseValuesWallRight = room.RoomData.rightWall ? PerlinGenerator.Generate(generatorData, room.RoomData.size, room.RoomData.maxWallOffset, room.RoomData.minWallOffset, 2, xStart) : null;
        
        return room;
    }

    public void CalculateWaypoints()
    {
        if(node.neighbours.left != null)
        {
            GameObject waypoint = ObjectPooler.Instance.Create(RoomData.waypointPrefab);
            waypoint.GetComponentInChildren<Waypoint>().dungeon = parent.gameObject;
            waypoint.GetComponentInChildren<Waypoint>().destination = parent.GetRoomById(node.neighbours.left.id);
            Vector2 coords = new Vector2(RoomData.waypointWallOffset + RoomData.maxWallOffset, noiseValuesFloor[RoomData.waypointWallOffset + RoomData.maxWallOffset] + 0.5f);
            objects.Add(coords, waypoint);
            waypoint.GetComponentInChildren<Waypoint>().direction = Direction.LEFT;
            parent.GetRoomById(node.neighbours.left.id).spawns.Add(Direction.RIGHT, coords);
        }
        if (node.neighbours.right != null)
        {
            GameObject waypoint = ObjectPooler.Instance.Create(RoomData.waypointPrefab);
            waypoint.GetComponentInChildren<Waypoint>().dungeon = parent.gameObject;
            waypoint.GetComponentInChildren<Waypoint>().destination = parent.GetRoomById(node.neighbours.right.id);
            Vector2 coords = new Vector2(RoomData.size.x - RoomData.waypointWallOffset - RoomData.maxWallOffset, noiseValuesFloor[(int)RoomData.size.x - RoomData.waypointWallOffset - RoomData.maxWallOffset - RoomData.maxWallOffset] + 0.5f);
            objects.Add(coords, waypoint);
            waypoint.GetComponentInChildren<Waypoint>().direction = Direction.RIGHT;
            parent.GetRoomById(node.neighbours.right.id).spawns.Add(Direction.LEFT, coords);
        }
        if (node.neighbours.up != null)
        {
            GameObject waypoint = ObjectPooler.Instance.Create(RoomData.waypointPrefab);
            waypoint.GetComponentInChildren<Waypoint>().dungeon = parent.gameObject;
            waypoint.GetComponentInChildren<Waypoint>().destination = parent.GetRoomById(node.neighbours.up.id);

            List<int> maxIndices = new List<int>();
            int max = noiseValuesFloor.Max();
            for (int i = 0; i < noiseValuesFloor.Count; ++i) if (noiseValuesFloor[i] == max) maxIndices.Add(i);

            int waypointIndex = Mathf.Max(Mathf.Min(maxIndices[Random.Range(0, maxIndices.Count)], (int)RoomData.size.x + RoomData.waypointWallOffset), RoomData.waypointWallOffset);

            Vector2 coords = new Vector2(waypointIndex, noiseValuesFloor[waypointIndex] + 0.5f);
            try
            {
                objects.Add(coords, waypoint);
            }
            catch (System.ArgumentException) {
                objects.Add(coords + new Vector2(1, 0), waypoint);
            }
            waypoint.GetComponentInChildren<Waypoint>().direction = Direction.UP;
            parent.GetRoomById(node.neighbours.up.id).spawns.Add(Direction.DOWN, coords);
        }
        if (node.neighbours.down != null)
        {
            GameObject waypoint = ObjectPooler.Instance.Create(RoomData.waypointPrefab);
            waypoint.GetComponentInChildren<Waypoint>().dungeon = parent.gameObject;
            waypoint.GetComponentInChildren<Waypoint>().destination = parent.GetRoomById(node.neighbours.down.id);

            List<int> maxIndices = new List<int>();
            int max = noiseValuesFloor.Min();
            for (int i = 0; i < noiseValuesFloor.Count; ++i) if (noiseValuesFloor[i] == max) maxIndices.Add(i);


            int waypointIndex = Mathf.Max(Mathf.Min(noiseValuesFloor[maxIndices[Random.Range(0, maxIndices.Count)]], (int)RoomData.size.x - RoomData.waypointWallOffset), RoomData.waypointWallOffset);

            Vector2 coords = new Vector2(waypointIndex, noiseValuesFloor[waypointIndex] + 0.5f);
            
            try
            {
                objects.Add(coords, waypoint);
            }
            catch (System.ArgumentException)
            {
                objects.Add(coords + new Vector2(1, 0), waypoint);
            }

            waypoint.GetComponentInChildren<Waypoint>().direction = Direction.DOWN;
            parent.GetRoomById(node.neighbours.down.id).spawns.Add(Direction.UP, coords);
        }
    }

    public void SpawnObjects()
    {


        GeneratorData data = new GeneratorData();
        data.alifConstant = Random.Range(0f, 1f);
        data.octaves = 4;

        foreach(int i in PerlinGenerator.Generate(data, RoomData.size, 1, 100, 1, xStart))
        {
            Debug.Log(i);
        }
    }

    public void Enter(int id=1)
    {
        parent.Enter(node.id);
    }
}
