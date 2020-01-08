using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using InputControls;

public class GameControlsManager : MonoBehaviour
{
    #region MouseClicks
    public static KeyInfo leftClick = new KeyInfo(KeyCode.Mouse0, false, false);
    public static KeyInfo leftClickDown = new KeyInfo(KeyCode.Mouse0, true, false);

    public static KeyInfo rightClick = new KeyInfo(KeyCode.Mouse1, false, false);
    public static KeyInfo rightClickDown = new KeyInfo(KeyCode.Mouse1, true, false);

    public static KeyInfo middleClick = new KeyInfo(KeyCode.Mouse2, false, false);
    public static KeyInfo middleClickDown = new KeyInfo(KeyCode.Mouse2, true, false);
    #endregion MouseClicks

    #region CameraMovement
    public static KeyInfo cameraForward = new KeyInfo(KeyCode.W, false, false);
    public static KeyInfo cameraBack = new KeyInfo(KeyCode.S, false, false);
    public static KeyInfo cameraLeft = new KeyInfo(KeyCode.A, false, false);
    public static KeyInfo cameraRight = new KeyInfo(KeyCode.D, false, false);
    public static KeyInfo cameraDown = new KeyInfo(KeyCode.Q, false, false);
    public static KeyInfo cameraUp = new KeyInfo(KeyCode.E, false, false);

    public static KeyInfo cameraSpeedModifier = new KeyInfo(KeyCode.LeftShift, false, false);
    #endregion CameraMovement

}
