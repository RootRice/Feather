using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IdlePathManager))]
public class EnemyController : MonoBehaviour
{
    Transform player;

    IdlePathManager idlePathManager;
    Transform target = null;
    [SerializeField] Transform body;
    Vector3 pos;

    EnemyState[] states;
    EnemyState currentState;

    public IdleState idleState;
    public void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        idlePathManager = GetComponent<IdlePathManager>();
        InitialiseStates();
        states = new EnemyState[] { idleState };
        
        currentState = states[0];
    }

    void InitialiseStates()
    {
        idleState.InitValues(player, transform, idlePathManager);
    }
    public void Update()
    {
        currentState.MainLoop(Time.deltaTime);
    }

}
