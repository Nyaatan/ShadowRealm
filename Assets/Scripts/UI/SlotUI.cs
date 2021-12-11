using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image background;
    public Image sprite;
    public int id;

    public void Start()
    {
        
    }

    public void OnClick()
    {
        gameObject.GetComponentInParent<InventoryUI>().OnClickSelectSlot(id);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Glyph glyph = gameObject.GetComponentInParent<InventoryUI>().OnPointerEnterSelectSlot(id);
        if (glyph != null) Tooltip.Instance.ShowTooltip(glyph.GetTooltipString());
        else if (gameObject.GetComponentInParent<InventoryUI>().GetComponentInParent<GlyphTableUI>() != null && id == 4)
            Tooltip.Instance.ShowTooltip("Slot for empty glyph");
        else Tooltip.Instance.ShowTooltip("Empty slot");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tooltip.Instance.HideTooltip();
    }
}
