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
            if(ID == 65535) ID = (ushort)((((ushort)NetworkManager.Singleton.Client.Id) << 12) + spellID++);
            spell.id = ID;
            AddSpell(spell);
        }
    }

    public ushort CreateSpellID(){
        if(EntityMP.inSession){
            ushort ID = (ushort)((((ushort)NetworkManager.Singleton.Client.Id) << 12) + spellID++);
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
                    "HIT", spellID.ToString(),
                     spell.gameObject.transform.position.ToString(), 
                     spell.data.nature.ToString(),
                     spell.data.width.ToString(), 
                     playerID.ToString(), 
                     GetSpellDistance(spell, playerID).ToString(),
                     ping.ToString()
                     }));
            }
            else if(destroyedSpells.TryGetValue(spellID, out DestroyedSpell dspell)){
                Log(string.Join(";",  new List<string> {
                    GetTimestamp().ToString(),
                    "DHT", 
                    spellID.ToString(), 
                    dspell.position.ToString(), 
                    dspell.nature.ToString(),
                    dspell.width.ToString(), 
                    playerID.ToString(), 
                    GetSpellDistance(dspell, playerID).ToString(),
                     ping.ToString()
                    }));
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

    public void HandlePositionChange(GameObject obj, Vector2 lagDistance){
        if(EntityMP.inSession){
            Vector3 pos = lagDistance;
            pos.z = obj.transform.position.z;
            //Log("POS;" + obj.transform.position + ";" + pos + ";" + obj.GetComponent<PlayerMovement>().horizontalMove + ";" + Vector3.Distance(obj.transform.position, pos));
            Log(string.Join(";", new List<string> {
                    GetTimestamp().ToString(),
                    "POS",
                    obj.transform.position.ToString(),
                    pos.ToString(),
                    obj.GetComponent<PlayerMovement>().horizontalMove.ToString(),
                    Vector3.Distance(obj.transform.position, pos).ToString()
                    })); ;
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
            //Log("COL;" + spell.id + ";" + obj.transform.position + ";" + spell.data.range);
            Log(string.Join(";", new List<string> {
                    GetTimestamp().ToString(),
                    "COL",
                    spell.id.ToString(),
                    obj.transform.position.ToString(),
                    spell.data.range.ToString(),
                    objID.ToString(),
                    ping.ToString()
                    }));
        }
    }

    public void Log(string data){
        log.Add(data);
    }

     public static long GetTimestamp()
    {
        return (System.DateTime.UtcNow.Ticks / System.TimeSpan.TicksPerMillisecond);
    }

    public void WriteToFile(){
        string path = @"research" + research_id + ".log";
        //Debug.Log("LOGGING " + log.Count + " LINES");
        if (File.Exists(path)) using (StreamWriter sw = File.AppendText(path))
            {
                //Debug.Log(GetTimestamp());
                foreach(string line in log) sw.WriteLine(line);
            }
            else using (StreamWriter sw = File.CreateText(path))
            {
                foreach(string line in log) sw.WriteLine(line);
            }
        log.Clear();
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
        log = new List<string>();
        spellID = 0;
    }

    public class DestroyedSpell{
        public ushort id;
        public Vector3 position;
        public float width;
        public GlyphData.Nature nature;
    }
}
