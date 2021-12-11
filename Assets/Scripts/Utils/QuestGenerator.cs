using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestGenerator : MonoBehaviour
{
    public List<string> texts = new List<string>{
        "BEWARE\nFROM HERE ON THE {location} IS INFESTED WITH\n{maxEnemiesPerRoom} {enemySize} {enemyType}\nTHE {enemyElement}LY CREATURES WANDER IN {maxGroupSize} GROUPS\n." +
        "IF YOU DECIDE TO ENTER, TAKE CAUTION!\n" +
        "A {bossSize} {bossType} WAS SPOTTED IN THE AREA\n." +
        "HE'S {bossSpeed} AND {bossPower} STRONG.\n" +
        "HIS {bossElement}LY ATTACKS TEAR DOWN BUILDINGS\n" +
        "AND HIS {bossHealth} BODY CAN WITHSTAND ANY SWORD.\n" +
        "SURVIVORS CLAIM HE WAS VERY {bossBurst}.",
    };
    public Schema defaultSchema;
    public Schema schemaPrefab;

    List<string> tags = new List<string>
    {
        "bossType" ,"bossSize", "bossPower", "bossSpeed", "bossBurst", "bossElement", "bossHealth", 
        "location", "maxEnemiesPerRoom", "maxGroupSize", 
        "enemyFlying", "enemyType", "enemyElement"
    };

    public List<object> GetRandom()
    {
        defaultSchema = Instantiate(schemaPrefab);
        string text = texts[Random.Range(0, texts.Count)];

        foreach (string tag in tags) if (text.Contains(tag))
            {
                Debug.Log(tag);
                switch (tag)
                {
                    case "bossType":
                        BossType bossType = (BossType)Random.Range(0, (int) BossType.COUNT);
                        switch (bossType)
                        {
                            case BossType.SLIME:
                                defaultSchema.bossRoomData.bossData.animationSet = BossType.SLIME;
                                break;
                            case BossType.TRAINING_DUMMY:
                                defaultSchema.bossRoomData.bossData.animationSet = BossType.TRAINING_DUMMY;
                                break;
                            case BossType.COUNT:
                                break;
                        }
                        Debug.Log(bossType);
                        text = text.Replace("{bossType}", bossType.ToString().ToUpper());
                        
                        break;
                    case "bossSize":
                        BossSize bossSize = (BossSize)Random.Range(0, (int)BossSize.COUNT);
                        switch (bossSize)
                        {
                            case BossSize.SMALL:
                                defaultSchema.bossRoomData.bossData.scale *= Random.Range(0.4f, 0.7f);
                                break;
                            case BossSize.MEDIUM:
                                defaultSchema.bossRoomData.bossData.scale *= Random.Range(0.8f, 1.3f);
                                break;
                            case BossSize.LARGE:
                                defaultSchema.bossRoomData.bossData.scale *= Random.Range(1.4f, 2f);
                                break;
                            case BossSize.COUNT:
                                break;
                        }
                        text = text.Replace("{bossSize}", bossSize.ToString().ToUpper());
                        break;
                    case "bossPower":
                        BossPower bossPower = (BossPower)Random.Range(0, (int)BossPower.COUNT);
                        switch (bossPower)
                        {
                            case BossPower.NOT_SO:
                                defaultSchema.bossRoomData.bossData.bossPowerModifier *= Random.Range(0.4f, 0.7f);
                                break;
                            case BossPower.KINDA:
                                defaultSchema.bossRoomData.bossData.bossPowerModifier *= Random.Range(0.8f, 1.3f);
                                break;
                            case BossPower.VERY:
                                defaultSchema.bossRoomData.bossData.bossPowerModifier *= Random.Range(1.4f, 2f);
                                break;
                            case BossPower.COUNT:
                                break;
                        }
                        text = text.Replace("{bossPower}", bossPower.ToString().ToUpper());
                        break;
                    case "bossSpeed":
                        BossSpeed bossSpeed = (BossSpeed)Random.Range(0, (int)BossSpeed.COUNT);
                        switch (bossSpeed)
                        {
                            case BossSpeed.SLOW:
                                defaultSchema.bossRoomData.bossData.speed *= Random.Range(0.4f, 0.7f);
                                break;
                            case BossSpeed.FAST:
                                defaultSchema.bossRoomData.bossData.speed *= Random.Range(0.8f, 1.3f);
                                break;
                            case BossSpeed.VERY_FAST:
                                defaultSchema.bossRoomData.bossData.speed *= Random.Range(1.4f, 2f);
                                break;
                            case BossSpeed.COUNT:
                                break;
                        }
                        text = text.Replace("{bossSpeed}", bossSpeed.ToString().ToUpper());
                        break;
                    case "bossBurst":
                        BossBurst bossBurst = (BossBurst)Random.Range(0, (int)BossBurst.COUNT);
                        switch (bossBurst)
                        {
                            case BossBurst.CALM:
                                defaultSchema.bossRoomData.bossData.burstCount = 1;
                                defaultSchema.bossRoomData.bossData.burstInterval = 0.3f;
                                break;
                            case BossBurst.UNRESTED:
                                defaultSchema.bossRoomData.bossData.burstCount = 2;
                                defaultSchema.bossRoomData.bossData.burstInterval = 0.3f;
                                break;
                            case BossBurst.ANGRY:
                                defaultSchema.bossRoomData.bossData.burstCount = 3;
                                defaultSchema.bossRoomData.bossData.burstInterval = 0.15f;
                                break;
                            case BossBurst.COUNT:
                                break;
                        }
                        text = text.Replace("{bossBurst}", bossBurst.ToString().ToUpper());
                        break;
                    case "bossElement":
                        BossElement bossElement = (BossElement)Random.Range(0, (int)BossElement.COUNT);
                        switch (bossElement)
                        {
                            case BossElement.EARTH:
                                defaultSchema.bossRoomData.bossData.minVectorValues = new Vector2(-1, -1);
                                defaultSchema.bossRoomData.bossData.maxVectorValues = new Vector2(0, 0);
                                break;
                            case BossElement.FIRE:
                                defaultSchema.bossRoomData.bossData.minVectorValues = new Vector2(0, -1);
                                defaultSchema.bossRoomData.bossData.maxVectorValues = new Vector2(1, 0);
                                break;
                            case BossElement.WATER:
                                defaultSchema.bossRoomData.bossData.minVectorValues = new Vector2(-1, 0);
                                defaultSchema.bossRoomData.bossData.maxVectorValues = new Vector2(0, 1);
                                break;
                            case BossElement.AIR:
                                defaultSchema.bossRoomData.bossData.minVectorValues = new Vector2(0, 0);
                                defaultSchema.bossRoomData.bossData.maxVectorValues = new Vector2(1, 1);
                                break;
                            case BossElement.COUNT:
                                break;
                        }
                        text = text.Replace("{bossElement}", bossElement.ToString().ToUpper());
                        break;
                    case "bossHealth":
                        BossHealth bossHealth = (BossHealth)Random.Range(0, (int)BossHealth.COUNT);
                        switch (bossHealth)
                        {
                            case BossHealth.FRAIL:
                                defaultSchema.bossRoomData.bossData.maxHealth = (int)Random.Range(500, 1200);
                                break;
                            case BossHealth.STURDY:
                                defaultSchema.bossRoomData.bossData.maxHealth = (int)Random.Range(1200, 1800);
                                break;
                            case BossHealth.HARD_AS_ROCK:
                                defaultSchema.bossRoomData.bossData.maxHealth = (int)Random.Range(1800, 2500);
                                break;
                            case BossHealth.COUNT:
                                break;
                        }
                        text = text.Replace("{bossHealth}", bossHealth.ToString().ToUpper());
                        break;
                    case "location":
                        LocationType locationType = (LocationType)Random.Range(0, (int)LocationType.COUNT);
                        switch (locationType)
                        {
                            case LocationType.MEADOW:
                                defaultSchema.RoomData.minFloorHeight = 1;
                                defaultSchema.RoomData.maxFloorHeight = 3;
                                defaultSchema.RoomData.minRoofOffset = -1;
                                defaultSchema.RoomData.maxRoofOffset = -1;
                                defaultSchema.maxFloors = 1;
                                defaultSchema.minFloors = 1;
                                break;
                            case LocationType.CAVE:
                                defaultSchema.noRoofOnTop = false;
                                defaultSchema.RoomData.minFloorHeight = 2;
                                defaultSchema.RoomData.maxFloorHeight = 6;
                                defaultSchema.RoomData.minRoofOffset = 2;
                                defaultSchema.RoomData.maxRoofOffset = 4;
                                defaultSchema.maxFloors = Random.Range(3, 6);
                                defaultSchema.minFloors = 3;
                                defaultSchema.GraphData.neighborDownChance = 0.7f;
                                defaultSchema.GraphData.neighborUpChance = 0.7f;
                                defaultSchema.GraphData.neighborLeftChance = 0.7f;
                                defaultSchema.GraphData.neighborRightChance = 0.7f;
                                break;
                            case LocationType.MOUNTAIN:
                                defaultSchema.GraphData.neighborDownChance = 0f;
                                defaultSchema.RoomData.minFloorHeight = 3;
                                defaultSchema.RoomData.maxFloorHeight = 7;
                                defaultSchema.RoomData.minRoofOffset = 1;
                                defaultSchema.RoomData.maxRoofOffset = 3;
                                defaultSchema.maxFloors = Random.Range(4, 7);
                                defaultSchema.minFloors = 4;
                                defaultSchema.GraphData.neighborUpChance = 0.5f;
                                defaultSchema.GraphData.neighborDownChance = 0f;
                                defaultSchema.GraphData.neighborLeftChance = 0f;
                                defaultSchema.GraphData.neighborRightChance = 0.5f;
                                break;
                            case LocationType.COUNT:
                                break;
                        }
                        text = text.Replace("{location}", locationType.ToString().ToUpper());
                        break;
                    case "maxEnemiesPerRoom":
                        EnemiesInRoom enemiesInRoom = (EnemiesInRoom)Random.Range(0, (int)EnemiesInRoom.COUNT);
                        switch (enemiesInRoom)
                        {
                            case EnemiesInRoom.FEW:
                                defaultSchema.RoomData.enemyData.maxEnemies = Random.Range(3, 6);
                                break;
                            case EnemiesInRoom.MANY:
                                defaultSchema.RoomData.enemyData.maxEnemies = Random.Range(6, 9);
                                break;
                            case EnemiesInRoom.HORDES:
                                defaultSchema.RoomData.enemyData.maxEnemies = Random.Range(9, 15);
                                break;
                            case EnemiesInRoom.COUNT:
                                break;
                        }
                        text = text.Replace("{maxEnemiesPerRoom}", enemiesInRoom.ToString().ToUpper());
                        break;
                    case "maxGroupSize":
                        EnemyGroups enemyGroups = (EnemyGroups)Random.Range(0, (int)EnemyGroups.COUNT);
                        switch (enemyGroups)
                        {
                            case EnemyGroups.SMALL:
                                defaultSchema.RoomData.enemyData.maxGroupSize = 2;
                                break;
                            case EnemyGroups.MEDIUM:
                                defaultSchema.RoomData.enemyData.maxGroupSize = 4;
                                break;
                            case EnemyGroups.BIG:
                                defaultSchema.RoomData.enemyData.maxGroupSize = 6;
                                break;
                            case EnemyGroups.COUNT:
                                break;
                        }
                        text = text.Replace("{maxGroupSize}", enemyGroups.ToString().ToUpper());
                        break;
                    case "enemyFlying":
                        EnemyFlying enemyFlying = (EnemyFlying)Random.Range(0, (int)EnemyFlying.COUNT);
                        switch (enemyFlying)
                        {
                            case EnemyFlying.GROUND:
                                defaultSchema.RoomData.enemyData.allowedTypes.Add(EnemyType.GENERIC);
                                break;
                            case EnemyFlying.FLYING:
                                defaultSchema.RoomData.enemyData.allowedTypes.Add(EnemyType.FLYING);
                                break;
                            case EnemyFlying.COUNT:
                                break;
                        }
                        text = text.Replace("{enemyFlying}", enemyFlying.ToString().ToUpper());
                        break;
                    case "enemyType":
                        EnemyTypes enemyType = (EnemyTypes)Random.Range(0, (int)EnemyTypes.COUNT);
                        switch (enemyType)
                        {
                            case EnemyTypes.SLIME:
                                defaultSchema.RoomData.enemyData.animatorOverriderId = (int)EnemyTypes.SLIME;
                                break;
                            case EnemyTypes.TRAINING_DUMMY:
                                defaultSchema.RoomData.enemyData.animatorOverriderId = (int)EnemyTypes.TRAINING_DUMMY;
                                break;
                            case EnemyTypes.COUNT:
                                break;
                        }
                        text = text.Replace("{enemyType}", enemyType.ToString().ToUpper());
                        break;
                    case "enemyElement":
                        EnemyElement enemyElement = (EnemyElement)Random.Range(0, (int)EnemyElement.COUNT);
                        switch (enemyElement)
                        {
                            case EnemyElement.EARTH:
                                defaultSchema.RoomData.enemyData.minVectorValues = new Vector2(-1, -1);
                                defaultSchema.RoomData.enemyData.maxVectorValues = new Vector2(0, 0);
                                break;
                            case EnemyElement.FIRE:
                                defaultSchema.RoomData.enemyData.minVectorValues = new Vector2(0, -1);
                                defaultSchema.RoomData.enemyData.maxVectorValues = new Vector2(1, 0);
                                break;
                            case EnemyElement.WATER:
                                defaultSchema.RoomData.enemyData.minVectorValues = new Vector2(-1, 0);
                                defaultSchema.RoomData.enemyData.maxVectorValues = new Vector2(0, 1);
                                break;
                            case EnemyElement.AIR:
                                defaultSchema.RoomData.enemyData.minVectorValues = new Vector2(0, 0);
                                defaultSchema.RoomData.enemyData.maxVectorValues = new Vector2(1, 1);
                                break;
                            case EnemyElement.COUNT:
                                break;
                        }
                        text = text.Replace("{enemyElement}", enemyElement.ToString().ToUpper());
                        break;
                }
            }
        text = text.Replace("_", " ");
        return new List<object> { text, defaultSchema };
    }

    void Start()
    {
        //testQuest = GenerateQuest();
    }

}

public enum BossType { SLIME, TRAINING_DUMMY, COUNT }
public enum BossSize { SMALL, MEDIUM, LARGE, COUNT }
public enum BossPower { NOT_SO, KINDA, VERY, COUNT }
public enum BossSpeed { SLOW, FAST, VERY_FAST, COUNT }
public enum BossBurst { CALM, UNRESTED, ANGRY, COUNT }
public enum BossElement { EARTH, FIRE, WATER, AIR, COUNT }
public enum BossHealth { FRAIL, STURDY, HARD_AS_ROCK, COUNT }
public enum LocationType { MEADOW, CAVE, MOUNTAIN, COUNT }
public enum EnemiesInRoom { FEW, MANY, HORDES, COUNT }
public enum EnemyGroups { SMALL, MEDIUM, BIG, COUNT }
public enum EnemyFlying { GROUND, FLYING, COUNT }
public enum EnemyTypes { SLIME, TRAINING_DUMMY, COUNT }
public enum EnemyElement { EARTH, FIRE, WATER, AIR, COUNT }
