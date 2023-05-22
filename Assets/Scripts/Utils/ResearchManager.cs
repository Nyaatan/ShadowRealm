using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResearchManager : MonoBehaviour
{
    public static ResearchManager Instance;
    public ushort spellID;
    public Dictionary<ushort, Spell> spells = new Dictionary<ushort, Spell>(); 
    public Dictionary<ushort, DestroyedSpell> destroyedSpells = new Dictionary<ushort, DestroyedSpell>(); 

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
                Debug.Log(spellID + " " + spell.gameObject.transform.position + " " + spell.data.range);
            }
            else if(destroyedSpells.TryGetValue(spellID, out DestroyedSpell dspell)){
                Debug.Log(spellID + " D " + dspell.position + " " + dspell.range);
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
            Debug.Log("POS " + obj.transform.position + " " + pos + " | HOR: " + obj.GetComponent<PlayerMovement>().horizontalMove + " | DIFF: " + Vector3.Distance(obj.transform.position, pos));
        }
    }

    public void HandleSpellCollision(Spell spell, GameObject obj){
        if(EntityMP.inSession){
            Debug.Log("COL " + spell.id + " " + obj.transform.position);
        }
    }

    public void Reset(){
        spells = new Dictionary<ushort, Spell>(); 
        destroyedSpells = new Dictionary<ushort, DestroyedSpell>(); 
        spellID = 0;
    }

    public class DestroyedSpell{
        public ushort id;
        public Vector3 position;
        public float range;
    }
}
