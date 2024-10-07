using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameUI : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshProUGUI acorn_count;
    public TextMeshProUGUI bunny_count;

    public void set_acorns_text(string text)
    {
        acorn_count.SetText(text);
    }

    public void set_bunnies_text(string text)
    {
        bunny_count.SetText(text);
    }
}
