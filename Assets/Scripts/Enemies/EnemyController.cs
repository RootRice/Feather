using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyController : MonoBehaviour
{
    public enum EnemyStates { Idle, Chasing, ReadyingAttack};

    Transform player;
    [SerializeField] Transform body;

    EnemyState[] states;
    EnemyState currentState;

    [HideInInspector] public Points path;

    public IdleState idleState;
    public ChasingState chaseState;
    public AttackPrepState prepState;

    public AttackProfile attackProfile;
    public void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        InitialiseStates();
        
        currentState = states[1];
    }

    void InitialiseStates()
    {
        idleState = Instantiate(idleState);
        chaseState = Instantiate(chaseState);
        prepState = Instantiate(prepState);

        states = new EnemyState[] { idleState, chaseState, prepState };
        idleState.InitValues(player, transform, path);
        chaseState.InitValues(player, transform);
        prepState.InitValues(player, transform);
        states[0].Init();
        states[1].Init();
        states[2].Init();
    }

    void FixedUpdate()
    {
        currentState.MainLoop(Time.fixedDeltaTime);
    }

    public void SetState(EnemyStates state)
    {
        currentState = states[(int)state];
    }

    public void SetTargetAngle(float angle)
    {
        prepState.SetTargetSlot(angle);
    }

    public void NewAttack()
    {

    }

}
