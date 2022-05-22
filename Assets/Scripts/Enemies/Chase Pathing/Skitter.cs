using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skitter", menuName = "MovementTypes/Chasing/Skitter", order = 1)]
public class Skitter : TrackingMovementType
{
    [SerializeField] float skitterFrequency;
    float lastSkitterTime;

    Rigidbody rigidbody;
    [SerializeField][Range(10.0f, 85.0f)] float skitterLength;

    [SerializeField] [Range(0.0f, 1.5f)] float driftFactor;
    delegate void ChaseProtocol(float deltaTime);
    ChaseProtocol[] chaseProtocols;
    int currentProtocol = 0;

    Vector3 target;
    Line boundary;
    GameObject DEBUG;

    public override void init(Transform _transform, Transform _player, float _speed, float _rotSpeed, float _minRadius, float _maxRadius)
    {
        base.init(_transform, _player, _speed, _rotSpeed, _minRadius, _maxRadius);

        rigidbody = transform.GetComponent<Rigidbody>();

        chaseProtocols = new ChaseProtocol[] { SkitterMove, ChaseMove, FleeMove, Idle };
        currentProtocol = 3;

        DEBUG = GameObject.Find("DebugCube");

        target = transform.position;
        boundary = new Line(Vector2.zero, Vector2.zero);
    }

    public override void RequestMove(float deltaTime)
    {
        CheckProtocol();
        chaseProtocols[currentProtocol](deltaTime);
    }



    void SetTarget(float rotOffset, float distFromPlayer)
    {
        Vector3 movementDir = Quaternion.Euler(0, -rotOffset, 0)*(SetHeightToPlayer(transform.position) - player.position).normalized;
        target = player.position + movementDir * distFromPlayer;
        lastSkitterTime = Time.timeSinceLevelLoad;
        boundary.RecalculateLine(Utils.V3ToV2(target), Utils.V3ToV2(transform.position));
        
    }



    void SkitterMove(float deltaTime)
    {
        lastSkitterTime = Time.timeSinceLevelLoad;
        Vector3 dir = (target - transform.position).normalized;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), rotSpeed * deltaTime);
        rigidbody.AddForce(transform.forward * maxSpeed * deltaTime * Mathf.Max(Vector3.Dot(dir, transform.forward), 0));
    }

    void ChaseMove(float deltaTime)
    {
        lastSkitterTime = Time.timeSinceLevelLoad;
        Vector3 dir = (player.position - transform.position).normalized;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), rotSpeed * deltaTime);
        rigidbody.AddForce(transform.forward * maxSpeed * deltaTime * Mathf.Max(Vector3.Dot(dir, transform.forward), 0));
    }

    void FleeMove(float deltaTime)
    {
        lastSkitterTime = Time.timeSinceLevelLoad;
        Vector3 dir = (player.position - transform.position).normalized;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), rotSpeed * deltaTime);
        rigidbody.AddForce(transform.forward * maxSpeed * deltaTime * Mathf.Min(Vector3.Dot(dir, -transform.forward), 0));
    }


    void Idle(float deltaTime)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(player.position - transform.position), rotSpeed * deltaTime);
        rigidbody.AddForce(-(SetHeightToPlayer(target) - SetHeightToPlayer(transform.position)).normalized*driftFactor);

    }


    void CheckProtocol()
    {
        if (Time.timeSinceLevelLoad - lastSkitterTime > skitterFrequency)
        {
            SetTarget(Random.Range(-skitterLength, skitterLength), Random.Range(minRadius + 1.5f, maxRadius - 1.5f));
            currentProtocol = ChangeProtocol(0);
        }
        else if (Vector3.SqrMagnitude(SetHeightToPlayer(transform.position) - player.position) > maxRadius * maxRadius)
        {
            SetTarget(0, maxRadius);
            currentProtocol = ChangeProtocol(1);
        }
        else if (Vector3.SqrMagnitude(SetHeightToPlayer(transform.position) - player.position) < minRadius * minRadius)
        {
            SetTarget(0, minRadius);
            currentProtocol = ChangeProtocol(2);
        }
        else if(boundary.HasCrossedLine(Utils.V3ToV2(transform.position)))
            currentProtocol = ChangeProtocol(3);

    }
    int ChangeProtocol(int newProtocol)
    {
        return newProtocol;
    }
    float GetProgress01(Vector3 a, Vector3 b, Vector3 pos)
    {
        return 1 - (Vector3.SqrMagnitude(pos - b) / Vector3.SqrMagnitude(b - a));
    }
    Vector3 SetHeightToPlayer(Vector3 vector)
    {
        return new Vector3(vector.x, player.position.y, vector.z);
    }

    
}

