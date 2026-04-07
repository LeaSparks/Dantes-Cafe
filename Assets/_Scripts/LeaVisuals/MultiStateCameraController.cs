using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MultiStateCameraController : MonoBehaviour
{
    [System.Serializable]
    public class CameraState
    {
        public Transform cameraPoint;

        public Transform object1Target;
        public Transform object2Target;
    }

    [Header("Camera States (4 slots)")]
    public CameraState[] states = new CameraState[4];

    [Header("Objects To Move")]
    public Transform object1;
    public Transform object2;

    [Header("Transition Settings")]
    public float transitionTime = 1.5f;

    [Header("Mouse Look Settings")]
    public float mouseSensitivity = 100f;

    [Header("Vertical Clamp")]
    public float minVerticalAngle = -30f;
    public float maxVerticalAngle = 60f;

    [Header("Horizontal Clamp")]
    public float minHorizontalAngle = -60f;
    public float maxHorizontalAngle = 60f;

    private float xRotation = 0f;
    private float yRotation = 0f;

    private bool isTransitioning = false;
    private int currentStateIndex = 0;

    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;

        ApplyStateInstant(0);

        // Initialize rotation from current transform
        Vector3 angles = transform.rotation.eulerAngles;
        xRotation = angles.x;
        yRotation = angles.y;
    }

    void Update()
    {
        if (Mouse.current == null || Keyboard.current == null)
            return;

        HandleMouseLook();
        HandleStateInput();
    }

    void HandleMouseLook()
    {
        if (isTransitioning) return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        float mouseX = mouseDelta.x * mouseSensitivity * Time.deltaTime;
        float mouseY = mouseDelta.y * mouseSensitivity * Time.deltaTime;

        yRotation += mouseX;
        xRotation -= mouseY;

        // Clamp axies
        xRotation = Mathf.Clamp(xRotation, minVerticalAngle, maxVerticalAngle);
        yRotation = Mathf.Clamp(yRotation, minHorizontalAngle, maxHorizontalAngle);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }

    void HandleStateInput()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame) SwitchState(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) SwitchState(1);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) SwitchState(2);
        if (Keyboard.current.digit4Key.wasPressedThisFrame) SwitchState(3);
    }

    public void SwitchState(int index)
    {
        if (index < 0 || index >= states.Length) return;
        if (states[index] == null || states[index].cameraPoint == null) return;
        if (isTransitioning || index == currentStateIndex) return;
        //Debug.Log($"Camera Mode: {index}");
        StartCoroutine(TransitionToState(index));
    }

    IEnumerator TransitionToState(int newIndex)
    {
        isTransitioning = true;

        CameraState targetState = states[newIndex];

        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        Vector3 targetPos = targetState.cameraPoint.position;
        Quaternion targetRot = targetState.cameraPoint.rotation;

        Vector3 obj1Start = object1 ? object1.position : Vector3.zero;
        Vector3 obj2Start = object2 ? object2.position : Vector3.zero;

        Vector3 obj1Target = targetState.object1Target ? targetState.object1Target.position : obj1Start;
        Vector3 obj2Target = targetState.object2Target ? targetState.object2Target.position : obj2Start;

        float elapsed = 0f;

        while (elapsed < transitionTime)
        {
            float t = elapsed / transitionTime;
            t = Mathf.SmoothStep(0f, 1f, t);

            transform.position = Vector3.Lerp(startPos, targetPos, t);
            transform.rotation = Quaternion.Slerp(startRot, targetRot, t);

            if (object1)
                object1.position = Vector3.Lerp(obj1Start, obj1Target, t);

            if (object2)
                object2.position = Vector3.Lerp(obj2Start, obj2Target, t);

            elapsed += Time.deltaTime;
            yield return null;
        }


        // Sync rotation values so mouse doesn't jump
        Vector3 finalAngles = transform.rotation.eulerAngles;
        xRotation = finalAngles.x;
        yRotation = finalAngles.y;

        currentStateIndex = newIndex;
        isTransitioning = false;
    }

    void ApplyStateInstant(int index)
    {
        if (states[index] == null || states[index].cameraPoint == null)
            return;

        CameraState state = states[index];

        transform.position = state.cameraPoint.position;
        transform.rotation = state.cameraPoint.rotation;

        if (object1 && state.object1Target)
            object1.position = state.object1Target.position;

        if (object2 && state.object2Target)
            object2.position = state.object2Target.position;

        currentStateIndex = index;
    }
}