using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject mainCamera;
    public GameObject player;
    public Dungeon dungeon;

    public Animator transition;

    public float transitionTime = 0.5f;

    public GameObject challengeRoomActivatorPrefab;
    public GameObject treasurePrefab;
    public GameObject glyphPrototype;

    [SerializeField]
    public List<SpellData> defaultSpellDatas = new List<SpellData>();

    public QuestGenerator questGenerator;

    [SerializeField]
    public List<Affinity> affinities;
    public GameObject floatDamage;

    void Awake()
    {
        GameManager.Instance = this;

    }

    public SpellData getDefaultSpellData(GlyphData.Nature nature, GlyphData.Element element)
    {
        foreach (SpellData data in defaultSpellDatas) if (data.nature == nature && data.elements[0] == element) return data;
        return null;
    }

    public void enterBlackScreen(Location destination) {
        StartCoroutine(MakeTransition(destination));
        player.gameObject.GetComponent<PlayerMovement>().enabled = false;
    }
    public void exitBlackScreen() {
        player.gameObject.GetComponent<PlayerMovement>().enabled = true;
        player.gameObject.GetComponent<Player>().invunerable = false;
        player.gameObject.GetComponent<Player>().enabled = true;
        foreach (GameObject entity in dungeon.enemiesOnScreen) entity.SetActive(true);
    }

    public IEnumerator MakeTransition(Location destination)
    {
        transition.SetTrigger("Start");
        player.gameObject.GetComponent<Player>().invunerable = true;
        player.gameObject.GetComponent<Player>().enabled = false;
        yield return new WaitForSeconds(transitionTime);
        player.gameObject.GetComponent<PlayerMovement>().enabled = false;
        if (destination == null) dungeon.GetComponent<Location>().Enter();
        else destination.Enter();
        transition.SetTrigger("Done");
        exitBlackScreen();
    }
}

[System.Serializable]
public class Affinity
{
    public GlyphData.Element element;
    public GlyphData.Element strength;
    public GlyphData.Element weakness;
}
