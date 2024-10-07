using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Acorn : Interactable
{

    bool collectable;

    float fall_time_min = 1.1f;
    float fall_time_max = 1.2f;
    Rigidbody2D rb;

    public float max_acorn_yeet_speed;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        collectable = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void fall_from_tree() {

        rb.velocity = new Vector2(0.7f, 2.2f);
        rb.gravityScale = 1;

        StartCoroutine(stop_acorn(Random.Range(fall_time_min, fall_time_max)));
    }


    IEnumerator stop_acorn(float seconds) {

        //Debug.Log("Stop acorn in: " + seconds);

        yield return new WaitForSeconds(seconds);

        Debug.Log("Stop acorn.");
        rb.velocity = new Vector2(0f, 0f);
        rb.gravityScale = 0;
        // Add the Resource tag to acorn
        gameObject.tag = "Resource";
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {

        check_indicate_can_interact(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {


        check_indicate_cannot_interact(collision);
    }

    public void pile()
    {
        
    }

    override public void interact()
    {
        //Debug.Log("yoink acorn.");
        AudioManager.instance.Play("Acorn Pickup");
        GameManager.instance.add_acorn();
        Destroy(gameObject);
    }
}
