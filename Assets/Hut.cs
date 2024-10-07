using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hut : MonoBehaviour
{
    public int max_villagers = 10;
    public int current_villagers;
    public float villager_spawn_time = 5;
    public Villager villager_prefab;
    public GameObject village_spawn_area;
    // Start is called before the first frame update

    bool invincible;

    void Start()
    {
        Debug.Log("hut started");
        StartCoroutine(spawn_villager());

        invincible = true;
    }

    // Update is called once per frame
    void Update()
    {
      
    }

    IEnumerator spawn_villager() {
        Debug.Log("spawn villager");
        yield return new WaitForSeconds(villager_spawn_time);
        current_villagers += 1;
        GameManager.instance.add_villager();
        Instantiate(villager_prefab, village_spawn_area.transform.position, village_spawn_area.transform.rotation);
        StartCoroutine(spawn_villager());
    }

    void crush()
    {
        // TODO: animation, etc...

        if (invincible != true)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Bear")
        {
            Debug.Log("ran over by bear");
            crush();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        invincible = false;
    }

}
