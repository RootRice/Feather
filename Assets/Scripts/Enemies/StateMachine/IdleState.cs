using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IdleState", menuName = "EnemyStates/Idle", order = 1)]
public class IdleState : ScriptableObject, EnemyState
{
    Transform player;
    Transform myTransform;
    IdlePathManager pathing;
    [SerializeField] float idleSpeed;

    public void InitValues(Transform _player, Transform _myTransform, IdlePathManager _pathing)
    {
        player = _player;
        myTransform = _myTransform;
        pathing = _pathing;
    }
    public float detectionRadius;
    public void EndState()
    {
        throw new System.NotImplementedException();
    }

    public void Init()
    {
        pathing.speed = idleSpeed;
    }

    public void MainLoop(float deltaTime)
    {
        myTransform.position = pathing.GetNewPosition(deltaTime);
        if(detectionRadius*detectionRadius > Vector3.SqrMagnitude(myTransform.position - player.position))
        {

        }
    }
}
