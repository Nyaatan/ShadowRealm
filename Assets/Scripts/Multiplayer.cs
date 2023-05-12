using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Net;
using System;

public class Multiplayer : Waypoint
{
    public Schema schema;
    public NetworkManager networkManager;
    public GameObject multiplayerUI;

    private Dungeon dung;

    public override async void Interact(GameObject obj)
    {
        obj.GetComponent<Player>().lastInteraction = this;
        OpenMenu(obj);
        
    }

    public async void OpenMenu(GameObject obj)
    {
        obj.GetComponent<PlayerMovement>().horizontalMove = 0;
        obj.GetComponent<PlayerMovement>().enabled = false;
        multiplayerUI.gameObject.SetActive(true);
    }


    public override void Start()
    {
        base.Start();
        dung = dungeon.GetComponent<Dungeon>();
    }

    public async void Host()
    {
        bool result = await networkManager.Host();
        if (result)
        {
            multiplayerUI.GetComponent<MultiplayerUI>().Close();
            dung.schema = schema;
            int seed = Guid.NewGuid().GetHashCode();
            UnityEngine.Random.seed = seed;
            
            dung.Create();
            networkManager.SendWorldData(seed);
            GameManager.Instance.enterBlackScreen(null);
        }
        multiplayerUI.GetComponent<MultiplayerUI>().Close();
    }

    public async void Connect(string IPtext)
    {
        IPEndPoint ip = new IPEndPoint(IPAddress.Parse(IPtext), networkManager.tcpPort);
        bool result = await networkManager.Connect(ip);
        if (result)
        {
            multiplayerUI.GetComponent<MultiplayerUI>().Close();
            object rec = await networkManager.ReceiveWorldData();
            if ((int)rec == -2137)
            {
                multiplayerUI.GetComponent<MultiplayerUI>().Close();
                return;
            }
            int seed = (int)rec;
            Debug.Log(seed);
            
            UnityEngine.Random.seed = seed;
            Debug.Log(UnityEngine.Random.seed);
            
            dung.schema = schema;
            dung.Create();
            GameManager.Instance.enterBlackScreen(null);
        }
        multiplayerUI.GetComponent<MultiplayerUI>().Close();
    }
}
