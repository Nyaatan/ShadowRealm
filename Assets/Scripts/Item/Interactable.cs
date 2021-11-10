using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public static KeyCode interactKey = KeyCode.E;
    public string popUpText = string.Format("Press {0}", interactKey.ToString());
    public bool interactable = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void Interact(GameObject obj) { }

    public void showPopUp()
    {
        Debug.Log(popUpText);
        Debug.Log(popUpText);
    }

    public void hidePopUp()
    {

    }

    public void OnCollisionEnter2D(Collision collision)
    {
        if(collision.gameObject.name == "Player") showPopUp();
    }

    public void OnCollisionStay2D(Collision2D collision)
    {
        if (Input.GetKeyDown(interactKey) && collision.gameObject.name == "Player") Interact(collision.gameObject);
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.name == "Player") hidePopUp();
    }
}
