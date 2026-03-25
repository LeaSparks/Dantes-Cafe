using UnityEngine;
using UnityEngine.InputSystem;

public class CursorManager : MonoBehaviour
{
    public enum CursorState
    {
        Idle,
        Hover,
        Grab,
        ValidPlace
    }

    [Header("Cursor Textures")]
    public Texture2D idleCursor;
    public Texture2D hoverCursor;
    public Texture2D grabCursor;
    public Texture2D validPlaceCursor;

    [Header("Hotspot")]
    public Vector2 hotspot = Vector2.zero;

    [Header("Raycast Settings")]
    public float rayDistance = 100f;
    public LayerMask interactableLayer;
    public LayerMask placeableLayer;

    private CursorState currentState;
    private bool isGrabbing;

    private int forceFrames = 5;

    void OnEnable()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        currentState = CursorState.Idle;
        ApplyCursor(currentState);
    }

    void Update()
    {
        if (Mouse.current == null || Camera.main == null)
            return;

        Vector2 mousePos = Mouse.current.position.ReadValue();

        bool isHovering = Physics.Raycast(
            Camera.main.ScreenPointToRay(mousePos),
            rayDistance,
            interactableLayer
        );

        bool isOverPlaceable = Physics.Raycast(
            Camera.main.ScreenPointToRay(mousePos),
            rayDistance,
            placeableLayer
        );

        bool mouseDown = Mouse.current.leftButton.wasPressedThisFrame;
        bool mouseUp = Mouse.current.leftButton.wasReleasedThisFrame;

        if (mouseDown && isHovering)
            isGrabbing = true;

        if (mouseUp)
            isGrabbing = false;

        CursorState targetState;

        if (isGrabbing)
            targetState = isOverPlaceable ? CursorState.ValidPlace : CursorState.Grab;
        else
            targetState = isHovering ? CursorState.Hover : CursorState.Idle;

        if (forceFrames > 0)
        {
            forceFrames--;
            ApplyCursor(targetState, true);
        }
        else
        {
            SetCursor(targetState);
        }
    }

    void SetCursor(CursorState state)
    {
        if (currentState == state) return;

        currentState = state;
        ApplyCursor(state);
    }

    void ApplyCursor(CursorState state, bool force = false)
    {
        Texture2D tex = null;

        switch (state)
        {
            case CursorState.Idle: tex = idleCursor; break;
            case CursorState.Hover: tex = hoverCursor; break;
            case CursorState.Grab: tex = grabCursor; break;
            case CursorState.ValidPlace: tex = validPlaceCursor; break;
        }

        if (tex != null)
        {
            Cursor.SetCursor(tex, hotspot, CursorMode.ForceSoftware);
        }
    }
}