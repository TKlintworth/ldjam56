using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tombstone : MonoBehaviour
{

    enum State { 
        Active,
        Inactive
    };

    public Enemy ghost_prefab;
    State current_state;

    // Start is called before the first frame update
    void Start()
    {
        current_state = State.Inactive;
        StartCoroutine(spawn_ghost());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator spawn_ghost() {
        if (current_state == State.Active) {
            Debug.Log("Spawning ghost ...");
            Instantiate(ghost_prefab, gameObject.transform.position, Quaternion.identity);
        }
        yield return new WaitForSeconds(get_ghost_spawn_delay());
        StartCoroutine(spawn_ghost());
    }

    public void activate_tombstone()
    {
        Debug.Log("TOMBSTONES ACTIVATED");
        current_state = State.Active;

    }

    public void deactivate_tombstone() {
        Debug.Log("TOMBSTONES DEACTIVATED");
        current_state = State.Inactive;
    }

    float get_ghost_spawn_delay() {
        return 10;
    }
}
