using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    List<CameraMode> cameraConstraints;
    CameraMode defaultCamera;
    Camera camera;

    Transform _transform;

    PlayerController player;
    Rigidbody playerRigidbody;

    [Header("Camera Properties")]
    [SerializeField] Vector3 defaultLookOffset;
    [SerializeField] Vector3 defaultPositionOffset;
    [SerializeField] float defaultSpeed;
    [SerializeField] float defaultRotationSpeed;


    private void Awake()
    {
        cameraConstraints = new List<CameraMode>();
        defaultCamera = new FollowCamera(defaultPositionOffset, defaultLookOffset, defaultSpeed, defaultRotationSpeed);

        _transform = transform;
        camera = GetComponent<Camera>();

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        playerRigidbody = player.gameObject.GetComponent<Rigidbody>();
    }


    void FixedUpdate()
    {
        CameraMode.OutputParameters cameraOutputParameters;
        int constraintCount = cameraConstraints.Count;

        CameraMode.InputParameters inputParameters = new CameraMode.InputParameters { playerPosition = player.transform.position, playerVelocity = playerRigidbody.velocity, playerInput = player.joystickAxis, _camera = camera };

        switch(constraintCount)
        {
            case 0:
                cameraOutputParameters = defaultCamera.GetPositionAndRotation(inputParameters);
                break;

            default:
                cameraOutputParameters = cameraConstraints[constraintCount - 1].GetPositionAndRotation(inputParameters);
                break;
        }
        _transform.SetPositionAndRotation(cameraOutputParameters.position, cameraOutputParameters.rotation);

        if (cameraOutputParameters.fieldOfView == 0) cameraOutputParameters.fieldOfView = 60.0f;
       camera.fieldOfView = cameraOutputParameters.fieldOfView;

    }

    public void SetCameraConstraint(CameraMode constraint)
    {
        defaultCamera.Reset();
        cameraConstraints.Add(constraint);

    }

    public void RemoveCameraConstraint(CameraMode constraint)
    {
        constraint.Reset();
        cameraConstraints.Remove(constraint);
    }
}

public interface CameraMode
{
    struct InputParameters
    {
        public Vector3 playerPosition;
        public Vector3 playerInput;
        public Vector3 playerVelocity;
        public Camera _camera;
    }
    struct OutputParameters
    {
        public Vector3 position;
        public Quaternion rotation;
        public float speed;
        public float fieldOfView;
    }
    enum Modes
    {
        LookAtObject,
        AnchorToPosition,
        Static,
        Follow
    }
    public OutputParameters GetPositionAndRotation(InputParameters inputData);

    public void Reset();
}

public class LookAtCamera : CameraMode
{
    Vector3 objectPos;
    float distToPlayer;
    float heightMod;

    //Camera movement
    float camSpeed;
    float camRotSpeed;
    Transform cameraTransform;
    Vector3 cameraVel;
    float cameraRotVel;
    float cameraRotProgress;
    public LookAtCamera(Vector3 objectPosition, float distanceFromPlayer, float heightAbovePlayer, float cameraSpeed, float cameraRotationSpeed)
    {
        objectPos = objectPosition;
        distToPlayer = distanceFromPlayer;
        heightMod = heightAbovePlayer;

        camSpeed = cameraSpeed;
        camRotSpeed = cameraRotationSpeed;
        cameraTransform = Camera.main.transform;
    }
    public CameraMode.OutputParameters GetPositionAndRotation(CameraMode.InputParameters inputData)
    {
        Vector3 adjustedPlayerPos = inputData.playerPosition + Vector3.up * heightMod;
        Vector3 cameraDir = (objectPos - adjustedPlayerPos).normalized;
        Vector3 cameraPos = objectPos + cameraDir * -distToPlayer;
        cameraPos = Vector3.SmoothDamp(cameraTransform.position, cameraPos, ref cameraVel, camSpeed);

        Quaternion cameraRot = Quaternion.LookRotation(objectPos - cameraPos);
        cameraRotProgress = Mathf.SmoothDamp(cameraRotProgress, 1f, ref cameraRotVel, camRotSpeed);
        cameraRot = Quaternion.Slerp(cameraTransform.rotation, cameraRot, cameraRotProgress);

        return new CameraMode.OutputParameters() { position = cameraPos, rotation = cameraRot, speed = 0 };
    }

    public void Reset()
    {
        cameraVel = Vector3.zero;
        cameraRotVel = 0f;
        cameraRotProgress = 0f;
    }
}

public class AnchorCamera : CameraMode
{
    Vector3 anchorPos;
    float addHeight;

    //Camera movement
    float camSpeed;
    float camRotSpeed;
    Transform cameraTransform;
    Vector3 cameraVel;
    float cameraRotVel;
    float cameraRotProgress;
    public AnchorCamera(Vector3 anchorPosition, float additionalHeight, float cameraSpeed, float cameraRotationSpeed)
    {
        anchorPos = anchorPosition;
        addHeight = additionalHeight;
        camSpeed = cameraSpeed;
        camRotSpeed = cameraRotationSpeed;
        cameraTransform = Camera.main.transform;
    }
    public CameraMode.OutputParameters GetPositionAndRotation(CameraMode.InputParameters inputData)
    {
        Vector3 cameraPos = new Vector3(anchorPos.x, inputData.playerPosition.y + addHeight, anchorPos.z);
        Vector3 cameraDir = (inputData.playerPosition - cameraPos).normalized;
        Quaternion cameraRot = Quaternion.LookRotation(cameraDir);

        cameraPos = Vector3.SmoothDamp(cameraTransform.position, cameraPos, ref cameraVel, camSpeed);
        cameraRotProgress = Mathf.SmoothDamp(cameraRotProgress, 1f, ref cameraRotVel, camRotSpeed);
        cameraRot = Quaternion.Slerp(cameraTransform.rotation, cameraRot, cameraRotProgress);

        return new CameraMode.OutputParameters() { position = cameraPos, rotation = cameraRot, speed = 0 };
    }

    public void Reset()
    {
        cameraVel = Vector3.zero;
        cameraRotVel = 0f;
        cameraRotProgress = 0f;
    }
}

public class StaticCamera : CameraMode
{
    Vector3 cameraPos;
    Quaternion lookDir;

    //Camera movement
    float camSpeed;
    float camRotSpeed;
    Transform cameraTransform;
    Vector3 cameraVel;
    float cameraRotVel;
    float cameraRotProgress;
    public StaticCamera(Vector3 cameraPosition, Vector3 lookDirection, float cameraSpeed, float cameraRotationSpeed)
    {
        cameraPos = cameraPosition;
        lookDir = Quaternion.Euler(lookDirection);
        camSpeed = cameraSpeed;
        camRotSpeed = cameraRotationSpeed;
        cameraTransform = Camera.main.transform;
    }
    public CameraMode.OutputParameters GetPositionAndRotation(CameraMode.InputParameters inputData)
    {
        Vector3 pos = Vector3.SmoothDamp(cameraTransform.position, cameraPos, ref cameraVel, camSpeed);
        cameraRotProgress = Mathf.SmoothDamp(cameraRotProgress, 1f, ref cameraRotVel, camRotSpeed);
        Quaternion rot = Quaternion.Slerp(cameraTransform.rotation, lookDir, cameraRotProgress);

        return new CameraMode.OutputParameters() { position = pos, rotation = rot, speed = 0 };
    }

    public void Reset()
    {
        cameraVel = Vector3.zero;
        cameraRotVel = 0f;
        cameraRotProgress = 0f;
    }
}

public class FollowCamera : CameraMode
{
    Vector3 lookOffset;
    Vector3 posOffset;

    //Camera movement
    float camSpeed;
    float camRotSpeed;
    Transform cameraTransform;
    Vector3 cameraVel;
    float cameraRotVel;
    float cameraRotProgress;
    float fieldOfViewVel;
    public FollowCamera(Vector3 cameraOffset, Vector3 _lookOffset, float cameraSpeed, float cameraRotationSpeed)
    {
        posOffset = cameraOffset;
        lookOffset = _lookOffset;
        camSpeed = cameraSpeed;
        camRotSpeed = cameraRotationSpeed;
        cameraTransform = Camera.main.transform;
    }
    public CameraMode.OutputParameters GetPositionAndRotation(CameraMode.InputParameters inputData)
    {
        Vector3 targetPos = inputData.playerPosition + posOffset;
        float targetHeight = targetPos.y;
        Vector3 cameraPos = cameraTransform.position;
        Vector3 cameraDir = (inputData.playerPosition - targetPos + lookOffset).normalized;
        Quaternion cameraAngle = Quaternion.LookRotation(cameraDir);
        cameraPos.y = 0;
        targetPos.y = 0;

        Vector3 pos = Vector3.SmoothDamp(cameraPos, targetPos, ref cameraVel, camSpeed);
        pos.y = targetHeight;

        Vector3 horizontalCameraDir = cameraDir;
        horizontalCameraDir.y = 0;
        Vector3 horizontalPlayerDir = inputData.playerVelocity.normalized;
        horizontalPlayerDir.y = 0;

        float fieldOfViewTarget = 60.0f;
        if (Vector3.Angle(horizontalPlayerDir, horizontalCameraDir) < 46.0f && inputData.playerVelocity.magnitude > 10.0f)
        {
            fieldOfViewTarget = 55.0f;
        }
        float fieldOfView = inputData._camera.fieldOfView;
        fieldOfView = Mathf.SmoothDamp(fieldOfView, fieldOfViewTarget, ref fieldOfViewVel, 0.5f);

        cameraRotProgress = Mathf.SmoothDamp(cameraRotProgress, 1f, ref cameraRotVel, camRotSpeed);
        Quaternion rot = Quaternion.Slerp(cameraTransform.rotation, cameraAngle, cameraRotProgress);

        return new CameraMode.OutputParameters() { position = pos, rotation = rot, speed = 0 , fieldOfView = fieldOfView };
    }

    public void Reset()
    {
        cameraVel = Vector3.zero;
        cameraRotVel = 0f;
        cameraRotProgress = 0f;
    }
}