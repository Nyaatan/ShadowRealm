using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : Interactable
{
    public GameObject dungeon;
    public Location destination;
    public Direction direction = Direction.NONE;
    internal System.Action<int> specialAction = null;

    public override void Interact(GameObject obj)
    {
        if (specialAction != null) specialAction(0);
        else
        {
            obj.GetComponent<Player>().lastInteraction = this;
            GameManager.Instance.enterBlackScreen(destination);

        }
    }

    public override void Start()
    {
        base.Start();
        popUpText += " to go " + direction;
    }
}

public enum Direction
{
    UP, DOWN, LEFT, RIGHT, NONE
}