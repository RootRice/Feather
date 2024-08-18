using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChaseState", menuName = "EnemyStates/Chasing", order = 1)]
public class ChasingState : ScriptableObject, EnemyState
{

    Transform player, myTransform;
    [SerializeField] float chasingSpeed;
    [SerializeField] float rotationSpeed;
    [SerializeField] TrackingMovementType pathing;
    public float minDist;
    public float maxDist;

    public void InitValues(Transform _player, Transform _myTransform)
    {
        player = _player;
        myTransform = _myTransform;
    }
    public void EndState()
    {
        throw new System.NotImplementedException();
    }

    public void Init()
    {
        pathing = Instantiate(pathing);
        pathing.init(myTransform, player, chasingSpeed, rotationSpeed, minDist, maxDist);
    }

    public void MainLoop(float deltaTime)
    {
        pathing.RequestMove(deltaTime);
    }

}
