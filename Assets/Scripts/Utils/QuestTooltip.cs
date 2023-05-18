using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestTooltip : MonoBehaviour
{
    public GameObject text;

    public void SetText(string text)
    {
        this.text.GetComponent<TextMeshProUGUI>().text = text;
    }
}
