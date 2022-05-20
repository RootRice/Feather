using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skitter", menuName = "MovementTypes/Chasing/Skitter", order = 1)]
public class Skitter : TrackingMovementType
{
    [SerializeField] float skitterFrequency;
    [SerializeField] [Range(1f, 3f)] float acceleration;
    float lastSkitterTime;

    Vector3 currentTargetNormalised;
    float currentDistance;

    float speed;

    Vector3 lastPos;
    Vector3 skitterTarget;

    int i = 0;
    int j = 0;
    
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

        AdjustSpeed(-deltaTime);
        Vector3 currentTarget = p.position + currentTargetNormalised * currentDistance;

        return Vector3.MoveTowards(t.position, EqualiseHeight(currentTarget, p.position.y), speed * deltaTime);

        
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
        Debug.Log("Skittering");
        AdjustSpeed(deltaTime);
        Vector3 currentTarget = p.position + currentTargetNormalised * currentDistance;
        Vector3 r = Vector3.MoveTowards(t.position, currentTarget, speed * deltaTime);
        Debug.Log(r);
        if (Vector3.SqrMagnitude(r - currentTarget) < tolerance)
        {
            speed = 0;
            lastSkitterTime = Time.timeSinceLevelLoad;
            lastPos = currentTarget;
            currentTargetNormalised = (EqualiseHeight(t.position, p.position.y) - p.position).normalized;
            currentDistance = Random.Range(minRadius, maxRadius);
            currentTargetNormalised = Quaternion.Euler(0, Random.Range(-67f, 67f), 0) * currentTargetNormalised;
            skitterTarget = p.position + currentTargetNormalised * currentDistance;
            r = Vector3.MoveTowards(t.position, skitterTarget, speed * deltaTime);
            Debug.Log(r);
        }
        return r;
    }

    Vector3 Chase(float deltaTime)
    {
        AdjustSpeed(deltaTime);
        currentDistance = Random.Range(minRadius, maxRadius);
        currentTargetNormalised = (EqualiseHeight(t.position, p.position.y) - p.position).normalized;
        Vector3 currentTarget = p.position + currentTargetNormalised * currentDistance;
        ResetSkitter(currentTarget);
        return Vector3.MoveTowards(t.position, currentTarget, speed * deltaTime);
    }

    Vector3 Flee(float deltaTime)
    {
        AdjustSpeed(deltaTime);
        currentTargetNormalised = (EqualiseHeight(t.position, p.position.y) - p.position).normalized;
        currentDistance = Random.Range(minRadius, maxRadius);
        Vector3 currentTarget = p.position + currentTargetNormalised * currentDistance;
        ResetSkitter(currentTarget);
        return Vector3.MoveTowards(t.position, currentTarget, speed * deltaTime);
    }

    Vector3 EqualiseHeight(Vector3 v, float h)
    {
        return new Vector3(v.x, h, v.z);
    }

    void  ResetSkitter(Vector3 currentTarget)
    {
        skitterTarget = currentTarget;
        lastSkitterTime = Time.timeSinceLevelLoad;
    }

    void AdjustSpeed(float deltaTime)
    {
        speed = Mathf.Clamp(speed + acceleration * deltaTime, 0.0f, maxSpeed);
    }
}
