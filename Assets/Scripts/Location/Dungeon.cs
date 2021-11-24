using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dungeon : MonoBehaviour, Location
{
    public int currentRoomId;

    public Dictionary<Node, Location> rooms;

    [SerializeField]
    public Schema schema;

    [SerializeField]
    public List<System.Tuple<Schema, Dictionary<string, int>>> extraSchemas;

    public Graph nodeGraph;

    public float scale = 1f;

    public bool isCreated = false;

    public GameObject Tavern;

    public GameObject defaultSpawn;

    public int nodes;

    public void Create()
    {

        Debug.Log(schema.GraphData.nodes);
        Debug.Log("OOGABOOGA");
        currentRoomId = 1;
        schema.GeneratorData.alifConstant = Random.Range(0f, 1f);
        schema.GraphData.nodes = Random.Range(schema.minRooms, schema.maxRooms);

        nodeGraph = new Graph(schema.GraphData);
        rooms = new Dictionary<Node, Location>();
        //Room firstRoom = Room.Create(GeneratorData, schema.RoomData, 0);
        //rooms.Add(graph.nodes[0], firstRoom);

        foreach (Node node in nodeGraph.nodes)
        {
            Vector2 size = GetRandomRoomSize();
            RoomData data = schema.RoomData;
            data.size = size;
            if (schema.noRoofOnTop && node.neighbours.up == null) data.roof = false;
            else data.roof = true;
            if (node.neighbours.left == null) data.leftWall = true;
            if (node.neighbours.right == null) data.rightWall = true;

            Room room = null;

            switch (node.type)
            {
                case NodeType.NORMAL:
                    room = Room.Create(this, schema.GeneratorData, data, node.layer, node);
                    break;
                case NodeType.BOSS:
                    room = Room.Create(this, schema.GeneratorData, schema.bossRoomData, node.layer, node);
                    break;
                case NodeType.TREASURE:
                    data.isTreasure = true;
                    room = Room.Create(this, schema.GeneratorData, data, node.layer, node);
                    data.isTreasure = false;
                    break;
                case NodeType.CHALLENGE:
                    data.isChallenge = true;
                    room = Room.Create(this, schema.GeneratorData, data, node.layer, node);
                    data.isChallenge = false;
                    break;
            }

            rooms.Add(node, room);
        }

        foreach (Node node in rooms.Keys) ((Room)rooms[node]).CalculateWaypoints();

        isCreated = true;
        nodes = nodeGraph.nodes.Count;
    }

    private Vector2 GetRandomRoomSize()
    {
        return new Vector2(Random.Range(schema.minRoomSize.x, schema.maxRoomSize.x), Random.Range(schema.minRoomSize.y, schema.maxRoomSize.y));
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public Vector2 ScaledVector(float x, float y) { return new Vector2(x * scale, y * scale); }

    public Room GetRoomById(int id) {
        return (Room)rooms[nodeGraph.GetNodeById(id)]; 
    }

    public void Enter(int id=1)
    {
        if(id == 0)
        {
            Tavern.SetActive(true);
            return;
        }
        if (!isCreated) Create();
        Tavern.SetActive(false);
        Room lastRoom = GetRoomById(currentRoomId);
        Room room = GetRoomById(id);
        Debug.Log(GetRoomById(2).node.type);

        currentRoomId = id;
        ObjectPooler.Instance.ResetPool();

        int xpos = 0;
        if(room.noiseValuesFloor != null)
        foreach(int perlinVal in room.noiseValuesFloor)
        {
            ObjectPooler.Instance.SpawnFromPool("Top", ScaledVector(xpos, perlinVal), scale);
            for (int ypos = perlinVal - 1; ypos >= 0; --ypos)
            {
                ObjectPooler.Instance.SpawnFromPool("Deep", ScaledVector(xpos, ypos), scale);
                // Debug.Log(ypos);
            }
            ++xpos;
        }

        xpos = 0;
        if(room.noiseValuesRoof != null)
        foreach (int perlinVal in room.noiseValuesRoof)
        {
            GameObject block = ObjectPooler.Instance.SpawnFromPool("Top", ScaledVector(xpos, (int)room.RoomData.size.y - perlinVal), scale);
            block.transform.rotation *= Quaternion.Euler(0, 0, 180);

            for (int ypos = (int) room.RoomData.size.y - perlinVal + 1; ypos <= room.RoomData.size.y; ++ypos)
            {
                ObjectPooler.Instance.SpawnFromPool("Deep", ScaledVector(xpos, ypos), scale);
                // Debug.Log(ypos);
            }
            ++xpos;
        }

        //if (room.noiseValuesWallLeft != null)
        //    foreach (int perlinVal in room.noiseValuesWallLeft)
        //    {
        //        ObjectPooler.Instance.SpawnFromPool("Top", new Vector2(xpos, perlinVal));
        //        for (int ypos = perlinVal - 1; ypos >= 0; --ypos)
        //        {
        //            ObjectPooler.Instance.SpawnFromPool("Deep", new Vector2(xpos, ypos));
        //            // Debug.Log(ypos);
        //        }
        //        ++xpos;
        //    }

        //if (room.noiseValuesWallRight != null)
        //    foreach (int perlinVal in room.noiseValuesFloor)
        //    {
        //        ObjectPooler.Instance.SpawnFromPool("Top", new Vector2(xpos, perlinVal));
        //        for (int ypos = perlinVal - 1; ypos >= 0; --ypos)
        //        {
        //            ObjectPooler.Instance.SpawnFromPool("Deep", new Vector2(xpos, ypos));
        //            // Debug.Log(ypos);
        //        }
        //        ++xpos;
        //    }

        Debug.Log(room.objects.Count);
        foreach(Vector2 key in room.objects.Keys)
        {
            GameObject obj = room.objects[key];
            obj.transform.position = key * scale;
            obj.transform.localScale = new Vector3(scale, scale, scale);
            obj.SetActive(true);
        }

        GameManager.Instance.player.transform.position = (lastRoom.spawns[((Waypoint)GameManager.Instance.player.GetComponent<Player>().lastInteraction).direction] + new Vector2(0, 1)) * scale;
        GameManager.Instance.player.GetComponent<Player>().lastInteraction = null;
        GameManager.Instance.mainCamera.GetComponent<CameraController>().warpToTarget();

        Debug.Log(nodeGraph.GetNodeById(room.node.id).type);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R)) { Create(); Enter(1); }

    }
}

public interface Location {
    public void Enter(int id=1);
}