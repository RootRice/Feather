using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IdleState", menuName = "EnemyStates/Idle", order = 1)]
public class IdleState : ScriptableObject, EnemyState
{
    Transform player;
    Transform myTransform;
    public IdleMovementType movementType;
    [SerializeField] float idleSpeed;
    [SerializeField] float idleRotSpeed;

    public void InitValues(Transform _player, Transform _myTransform)
    {
        player = _player;
        myTransform = _myTransform;
    }
    public float detectionRadius;
    public void EndState()
    {
        throw new System.NotImplementedException();
    }

    public void Init()
    {
        
    }

    public void MainLoop(float deltaTime)
    {
        myTransform.position = movementType.GetTargetPosition(deltaTime);
        if(detectionRadius*detectionRadius > Vector3.SqrMagnitude(myTransform.position - player.position))
        {

        }
    }
}
