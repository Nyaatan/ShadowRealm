using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Riptide;
using System.Linq;

public class EntityMP : Entity
{
    internal static Dictionary<ushort, EntityMP> List = new Dictionary<ushort, EntityMP>();
    public ushort id;
    public static bool inSession = false;
    public Animator animator;
    public bool shouldLerp = false;
    public Vector2 lerpDest;
    public float lerpStep = 0.05f;
    public bool isLocal = false;
    [SerializeField]
    public Dictionary<long, Vector3> movementHistory = new Dictionary<long, Vector3>();
    ushort maxSnapshots = 100;

    private void OnDestroy()
    {
        List.Remove(id);
    }
    public void AddToHistory(Vector2 pos, long ts)
    {
        movementHistory.Add(ts, pos);
        if (movementHistory.Count > maxSnapshots)
        {
            movementHistory.Remove(movementHistory.Keys.Min());
        }
    }
    public static void Spawn(ushort id, Vector2 pos, bool shouldSendSpawn=false)
    {
        EntityMP player;
        if (id == NetworkManager.Singleton.Client.Id)
            player = NetworkManager.Singleton.LocalPlayerPrefab;
            
        else
            player = NetworkManager.Singleton.PlayerPrefab;
            player.gameObject.SetActive(true);
        player.transform.position = pos;
        player.id = id;

        List.Add(id, player);
        if (shouldSendSpawn)
            player.SendSpawn();

        //if (List.TryGetValue(1, out EntityMP host))
        {
            if (id != NetworkManager.Singleton.Client.Id && 1 != NetworkManager.Singleton.Client.Id)
                player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            else player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        }
        //else player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
    }

    public GameObject CreateTarget(Vector2 pos)
    {
        GameObject target = new GameObject("Target");
        target.transform.position = pos;
        target.transform.position += new Vector3(0, 0, transform.position.z);
        return target;
    }

    private void Attack(Vector2 target, Vector2 glyphVector, short tier, ushort spellID)
    {
        Glyph glyph = Glyph.GetFromValues(glyphVector, tier).GetComponent<Glyph>();

        if (glyph.data.nature == GlyphData.Nature.SELF) animator.SetTrigger("SpellCastSelf");
        else animator.SetTrigger("SpellCast");
        //Debug.Log(glyph.data.vector);
        Spell spell = glyph.Use(this, CreateTarget(target));
        spell.id = spellID;
        ResearchManager.Instance.AddSpell(spell);
        //Destroy(glyph.gameObject);
    }

    private void Move(Vector2 newPosition, long timestamp, float velocityY)
    {
        Vector2 lagDistance;
        Debug.Log(movementHistory.Count);
        if (movementHistory.TryGetValue(timestamp, out Vector3 oldPos))
        {
            Debug.Log("BABABABA");
            lagDistance = newPosition - (Vector2)oldPos;
        }
        else
        {
            lagDistance = newPosition - (Vector2)transform.position;
        }
        if (isLocal)
        {
            Debug.Log("DUPSKO " + lagDistance);
            if (lagDistance.magnitude > 5f)
            {
                movementHistory.Clear();
            }
            if (lagDistance.magnitude > 1f)
            {
                //Debug.Log(velocityY);
                //lerpDest = newPosition;
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                GetComponent<Rigidbody2D>().AddForce(new Vector2(0, velocityY));
                //shouldLerp = true;
                transform.position = newPosition;
            }
        } else if (lagDistance.magnitude > 0.2f)
        {
            lerpDest = newPosition;
            shouldLerp = true;
        }
        //Debug.Log(lagDistance);
        ResearchManager.Instance.HandlePositionChange(gameObject, lagDistance);
    }
    private void SendSpawn()
    {
        Message message = Message.Create(MessageSendMode.Reliable, MessageId.SpawnPlayer);
        message.AddUShort(id);
        message.AddVector2(transform.position);
        NetworkManager.Singleton.Client.Send(message);
    }
    public void SendSpawn(ushort clientID, Vector2 pos)
    {
        Message message = Message.Create(MessageSendMode.Reliable, MessageId.SpawnPlayer);
        message.AddUShort(id);
        message.AddVector2(transform.position);
        NetworkManager.Singleton.Server.Send(message, clientID);
    }

    public static void SendSeed(int seed, ushort clientID)
    {
        Message message = Message.Create(MessageSendMode.Reliable, MessageId.Seed);
        message.AddInt(seed);
        NetworkManager.Singleton.Server.Send(message, clientID);
    }

    [MessageHandler((ushort)MessageId.SpawnPlayer)]
    private static void SpawnPlayer(Message message)
    {
        Spawn(message.GetUShort(), message.GetVector2());
    }

    [MessageHandler((ushort)MessageId.PlayerMovement)]
    private static void PlayerMovement(Message message)
    {
        ushort playerId = message.GetUShort();
        if (List.TryGetValue(playerId, out EntityMP player))
        {
            Vector2 pos = message.GetVector2();
            float velocityY = message.GetFloat();
            long timestamp = message.GetLong();
            player.Move(pos, player.isLocal ? timestamp : 0, velocityY);
        }
        //Debug.Log("MoveReceive");
        //Debug.Log(playerId);
    }

    [MessageHandler((ushort)MessageId.PlayerAttack)]
    private static void PlayerAttack(Message message)
    {
        Debug.Log("ADADADADA");
        ushort playerId = message.GetUShort();
        ushort spellID = message.GetUShort();
        Vector2 targetTransform = message.GetVector2();
        Vector2 glyphVector = message.GetVector2();
        short tier = message.GetShort();
        Debug.Log(glyphVector);
        Debug.Log(tier);
        if (List.TryGetValue(playerId, out EntityMP player))
            player.Attack(targetTransform, glyphVector, tier, spellID);

    }

    [MessageHandler((ushort)MessageId.PlayerAttack)]
    private static void PlayerAttack(ushort fromClientId, Message message){
        if(!(List[1] as Player).isLocal || fromClientId != 1) PlayerAttack(message);

        foreach (Player player in List.Values) if(player.id != 1){
            Debug.Log("DUPSKO " + player.id);
            NetworkManager.Singleton.Server.Send(message, player.id);
        }
    }

    [MessageHandler((ushort)MessageId.Seed)]
    private static void SetSeed(Message message)
    {
        UnityEngine.Random.seed = message.GetInt();
        EntityMP.inSession = true;
        NetworkManager.Singleton.Enter();
    }

    [MessageHandler((ushort)MessageId.PlayerInput)]
    private static void PlayerInput(ushort fromClientId, Message message)
    {
        Player player = Player.List[fromClientId] as Player;
        float move = message.GetFloat();
        bool jump = message.GetBool();
        PlayerMovement contr = player.gameObject.GetComponent<PlayerMovement>();
        contr.horizontalMove = move;
        contr.jump = jump;
        contr.ForceMove(message.GetLong());
    }

    [MessageHandler((ushort)MessageId.ServerSpellHit)]
    private static void ServerSpellHit(Message message)
    {
        ushort id = message.GetUShort();
        ushort spellID = message.GetUShort();
        float value = message.GetFloat();
        ushort len = message.GetUShort();
        ushort[] elems = message.GetUShorts(len);
        GlyphData.Element[] elements = new GlyphData.Element[len];
        for (int i = 0; i < len; ++i) elements[i] = (GlyphData.Element)elems[i];
        EntityMP.List[id].ReceiveDamage(value, elements, true);
        ResearchManager.Instance.RegisterHit(spellID, id);
    }

    [MessageHandler((ushort)MessageId.PlayerHeal)]
    private static void PlayerHeal(Message message)
    {
        ushort id = message.GetUShort();
        float value = message.GetFloat();
        EntityMP.List[id].Heal(value, true);
    }
}
