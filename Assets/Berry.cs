using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Berry : Interactable
{

    bool collectable;

    float fall_time_min = 0.5f;
    float fall_time_max = 0.8f;

    bool picked;
    public bool is_piled;

    Rigidbody2D rb;

    public float max_berry_yeet_speed;
    public GameObject me_prefab;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        collectable = false;
        picked = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void pick()
    {
        if (picked == false)
        {
            picked = true;
            rb.velocity = new Vector2(0.7f, 2.2f);
            rb.gravityScale = 1;

            StartCoroutine(stop_berry(Random.Range(fall_time_min, fall_time_max)));
        }
    }


    IEnumerator stop_berry(float seconds)
    {
        //Debug.Log("Stop berry in: " + seconds);
        gameObject.tag = "Resource";

        yield return new WaitForSeconds(seconds);

        //Debug.Log("Stop berry.");
        rb.velocity = new Vector2(0f, 0f);
        rb.gravityScale = 0;
        // Add the Resource tag to berry
        
    }

    public void pile()
    {
        is_piled = true;
        gameObject.tag = "Untagged";
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Can interact.");
        if (is_piled)
        {
            check_indicate_can_interact(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("Cannot interact.");

        if (is_piled)
        {
            check_indicate_cannot_interact(collision);
        }
    }

    override public void interact()
    {
        Debug.Log("Ate berry");
        GameManager.instance.eat_berry();
        Destroy(gameObject);
    }

}

