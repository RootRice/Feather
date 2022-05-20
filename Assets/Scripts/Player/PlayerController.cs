using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    List<ActiveForce> playerForces = new List<ActiveForce>();
    List<ConstForce> constantForces = new List<ConstForce>();
    List<RotationAnimation> animations = new List<RotationAnimation>();

    [SerializeField] public ActiveForce dodgeForce;
    [SerializeField] public ActiveForce jumpForce;
    [SerializeField] public ActiveForce doubleJumpForce;
    [SerializeField] ConstForce gravity;
    [SerializeField] public RotationAnimation dodgeAnimation;
    [SerializeField] public RotationAnimation jumpAnimation;

    ControlScheme currentControls = new GroundControls();
    ControlScheme[] controls;

    BlockState blockingState = new NeutralBlock();


    Rigidbody myRigidbody;
    CapsuleCollider myCollider;
    Vector3 prevAxis = Vector3.forward;
    Vector3 axis = Vector3.forward;

    public float maxSpeed;
    public float speed;
    public float rotSpeed;

    bool[] ongoingConstraints = new bool[Enum.GetValues(typeof(Constraints.OngoingTag)).Length];
    bool grounded;

    delegate void ConstraintAction();
    ConstraintAction[] actionConstraint;

    delegate void InitialAction();
    InitialAction[] initialAction;

    [SerializeField] Transform body;

    Vector3 startPos;
    MeshRenderer myRenderer;
    Color[] colours = new Color[4] { Color.red, Color.blue, Color.green, Color.white };
    void CreateArr()
    {
        actionConstraint = new ConstraintAction[Enum.GetValues(typeof(Constraints.OngoingTag)).Length];
        initialAction = new InitialAction[Enum.GetValues(typeof(Constraints.InitialTag)).Length];
        actionConstraint[(int)Constraints.OngoingTag.FreezeGrav] = ConstrainGravity;
        actionConstraint[(int)Constraints.OngoingTag.FreezeMovementInput] = ConstrainMovementInput;
        initialAction[(int)Constraints.InitialTag.ResetForces] = ResetForces;
        initialAction[(int)Constraints.InitialTag.ResetAnimations] = ResetAnimations;
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
        myRenderer = body.GetComponent<MeshRenderer>();
    }

    private void FixedUpdate()
    {
        ResetConstraints();
        GroundCheck();
        ApplyForces();
        Animate();
    }

    void Update()
    {
        Controls();
        ApplyBlock();
    }

    void ApplyBlock()
    {
        if(blockingState.Progress(Time.deltaTime))
        {
            blockingState = new NeutralBlock();
        }
        myRenderer.material.color = colours[(int)blockingState.myState];
        foreach (Constraints.OngoingTag t in blockingState.CheckOngoingConstraints())
        {
            ongoingConstraints[(int)t] = true;
        }
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
        
        for (int i = 0; i < playerForces.Count; i++)
        {
            if (!playerForces[i].ShouldTerminate())
            {
                myRigidbody.AddForce(playerForces[i].ApplyForce(Time.fixedDeltaTime));
                foreach(Constraints.OngoingTag t in playerForces[i].CheckOngoingConstraints())
                {
                    ongoingConstraints[(int)t] = true;
                    //actionConstraint[(int)t]();
                }
            }
            else
            {
                playerForces.RemoveAt(i);
                i--;
            }
        }
        myRigidbody.AddForce(gravity.ApplyForce(Time.fixedDeltaTime, grounded || ongoingConstraints[(int)Constraints.OngoingTag.FreezeGrav]));

        myRigidbody.AddForce(speed * axis* Time.fixedDeltaTime);
    }

    bool GroundCheck()
    {
        float sphereRadius = myCollider.radius *0.95f;
        float cylinderHeight = myCollider.bounds.extents.y - sphereRadius + 0.1f;
        RaycastHit hit;
        if(Physics.SphereCast(myRigidbody.position, sphereRadius, Vector3.down, out hit,cylinderHeight, 2147483647, QueryTriggerInteraction.Ignore))
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
        Vector3 joystickAxis = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        if(Mathf.Abs(joystickAxis.x) + Mathf.Abs(joystickAxis.z) != 0)
        {
            
            
            prevAxis = axis;
            axis.x = joystickAxis.x;
            axis.z = joystickAxis.z;
            axis = Quaternion.AngleAxis(Camera.main.transform.rotation.eulerAngles.y, Vector3.up) * axis;
            axis = axis.normalized;
            

            speed = maxSpeed;
            if (ongoingConstraints[(int)Constraints.OngoingTag.FreezeMovementInput])
            {
                speed = 0;
            }
        }
        else
        {
            speed = 0;
        }

        


        if (Input.GetKeyDown(KeyCode.Period) && !ongoingConstraints[(int)Constraints.OngoingTag.FreezeBlockInput])
        {
            currentControls.LBlock(this);
        }
        if (Input.GetKeyDown(KeyCode.Comma) && !ongoingConstraints[(int)Constraints.OngoingTag.FreezeBlockInput])
        {
            currentControls.RBlock(this);
        }
        if (Input.GetKeyDown(KeyCode.Space) && !ongoingConstraints[(int)Constraints.OngoingTag.FreezeDodgeInput])
        {
            currentControls.Dodge(this, new ActiveForce.InitParams(axis.x, axis.magnitude, axis.z, axis.magnitude));
        }
        if (Input.GetKeyDown(KeyCode.RightShift) && !ongoingConstraints[(int)Constraints.OngoingTag.FreezeJumpInput])
        {
            currentControls.Jump(this);
        }
        
    }

    public void AddActiveForce(ActiveForce force)
    {
        foreach(Constraints.InitialTag fc in force.CheckInitialConstraints())
        {
            initialAction[(int)fc]();
        }
        force.Initialise();
        playerForces.Add(force);
    }

    public void AddActiveForce(ActiveForce force, ActiveForce.InitParams initParams)
    {
        foreach(Constraints.InitialTag fc in force.CheckInitialConstraints())
        {
            initialAction[(int)fc]();
        }
        force.Initialise(initParams);
        playerForces.Add(force);
    }

    public void SetBlock(BlockState state)
    {
        foreach (Constraints.InitialTag fc in state.CheckInitialConstraints())
        {
            initialAction[(int)fc]();
        }
        blockingState = state;
    }
    public void AddAnimation(RotationAnimation anim)
    {
        anim.Initialise();
        animations.Add(anim);
    }

    public void AddAnimation(RotationAnimation anim, RotationAnimation.InitParams initParams)
    {
        anim.Initialise(initParams);
        animations.Add(anim);
    }

    void Animate()
    {
        Quaternion animationRot = Quaternion.RotateTowards(body.localRotation, Quaternion.identity, 400f * Time.fixedDeltaTime);
        if(animations.Count > 0)
        {
            animationRot = body.localRotation;
        }
        for(int i = 0; i < animations.Count; i++)
        {
            if(!animations[i].ShouldTerminate())
            {
                animationRot = Quaternion.RotateTowards(animationRot, animations[i].ApplyRotation(Time.fixedDeltaTime), animations[i].animationRate * Time.fixedDeltaTime);
            }
            else
            {
                animations.RemoveAt(i);
                i--;
            }
        }

        body.localRotation = animationRot;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.AngleAxis(Vector3.SignedAngle(Vector3.forward, axis, Vector3.up), Vector3.up), rotSpeed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger)
            return;
        AttackTest damage = other.GetComponent<AttackTest>();
        if (damage != null)
        {
            if (blockingState.CompareStates(damage.myDamage.myType))
            {
                damage.myDamage.Blocked();
                currentControls.SuccessfulBlock();
            }
            else
            {
                damage.myDamage.Hit();
            }
        }
        else
        {
            transform.position = startPos;
        }
    }

    void ConstrainGravity()
    {
        ongoingConstraints[(int)Constraints.OngoingTag.FreezeGrav] = true;
    }

    void ConstrainMovementInput()
    {
        ongoingConstraints[(int)Constraints.OngoingTag.FreezeMovementInput] = true;
    }

    void ResetForces()
    {
        myRigidbody.velocity -= new Vector3(0f, myRigidbody.velocity.y, 0f);
        playerForces.Clear();
    }

    void ResetAnimations()
    {
        //transform.rotation = Quaternion.AngleAxis(Vector3.SignedAngle(Vector3.forward, axis, Vector3.up), Vector3.up);
        animations.Clear();
    }


}
