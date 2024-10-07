using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IState
{
    float fear_cooldown;
    public int max_move_distance;
    public int min_move_distance;

    Vector2 moveTarget;

    public void OnEnter() {
        fear_cooldown = 0.0f;
        min_move_distance = 5;
        max_move_distance = 7;
    }

    public void UpdateState() {
        Debug.Log("Idling");
        // Pick a random location nearby and move there...

        Vector2 random_loc = Random.insideUnitCircle * Utils.RandomGaussian(min_move_distance, max_move_distance);
        //moveTarget = (Vector2) gameObject.transform.position + random_loc;
    }

    public void FixedUpdateState() { }

    public void OnExit() { }
}
