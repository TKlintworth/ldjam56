using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndMenu : MonoBehaviour
{
    public GameObject end_menu;
    public TextMeshProUGUI stats_text;

    public void set_stats_text(string text)
    {
        Debug.Log("Set text to: " + text);
        stats_text.SetText(text);
    }
}
