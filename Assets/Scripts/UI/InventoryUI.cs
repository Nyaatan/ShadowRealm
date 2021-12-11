using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public List<SlotUI> slots;
    public SlotUI selectedSlot;

    public Color defaultColor;
    public Color selectedColor;


    public void Start()
    {
        for (int i = 0; i < slots.Count; ++i)
        {
            slots[i].id = i;
        }
        selectedSlot = slots[0];
        SelectSlot(0);
    }

    public void SelectSlot(int number)
    {
        SlotUI slot = slots[number];
        selectedSlot.background.color = defaultColor;
        selectedSlot = slot;
        selectedSlot.background.color = selectedColor;
    }

    public void OnClickSelectSlot(int number)
    {
        try
        {
            transform.parent.GetComponent<GlyphTableUI>().OnClickSlot(number);
        }
        catch (System.Exception e) {
            Debug.LogWarning(e);
            GameManager.Instance.player.GetComponent<Player>().selected = number;
        }
    }

    public Glyph OnPointerEnterSelectSlot(int number)
    {
        try
        {
            return transform.parent.GetComponent<GlyphTableUI>().table.glyphs[number];
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e);
            if (GameManager.Instance.player.GetComponent<Player>().inventory.pickables.Count > number)
                return GameManager.Instance.player.GetComponent<Player>().inventory.pickables[number] as Glyph;
        }
        return null;
    }

    public void SetSpriteInSlot(Sprite sprite, int slotId)
    {
        if(sprite == null)
        {
            slots[slotId].sprite.sprite = sprite;
            slots[slotId].sprite.color = new Color();
        }
        else
        {
            slots[slotId].sprite.color = new Color(255,255,255,255);
            slots[slotId].sprite.sprite = sprite;
        }
    }


}
