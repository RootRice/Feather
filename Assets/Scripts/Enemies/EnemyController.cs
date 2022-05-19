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

    public ChasingState chaseState;
    public IdleState idleState;
    public void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        idlePathManager = GetComponent<IdlePathManager>();
        InitialiseStates();
        states = new EnemyState[] { idleState, chaseState };
        states[0].Init();
        states[1].Init();
        currentState = states[1];
    }

    void InitialiseStates()
    {
        idleState.InitValues(player, transform, idlePathManager);
        chaseState.InitValues(player, transform);
    }
    public void FixedUpdate()
    {
        currentState.MainLoop(Time.fixedDeltaTime);
    }

}
