using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerryBush : MonoBehaviour
{

	public float fall_speed_thresh;
	public enum State
	{
		Default,
		Empty
	}

	public int avg_berry_spawn_min = 16;
	public int avg_berry_spawn_max = 24;

	public float berry_generation_radius;

	public GameObject berry_prefab;

	State curr_bush_state;

	bool has_berry;
	bool full;

	public List<GameObject> berry_spawn_points;

	// Start is called before the first frame update
	void Start()
	{
		curr_bush_state = State.Default;
		has_berry = false;
		StartCoroutine(generate_berry());
	}

	// Update is called once per frame
	void Update()
	{

	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.name == "Bear")
		{

			Vector2 bear_vel = collision.gameObject.GetComponent<Bear>().get_velocity();
			//Debug.Log(bear_vel);

			if (bear_vel.magnitude >= fall_speed_thresh)
			{
				fall(collision.gameObject.GetComponent<Bear>());
			}
			else
			{
				shake();
			}

		}
	}


	void fall(Bear bear)
	{
		if (curr_bush_state == State.Default)
		{
			bear.rb.velocity = new Vector2(0, 0);
			// fall animation

			curr_bush_state = State.Empty;
			Debug.Log("Tree Fall");

			//StartCoroutine(transition_to_hut());
		}
	}

	public void shake()
	{
		Debug.Log("shaking bush");
		if (curr_bush_state == State.Default)
		{
			// rustle animation
			foreach (GameObject bsp in berry_spawn_points)
			{
				BerrySpawnPoint bsp_ref = bsp.GetComponent<BerrySpawnPoint>();
				if (bsp_ref.has_berry())
				{
					// TODO: drop berry
					bsp_ref.drop();
					full = false;
					break;
				}
			}
		}
	}


	IEnumerator generate_berry()
	{
		float berry_gen_time = RandomGaussian(avg_berry_spawn_min, avg_berry_spawn_max);
		//Debug.Log("Generate berry in: " + berry_gen_time);
		yield return new WaitForSeconds(berry_gen_time);

		//Debug.Log("Generated berry");

		//Spawn berry at locations
		foreach (GameObject bsp in berry_spawn_points) {
			BerrySpawnPoint curr_bsp = bsp.GetComponent<BerrySpawnPoint>();
			//Debug.Log("CURR BSP: " + curr_bsp);
			if (curr_bsp.has_berry() == false)
            {
				curr_bsp.spawn_berry(berry_prefab);
				break;
            }
		}

		has_berry = true;
		StartCoroutine(generate_berry());
	}


	public bool bush_has_berry()
    {
		return has_berry;
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
