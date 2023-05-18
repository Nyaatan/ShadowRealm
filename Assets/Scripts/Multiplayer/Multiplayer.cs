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

    public override void Interact(GameObject obj)
    {
        obj.GetComponent<Player>().lastInteraction = this;
        OpenMenu(obj);
        
    }

    public void OpenMenu(GameObject obj)
    {
        obj.GetComponent<PlayerMovement>().horizontalMove = 0;
        obj.GetComponent<PlayerMovement>().enabled = false;
        multiplayerUI.gameObject.SetActive(true);
    }

    public void Enter()
    {
        multiplayerUI.GetComponent<MultiplayerUI>().Close();
        dung.schema = schema;

        GameManager.Instance.enterBlackScreen(destination);
    }

    public override void Start()
    {
        base.Start();
        dung = dungeon.GetComponent<Dungeon>();
    }

    public void Host()
    {
        networkManager.StartHost();
    }

    public void Connect(string IPtext)
    {
        networkManager.JoinGame(IPtext);
    }
}
