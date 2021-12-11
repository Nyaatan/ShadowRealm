using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlyphTableUI : MonoBehaviour
{
    public GlyphTable table;
    public InventoryUI slots;
    public Player player;
    public RectTransform vectorSpace;
    public RectTransform point;

    Vector2 vectorSpaceSize;
    
    public void Start()
    {
        vectorSpaceSize = vectorSpace.rect.size;
        gameObject.SetActive(false);
        player = GameManager.Instance.player.GetComponent<Player>();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Close();
        }
    }

    void Close()
    {
        GameManager.Instance.player.GetComponent<PlayerMovement>().enabled = true;
        gameObject.SetActive(false);
    }

    public void OnClickSlot(int number)
    {
        /*foreach (Glyph glyph in table.glyphs) if (glyph != null)
            {
                Debug.Log(glyph.isOnTable);
                Debug.Log(number);
            }
            else Debug.Log(glyph);
        Debug.Log(table.glyphs[0]);*/
        try
        {
            if (number == slots.slots.Count - 1)
            {
                if (table.resultGlyph == null)
                {
                    table.AddEmptyGlyph(player.GetComponent<Player>().pickableInHand as Glyph);

                    slots.SetSpriteInSlot((player.GetComponent<Player>().pickableInHand as Glyph).prefab.GetComponent<SpriteRenderer>().sprite, number);
                }
                else
                {
                    table.RemoveEmptyGlyph();
                    slots.SetSpriteInSlot(null, number);
                }
            }

            else if (table.glyphs[number] == null) {
                table.AddGlyph(player.GetComponent<Player>().pickableInHand as Glyph, number);
                slots.SetSpriteInSlot((player.GetComponent<Player>().pickableInHand as Glyph).prefab.GetComponent<SpriteRenderer>().sprite, number);
            }
            else
            {
                table.RemoveGlyph(number);

                slots.SetSpriteInSlot(null, number);
            }
        } catch (GlyphTable.GlyphCreationException e)
        {
            Debug.LogWarning(e);
        }
        catch (System.NullReferenceException e)
        {
            Debug.LogWarning(e);
        }
        MovePoint(table.currentVector);
    }

    public void OnClickCraft()
    {
        for(int i = 0; i < slots.slots.Count; ++i)
        {
            slots.SetSpriteInSlot(null, i);
        }
    }

    public void MovePoint(Vector2 currentVector)
    {
        point.localPosition = currentVector * vectorSpaceSize;
    }
}
