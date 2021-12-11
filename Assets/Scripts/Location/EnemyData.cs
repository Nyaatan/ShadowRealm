using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyData
{
    public List<EnemyType> allowedTypes;
    public int maxEnemies;
    public List<GlyphData> allowedGlyphs;
    public int minWallOffset;
    public int minGroupSize;
    public int maxGroupSize;
    public float perlinTreshold;
    public int groupMinDistance;

    public float speed = 10f; 
    public Vector3 scale = new Vector3(2, 2, 2);
    public Vector2 minMaxGlyphTier = new Vector2(1, 4);
    public Vector2 minVectorValues = new Vector2(-1, -1);
    public Vector2 maxVectorValues = new Vector2(1, 1);
    public float maxHealth = 100;
    public float powerModifier = 2f;
    public int animatorOverriderId = 0;
    public EnemyData Copy()
    {
        return MemberwiseClone() as EnemyData;
    }
}

public enum EnemyType
{
    FLYING, GENERIC
}