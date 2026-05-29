using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Simple first-person camera controller using Unity's new Input System.
///
/// Attach this to the Main Camera.
///
/// Controls:
///   W / A / S / D  — move forward / left / back / right
///   Q / E          — move down / up
///   Mouse           — look around
///   Escape          — toggle cursor lock
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 7f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 0.15f;

    private float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleLook();
        HandleMove();
        HandleCursorToggle();
    }

    void HandleLook()
    {
        if (Mouse.current == null) return;

        Vector2 delta = Mouse.current.delta.ReadValue();
        float mouseX =  delta.x * mouseSensitivity;
        float mouseY =  delta.y * mouseSensitivity;

        xRotation -= mouseY;
        xRotation  = Mathf.Clamp(xRotation, -80f, 80f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX, Space.World);
    }

    void HandleMove()
    {
        if (Keyboard.current == null) return;

        float h = 0f, v = 0f, upDown = 0f;

        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)    v += 1f;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)  v -= 1f;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)  h -= 1f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) h += 1f;
        if (Keyboard.current.eKey.isPressed) upDown += 1f;
        if (Keyboard.current.qKey.isPressed) upDown -= 1f;

        Vector3 move = transform.right * h + transform.forward * v + Vector3.up * upDown;

        if (move.sqrMagnitude > 0.01f)
            transform.position += move.normalized * moveSpeed * Time.deltaTime;
    }

    void HandleCursorToggle()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            bool locked = Cursor.lockState == CursorLockMode.Locked;
            Cursor.lockState = locked ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible   = locked;
        }
    }
}
