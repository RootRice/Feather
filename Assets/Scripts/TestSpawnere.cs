using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSpawnere : MonoBehaviour
{
    [SerializeField] GameObject g;
    [SerializeField] float spawnTime;
    float t;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        if(t > spawnTime)
        {
            t = 0;
            Instantiate(g, transform.position, Quaternion.identity);
        }
    }
}
