using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Draws either an arrow or a sphere gizmo with adjustable line thickness.
/// </summary>
public class ArrowGizmo : MonoBehaviour
{
    public enum GizmoType
    {
        Arrow,
        Sphere
    }

    [Header("General Settings")]
    public GizmoType gizmoType = GizmoType.Arrow;
    public Color gizmoColor = Color.red;

    [Header("Line Settings")]
    [Range(1f, 10f)]
    public float lineThickness = 2f;

    [Header("Arrow Settings")]
    public float arrowLength = 2f;
    public float headLength = 0.5f;
    public float headAngle = 20f;

    [Header("Sphere Settings")]
    public float sphereRadius = 0.5f;

    private void OnDrawGizmos()
    {
        #if UNITY_EDITOR
        Handles.color = gizmoColor;

        switch (gizmoType)
        {
            case GizmoType.Arrow:
                DrawArrow();
                break;

            case GizmoType.Sphere:
                Handles.DrawWireDisc(transform.position, Vector3.up, sphereRadius, lineThickness);
                break;
        }
        #endif
    }

#if UNITY_EDITOR
    private void DrawArrow()
    {
        Vector3 start = transform.position;
        Vector3 end = start + transform.forward * arrowLength;

        // Shaft
        Handles.DrawLine(start, end, lineThickness);

        // Head
        DrawArrowHead(end, transform.forward);
    }

    private void DrawArrowHead(Vector3 position, Vector3 direction)
    {
        if (direction == Vector3.zero) return;

        direction.Normalize();

        Vector3 right = Quaternion.LookRotation(direction) *
                        Quaternion.Euler(0, 180 + headAngle, 0) *
                        Vector3.forward;

        Vector3 left = Quaternion.LookRotation(direction) *
                       Quaternion.Euler(0, 180 - headAngle, 0) *
                       Vector3.forward;

        Handles.DrawLine(position, position + right * headLength, lineThickness);
        Handles.DrawLine(position, position + left * headLength, lineThickness);
    }
#endif
}