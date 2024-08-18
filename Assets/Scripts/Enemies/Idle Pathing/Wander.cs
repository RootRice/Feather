using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wander", menuName = "MovementTypes/Idle/Wander", order = 1)]
public class Wander : IdleMovementType
{
    Vector3 targetPos = Vector3.zero;
    [SerializeField] float waitTime;
    float elapsedTime;
    GameObject DEBUG;
    public override void AddPoint(Points p, Vector3 position)
    {
        
    }

    public override void RequestMove(float deltaTime)
    {
        if(!lines[0].HasCrossedLine(Utils.V3ToV2(transform.position)))
        {
            Vector3 position = transform.position;
            Vector3 forward = transform.forward;
            Vector3 dir = (targetPos - position).normalized;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), rotSpeed * deltaTime);
            rigidbody.AddForce(forward * speed * deltaTime * Mathf.Max(Vector3.Dot(dir, forward), 0.2f));
            elapsedTime = Time.timeSinceLevelLoad;
            DEBUG.transform.position = targetPos;
        }
        if (Time.timeSinceLevelLoad - elapsedTime < waitTime)
            return;
        CalculateNewPosition();
        

    }

    void CalculateNewPosition()
    {
        targetPos = patrol.points[0] + Vector3.Scale(new Vector3(Random.Range(0.0f, 1.0f), 0f, Random.Range(0.0f, 1.0f)), patrol.points[1]); 
        Vector2 currentPoint = Utils.V3ToV2(targetPos);
        Vector2 prevPoint = Utils.V3ToV2(transform.position);
        lines[0].RecalculateLine(currentPoint, prevPoint);

    }

    public override void init(Transform _t, float _s, float _rs)
    {
        base.init(_t, _s, _rs);
        drawType = DrawType.Box;
        targetPos = transform.position;
        elapsedTime = 0;
        lines = new Line[1] { new Line(Vector2.zero, Vector2.zero) } ;
        CalculateNewPosition();
        DEBUG = GameObject.Find("DebugCube");
    }

    public override void RemovePoint(Points p, int i)
    {
        throw new System.NotImplementedException();
    }



}
