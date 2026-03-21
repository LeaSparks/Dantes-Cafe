using UnityEngine;
using UnityEngine.InputSystem;

public class CardOutLineTester : MonoBehaviour
{
    [SerializeField] private CardOutLineVisual outlineVisual;

    private void Update()
    {
        if (outlineVisual == null)
            return;

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
            outlineVisual.ShowHover();

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
            outlineVisual.ShowValid();

        if (Keyboard.current.digit3Key.wasPressedThisFrame)
            outlineVisual.ShowInvalid();

        if (Keyboard.current.digit4Key.wasPressedThisFrame)
            outlineVisual.ShowSelected();

        if (Keyboard.current.digit0Key.wasPressedThisFrame)
            outlineVisual.Hide();
    }
}