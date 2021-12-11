using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestWaypoint : Waypoint
{
    Schema schema;
    public QuestGenerator questGenerator;
    public QuestTooltip tooltip;

    public override void Start()
    {
        base.Start();
        List<object> quest = questGenerator.GetRandom();
        popUpText = "Press E to Enter Dungeon";
        popUpText += "\n" + ((string) quest[0]).Replace("\\n", "\n");
        schema = (Schema) quest[1];
    }

    public override void Interact(GameObject obj)
    {
        dungeon.GetComponent<Dungeon>().schema = schema;
        base.Interact(obj);
    }
    public override void showPopUp()
    {
        tooltip.gameObject.SetActive(true);
        tooltip.SetText(popUpText);
    }

    public override void hidePopUp()
    {
        tooltip.gameObject.SetActive(false);
    }

    public void OnEnable()
    {
        Start();
    }
}
