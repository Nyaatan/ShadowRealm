using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoomData
{
    public Vector2 size;
    public int minFloorHeight;
    public int maxFloorHeight;
    public int minRoofOffset;
    public int maxRoofOffset;
    public int minWallOffset;
    public int maxWallOffset;
    public bool roof = false;
    public bool leftWall = false;
    public bool rightWall = false;
    public bool floor = true;
    public GameObject waypointPrefab;
    public bool isTreasure = false;
    public bool isChallenge = false;
    public int waypointWallOffset = 3;
    public EnemyData enemyData;
    public BossData bossData;

    public RoomData Copy()
    {
        return MemberwiseClone() as RoomData;
    }
}
