using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Riptide;

public class EntityMP : Entity
{
    internal static Dictionary<ushort, EntityMP> List = new Dictionary<ushort, EntityMP>();
    public ushort id;
    public static bool inSession = false;
    public Animator animator;
    private void OnDestroy()
    {
        List.Remove(id);
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
        Debug.Log(id);
        if (shouldSendSpawn)
            player.SendSpawn();
    }

    public GameObject CreateTarget(Vector2 pos)
    {
        GameObject target = new GameObject("Target");
        target.transform.position = pos;
        target.transform.position += new Vector3(0, 0, transform.position.z);
        return target;
    }

    private void Attack(Vector2 target, Vector2 glyphVector, short tier)
    {
        Debug.Log("DUPADUPA");
        Debug.Log(tier);
        Debug.Log(glyphVector);
        Debug.Log("DUPADUPA");
        
        Glyph glyph = Glyph.GetFromValues(glyphVector, tier).GetComponent<Glyph>();

        if (glyph.data.nature == GlyphData.Nature.SELF) animator.SetTrigger("SpellCastSelf");
        else animator.SetTrigger("SpellCast");
        Debug.Log(glyph.data.vector);
        glyph.Use(this, CreateTarget(target));
        //Destroy(glyph.gameObject);
    }

    private void Move(Vector2 newPosition)
    {
        transform.position = newPosition;
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
            player.Move(message.GetVector2());
        //Debug.Log("MoveReceive");
        //Debug.Log(playerId);
    }

    [MessageHandler((ushort)MessageId.PlayerAttack)]
    private static void PlayerAttack(Message message)
    {
        ushort playerId = message.GetUShort();
        Vector2 targetTransform = message.GetVector2();
        Vector2 glyphVector = message.GetVector2();
        short tier = message.GetShort();
        Debug.Log(glyphVector);
        Debug.Log(tier);
        if (List.TryGetValue(playerId, out EntityMP player))
            player.Attack(targetTransform, glyphVector, tier);

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
        Debug.Log("PLAYERINPUT " + fromClientId);
        Player player = Player.List[fromClientId] as Player;
        float move = message.GetFloat();
        bool jump = message.GetBool();
        Debug.Log(player.isLocal);
        PlayerMovement contr = player.gameObject.GetComponent<PlayerMovement>();
        contr.horizontalMove = move;
        contr.jump = jump;
        contr.ForceMove();
    }
}
