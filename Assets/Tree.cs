using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : Interactable
{

	public float fall_speed_thresh;
	public enum State
	{
		Default,
		Fallen
	}

	public GameObject acorn_spawn_area;

	public int avg_acorn_spawn_min = 10;
	public int avg_acorn_spawn_max = 16;

	Transform acorn_generation_transform;
	public float acorn_generation_radius;

	public Hut hut_prefab;
	public GameObject acorn_prefab;
	public AnimationClip fall_clip;
	public AnimationClip shake_clip;

	State curr_tree_state;

	GameObject current_acorn;
	Animator tree_animator;
	bool has_acorn;
	bool is_tree_fall;
	bool is_tree_shake;
	public int tree_on_ground_time = 5;
	bool can_transition;

	// Start is called before the first frame update
	void Start()
	{
		acorn_generation_transform = acorn_spawn_area.transform;
		curr_tree_state = State.Default;
		has_acorn = false;
		StartCoroutine(generate_acorn());
		tree_animator = GetComponent<Animator>();
		is_tree_fall = false;
		is_tree_shake = false;
		tree_animator.SetBool("is_tree_shake", false);
		can_transition = false;
	}

	// Update is called once per frame
	void Update()
	{
		
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.name == "Bear") {

			Vector2 bear_vel = collision.gameObject.GetComponent<Bear>().get_velocity();
			Debug.Log(bear_vel);

			if (bear_vel.magnitude >= fall_speed_thresh)
			{
				fall(collision.gameObject.GetComponent<Bear>());
			}
			else {
				shake();
			}

		}

		check_indicate_can_interact(collision);
	}

    private void OnTriggerExit2D(Collider2D collision)
    {

		check_indicate_cannot_interact(collision);
    }


    void fall(Bear bear)
	{
		if (curr_tree_state == State.Default)
		{
			bear.rb.velocity = new Vector2(0, 0);
			// fall animation
			is_tree_fall = true;
			tree_animator.SetBool("is_tree_fall", true);

			curr_tree_state = State.Fallen;
			//Debug.Log("Tree Fall");

			Destroy(current_acorn);

			StartCoroutine(allow_transition_to_hut());
		}
	}

    public override void interact()
    {
        if (can_transition && GameManager.instance.get_num_acorns() >= 5)
        {
			GameManager.instance.use_acorns(5);

			Instantiate(hut_prefab, gameObject.transform.position, gameObject.transform.rotation);

			// Destroy acorn, if any
			if (has_acorn)
			{
				//Debug.Log("Destroy Acorn on Tree");
				//Debug.Log(current_acorn);
				Destroy(current_acorn);
			}
			// Destroy the tree
			Destroy(gameObject);
		}
    }

    IEnumerator allow_transition_to_hut()
	{
		float wait_time = fall_clip.length;
		//yield return new WaitForSeconds(wait_time + tree_on_ground_time);
		yield return new WaitForSeconds(wait_time);
		can_transition = true;
		
	}

	IEnumerator wait_for_tree_shake_anim()
    {
		float wait_time = shake_clip.length;
		yield return new WaitForSeconds(wait_time);
		is_tree_shake = false;
    }

	void shake()
	{
		if (curr_tree_state == State.Default) {
			// rustle animation
			is_tree_shake = true;
			tree_animator.SetBool("is_tree_shake", true);
			if (has_acorn) {
				// TODO: drop acorn here'
				//Debug.Log("Drop Acorn");
				current_acorn.GetComponent<Acorn>().fall_from_tree();
				StartCoroutine(generate_acorn());
			}
			StartCoroutine(wait_for_tree_shake_anim());
		}
	}


	IEnumerator generate_acorn()
	{
		float acorn_gen_time = RandomGaussian(avg_acorn_spawn_min, avg_acorn_spawn_max);
		//Debug.Log("Generate acorn in: " + acorn_gen_time);
		yield return new WaitForSeconds(acorn_gen_time);

		//Debug.Log("Generated Acorn");

		// Spawn acorn at locations
		current_acorn = Instantiate(acorn_prefab, acorn_generation_transform.position + (transform.forward * -1), acorn_generation_transform.rotation);
		//gen_circle.transform.position
		has_acorn = true;
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
