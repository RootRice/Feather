using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackPrepState", menuName = "EnemyStates/AttackPrep", order = 1)]
public class AttackPrepState : ScriptableObject, EnemyState
{
    Transform player, myTransform;
    Rigidbody rigidbody;
    float angle;
    [SerializeField] float targetDistance;
    [SerializeField] float approachSpeed;
    [SerializeField] float rotSpeed;

    public void EndState()
    {
        throw new System.NotImplementedException();
    }
    public void InitValues(Transform _player, Transform _myTransform)
    {
        player = _player;
        myTransform = _myTransform;
        rigidbody = myTransform.GetComponent<Rigidbody>();
    }
    public void Init()
    {
        
    }

    public void MainLoop(float deltaTime)
    {
        Vector3 target = player.position + (Quaternion.AngleAxis(-angle, Vector3.up) * Vector3.forward)*targetDistance;
        Vector3 moveDir = target - myTransform.position;
        float slowingFactor = moveDir.magnitude;
        if(slowingFactor < 2f)
        {
            myTransform.rotation = Quaternion.Slerp(myTransform.rotation, Quaternion.LookRotation(player.position-myTransform.position), rotSpeed * deltaTime);
        }
        else
        {
            myTransform.rotation = Quaternion.Slerp(myTransform.rotation, Quaternion.LookRotation(moveDir.normalized), rotSpeed * deltaTime);
        }
        
        rigidbody.AddForce(moveDir * approachSpeed * slowingFactor);
    }

    public void SetTargetSlot(float _angle)
    {
        angle = _angle;
    }

    public void SetTargetSlot(float _angle, float _distance)
    {
        angle = _angle;
        targetDistance = _distance;
    }
}
