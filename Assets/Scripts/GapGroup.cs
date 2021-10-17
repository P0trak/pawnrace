using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GapGroup : MonoBehaviour
{
    public void setGroupInteractable(bool interactable)
    {
        Button[] buttons = GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            button.interactable = interactable;
        }
    }

    public void setWhiteText(string gap)
    {
        TextMeshProUGUI text = GetComponentInChildren<TextMeshProUGUI>();
        if (gap.Equals(""))
        {
            text.text = "";
        }
        else
        {
           text.text = "White gap at " + gap; 
        }
    }

    public void setBlackText(string gap)
    {
        TextMeshProUGUI text = GetComponentInChildren<TextMeshProUGUI>();
        if (gap.Equals(""))
        {
            text.text = "";
        }
        else
        {
           text.text = "Black gap at " + gap; 
        }
        
    }
}
