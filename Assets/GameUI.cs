using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameUI : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshProUGUI acorn_count;
    public TextMeshProUGUI bunny_count;

    public GameObject dial;
    public GameObject time_dial;


    public void set_acorns_text(string text)
    {
        acorn_count.SetText(text);
    }

    public void set_bunnies_text(string text)
    {
        bunny_count.SetText(text);
    }

    public void Update()
    {
        float bs = GameManager.instance.get_bear_speed();
        float percent = bs / (float)GameManager.instance.get_max_bear_speed();

        float dial_rot = Mathf.Lerp(50, -50, percent);

        dial.transform.rotation = Quaternion.Euler(0, 0, dial_rot);


        float tod = GameManager.instance.get_time_of_day();
        float perc_day = tod / GameManager.instance.day_night_cycle_time;

        float dial_rot_time = Mathf.Lerp(-50, 50, perc_day);


        time_dial.transform.rotation = Quaternion.Euler(0, 0, dial_rot_time);
    }
}
