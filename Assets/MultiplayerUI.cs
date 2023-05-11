using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MultiplayerUI : MonoBehaviour
{

    public Multiplayer Multiplayer;
    public GameObject inputField;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Close();
        }
    }

    void Close()
    {
        GameManager.Instance.player.GetComponent<PlayerMovement>().enabled = true;
        gameObject.SetActive(false);
    }

    public void Host()
    {
        Multiplayer.Host();
    }

    public void Connect()
    {
        string IPtext = inputField.GetComponent<TMP_InputField>().text;
        Multiplayer.Connect(IPtext);
    }
}
