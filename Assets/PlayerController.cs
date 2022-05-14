using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    List<ActiveForce> playerForces = new List<ActiveForce>();
    List<ConstForce> constantForces = new List<ConstForce>();
    List<Animation> animations = new List<Animation>();

    [SerializeField] public ActiveForce dodgeForce;
    [SerializeField] public ActiveForce jumpForce;
    [SerializeField] public ActiveForce doubleJumpForce;
    [SerializeField] ConstForce gravity;
    [SerializeField] public Animation dodgeAnimation;
    [SerializeField] public Animation jumpAnimation;

    ControlScheme currentControls = new GroundControls();
    ControlScheme[] controls;

    Vector3 startPos;

    Rigidbody myRigidbody;
    CapsuleCollider myCollider;
    Vector3 axis;

    public float maxSpeed;
    public float speed;

    bool[] ongoingConstraints = new bool[Enum.GetValues(typeof(ForceConstraint.OngoingTag)).Length];
    bool grounded;

    delegate void ConstraintAction();
    ConstraintAction[] actionConstraint;

    delegate void InitialAction();
    InitialAction[] initialAction;

    [SerializeField] Transform body;
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
        controls = new ControlScheme[Enum.GetValues(typeof(ControlScheme.ControlType)).Length];
        controls[0] = new GroundControls();
        controls[1] = new MidAirControls();
        CreateArr();
        startPos = transform.position;
        myRigidbody = GetComponent<Rigidbody>();
        myCollider = GetComponent<CapsuleCollider>();
        gravity.Initialise();
    }

    private void FixedUpdate()
    {
        GroundCheck();
        ApplyForces();
        Animate();
    }

    void Update()
    {
        Controls();
    }

    void ResetConstraints()
    {
        for(int i = 0; i < ongoingConstraints.Length; i++)
        {
            ongoingConstraints[i] = false;
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
        myRigidbody.AddForce(gravity.ApplyForce(Time.fixedDeltaTime, grounded || ongoingConstraints[(int)ForceConstraint.OngoingTag.FreezeGrav]));

        myRigidbody.AddForce(speed * axis* Time.fixedDeltaTime);
    }

    bool GroundCheck()
    {
        float sphereRadius = myCollider.radius;
        float cylinderHeight = myCollider.bounds.extents.y - sphereRadius + 0.1f;
        RaycastHit hit;
        if(Physics.SphereCast(myRigidbody.position, sphereRadius, Vector3.down, out hit,cylinderHeight))
        {
            grounded = true;
            currentControls = controls[currentControls.ChangeControls((int)ControlScheme.ControlType.Grounded)];
            return grounded;
        }
        grounded = false;
        currentControls = controls[currentControls.ChangeControls((int)ControlScheme.ControlType.MidAir)];
        return grounded;
    }

    void Controls()
    {
        if (ongoingConstraints[(int)ForceConstraint.OngoingTag.FreezeMovementInput])
            return;

        if(Mathf.Abs(Input.GetAxisRaw("Horizontal")) + Mathf.Abs(Input.GetAxisRaw("Vertical")) != 0)
        {
            axis.x = Input.GetAxisRaw("Horizontal");
            axis.z = Input.GetAxisRaw("Vertical");
            axis = Quaternion.AngleAxis(Camera.main.transform.rotation.eulerAngles.y, Vector3.up) * axis;
            axis = axis.normalized;
            speed = maxSpeed;
        }
        else
        {
            speed = 0;
        }
        

        if (Input.GetKeyDown(KeyCode.Q))
        {
            currentControls.LBlock(this);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            currentControls.Dodge(this, new ActiveForce.InitParams(axis.x, axis.magnitude, axis.z, axis.magnitude));
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            currentControls.Jump(this);
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
        foreach(ForceConstraint.InitialTag fc in force.CheckInitialConstraints())
        {
            initialAction[(int)fc]();
        }
        force.Initialise(initParams);
        playerForces.Add(force);
    }

    public void AddAnimation(Animation anim)
    {
        anim.Initialise();
        animations.Add(anim);
    }

    public void AddAnimation(Animation anim, Animation.InitParams initParams)
    {
        anim.Initialise(initParams);
        animations.Add(anim);
    }

    void Animate()
    {
        Quaternion animationRot = Quaternion.identity;
        for(int i = 0; i < animations.Count; i++)
        {
            if(!animations[i].ShouldTerminate())
            {
                animationRot *= animations[i].ApplyRotation(Time.fixedDeltaTime);
            }
            else
            {
                
                animations.RemoveAt(i);
                i--;
            }
        }

        body.localRotation = animationRot;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.AngleAxis(Vector3.SignedAngle(Vector3.forward, axis, Vector3.up), Vector3.up), 400f * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        transform.position = startPos;
    }

    void ConstrainGravity()
    {
        ongoingConstraints[(int)ForceConstraint.OngoingTag.FreezeGrav] = true;
    }

    void ConstrainMovementInput()
    {
        ongoingConstraints[(int)ForceConstraint.OngoingTag.FreezeMovementInput] = true;
    }

    void ResetForces()
    {
        myRigidbody.velocity -= new Vector3(0f, myRigidbody.velocity.y, 0f);
        playerForces.Clear();
    }

}
