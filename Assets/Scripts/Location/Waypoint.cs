using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : Interactable
{
    public GameObject dungeon;
    public Location destination;
    public Direction direction = Direction.NONE;

    public override void Interact(GameObject obj)
    {
        obj.GetComponent<Player>().lastInteraction = this;
        GameManager.Instance.enterBlackScreen();
        if (destination == null) dungeon.GetComponent<Location>().Enter();
        else destination.Enter();
        GameManager.Instance.exitBlackScreen();
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