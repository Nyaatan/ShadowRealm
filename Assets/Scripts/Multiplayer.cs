using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Net;

public class Multiplayer : Waypoint
{
    public Schema schema;
    public NetworkManager networkManager;
    public GameObject multiplayerUI;

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
    }

    public async void Host()
    {
        bool result = await networkManager.Host();
        if (result)
        {
            multiplayerUI.GetComponent<MultiplayerUI>().Close();
            dungeon.GetComponent<Dungeon>().schema = schema;
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
            dungeon.GetComponent<Dungeon>().schema = schema;
            GameManager.Instance.enterBlackScreen(null);
        }
        multiplayerUI.GetComponent<MultiplayerUI>().Close();
    }
}
