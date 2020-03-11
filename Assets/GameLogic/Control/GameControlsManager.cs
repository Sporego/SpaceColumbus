using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using InputControls;

public class GameControlsManager : MonoBehaviour
{
    #region MouseClicks
    public static KeyInfo leftClick = new KeyInfo(KeyCode.Mouse0, KeyInfo.OnKey.Hold, false);
    public static KeyInfo leftClickDown = new KeyInfo(KeyCode.Mouse0, KeyInfo.OnKey.Down, false);
    public static KeyInfo leftClickDownDouble = new KeyInfo(KeyCode.Mouse0, KeyInfo.OnKey.Down, true);
    public static KeyInfo leftClickUp = new KeyInfo(KeyCode.Mouse0, KeyInfo.OnKey.Up, false);

    public static KeyInfo rightClick = new KeyInfo(KeyCode.Mouse1, KeyInfo.OnKey.Hold, false);
    public static KeyInfo rightClickDown = new KeyInfo(KeyCode.Mouse1, KeyInfo.OnKey.Down, false);
    public static KeyInfo rightClickDownDouble = new KeyInfo(KeyCode.Mouse1, KeyInfo.OnKey.Down, true);
    public static KeyInfo rightClickUp = new KeyInfo(KeyCode.Mouse1, KeyInfo.OnKey.Up, false);

    public static KeyInfo middleClick = new KeyInfo(KeyCode.Mouse2, KeyInfo.OnKey.Hold, false);
    public static KeyInfo middleClickDown = new KeyInfo(KeyCode.Mouse2, KeyInfo.OnKey.Down, false);
    #endregion MouseClicks

    #region CameraMovement
    public static KeyInfo cameraForward = new KeyInfo(KeyCode.W, KeyInfo.OnKey.Hold, false);
    public static KeyInfo cameraBack = new KeyInfo(KeyCode.S, KeyInfo.OnKey.Hold, false);
    public static KeyInfo cameraLeft = new KeyInfo(KeyCode.A, KeyInfo.OnKey.Hold, false);
    public static KeyInfo cameraRight = new KeyInfo(KeyCode.D, KeyInfo.OnKey.Hold, false);
    public static KeyInfo cameraDown = new KeyInfo(KeyCode.Q, KeyInfo.OnKey.Hold, false);
    public static KeyInfo cameraUp = new KeyInfo(KeyCode.E, KeyInfo.OnKey.Hold, false);

    public static KeyInfo cameraSpeedModifier = new KeyInfo(KeyCode.LeftShift, KeyInfo.OnKey.Hold, false);
    #endregion CameraMovement

    public static KeyInfo agentStopHotkey = new KeyInfo(KeyCode.Space, KeyInfo.OnKey.Down, false);
}
