using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest
{
    public string name;
    public string flavourText;
    public Dictionary<Keyword, string> keywords;

}

public enum Keyword
{
    BOSS_NAME, BOSS_TYPE, LAYOUT, FLOORS, WIDTH, ENEMY_TYPE, BIOME, LOOT_VASTNESS,
}

