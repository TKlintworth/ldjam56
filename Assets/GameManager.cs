using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using TMPro;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    public enum TimeState {
        Day,
        Night
    };

    public enum GameState
    {
        Playing,
        Win,
        Lose
    }

    float time_of_day = 0;
    
    public float day_night_cycle_time = 86; // s
    public float night_transition_time = 28; // s
    public float color_transition_time = 10;

    Color night_color = new Color(161.0f/255.0f, 87.0f/255.0f, 203.0f/255.0f);
    Color day_color = Color.white;

    public GameObject pile;
    public GameObject night_light;
    public GameObject bear;
    public GameObject pause_menu;
    public GameObject end_menu;
    public GameObject game_ui;

    TimeState current_state;
    Vector2 average_villager_center;
    int num_days_passed;
    int current_bear_size = 1;
    int max_bear_size = 10;
    int consumables_to_increase_bear_size = 1;
    int current_eaten_consumables = 0;
    int total_eaten_consumables = 0;
    int total_villagers = 0;
    int total_villagers_killed = 0;
    int score = 0;
    int total_enemies_killed = 0;
    int num_acorns;
    bool is_paused = false;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        current_state = TimeState.Day;
        num_days_passed = 0;
        num_acorns = 0;
        total_villagers = FindObjectsOfType<Villager>().Length;
        game_ui.GetComponent<GameUI>().set_bunnies_text(total_villagers.ToString());

        Debug.Log("initial villagers: " + total_villagers);
        AudioManager.instance.Play("bear_anthem");

    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (Input.GetKey(KeyCode.Escape) && is_paused == false)
        {
            pause_menu.SetActive(true);
            pause_menu.GetComponent<PauseMenu>().Pause();
            is_paused = true;
            Time.timeScale = 0;
        }
        else if (Input.GetKey(KeyCode.Escape) && is_paused == true)
        {
            // unpause
            pause_menu.SetActive(false);
            pause_menu.GetComponent<PauseMenu>().Resume();
            is_paused = false;
            Time.timeScale = 1;
        }


        time_of_day += Time.fixedDeltaTime;

        if (current_state == TimeState.Day && time_of_day >= night_transition_time)
        {
            Debug.Log("Night Time");

            current_state = TimeState.Night;
            invoke_night_transition_events();
        }

        else if (current_state == TimeState.Night && time_of_day >= day_night_cycle_time)
        {
            Debug.Log("Day Time");
            time_of_day = 0;
            current_state = TimeState.Day;
            invoke_day_transition_events();
            num_days_passed += 1;
        }


        if (time_of_day > (night_transition_time - color_transition_time) && time_of_day < night_transition_time) {
            night_light.GetComponent<Light2D>().color = Color.Lerp(night_color, day_color, (night_transition_time - time_of_day) / color_transition_time);
        }

        if (time_of_day > (day_night_cycle_time - color_transition_time) && time_of_day < day_night_cycle_time)
        {
            night_light.GetComponent<Light2D>().color = Color.Lerp(day_color, night_color, (day_night_cycle_time - time_of_day) / color_transition_time);
        }


    }

    void invoke_night_transition_events() {

        GameObject[] tombstones = GameObject.FindGameObjectsWithTag("Tombstone");

        foreach (GameObject t in tombstones) {
            t.GetComponent<Tombstone>().activate_tombstone();
        }

        night_light.GetComponent<Light2D>().color = night_color;
    }

    void invoke_day_transition_events()
    {
        GameObject[] tombstones = GameObject.FindGameObjectsWithTag("Tombstone");

        foreach (GameObject t in tombstones)
        {
            t.GetComponent<Tombstone>().deactivate_tombstone();
        }

        night_light.GetComponent<Light2D>().color = new Color(1.0f, 1.0f, 1.0f);
    }


    public GameObject[] get_all_villagers() {
        return GameObject.FindGameObjectsWithTag("Villager");
    }

    public Vector2 get_average_villager_center()
    {

        GameObject[] villagers = GameObject.FindGameObjectsWithTag("Villager");

        float x_sum = 0;
        float y_sum = 0;
        foreach (GameObject v in villagers) {
            x_sum += v.transform.position.x;
            y_sum += v.transform.position.y;
        }

        float x_bar = x_sum / villagers.Length;
        float y_bar = y_sum / villagers.Length;

        average_villager_center = new Vector2(x_bar, y_bar);

        return average_villager_center;
    }

    public GameObject find_closest_resource(GameObject gameo) {
        GameObject[] available_resources = GameObject.FindGameObjectsWithTag("Resource");
        if (available_resources.Length == 0)
        {
            return null;
        } else
        {
            //Get the closest resource
            GameObject closest_resource = null;
            float dist_to_closest = float.PositiveInfinity;
            foreach (GameObject v in available_resources)
            {
                if ((gameo.transform.position - v.transform.position).magnitude < dist_to_closest)
                {
                    dist_to_closest = (gameo.transform.position - v.transform.position).magnitude;
                    closest_resource = v;
                }
            }
            return closest_resource;
        }
    }


    public GameObject find_closest_bush(GameObject gameo) {
        GameObject[] available_bushes = GameObject.FindGameObjectsWithTag("Bush");
        if (available_bushes.Length == 0)
        {
            return null;
        } else
        {
            // Get the closest bush
            GameObject closest_bush = null;
            float dist_to_closest = float.PositiveInfinity;
            foreach (GameObject v in available_bushes)
            {
                if ((gameo.transform.position - v.transform.position).magnitude < dist_to_closest)
                {
                    dist_to_closest = (gameo.transform.position - v.transform.position).magnitude;
                    closest_bush = v;
                }
            }
            return closest_bush;
        }
    }

    public GameObject[] get_all_spawners()
    {
        return GameObject.FindGameObjectsWithTag("Spawner");
    }

    public TimeState get_current_time_state()
    {
        return current_state;
    }

    public int get_days_passed()
    {
        return num_days_passed;
    }

    public void eat_berry()
    {
        if (current_eaten_consumables == consumables_to_increase_bear_size)
        {
            increase_bear_size();
        } else
        {
            current_eaten_consumables += 1; 
        }
    }

    void increase_bear_size()
    {
        // TODO: Play a sound
        // TODO: Increase the size of the bear
        bear.transform.localScale += new Vector3(5, 5, 1);
        if (current_bear_size != max_bear_size)
        {
            current_bear_size += 1;
        } 
        current_eaten_consumables = 0;
    }


    public void add_acorn()
    {

        num_acorns += 1;
        //GameObject[] trees = GameObject.FindGameObjectsWithTag("Tree");
        //foreach (GameObject t in trees)
        //{
        //    t.GetComponent<Tree>().update_acorn_text(num_acorns);

        //}
        // TODO: update acorn UI
        // 
        game_ui.GetComponent<GameUI>().set_acorns_text(num_acorns.ToString());
        Debug.Log(num_acorns);
        
    }

    public void use_acorns(int num)
    {
        num_acorns -= num;
        game_ui.GetComponent<GameUI>().set_acorns_text(num_acorns.ToString());
    }

    public int get_num_acorns()
    {
        return num_acorns;
    }

    public void add_villager()
    {
        total_villagers += 1;
        game_ui.GetComponent<GameUI>().set_bunnies_text(total_villagers.ToString());
    }

    public void remove_villager()
    {
        total_villagers_killed += 1;
        total_villagers -= 1;
        game_ui.GetComponent<GameUI>().set_bunnies_text(total_villagers.ToString());
    }

    public Vector3 get_pile_location() {
        return pile.gameObject.transform.position;
    }

    public void check_end_game_state()
    {

        GameObject[] villagers = GameObject.FindGameObjectsWithTag("Villager");
        if (villagers.Length <= 0){
            end_game(GameState.Lose);
        }

    }

    void end_game(GameState final_state)
    {

        if (final_state == GameState.Lose)
        {
            Debug.Log("You lost!!!");
            end_menu.SetActive(true);
            end_menu.GetComponent<EndMenu>().set_stats_text("You Lose!");
            Time.timeScale = 0;
            // Show the lose version of the end screen
            // TODO: more stats
        }

        else if (final_state == GameState.Win)
        {
            // Show the win version of the end screen
            // TODO: more stats
            end_menu.SetActive(true);
            end_menu.GetComponent<EndMenu>().set_stats_text("You Win!");
        }
    }


    public float get_bear_speed()
    {
        return bear.GetComponent<Bear>().rb.velocity.magnitude;
    }

    public float get_max_bear_speed()
    {
        return bear.GetComponent<Bear>().max_speed;
    }

    public float get_time_of_day()
    {
        return time_of_day;
    }
}
