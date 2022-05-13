using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    List<ActiveForce> playerForces = new List<ActiveForce>();
    List<ConstForce> constantForces = new List<ConstForce>();

    [SerializeField] ActiveForce dodgeForce;
    [SerializeField] ActiveForce jumpForce;
    [SerializeField] ActiveForce doubleJumpForce;
    [SerializeField] ConstForce gravity;


    Vector3 startPos;

    Rigidbody myRigidbody;
    CapsuleCollider myCollider;
    Vector3 axis;

    public float maxSpeed;
    public float accel;

    bool[] constraints = new bool[3];
    bool grounded;

    delegate void ConstraintAction();
    ConstraintAction[] actionConstraint;

    delegate void InitialAction();
    InitialAction[] initialAction;

    bool rolling;
    float rollingAngle = 0;
    void CreateArr()
    {
        actionConstraint = new ConstraintAction[2];
        initialAction = new InitialAction[1];
        actionConstraint[(int)ForceConstraint.OngoingTag.FreezeGrav] = ConstrainGravity;
        actionConstraint[(int)ForceConstraint.OngoingTag.FreezeMovementInput] = ConstrainMovementInput;
        initialAction[(int)ForceConstraint.InitialTag.ResetForces] = ResetForces;
    }

    void Start()
    {
        CreateArr();
        startPos = transform.position;
        myRigidbody = GetComponent<Rigidbody>();
        myCollider = GetComponent<CapsuleCollider>();
        gravity.Initialise();
    }

    private void FixedUpdate()
    {
        ApplyForces();

        Animate();
    }

    void Update()
    {
        Controls();
    }

    void ResetConstraints()
    {
        for(int i = 0; i < constraints.Length; i++)
        {
            constraints[i] = false;
        }
    }

    void ApplyForces()
    {
        ResetConstraints();
        for (int i = 0; i < playerForces.Count; i++)
        {
            if (!playerForces[i].ShouldTerminate())
            {
                myRigidbody.AddForce(playerForces[i].ApplyForce(Time.fixedDeltaTime));
                foreach(ForceConstraint.OngoingTag t in playerForces[i].CheckOngoingConstraints())
                {
                    actionConstraint[(int)t]();
                }
            }
            else
            {
                playerForces.RemoveAt(i);
                i--;
            }
        }
        
        grounded = GroundCheck();

        myRigidbody.AddForce(gravity.ApplyForce(Time.fixedDeltaTime, grounded || constraints[(int)ForceConstraint.OngoingTag.FreezeGrav]));


        myRigidbody.AddForce(maxSpeed * axis* Time.fixedDeltaTime);
    }

    bool GroundCheck()
    {
        float sphereRadius = myCollider.radius;
        float cylinderHeight = myCollider.bounds.extents.y - sphereRadius + 0.1f;
        RaycastHit hit;
        return Physics.SphereCast(myRigidbody.position, sphereRadius, Vector3.down, out hit,cylinderHeight);
    }

    void Controls()
    {
        if (constraints[(int)ForceConstraint.OngoingTag.FreezeMovementInput])
            return;

        axis.x = Input.GetAxisRaw("Horizontal");
        axis.z = Input.GetAxisRaw("Vertical");
        axis = Quaternion.AngleAxis(45, Vector3.up) * axis;
        axis = axis.normalized;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            rolling = true;
            AddActiveForce(doubleJumpForce);
            startPos = transform.position;
        }

        if (!grounded)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddActiveForce(dodgeForce, new ActiveForce.InitParams(axis.x, axis.magnitude, axis.z, axis.magnitude));
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            AddActiveForce(jumpForce);
        }
        
    }

    public void AddActiveForce(ActiveForce force)
    {
        foreach(ForceConstraint.InitialTag fc in force.CheckInitialConstraints())
        {
            initialAction[(int)fc]();
        }
        force.Initialise();
        playerForces.Add(force);
    }

    public void AddActiveForce(ActiveForce force, ActiveForce.InitParams initParams)
    {
        foreach (ForceConstraint.InitialTag fc in force.CheckInitialConstraints())
        {
            initialAction[(int)fc]();
        }
        force.Initialise(initParams);
        playerForces.Add(force);
    }

    void Animate()
    {
        if(rolling)
        {
            rollingAngle += 1500 * Time.fixedDeltaTime;
            transform.rotation = Quaternion.Euler(new Vector3(0f, rollingAngle, 0f));
            if(rollingAngle > 360)
            {
                rolling = false;
                rollingAngle = 0;
                transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        transform.position = startPos;
    }

    void ConstrainGravity()
    {
        constraints[(int)ForceConstraint.OngoingTag.FreezeGrav] = true;
    }

    void ConstrainMovementInput()
    {
        constraints[(int)ForceConstraint.OngoingTag.FreezeMovementInput] = true;
    }

    void ResetForces()
    {
        myRigidbody.velocity -= new Vector3(0f, myRigidbody.velocity.y, 0f);
        playerForces.Clear();
    }

}
