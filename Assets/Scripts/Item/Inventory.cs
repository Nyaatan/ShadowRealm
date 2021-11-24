using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory
{
    public Dictionary<Pickable, int> inventory = new Dictionary<Pickable, int>();
    public List<Pickable> pickables = new List<Pickable>();
    public int count = 0;
    public int size = 5;

    public bool Add(Pickable item)
    {
        if (count < size)
        {
            if (inventory.ContainsKey(item))
            {
                inventory[item]++;
            }
            else
            {
                inventory.Add(item, 1);
                pickables.Add(item);
                ++count;
            }
            return true;
        }
        else return false;
    }

    public void Remove(Pickable item)
    {
        try
        {
            inventory[item]--;
            if (inventory[item] <= 0)
            {
                inventory.Remove(item);
                pickables.Remove(item);
                --count;
            }
        }
        catch (KeyNotFoundException) { }
    }

    public void Remove(int i)
    {
        try
        {
            Pickable item = pickables[i];
            inventory[item]--;
            if (inventory[item] <= 0)
            {
                inventory.Remove(item);
                pickables.Remove(item);
                --count;
            }
        }
        catch (System.Exception) { }
    }

    public Pickable Get(int i)
    {
        if (count > i) return pickables[i];
        else return null;
    }

    public int GetStack(int i)
    {
        if (count > i) return inventory[pickables[i]];
        else return -1;
    }
}
