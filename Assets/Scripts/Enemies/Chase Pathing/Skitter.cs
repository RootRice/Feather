using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skitter", menuName = "MovementTypes/Chasing/Skitter", order = 1)]
public class Skitter : TrackingMovementType
{
    [SerializeField] float skitterFrequency;
    [SerializeField] [Range(0, 1f)] float acceleration;
    float lastSkitterTime;

    Vector3 currentTargetNormalised;
    float currentDistance;

    Vector3 lastPos;

    public override void init(Transform _t, Transform _p, float _s, float _min, float _max)
    {
        base.init(_t, _p, _s, _min, _max);
        lastSkitterTime = 0;
        currentTargetNormalised = (EqualiseHeight(t.position, p.position.y) - p.position).normalized;
        currentDistance = Random.Range(minRadius, maxRadius);
        maxRadius = _max;
        minRadius = _min;
    }
    public override Vector3 GetTargetPosition(float deltaTime)
    {
        if (Vector3.SqrMagnitude(EqualiseHeight(t.position, p.position.y) - p.position) > maxRadius * maxRadius)
            return Chase(deltaTime);
        else if (Vector3.SqrMagnitude(EqualiseHeight(t.position, p.position.y) - p.position) < minRadius * minRadius)
            return Flee(deltaTime);
        else if(Time.timeSinceLevelLoad - lastSkitterTime > skitterFrequency)
            return GetNewSkitterTarget(deltaTime);

        return EqualiseHeight(t.position, p.position.y);

        
    }

    float GetDistanceFromMiddle01(Vector3 a, Vector3 b, Vector3 pos)
    {
        Vector3 maxDistFromMiddle = (b - a) / 2;
        Vector3 middle = a + maxDistFromMiddle;
        float distModifier = (Vector3.SqrMagnitude(maxDistFromMiddle) / ((maxRadius - minRadius) / 2));
        return Mathf.Max((1 - Vector3.SqrMagnitude(pos - middle) / Vector3.SqrMagnitude(maxDistFromMiddle)) * distModifier, acceleration * (1 - distModifier));
    }


    Vector3 GetNewSkitterTarget(float deltaTime)
    {
        Vector3 currentTarget = EqualiseHeight(p.position + currentTargetNormalised * currentDistance, p.position.y);
        Vector3 r = Vector3.MoveTowards(t.position, currentTarget, speed * deltaTime);
        if (Vector3.SqrMagnitude(r - currentTarget) < tolerance)
        {
            lastSkitterTime = Time.timeSinceLevelLoad;
            lastPos = currentTarget;
            currentTargetNormalised = Quaternion.Euler(0, Random.Range(-67f, 67f), 0) * currentTargetNormalised;
            currentDistance = Random.Range(minRadius, maxRadius);
            if ((currentTargetNormalised *currentDistance).sqrMagnitude > maxRadius*maxRadius)
            {
                Debug.Log(currentDistance);
                Debug.Log(currentTargetNormalised.normalized);
                Debug.Log((currentTargetNormalised * currentDistance).sqrMagnitude);
                Debug.Log(maxRadius * maxRadius);
            }
            currentTarget = currentTargetNormalised * currentDistance;
            r = Vector3.MoveTowards(t.position, currentTarget, speed * deltaTime);
        }
        return r;
    }

    Vector3 Chase(float deltaTime)
    {
        Debug.Log("c");
        currentDistance = Random.Range(minRadius, maxRadius);
        currentTargetNormalised = (EqualiseHeight(t.position, p.position.y) - p.position).normalized;
        Vector3 currentTarget = p.position + currentTargetNormalised * currentDistance;
        return Vector3.MoveTowards(t.position, currentTarget, speed * deltaTime);
    }

    Vector3 Flee(float deltaTime)
    {

        currentTargetNormalised = (EqualiseHeight(t.position, p.position.y) - p.position).normalized;
        currentDistance = Random.Range(minRadius, maxRadius);
        Vector3 currentTarget = p.position + currentTargetNormalised * currentDistance;
        return Vector3.MoveTowards(t.position, currentTarget, speed * deltaTime);
    }

    Vector3 EqualiseHeight(Vector3 v, float h)
    {
        return new Vector3(v.x, h, v.z);
    }
}
