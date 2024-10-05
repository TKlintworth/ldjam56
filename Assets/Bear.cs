using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bear : MonoBehaviour
{

    Vector3 accel_dir;
    Vector3 pos;
    bool accelerating;
    Rigidbody2D rb;

    int accel_mult;

    // Start is called before the first frame update
    void Start()
    {
        accel_mult = 5;
        accel_dir = new Vector3(0, 0, 0);
        rb = gameObject.GetComponent<Rigidbody2D>();

        //rb.velocity = new Vector2(10, 0);
    }

    // Update is called once per frame
    void Update()
    {
        pos = gameObject.transform.position;

        if (accelerating)
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            accel_dir = worldPosition - gameObject.transform.position;
            accel_dir = accel_dir.normalized;

            rb.velocity = rb.velocity + new Vector2(accel_dir.x, accel_dir.y) * Time.deltaTime * accel_mult;
        }

        if (Input.GetMouseButtonUp(0)) {
            Debug.Log("Stopped accelerating.");
            accelerating = false;
        }

    }

    private void OnMouseDown() {
        Debug.Log("Start Accelerating ...");

        accelerating = true;
        Debug.Log(Input.mousePosition);
    }

}
