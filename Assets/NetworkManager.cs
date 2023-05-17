using Riptide.Utils;
using Riptide;
using System;
using UnityEngine;
using System.Collections.Generic;

internal enum MessageId : ushort
{
    SpawnPlayer,
    PlayerMovement,
    PlayerAttack,
    Seed
}

public class NetworkManager : MonoBehaviour
{
    private static NetworkManager _singleton;
    public static NetworkManager Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
                _singleton = value;
            else if (_singleton != value)
            {
                Debug.Log($"{nameof(NetworkManager)} instance already exists, destroying object!");
                Destroy(value);
            }
        }
    }

    [SerializeField] private ushort port;
    [SerializeField] private ushort maxPlayers=2;
    [SerializeField] public Multiplayer multiplayer;
    [Header("Prefabs")]
    [SerializeField] private EntityMP playerPrefab;
    [SerializeField] private EntityMP localPlayerPrefab;

    public EntityMP PlayerPrefab => playerPrefab;
    public EntityMP LocalPlayerPrefab => localPlayerPrefab;

    public int seed;

    internal Server Server { get; private set; }
    internal Client Client { get; private set; }

    private void Awake()
    {
        Singleton = this;
    }

    private void Start()
    {
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
        seed = Guid.NewGuid().GetHashCode();
        Server = new Server();
        Server.ClientConnected += PlayerJoined;
        Server.RelayFilter = new MessageRelayFilter(typeof(MessageId), 
            MessageId.SpawnPlayer, MessageId.PlayerMovement, MessageId.PlayerAttack, MessageId.Seed);

        Client = new Client();
        Client.Connected += DidConnect;
        Client.ConnectionFailed += FailedToConnect;
        Client.ClientDisconnected += PlayerLeft;
        Client.Disconnected += DidDisconnect;

    }

    private void FixedUpdate()
    {
        if (Server.IsRunning)
            Server.Update();

        Client.Update();
    }

    private void OnApplicationQuit()
    {
        Server.Stop();
        Client.Disconnect();
    }

    internal void StartHost()
    {
        Server.Start(port, maxPlayers);
        Client.Connect($"127.0.0.1:{port}");
        seed = Guid.NewGuid().GetHashCode();
    }

    internal void JoinGame(string ipString)
    {
        Client.Connect($"{ipString}:{port}");
    }

    internal void LeaveGame()
    {
        Server.Stop();
        Client.Disconnect();
    }

    private void DidConnect(object sender, EventArgs e)
    {
        Debug.Log(Client.Id);
        Player.Spawn(Client.Id, new Vector2(60, 10), true);
    }

    private void FailedToConnect(object sender, EventArgs e)
    {
        //UIManager.Singleton.BackToMain();
    }

    private void PlayerJoined(object sender, ServerConnectedEventArgs e)
    {
        Debug.Log(Server.Clients);
        Player.SendSeed(seed, e.Client.Id);
        foreach (EntityMP player in Player.List.Values)
            if (player.id != e.Client.Id)
            {
                player.SendSpawn(e.Client.Id, new Vector2(60, 10));
            }
                
    }

    private void PlayerLeft(object sender, ClientDisconnectedEventArgs e)
    {
        Destroy(Player.List[e.Id].gameObject);
    }

    private void DidDisconnect(object sender, DisconnectedEventArgs e)
    {
        foreach (Player player in Player.List.Values)
            if (!player.isLocal) Destroy(player.gameObject);
        GameManager.Instance.dungeon.EnterTavern();
        //UIManager.Singleton.BackToMain();
    }

    public void Enter()
    {
        multiplayer.Enter();
    }
}
