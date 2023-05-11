using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class Multiplayer : Waypoint
{
    public Schema schema;
    public NetworkManager networkManager;
    public GameObject multiplayerUI;

    public override async void Interact(GameObject obj)
    {
        obj.GetComponent<Player>().lastInteraction = this;
        OpenMenu(obj);
        //dungeon.GetComponent<Dungeon>().schema = schema;
        //bool result = await networkManager.Host();
        //if (result) GameManager.Instance.enterBlackScreen(null);
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

    public void Host()
    {
        Debug.Log("Hosting");
    }

    public void Connect(string IPtext)
    {
        Debug.Log("Connecting to" + IPtext);
    }
}
