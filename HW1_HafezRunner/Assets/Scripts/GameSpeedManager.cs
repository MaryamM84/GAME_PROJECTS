
using UnityEngine;
using UnityEngine.InputSystem;

public class GameSpeedManager : MonoBehaviour
{
    [Header("Speed Settings")]
    public float baseSpeed = 5f;
    public float dashMultiplier = 2f;

    public bool IsDashing
    {
        get
        {
            if (Keyboard.current == null)
                return false;

            return Keyboard.current.leftShiftKey.isPressed
                || Keyboard.current.rightShiftKey.isPressed;
        }
    }

    public float CurrentSpeed
    {
        get
        {
            return baseSpeed *
                (IsDashing ? dashMultiplier : 1f);
        }
    }
}
