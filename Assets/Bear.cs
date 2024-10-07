using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bear : MonoBehaviour
{

    Vector3 accel_dir;
    
    bool accelerating;
    public Rigidbody2D rb;

    public int accel_mult;
    public int drag_scale;
    public int max_speed;
    public float c_drag;

    float curr_size;

    GameObject current_interactable;

    Animator m_animator;

    private void Awake()
    {
        m_animator = gameObject.GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        accel_dir = new Vector3(0, 0, 0);
        rb = gameObject.GetComponent<Rigidbody2D>();
        curr_size = 1;
    }


    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown("space"))
        {
            Debug.Log("pressed space");
            if (current_interactable != null)
            {
                current_interactable.GetComponent<Interactable>().interact();
            }
        }

        m_animator.speed = rb.velocity.magnitude / max_speed;

        if (accelerating)
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            accel_dir = worldPosition - gameObject.transform.position;
            accel_dir = accel_dir.normalized;

            Vector2 new_velocity = rb.velocity + new Vector2(accel_dir.x, accel_dir.y) * Time.fixedDeltaTime * accel_mult;
            if (new_velocity.magnitude > max_speed)
            {
                //Debug.Log("Max reached.");
                Vector2 vel_dir = new_velocity.normalized;
                rb.velocity = vel_dir * max_speed;
            }
            else {
                rb.velocity = new_velocity;
            } 

            if (rb.velocity.x >= 0.1f)
            {
                GetComponent<SpriteRenderer>().flipX = false;
            }
            else if (rb.velocity.x <= 0.1f) {
                GetComponent<SpriteRenderer>().flipX = true;
            }
        }

        //float drag_accel = c_drag * rb.velocity.magnitude * rb.velocity.magnitude;
        float drag_accel = c_drag * curr_size;
        rb.velocity = rb.velocity + (-1) * rb.velocity.normalized * drag_accel * Time.fixedDeltaTime;

        if (Input.GetMouseButtonUp(0)) {
            //Debug.Log("Stopped accelerating.");
            accelerating = false;
        }

    }

    private void OnMouseDown() {
        //Debug.Log("Start Accelerating ...");

        accelerating = true;
        //Debug.Log(Input.mousePosition);
    }

    public Vector2 get_velocity() {
        return rb.velocity;
    }

    public void enter_interactables_interactable_radius(GameObject interactable)
    {
        Debug.Log("Added interactable");
        current_interactable = interactable;
    }

    public void exit_interactables_interactable_radius(GameObject interactable)
    {
        Debug.Log("Removed interactable");
        if (current_interactable == interactable)
        {
            current_interactable = null;
        }
    }

}
