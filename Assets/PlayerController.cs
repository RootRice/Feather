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
    [SerializeField] ConstForce gravity;


    Vector3 startPos;

    Rigidbody myRigidbody;
    CapsuleCollider myCollider;
    Vector3 axis;

    public float maxSpeed;
    public float accel;

    bool[] constraints = new bool[3];
    bool grounded;

    

    void Start()
    {
        startPos = transform.position;
        myRigidbody = GetComponent<Rigidbody>();
        myCollider = GetComponent<CapsuleCollider>();
        gravity.Initialise();
    }

    private void FixedUpdate()
    {
        ApplyForces();

    }

    // Update is called once per frame
    void Update()
    {
        Controls();
        Animate();
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
                foreach(ForceConstraint.Tag t in playerForces[i].CheckConstraints())
                {
                    constraints[(int)t] = true;
                }
            }
            else
            {
                playerForces.RemoveAt(i);
                i--;
            }
        }
        
        grounded = GroundCheck();

        myRigidbody.AddForce(gravity.ApplyForce(Time.fixedDeltaTime, grounded || constraints[(int)ForceConstraint.Tag.FreezeGrav]));


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
        if (constraints[(int)ForceConstraint.Tag.FreezeMovementInput])
            return;

        axis.x = Input.GetAxisRaw("Horizontal");
        axis.z = Input.GetAxisRaw("Vertical");
        axis = Quaternion.AngleAxis(45, Vector3.up) * axis;
        axis = axis.normalized;

        if (!grounded)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            dodgeForce.Initialise(axis.x, axis.magnitude, axis.z, axis.magnitude);
            playerForces.Add(dodgeForce);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            jumpForce.Initialise();
            playerForces.Add(jumpForce);
        }
        
    }

    public void AddActiveForce(ActiveForce force)
    {
        playerForces.Add(force);
    }

    void Animate()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        transform.position = startPos;
    }

    
}
