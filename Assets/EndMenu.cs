using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class EndMenu : MonoBehaviour
{
    public GameObject end_menu;
    public TextMeshProUGUI stats_text;

    public void set_stats_text(string text)
    {
        Debug.Log("Set text to: " + text);
        stats_text.SetText(text);
    }

    public void home_button_pressed()
    {
        SceneManager.LoadScene("mainmenu");
    }

    public void quit_button_pressed()
    {
        Application.Quit();
    }
}
