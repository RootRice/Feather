using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skitter", menuName = "MovementTypes/Chasing/Skitter", order = 1)]
public class Skitter : TrackingMovementType
{
    [SerializeField] float skitterFrequency;
    float lastSkitterTime;

    Rigidbody rigidbody;
    [SerializeField] float acceleration;
    [SerializeField] float rotationSpeed;


    delegate Vector3 ChaseProtocol(float deltaTime);
    ChaseProtocol[] chaseProtocols;
    int currentProtocol = 0;

    Vector3 target;
    Line boundary;
    GameObject DEBUG;

    public override void init(Transform _transform, Transform _player, float _speed, float _minRadius, float _maxRadius)
    {
        base.init(_transform, _player, _speed, _minRadius, _maxRadius);

        rigidbody = transform.GetComponent<Rigidbody>();

        chaseProtocols = new ChaseProtocol[] { SkitterMove, ChaseMove, FleeMove, Idle };
        currentProtocol = 3;

        DEBUG = GameObject.Find("DebugCube");

        target = transform.position;
        boundary = new Line(Vector2.zero, Vector2.zero);
    }

    public override Vector3 RequestMove(float deltaTime)
    {
        CheckProtocol();
        return chaseProtocols[currentProtocol](deltaTime);
    }



    void SetTarget(float rotOffset, float distFromPlayer)
    {
        Vector3 movementDir = Quaternion.Euler(0, -rotOffset, 0)*(SetHeightToPlayer(transform.position) - player.position).normalized;
        target = player.position + movementDir * distFromPlayer;
        lastSkitterTime = Time.timeSinceLevelLoad;
        boundary.RecalculateLine(V3ToV2(target), V3ToV2(transform.position));
        
    }



    Vector3 SkitterMove(float deltaTime)
    {
        lastSkitterTime = Time.timeSinceLevelLoad;
        Vector3 dir = (target - transform.position).normalized;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(dir), rotationSpeed * deltaTime);
        rigidbody.AddForce(transform.forward * acceleration * deltaTime * Mathf.Max(Vector3.Dot(dir, transform.forward), 0));
        return Vector3.zero;
    }

    Vector3 ChaseMove(float deltaTime)
    {
        lastSkitterTime = Time.timeSinceLevelLoad;
        Vector3 dir = (player.position - transform.position).normalized;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(dir), rotationSpeed * deltaTime);
        rigidbody.AddForce(transform.forward * acceleration * deltaTime * Mathf.Max(Vector3.Dot(dir, transform.forward), 0));
        return Vector3.zero;
    }

    Vector3 FleeMove(float deltaTime)
    {
        lastSkitterTime = Time.timeSinceLevelLoad;
        Vector3 dir = (player.position - transform.position).normalized;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(dir), rotationSpeed * deltaTime);
        rigidbody.AddForce(transform.forward * acceleration * deltaTime * Mathf.Min(Vector3.Dot(dir, -transform.forward), 0));
        return Vector3.zero;
    }


    Vector3 Idle(float deltaTime)
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(player.position - transform.position), rotationSpeed * deltaTime);
        rigidbody.AddForce(-(SetHeightToPlayer(target) - SetHeightToPlayer(transform.position)));
        return Vector3.zero;

    }


    void CheckProtocol()
    {
        if (Time.timeSinceLevelLoad - lastSkitterTime > skitterFrequency)
        {
            SetTarget(Random.Range(-45f, 45f), Random.Range(minRadius, maxRadius));
            currentProtocol = ChangeProtocol(0);
        }
        else if (Vector3.SqrMagnitude(SetHeightToPlayer(transform.position) - player.position) > maxRadius * maxRadius)
        {
            SetTarget(0, Random.Range(minRadius, maxRadius));
            currentProtocol = ChangeProtocol(1);
        }
        else if (Vector3.SqrMagnitude(SetHeightToPlayer(transform.position) - player.position) < minRadius * minRadius)
        {
            SetTarget(0, Random.Range(minRadius, maxRadius));
            currentProtocol = ChangeProtocol(2);
        }
        else if(boundary.HasCrossedLine(V3ToV2(transform.position)))
            currentProtocol = ChangeProtocol(3);

    }
    int ChangeProtocol(int newProtocol)
    {
        if(newProtocol != currentProtocol)
        {
            Debug.Log(newProtocol);
        }
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

    Vector2 V3ToV2(Vector3 vec)
    {
        return new Vector2(vec.x, vec.z);
    }
}

public struct Line
{

    const float verticalLineGradient = 1e5f;

    float gradient;
    float y_intercept;
    Vector2 pointOnLine_1;
    Vector2 pointOnLine_2;

    //float gradientPerpendicular;

    bool approachSide;

    public Line(Vector2 pointOnLine, Vector2 pointPerpendicularToLine)
    {
        float dx = pointOnLine.x - pointPerpendicularToLine.x;
        float dy = pointOnLine.y - pointPerpendicularToLine.y;

        if (dy == 0)
        {
            gradient = verticalLineGradient;
        }
        else
        {
            gradient = -dx / dy;
        }


        y_intercept = pointOnLine.y - gradient * pointOnLine.x;
        pointOnLine_1 = pointOnLine;
        pointOnLine_2 = pointOnLine + new Vector2(1, gradient);

        approachSide = false;
        approachSide = GetSide(pointPerpendicularToLine);
    }

    bool GetSide(Vector2 p)
    {
        return (p.x - pointOnLine_1.x) * (pointOnLine_2.y - pointOnLine_1.y) > (p.y - pointOnLine_1.y) * (pointOnLine_2.x - pointOnLine_1.x);
    }

    public bool HasCrossedLine(Vector2 p)
    {
        return GetSide(p) != approachSide;
    }

    public void RecalculateLine(Vector2 pointOnLine, Vector2 pointPerpendicularToLine)
    {
        float dx = pointOnLine.x - pointPerpendicularToLine.x;
        float dy = pointOnLine.y - pointPerpendicularToLine.y;

        if (dy == 0)
        {
            gradient = verticalLineGradient;
        }
        else
        {
            gradient = -dx / dy;
        }


        y_intercept = pointOnLine.y - gradient * pointOnLine.x;
        pointOnLine_1 = pointOnLine;
        pointOnLine_2 = pointOnLine + new Vector2(1, gradient);

        approachSide = false;
        approachSide = GetSide(pointPerpendicularToLine);
    }

}