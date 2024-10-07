using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    enum State
    {
        Active,
        Inactive
    };

    public GameObject enemy_prefab;
    public float base_respawn_cadence;

    public float day_mult;
    public float night_mult;
    
    float respawn_timer;
    State current_state;

    float min_spawn_delay;

    // Start is called before the first frame update
    void Start()
    {
        current_state = State.Active;
        respawn_timer = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        respawn_timer += Time.fixedDeltaTime;

        if (current_state == State.Active)
        {
            if (respawn_timer >= compute_respawn_delay())
            {
                Instantiate(enemy_prefab, gameObject.transform.position, Quaternion.identity);
                respawn_timer = 0;
            }
        }
    }

    float compute_respawn_delay()
    {
        float spawn_del = base_respawn_cadence;
        if (GameManager.instance.get_current_time_state() == GameManager.TimeState.Day)
        {
            spawn_del *= day_mult;
        }
        else if (GameManager.instance.get_current_time_state() == GameManager.TimeState.Night)
        {
            spawn_del *= night_mult;
        }

        int num_days = GameManager.instance.get_days_passed();
        spawn_del *= Mathf.Max(0.1f, 1 - (num_days / 10));

        int num_villagers = GameManager.instance.get_all_villagers().Length;
        spawn_del *= Mathf.Max(0.1f, 1 - (num_villagers / 100));

        return spawn_del;
    }



    public void activate_spawner()
    {
        current_state = State.Active;
    }

    public void deactivate_spawner()
    {
        current_state = State.Inactive;
    }
}
