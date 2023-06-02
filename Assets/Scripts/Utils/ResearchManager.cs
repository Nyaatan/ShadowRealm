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
    private List<string> pos_log = new List<string>();
    private List<string> hit_log = new List<string>();
    public ushort research_id = 0;
    public bool invunerability = false;
    short ping = 0;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(EntityMP.inSession) {
            foreach(Player player in EntityMP.List.Values) 
            {   
                player.invunerable = invunerability;
            }
        }
    }

    public void CalculatePing(long timestamp)
    {
        ping = NetworkManager.Singleton.Client.RTT;
    }

    void Awake()
    {
        Instance = this;
        StartCoroutine(WriteLog());
    }

    public void AssignSpellID(Spell spell, ushort ID=65535){
        if(EntityMP.inSession){
            if(ID == 65535) ID = (ushort)((((ushort)NetworkManager.Singleton.Client.Id) << 15) + spellID++);
            spell.id = ID;
            AddSpell(spell);
        }
    }

    public ushort CreateSpellID(){
        if(EntityMP.inSession){
            ushort ID = (ushort)((((ushort)NetworkManager.Singleton.Client.Id) << 15) + spellID++);
            return ID;
        }
        else return 65535;
    }

    public void AddSpell(Spell spell){
        if(EntityMP.inSession) spells.Add(spell.id, spell);
    }

    private float GetSpellDistance(Spell spell, ushort playerID){
        return EntityMP.List[playerID].gameObject.GetComponent<Collider2D>().Distance(spell.gameObject.GetComponent<Collider2D>()).distance;
    }
    private float GetSpellDistance(DestroyedSpell spell, ushort playerID){

        return Mathf.Max(Vector3.Distance(EntityMP.List[playerID].gameObject.GetComponent<Collider2D>().ClosestPoint(spell.position), spell.position) - spell.width, 0);
    }
    public void RegisterHit(ushort spellID, ushort playerID){
        if(EntityMP.inSession){
            if(spells.TryGetValue(spellID, out Spell spell)){
                Log(string.Join(";", new List<string> {
                    GetTimestamp().ToString(),
                    ping.ToString(),
                    "HIT",
                     spellID.ToString(),
                     playerID.ToString(), 
                     }), "hit");
            }
            else if(destroyedSpells.TryGetValue(spellID, out DestroyedSpell dspell)){
                Log(string.Join(";",  new List<string> {
                    GetTimestamp().ToString(),
                    ping.ToString(),
                    "HIT", 
                    spellID.ToString(), 
                    playerID.ToString()
                    }), "hit");
            }
        }
    }

    public void HandleDestroySpell(Spell spell){
        if(EntityMP.inSession){
            DestroyedSpell dspell = new DestroyedSpell();
            dspell.id = spell.id;
            dspell.position = spell.gameObject.transform.position;
            dspell.width = spell.data.width;
            dspell.nature = spell.data.nature;
            spells.Remove(spell.id);
            destroyedSpells.Add(dspell.id, dspell);
        }
    }

    public void HandlePositionChange(GameObject obj, Vector2 newPosition){
        if(EntityMP.inSession){
            Log(string.Join(";", new List<string> {
                    GetTimestamp().ToString(),
                    ping.ToString(),
                    Vector2.Distance(obj.transform.position, newPosition).ToString(),
                    "null",
                    obj.GetComponent<PlayerMovement>().horizontalMove.ToString()
            }), "pos");
        }
    }

    public void HandleSpellCollision(Spell spell, GameObject obj){
        ushort objID = 0;
        try
        {
            objID = obj.GetComponent<Player>().id;
        }
        catch
        {   }
        if(EntityMP.inSession){
            Log(string.Join(";", new List<string> {
                    GetTimestamp().ToString(),
                    ping.ToString(),
                    "COL",
                    spell.id.ToString(),
                    objID.ToString()
                    }), "hit");
        }
    }

    public void Log(string data, string file){
        if(file == "pos")
            pos_log.Add(data);
        else if (file == "hit")
            hit_log.Add(data);
    }

     public static long GetTimestamp()
    {
        return (System.DateTime.UtcNow.Ticks / System.TimeSpan.TicksPerMillisecond);
    }

    public void WriteToFile(){
        string path = @"pos" + research_id + ".log";
        if (File.Exists(path)) using (StreamWriter sw = File.AppendText(path))
            {
                foreach(string line in pos_log) sw.WriteLine(line);
            }
            else using (StreamWriter sw = File.CreateText(path))
            {
                foreach(string line in pos_log) sw.WriteLine(line);
            }
        pos_log.Clear();
        path = @"hit" + research_id + ".log";
        if (File.Exists(path)) using (StreamWriter sw = File.AppendText(path))
            {
                foreach(string line in hit_log) sw.WriteLine(line);
            }
            else using (StreamWriter sw = File.CreateText(path))
            {
                foreach(string line in hit_log) sw.WriteLine(line);
            }
        hit_log.Clear();
    }

    private IEnumerator WriteLog(){
        while(true){
            ResearchManager.Instance.WriteToFile();
            yield return new WaitForSeconds(1);
        }
    }

    public void Reset(){
        spells = new Dictionary<ushort, Spell>(); 
        destroyedSpells = new Dictionary<ushort, DestroyedSpell>(); 
        pos_log = new List<string>();
        hit_log = new List<string>();
        spellID = 0;
    }

    public class DestroyedSpell{
        public ushort id;
        public Vector3 position;
        public float width;
        public GlyphData.Nature nature;
    }
}
