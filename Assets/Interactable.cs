using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    // Start is called before the first frame update
    
    public abstract void interact();

    public void check_indicate_can_interact(Collider2D collision)
    {
        if (collision.name == "Bear")
        {
            collision.GetComponent<Bear>().enter_interactables_interactable_radius(gameObject);
        }
    }

    public void check_indicate_cannot_interact(Collider2D collision)
    {
        if (collision.name == "Bear")
        {
            collision.GetComponent<Bear>().exit_interactables_interactable_radius(gameObject);
        }
    }
}
