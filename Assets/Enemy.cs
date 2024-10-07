using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
	public int moveSpeed;
	Transform attackTarget;
	GameObject[] active_villagers;
	GameObject target_villager;
	Animator animator;
	int dead_on_ground_time;
	bool can_move;
	public AnimationClip animationSquish;



    private void Awake()
    {
		animator = GetComponent<Animator>();
	}

    // Start is called before the first frame update
    void Start()
	{
		active_villagers = GameObject.FindGameObjectsWithTag("Villager");
		if (active_villagers.Length > 0)
		{
			target_villager = active_villagers[Random.Range(0, active_villagers.Length)];
			//Debug.Log(target_villager);
		}
		
		dead_on_ground_time = 2;
		can_move = true;
	}

	// Update is called once per frame
	void Update()
	{
		// TODO: Move towards the attack target in an arc
		// Debug.Log(target_villager);
		if (can_move)
		{
			//Debug.Log(target_villager);
			if (target_villager == null)
			{
				GameObject[] villagers = GameManager.instance.get_all_villagers();
				GameObject closest = null;
				float dist_to_closest = float.PositiveInfinity;
				foreach (GameObject v in villagers) {
					if ((gameObject.transform.position - v.transform.position).magnitude < dist_to_closest) {
						dist_to_closest = (gameObject.transform.position - v.transform.position).magnitude;
						closest = v;
					}
				}

				target_villager = closest;

			}
			
			if (target_villager != null)
			{
				//
				float step = moveSpeed * Time.deltaTime;
				transform.position = Vector2.MoveTowards(transform.position, target_villager.transform.position, step);
			}
		}
	}

	void squish()
	{
		can_move=false;

		animator.SetBool("squished", true);
		// TODO: Play the death animation and update some metrics... 
		AudioManager.instance.Play("Squish");

		StartCoroutine(Die());
	}

	private IEnumerator Die()
	{
		// TODO: Wait for death animation length 
		float wait_time = animationSquish.length + dead_on_ground_time;
		//Debug.Log("wait time " + wait_time);
		yield return new WaitForSeconds(wait_time);
		
		Destroy(gameObject);
	}

	void OnTriggerEnter2D(Collider2D collision)
	{
		//Debug.Log("Villager hit: " + collision.gameObject);
		if (collision.name == "Bear")
		{
			squish();
		}
	}
}
