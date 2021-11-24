using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestGenerator : MonoBehaviour
{
    [SerializeField]
    public GeneratorData GeneratorData;
    public List<string> namePool;
    public List<string> flavourTextPool;
    public Dictionary<Keyword, List<string>> keywordPool = new Dictionary<Keyword, List<string>>();

    [SerializeField]
    public Quest testQuest;
    public Quest GenerateQuest()
    {
        Quest quest = new Quest();
        quest.name = namePool[Random.Range(0, namePool.Count)];
        quest.flavourText = flavourTextPool[Random.Range(0, flavourTextPool.Count)];

        List<int> keywordValues = PerlinGenerator.Generate(GeneratorData, new Vector2(keywordPool.Count, 1), 100, 0, 1, Random.Range(0, 1000000));

        int i = 0;
        foreach(Keyword keyword in keywordPool.Keys)
        {
            quest.keywords.Add(keyword, keywordPool[keyword][(int)(Mathf.InverseLerp(0, 100, keywordValues[i])*keywordPool[keyword].Count)]);
            ++i;
        }

        return quest;
    }

    void Start()
    {
        testQuest = GenerateQuest();
    }
}
