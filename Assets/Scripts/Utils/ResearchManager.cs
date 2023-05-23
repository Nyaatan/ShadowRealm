using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ResearchManager : MonoBehaviour
{
    public static ResearchManager Instance;
    public ushort spellID;
    public Dictionary<ushort, Spell> spells = new Dictionary<ushort, Spell>(); 
    public Dictionary<ushort, DestroyedSpell> destroyedSpells = new Dictionary<ushort, DestroyedSpell>(); 
    private List<string> log = new List<string>();
    public ushort research_id = 0;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Awake()
    {
        Instance = this;
        StartCoroutine(WriteLog());
    }

    public void AssignSpellID(Spell spell){
        if(EntityMP.inSession){
            ushort ID = (ushort)((((ushort)NetworkManager.Singleton.Client.Id) << 8) + spellID++);
            spell.id = ID;
            AddSpell(spell);
        }
    }

    public void AddSpell(Spell spell){
        if(EntityMP.inSession) spells.Add(spell.id, spell);
    }

    public void RegisterHit(ushort spellID){
        if(EntityMP.inSession){
            if(spells.TryGetValue(spellID, out Spell spell)){
                Log("HIT;" + spellID + ";" + spell.gameObject.transform.position + ";" + spell.data.range);
            }
            else if(destroyedSpells.TryGetValue(spellID, out DestroyedSpell dspell)){
                Log("DHT;" + spellID + ";" + dspell.position + ";" + dspell.range);
            }
        }
    }

    public void HandleDestroySpell(Spell spell){
        if(EntityMP.inSession){
            DestroyedSpell dspell = new DestroyedSpell();
            dspell.id = spell.id;
            dspell.position = spell.gameObject.transform.position;
            dspell.range = spell.data.range;
            spells.Remove(spell.id);
            destroyedSpells.Add(dspell.id, dspell);
        }
    }

    public void HandlePositionChange(GameObject obj, Vector2 newPosition){
        if(EntityMP.inSession){
            Vector3 pos = newPosition;
            pos.z = obj.transform.position.z;
            Log("POS;" + obj.transform.position + ";" + pos + ";" + obj.GetComponent<PlayerMovement>().horizontalMove + ";" + Vector3.Distance(obj.transform.position, pos));
        }
    }

    public void HandleSpellCollision(Spell spell, GameObject obj){
        if(EntityMP.inSession){
            Log("COL;" + spell.id + ";" + obj.transform.position + ";" + spell.data.range);
        }
    }

    public void Log(string data){
        log.Add(data);
    }

    private IEnumerator WriteLog(){
        Debug.Log("DUPADUPADUPAUDPAUDPAUDPAUDPUPAD" + log.Count);
        string path = @"research" + research_id + ".log";
        if (File.Exists(path)) using (StreamWriter sw = File.AppendText(path))
        {
            foreach(string line in log) sw.WriteLine(line);
        }
        else using (StreamWriter sw = File.CreateText(path))
        {
            foreach(string line in log) sw.WriteLine(line);
        }
        yield return new WaitForSeconds(1);
        log.Clear();
        StartCoroutine(WriteLog());
    }

    public void Reset(){
        spells = new Dictionary<ushort, Spell>(); 
        destroyedSpells = new Dictionary<ushort, DestroyedSpell>(); 
        log = new List<string>();
        spellID = 0;
    }

    public class DestroyedSpell{
        public ushort id;
        public Vector3 position;
        public float range;
    }
}
