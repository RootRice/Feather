using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IdlePathManager))]
public class EnemyController : MonoBehaviour
{


    IdlePathManager idlePathManager;
    Transform target = null;

    public void Start()
    {
        idlePathManager = GetComponent<IdlePathManager>();
    }
    public void Update()
    {
        transform.position = idlePathManager.GetNewPosition(Time.deltaTime);
    }

}
