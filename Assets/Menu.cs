using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void on_play_button()
    {
        SceneManager.LoadScene(1);
    }

    public void on_quit_button()
    {
        Application.Quit();
    }
}
