using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerrySpawnPoint : MonoBehaviour
{
    // Start is called before the first frame update
    bool is_filled;
    GameObject berry;

    void Start()
    {
        is_filled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject spawn_berry(GameObject berry_prefab) {
        is_filled = true;
        berry = Instantiate(berry_prefab, gameObject.transform.position, Quaternion.identity);
        return berry;
    }

    public void drop()
    {
        if (is_filled)
        {
            is_filled = false;
            berry.GetComponent<Berry>().pick();
        } 
    }


    public GameObject get_berry()
    {
        return berry;
    }
    

    public bool has_berry()
    {
        return is_filled;
    }




}
