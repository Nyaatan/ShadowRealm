using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : Interactable
{
    public GameObject loot;

    public override void Interact(GameObject obj)
    {
        GameObject lootObj = Instantiate(loot);
        lootObj.transform.position = this.transform.position;
        lootObj.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
