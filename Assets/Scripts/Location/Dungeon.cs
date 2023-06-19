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

    bool ChallengeActivated = false;
    ChallengeRoomActivator ChallengeRoomActivator;

    public static Dungeon Instance;

    public Vector2 spawnPos = Vector2.zero;

    public void Awake(){
        Instance = this;
    }

    internal void ActivateChallenge(ChallengeRoomActivator activator)
    {
        foreach (GameObject obj in roomObjectsOnScreen) if(obj != null) if (obj.GetComponentInChildren<Waypoint>() != null)
            {
                obj.GetComponentInChildren<Waypoint>().interactable = false;

            }
        SpawnEnemies(GetRoomById(currentRoomId));
        ChallengeRoomActivator = activator;
        ChallengeActivated = true;
        foreach (GameObject obj in enemiesOnScreen)
        {
            obj.SetActive(true);
            obj.GetComponent<Enemy>().target = GameManager.Instance.player.transform;
        }
    }

    public void FixedUpdate()
    {
        if (ChallengeActivated)
        {
            bool done = true;
            foreach(GameObject enemy in enemiesOnScreen)
            {
                try
                {
                    if (enemy.activeSelf) done = false;
                }
                catch { }
            }
            if (done) ChallengeWon();
        }
    }

    public GameObject Tavern;

    public GameObject defaultSpawn;

    public int nodes;

    public List<GameObject> enemiesOnScreen = new List<GameObject>();
    public List<GameObject> roomObjectsOnScreen = new List<GameObject>();

    public Entity enemyPrototype;

    public Collider2D borderLeft;
    public Collider2D borderRight;

    public GameObject bossObject;

    public Transform tavernSpawn;


    internal void ChallengeWon()
    {
        ChallengeActivated = false;
        foreach (GameObject obj in roomObjectsOnScreen) if (obj.GetComponentInChildren<Waypoint>() != null) obj.GetComponentInChildren<Waypoint>().interactable = true;
        ChallengeRoomActivator.OnVicotry();
    }

    public void Create()
    {
        currentRoomId = 1;
        schema.GeneratorData.alifConstant = Random.Range(0f, 1f);

        schema.GraphData.nodes = Random.Range(schema.minRooms, schema.maxRooms + 1);
        schema.GraphData.layers = Random.Range(schema.minFloors, schema.maxFloors + 1);

        nodeGraph = new Graph(schema.GraphData);
        rooms = new Dictionary<Node, Location>();

        foreach (Node node in nodeGraph.nodes)
        {
            //Debug.Log(node);
            Vector2 size = GetRandomRoomSize();
            RoomData data = schema.RoomData.Copy();
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
                    room = Room.Create(this, schema.GeneratorData, schema.treasureRoomData, node.layer, node);
                    break;
                case NodeType.CHALLENGE:
                    room = Room.Create(this, schema.GeneratorData, schema.challengeRoomData, node.layer, node);
                    break;
            }

            rooms.Add(node, room);
        }

        foreach (Node node in rooms.Keys) ((Room)rooms[node]).CalculateWaypoints();
        isCreated = true;
        nodes = nodeGraph.nodes.Count;
       // Debug.Log("DUPA");
        //Debug.Log(schema.GeneratorData.alifConstant);
    }

    internal void BossKilled()
    {
        Room room = GetRoomById(currentRoomId);
        GameObject Waypoint = Instantiate(room.RoomData.waypointPrefab);
        Waypoint.transform.position = new Vector2(room.RoomData.size.x / 2, room.noiseValuesFloor[(int)room.RoomData.size.x / 2] + 0.5f) * scale;
        Waypoint.SetActive(true);
        Waypoint.GetComponentInChildren<Waypoint>().interactable = true;
        Waypoint.GetComponentInChildren<Waypoint>().specialAction = EnterTavern;

    }

    public void EnterTavern(int i=0)
    {
        foreach (GameObject obj in roomObjectsOnScreen) obj.SetActive(false);
        foreach (GameObject enemy in enemiesOnScreen) enemy.SetActive(false);
        GameManager.Instance.player.transform.position = tavernSpawn.position;

        GameManager.Instance.mainCamera.GetComponent<CameraController>().warpToTarget();
        isCreated = false;
        NetworkManager.Singleton.LeaveGame();
        Enter(0);
    }

    private Vector2 GetRandomRoomSize()
    {
        return new Vector2(Random.Range(schema.minRoomSize.x, schema.maxRoomSize.x), Random.Range(schema.minRoomSize.y, schema.maxRoomSize.y));
    }
    // Start is called before the first frame update
    void Start()
    {
        schema = Instantiate(schema);
        //Debug.Log(schema.bossRoomData.bossData.nextPhaseData);
        schema.bossRoomData.bossData.nextPhaseData = schema.bossRoomData.bossData.Copy();
        schema.bossRoomData.bossData.nextPhaseData.scale *= 1.3f;
        schema.bossRoomData.bossData.nextPhaseData.bossPowerModifier *= 1.5f;
        schema.bossRoomData.bossData.nextPhaseData.speed *= 1.5f;
        schema.bossRoomData.bossData.nextPhaseData.burstCount += 2;
        schema.bossRoomData.bossData.nextPhaseData.nGlyphs += 1;
        //Debug.Log(schema.bossRoomData.bossData.nextPhaseData);
    }

    public Vector2 ScaledVector(float x, float y) { return new Vector2(x * scale, y * scale); }

    public Room GetRoomById(int id) {
        return (Room)rooms[nodeGraph.GetNodeById(id)]; 
    }

    void KillAll()
    {
        foreach (GameObject obj in enemiesOnScreen) Destroy(obj);
        enemiesOnScreen = new List<GameObject>();
    }

    public void Enter(int id=1)
    {
        KillAll();


        if(id == 0)
        {
            ObjectPooler.Instance.ResetPool();
            currentRoomId = 0;
            Tavern.SetActive(true);
            return;
        }
        if (!isCreated) Create();
        Tavern.SetActive(false);
        Room lastRoom = GetRoomById(currentRoomId);
        Room room = GetRoomById(id);

        //Debug.Log(room.node);

        currentRoomId = id;
        ObjectPooler.Instance.ResetPool();
        foreach (GameObject roomObj in roomObjectsOnScreen) if(roomObj != null) roomObj.SetActive(false);

        int xpos = 0;
        if(room.noiseValuesFloor != null)
        foreach(int perlinVal in room.noiseValuesFloor)
        {
            ObjectPooler.Instance.SpawnFromPool("Top", ScaledVector(xpos, perlinVal), scale);
            for (int ypos = perlinVal - 1; ypos >= 0; --ypos)
            {
                ObjectPooler.Instance.SpawnFromPool("Deep", ScaledVector(xpos, ypos), scale);
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
            }
            ++xpos;
        }

        if (room.noiseValuesWallLeft != null)
            for (int ypos = 0; ypos < room.RoomData.size.y; ++ypos)
            {
                for (xpos = 0; xpos < room.noiseValuesWallLeft[ypos]; ++xpos)
                {
                    ObjectPooler.Instance.SpawnFromPool("Deep", ScaledVector(xpos, ypos + 1), scale);
                }
                ObjectPooler.Instance.SpawnFromPool("Wall", ScaledVector(xpos, ypos), scale);
            }


        if (room.noiseValuesWallRight != null)
            for (int ypos = 0; ypos < room.RoomData.size.y; ++ypos)
            {
                for (xpos = (int)room.RoomData.size.x; xpos >= room.RoomData.size.x - room.noiseValuesWallRight[ypos]; --xpos)
                {
                    ObjectPooler.Instance.SpawnFromPool("Deep", ScaledVector(xpos, ypos + 1), scale);
                }
                ObjectPooler.Instance.SpawnFromPool("Wall", ScaledVector(xpos, ypos), scale);
            }

        foreach (Vector2 key in room.objects.Keys)
        {
            GameObject obj = room.objects[key];
            if (obj != null)
            {
                obj.transform.position = key * scale;
                obj.transform.localScale = new Vector3(scale, scale, scale);
                obj.SetActive(true);
                if (obj.GetComponent<Interactable>() != null) obj.GetComponent<Interactable>().interactable = true;
                roomObjectsOnScreen.Add(obj);
            }
        }

        borderLeft.gameObject.transform.position = new Vector2(0, 0);
        borderRight.gameObject.transform.position = new Vector2(room.RoomData.size.x * scale, 0);


        if(room.node.type == NodeType.NORMAL) SpawnEnemies(room);

        if (spawnPos.Equals(Vector2.zero)) spawnPos = (lastRoom.spawns[((Waypoint)GameManager.Instance.player.GetComponent<Player>().lastInteraction).direction] + new Vector2(0, 1)) * scale;
        GameManager.Instance.player.transform.position = spawnPos;
        spawnPos = Vector2.zero;
        GameManager.Instance.player.GetComponent<Player>().lastInteraction = null;
        GameManager.Instance.mainCamera.GetComponent<CameraController>().warpToTarget();

        if (schema.GraphData.isMultiplayer) KillAll();

        if (room.node.type == NodeType.BOSS) 
            foreach (GameObject obj in roomObjectsOnScreen)
                if (obj.GetComponentInChildren<Waypoint>() != null) 
                    obj.GetComponentInChildren<Waypoint>().interactable = false;
    }

    void SpawnEnemies(Room room)
    {
        foreach (Vector2 pos in room.enemySpawns)
        {
            Entity obj = Instantiate(enemyPrototype);
            obj.transform.position = pos + new Vector2(0, 10);
            Enemy enemy = obj.GetComponent<Enemy>();
            enemy.data = schema.RoomData.enemyData.Copy();
            enemy.isActive = true;
            enemy.target = null;
            enemiesOnScreen.Add(obj.gameObject);
        }
    }
}

public interface Location {
    public void Enter(int id=1);
}