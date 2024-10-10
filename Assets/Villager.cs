using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Villager : MonoBehaviour
{
    public int move_speed = 2;
    public int flee_speed = 3;
    public int gather_time = 3;
    public int max_move_distance = 10;
    public int min_move_distance = 5;
    public Tombstone tombstone_prefab;
    public TMP_Text state_debug_text;

    public float fear_cooldown = 2;
    float current_fear_cooldown;
    Vector2 flee_direction;
    float step;

    public int bear_flee_radius;
    public int enemy_flee_radius;
    public int resource_gather_radius;

    GameObject carrying_resource;
    public Collider2D interior_collider;

    Vector2 moveTarget;
    bool fleeing_enemy;

    private enum VillagerState
    {
        Moving,
        Fleeing,
        Gathering,
        Idle,
        Piling
    }

    VillagerState current_state;
    
    // Start is called before the first frame update
    void Start()
    {
        // Find an available resource
        
        // Get the closest resource
        
        current_fear_cooldown = 0;
        current_state = VillagerState.Idle;

        bear_flee_radius = 4;
        fleeing_enemy = false;

        // Path (or just move) to the resource
        // Perform gather on the resource
        // Carry the resource to a Pile
    }

    // Update is called once per frame
    void Update()
    {
        state_debug_text.text = current_state.ToString();
        //RaycastHit2D bearCastHit = Physics2D.CircleCast(transform.position, bear_flee_radius, gameObject.transform.position);
        //if (bearCastHit.collider != null)
        //{
        //    Debug.Log(bearCastHit.collider.name);
        //}
        //transform.position += Vector3.down * move_speed * Time.deltaTime;

        // if bear in flee radius
        bool bearInFleeRadius = checkBearFleeStatus();
        if (bearInFleeRadius && current_state!=VillagerState.Fleeing)
        {
            current_state = VillagerState.Fleeing;
            flee_direction = calculateFleeDirection();
        }
        Debug.Log("Current state: " + current_state);
        switch (current_state)
        {

            case VillagerState.Idle:
                Debug.Log("Idling");
                // Pick a random location nearby and move there...
                Vector2 random_loc = Random.insideUnitCircle * RandomGaussian(min_move_distance, max_move_distance);
                moveTarget = (Vector2)gameObject.transform.position + random_loc;
                enter_moving();
                return;
            case VillagerState.Moving:
                // Debug.Log("Moving");
                // Move towards the moveTarget
                step = move_speed * Time.deltaTime;
                transform.position = Vector2.MoveTowards(transform.position, moveTarget, step);
                return;

            case VillagerState.Piling:
                step = move_speed * Time.deltaTime;
                transform.position = Vector2.MoveTowards(transform.position, moveTarget, step);
                return;

            case VillagerState.Gathering:
                //Debug.Log("Gathering");
                step = move_speed * Time.deltaTime;
                transform.position = Vector2.MoveTowards(transform.position, moveTarget, step);

                return;
            case VillagerState.Fleeing:
                Debug.Log("Fleeing");
                if (!bearInFleeRadius && !fleeing_enemy)
                {
                    enter_idle();
                } else
                {
                    step = flee_speed * Time.deltaTime;
                    transform.position = Vector2.MoveTowards(transform.position, flee_direction, step);
                }

                return;
            default:
                break;
        }
    }

    bool checkBearFleeStatus()
    {
        Debug.Log("bear distance " + Vector3.Distance(GameManager.instance.bear.transform.position, gameObject.transform.position));
        if (Vector3.Distance(GameManager.instance.bear.transform.position, gameObject.transform.position) < bear_flee_radius)
        {
            return true;
        } else
        {
            return false;
        }
    }

    Vector2 calculateFleeDirection()
    {
        Vector2 toBear = ((Vector2)GameManager.instance.bear.transform.position - (Vector2)gameObject.transform.position).normalized;
        float currAngle = Mathf.Rad2Deg * Mathf.Atan2(toBear.y, toBear.x); // - pi <= x <= pi
        float limit1 = currAngle - 22.5f;
        float limit2 = (360 - currAngle) - 22.5f;
        float percent = Random.Range(0, 315) / 315.0f;
        float fleeAngle;

        if (currAngle >= 0) {
            fleeAngle = Mathf.Min(limit2, limit1) - (-1 * Mathf.Max(limit2, limit1)) * percent;
        }
        else {
            fleeAngle = Mathf.Max(limit2, limit1) - (-1 * Mathf.Min(limit2, limit1)) * percent;
        }

        Debug.Log("Angles: " + currAngle + " " + limit1 + " " + limit2 + " " + fleeAngle);

        Vector2 fleeDirection = new Vector2(Mathf.Cos(fleeAngle * Mathf.Deg2Rad), Mathf.Sin(fleeAngle * Mathf.Deg2Rad));
        Debug.Log("FleeDirection " + fleeDirection);
        return fleeDirection * 25;
    }
  

    void FixedUpdate()
    {
        current_fear_cooldown += Time.fixedDeltaTime;
        if (current_fear_cooldown >= fear_cooldown && current_state != VillagerState.Gathering && current_state != VillagerState.Piling && current_state != VillagerState.Fleeing)
        {
            current_state = VillagerState.Gathering;
            enter_gathering();
        }

    }

    private void OnDestroy()
    {
        GameManager.instance.check_end_game_state();
    }

    void enter_moving()
    {
        Debug.Log("Enter Moving");
        current_fear_cooldown = 0;
        current_state = VillagerState.Moving;
    }

    void enter_idle()
    {
        current_fear_cooldown = 0;
        current_state = VillagerState.Idle;
    }

    void enter_piling()
    {
        current_state = VillagerState.Piling;

        // set move target to pile location
        moveTarget = (Vector2)GameManager.instance.get_pile_location();

    }

    void enter_gathering()
    {
        Debug.Log("Enter Gathering");
        current_state = VillagerState.Gathering;
        // find resource or bush
        GameObject closest_resource = GameManager.instance.find_closest_resource(gameObject);
        if (closest_resource != null)
        {
            // Set target to closest_resource
            moveTarget = closest_resource.transform.position;
        } else
        {
            // Try to find a bush
            GameObject closest_bush = GameManager.instance.find_closest_bush(gameObject);
            if (closest_bush != null)
            {
                // Set target to the closest bush
                moveTarget = closest_bush.transform.position;
            } else
            {
                enter_idle();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (interior_collider.IsTouching(collision))
        {

        } else
        {
            if (collision.gameObject.tag == "Enemy")
            {
                fleeing_enemy = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision);
        
        if (interior_collider.IsTouching(collision))
        {
            if (collision.name == "Bear")
            {
                crushed_by_bear();
            }
            else if (collision.gameObject.tag == "Enemy")
            {
                //Debug.Log("Die by enemy.");
                die_by_enemy();
            }

            if (current_state == VillagerState.Gathering) {
                if (collision.gameObject.tag == "Resource")
                {
                    Debug.Log("near the resource. gather");
                    gather(collision);
                }
                else if (collision.gameObject.tag == "Bush") {
                    Debug.Log("near the bush");
                    StartCoroutine(harvesting(collision));
                }
            }

            if (current_state == VillagerState.Piling)
            {
                if (collision.gameObject.tag == "Pile")
                {
                    Debug.Log("Hit pile with: " + carrying_resource.name);
                    // instantiate a berry
                    if(carrying_resource)
                    {
                        //float random = Random.Range(0f, 260f);
                        //Vector2 dir = new Vector2(Mathf.Cos(random), Mathf.Sin(random));
                        //Vector3 placement_point = new Vector3(
                        //    collision.gameObject.transform.position.x + dir.x + Random.Range(0f, 1.5f), 
                        //    collision.gameObject.transform.position.y + dir.y + Random.Range(0f, 1.5f), 
                        //    collision.gameObject.transform.position.z);


                        carrying_resource.transform.position = gameObject.transform.position;
                        Debug.Log(carrying_resource.name);
                        if (carrying_resource.name.Contains("Acorn"))
                        {
                            carrying_resource.GetComponent<Acorn>().pile();

                        } else if (carrying_resource.name.Contains("Berry"))
                        {
                            Debug.Log("piled berry");
                            carrying_resource.GetComponent<Berry>().pile();
                        }
                    }
                    enter_idle();
                }
            }
        } else
        {
            // exterior collider 
            if (collision.gameObject.tag == "Enemy")
            {
                // flee
                Vector2 toEnemy = ((Vector2)collision.gameObject.transform.position - (Vector2)gameObject.transform.position).normalized;
                float currAngle = Mathf.Rad2Deg * Mathf.Atan2(toEnemy.y, toEnemy.x); // - pi <= x <= pi
                float limit1 = currAngle - 22.5f;
                float limit2 = (360 - currAngle) - 22.5f;
                float percent = Random.Range(0, 315) / 315.0f;
                float fleeAngle;

                if (currAngle >= 0)
                {
                    fleeAngle = Mathf.Min(limit2, limit1) - (-1 * Mathf.Max(limit2, limit1)) * percent;
                }
                else
                {
                    fleeAngle = Mathf.Max(limit2, limit1) - (-1 * Mathf.Min(limit2, limit1)) * percent;
                }

                Debug.Log("Angles: " + currAngle + " " + limit1 + " " + limit2 + " " + fleeAngle);

                flee_direction = new Vector2(Mathf.Cos(fleeAngle * Mathf.Deg2Rad), Mathf.Sin(fleeAngle * Mathf.Deg2Rad)) * 25;
                current_state = VillagerState.Fleeing;
                fleeing_enemy = true;
            }
        }
    }

    void harvest(GameObject bush)
    {
        Debug.Log("harvesting");
        if (bush.GetComponent<BerryBush>().bush_has_berry())
        {
            bush.GetComponent<BerryBush>().shake();
            enter_gathering();

        } else
        {
            Debug.Log("no berry. idling");

            enter_idle();
        }
    }

    IEnumerator harvesting(Collider2D collision) {
        yield return new WaitForSeconds(3);

        harvest(collision.gameObject);
        enter_idle();
    }

    void crushed_by_bear() {
        //Debug.Log("villager ran over by bear");
        AudioManager.instance.Play("Squish");
        Instantiate(tombstone_prefab, gameObject.transform.position, Quaternion.identity);
        GameManager.instance.remove_villager();
        Destroy(gameObject);
    }

    void die_by_enemy()
    {
        // TODO: death animation
        GameManager.instance.remove_villager();
        Destroy(gameObject);
    }

    void gather(Collider2D collision)
    {
        Debug.Log("gathering resource");

        //carrying_resource = Instantiate(collision.gameObject.GetComponent<Berry>().me_prefab, Vector3.zero, Quaternion.identity);

        carrying_resource = collision.gameObject;
        
        if (carrying_resource.name.Contains("Berry"))
        {
            carrying_resource.GetComponent<Berry>().scoop();
        }

        enter_piling();
    }

    public static float RandomGaussian(float minValue = 0.0f, float maxValue = 1.0f)
    {
        float u, v, S;

        do
        {
            u = 2.0f * UnityEngine.Random.value - 1.0f;
            v = 2.0f * UnityEngine.Random.value - 1.0f;
            S = u * u + v * v;
        }
        while (S >= 1.0f);

        // Standard Normal Distribution
        float std = u * Mathf.Sqrt(-2.0f * Mathf.Log(S) / S);

        // Normal Distribution centered between the min and max value
        // and clamped following the "three-sigma rule"
        float mean = (minValue + maxValue) / 2.0f;
        float sigma = (maxValue - mean) / 3.0f;
        return Mathf.Clamp(std * sigma + mean, minValue, maxValue);
    }

}
