using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    List<ActiveForce> playerForces = new List<ActiveForce>();
    List<ConstForce> constantForces = new List<ConstForce>();
    List<RotationAnimation> animations = new List<RotationAnimation>();

    [Header("Forces")]
    [SerializeField] public ActiveForce dodgeForce;
    [SerializeField] public ActiveForce jumpForce;
    [SerializeField] public ActiveForce doubleJumpForce;
    [SerializeField] ConstForce gravity;

    [Header("Transform Animations")]
    [SerializeField] public RotationAnimation dodgeAnimation;
    [SerializeField] public RotationAnimation jumpAnimation;
    ControlScheme currentControls = new GroundControls();
    ControlScheme[] controls;
    BlockState blockingState = new NeutralBlock();
    Rigidbody myRigidbody;
    CapsuleCollider myCollider;
    Vector3 prevAxis = Vector3.forward;
    Vector3 axis = Vector3.forward;

    [Header("Speed Control")]
    private float timeOfAxisSwitch;
    [SerializeField] float speedSwitchTime;
    public float speed;
    private int speedTierIndex = 0;
    public float[] speedTiers;
    public float rotSpeed;
    float previousSpeed = 0;

    bool[] ongoingConstraints = new bool[Enum.GetValues(typeof(Constraints.OngoingTag)).Length];
    float timeOfFall;
    bool grounded;

    delegate void ConstraintAction();
    ConstraintAction[] actionConstraint;

    delegate void InitialAction();
    InitialAction[] initialAction;

    [SerializeField] Transform body;

    [Header("Animation Parameters")]
    [SerializeField] float landAnimFallTime;
    Vector3 startPos;
    MeshRenderer myRenderer;
    Color[] colours = new Color[4] { Color.red, Color.blue, Color.green, Color.white };

    Animator animationController;
    float timeOfAnimationEnd;
    Vector3 joystickAxis;
    int groundedHash = Animator.StringToHash("Grounded");
    int horizontalSpeedHash = Animator.StringToHash("HorizontalSpeed");
    int landHash = Animator.StringToHash("Land");
    int fallingTimeHash = Animator.StringToHash("FallingTime");
    int overrideTimeHash = Animator.StringToHash("OverrideTime");
    int speedDirectionHash = Animator.StringToHash("SpeedDirection");
    int inputHash = Animator.StringToHash("InputSpeed");

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
        animationController = GetComponent<Animator>();
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
        CheckForces();
    }

    void ApplyBlock()
    {
        if (blockingState.Progress(Time.deltaTime))
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
        for (int i = 0; i < ongoingConstraints.Length; i++)
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
                foreach (Constraints.OngoingTag t in playerForces[i].CheckOngoingConstraints())
                {
                    ongoingConstraints[(int)t] = true;
                }
            }
            else
            {
                playerForces.RemoveAt(i);
                i--;
            }
        }

        myRigidbody.AddForce(gravity.ApplyForce(Time.fixedDeltaTime, grounded || ongoingConstraints[(int)Constraints.OngoingTag.FreezeGrav]));
        ApplyPlayerSpeed();
    }

    void ApplyPlayerSpeed()
    {
        if (joystickAxis.magnitude != 0)
        {
            Vector3 horizontalVelocity = myRigidbody.velocity;
            horizontalVelocity.y = 0;
            if (Vector3.Angle(horizontalVelocity, axis) > 91.0f || horizontalVelocity.magnitude < 1.0f)
            {
                speedTierIndex = 0;
                timeOfAxisSwitch = Time.timeSinceLevelLoad;
            }
            else if (Time.timeSinceLevelLoad - timeOfAxisSwitch > speedSwitchTime && grounded) speedTierIndex = 1;
            prevAxis = axis;
            speed = speedTiers[speedTierIndex];
            if (ongoingConstraints[(int)Constraints.OngoingTag.FreezeMovementInput])
            {
                speed = 0;
                speedTierIndex = 0;
            }
        }
        else
        {
            speed = 0;
            speedTierIndex = 0;
        }
        myRigidbody.AddForce(speed * axis * Time.fixedDeltaTime);
    }

    // By the time I reach Check forces, all forces have been applied
    void CheckForces()
    {
        Vector3 characterDirection = myRigidbody.velocity;
        if (characterDirection.y >= 0) timeOfFall = Time.timeSinceLevelLoad;
        characterDirection.y = 0.0f;
        float directionDifference = characterDirection.magnitude - previousSpeed;
        animationController.SetFloat(speedDirectionHash, directionDifference);
        if (directionDifference != 0)  previousSpeed = characterDirection.magnitude;
        animationController.SetFloat(horizontalSpeedHash, characterDirection.magnitude);
        animationController.SetFloat(fallingTimeHash, Time.timeSinceLevelLoad - timeOfFall);
        animationController.SetBool(groundedHash, grounded);
        animationController.SetFloat(overrideTimeHash, Mathf.Max(-1.0f, timeOfAnimationEnd - Time.timeSinceLevelLoad));
    }

    bool GroundCheck()
    {
        float sphereRadius = myCollider.radius * 0.95f;
        float cylinderHeight = myCollider.bounds.extents.y - sphereRadius + 0.1f;
        RaycastHit hit;
        if(Physics.SphereCast(myRigidbody.position, sphereRadius, Vector3.down, out hit,cylinderHeight, 2147483647, QueryTriggerInteraction.Ignore))
        {
            if (!grounded) HitGroundEffects();
            grounded = true;
            currentControls = controls[currentControls.ChangeControls((int)ControlScheme.ControlType.Grounded)];
            timeOfFall = Time.timeSinceLevelLoad;
            return grounded;
        }
        grounded = false;
        currentControls = controls[currentControls.ChangeControls((int)ControlScheme.ControlType.MidAir)];
        return grounded;
    }

    void HitGroundEffects()
    {
        if (Time.timeSinceLevelLoad - timeOfFall > landAnimFallTime && joystickAxis.magnitude == 0.0f) PlayAnimation(landHash, 0.5f);
    }

    void Controls()
    {
        joystickAxis = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        animationController.SetFloat(inputHash, joystickAxis.magnitude);
        if (Mathf.Abs(joystickAxis.x) + Mathf.Abs(joystickAxis.z) != 0)
        {
            
            axis.x = joystickAxis.x;
            axis.z = joystickAxis.z;
            axis = Quaternion.AngleAxis(Camera.main.transform.rotation.eulerAngles.y, Vector3.up) * axis;
            axis = axis.normalized;
        }
        else
        {
            timeOfAxisSwitch = Time.timeSinceLevelLoad;
        }

        if (Input.GetKeyDown(KeyCode.Period) && !ongoingConstraints[(int)Constraints.OngoingTag.FreezeBlockInput])
        {
            currentControls.LBlock(this);
        }
        if (Input.GetKeyDown(KeyCode.Slash) && !ongoingConstraints[(int)Constraints.OngoingTag.FreezeBlockInput])
        {
            currentControls.RBlock(this);
        }
        if (Input.GetKeyDown(KeyCode.RightShift) && !ongoingConstraints[(int)Constraints.OngoingTag.FreezeDodgeInput])
        {
            currentControls.Dodge(this, new ActiveForce.InitParams(axis.x, axis.magnitude, axis.z, axis.magnitude));
        }
        if (Input.GetKeyDown(KeyCode.Space) && !ongoingConstraints[(int)Constraints.OngoingTag.FreezeJumpInput])
        {
            currentControls.Jump(this);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
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
    public void AddTransformAnimation(RotationAnimation anim)
    {
        anim.Initialise();
        animations.Add(anim);
    }

    public void PlayAnimation(int anim, float overrideTime)
    {
        animationController.Play(anim);
        animationController.StopPlayback();
        timeOfAnimationEnd = Time.timeSinceLevelLoad + overrideTime;
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
            //transform.position = startPos;
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
