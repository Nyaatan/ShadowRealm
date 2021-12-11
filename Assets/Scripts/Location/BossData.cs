using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BossData
{
    public float speed = 10f;
    public int burstCount = 1;
    public float burstInterval = 0.1f;
    public Vector3 scale = new Vector3(2, 2, 2);
    public Vector2 minMaxGlyphTier = new Vector2(1, 4);
    public Vector2 minVectorValues = new Vector2(-1, -1);
    public Vector2 maxVectorValues = new Vector2(1, 1);
    public System.Action<Boss> specialAttack = null;
    public int nGlyphs = 2;
    public float maxHealth = 2000;
    public float phaseChangeThreshold = 0.5f;
    public CastSequence castSequence = CastSequence.RANDOM;
    public float bossPowerModifier = 2f;
    [SerializeField]
    public BossData nextPhaseData;
    public BossType animationSet;

    public BossData Copy()
    {
        return MemberwiseClone() as BossData;
    }
}

public enum CastSequence
{
    RANDOM, SEQUENTIAL
}