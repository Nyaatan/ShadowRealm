using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeRoomActivator : Interactable, TimedObject
{

    public Vector2 minMaxTier = new Vector2(2, 4);
    public Vector2 minVectorBoundary = new Vector2(-1, -1);
    public Vector2 maxVectorBoundary = new Vector2(1, 1);

    public override void Interact(GameObject obj)
    {
        base.Interact(obj);
        interactable = false;
        GameManager.Instance.dungeon.ActivateChallenge(this);
    }

    public void OnVicotry()
    {
        GameObject reward = Glyph.GetRandom(minMaxTier,
                                       minVectorBoundary,
                                       maxVectorBoundary,
                                       false);
        reward.transform.position = this.transform.position;
        gameObject.SetActive(false);
    }

    public void OnTtlEnd()
    {
        throw new System.NotImplementedException();
    }
}
