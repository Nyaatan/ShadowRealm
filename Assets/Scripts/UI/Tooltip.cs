using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    public static Tooltip Instance;

    public Text tooltipText;
    public RectTransform backgroundRectTransform;
    public Camera mainCamera;

    public void ShowTooltip(string text)
    {
        gameObject.SetActive(true);

        tooltipText.text = text;

        float textPadding = 10f;
        Vector2 bgSize = new Vector2(tooltipText.preferredWidth, tooltipText.preferredHeight);

        //backgroundRectTransform.sizeDelta = bgSize;
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }

    public void Awake()
    {
        Tooltip.Instance = this;
        HideTooltip();
    }
}
