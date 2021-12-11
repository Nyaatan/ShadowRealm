using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : Interactable
{
    public GameObject loot;

    public override void Interact(GameObject obj)
    {
        base.Interact(obj);
        Dictionary<Vector2, GameObject> objects = GameManager.Instance.dungeon.GetRoomById(GameManager.Instance.dungeon.currentRoomId).objects;
        Vector2 k = new Vector2();
        foreach(Vector2 key in objects.Keys)
        {
            if (objects[key] == gameObject)
            {
                k = key;
                break;
            }
        }
        objects.Remove(k);
        GameObject lootObj = Glyph.GetRandom(new Vector2(1, 4), new Vector2(-1, -1), new Vector2(1, 1), true);
        lootObj.transform.position = this.transform.position;
        lootObj.SetActive(true);
        lootObj.GetComponent<Rigidbody2D>().simulated = true;
        gameObject.SetActive(false);

    }


}
