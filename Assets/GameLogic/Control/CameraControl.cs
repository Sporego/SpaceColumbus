﻿//
//Filename: KeyboardCameraControl.cs
//

using System;
using System.Collections;
using UnityEngine;

using Regions;
using InputControls;

[AddComponentMenu("Camera-Control/Keyboard")]
public class CameraControl : MonoBehaviour
{
    public float globalSensitivity = 10f; // global camera speed sensitivity
    public float cameraSpeedModifierMultiplier = 2.5f; // global camera speed sensitivity multipler when a special modifer key is held down
    
    #region MouseControlConfiguration

    // camera scrolling sensitivity
    [Header("Scrolling")]
    public float scrollingSensitivityModifier = 10f;

    // edge scrolling
    [Header("Edge scrolling")]
    public bool allowEdgeScrolling = false;
    public int edgeScrollDetectBorderThickness = 15;

    // mouse control camera translation
    [Header("Mouse Scrolling")]
    public bool allowMouseTranslation = true;
    public float mouseTranslationSensitivityModifier = 0.75f; // mouse translation movement speed modifier

    // mouse rotation control
    [Header("Mouse Rotation")]
    public bool allowMouseRotation = true;
    public float mouseRotationSensitivityModifier = 50f; // mouse rotation movement speed modifier

    // zoom with FOV 
    [Header("Camera zoom")]
    public bool allowCameraZoom = true;
    public float cameraZoomSensitivityModifier = 2f; // mouse zoom speed modifier
    public float cameraFovMin = 30f;
    public float cameraFovMax = 120f;

    #endregion

    #region CameraControlConfiguration

    [Header("Camera movement inertia")]
    public bool allowCameraInertia = true;
    [Range(0.01f, 0.99f)]
    public float inertiaDecay = 0.95f;

    // camera restriction
    [Header("Camera restriction")]

    public float cameraVerticalAngleMin = 10f;
    public float cameraVerticalAngleMax = 80f;

    public float viewCenterOffset = 200f; // camera view center point offset; calculated as this far in front of camera
    public float viewCenterOnPlayerOffset = 75f; // how far from player position the camera will be set when focusing on player
    public float viewCenterOnPlayerLimiterInertia = 0.5f; // how 

    // speed limiter must be adjusted given maxCameraToGroundDistance; shorter max dist requires higher limiter
    public float limiterInertia = 0.1f;

    public float cameraLimitDistance = 500f; // how far camera can move away from the player
    public float minCameraToGroundDistance = 2f; // how close to ground the camera can go before limiter will start resisting
    public float maxCameraToGroundDistance = 200f; // how high camera can go before limiter will start resisting

    public float cameraTooHighSpeedLimiter = 1.5f; // lower means less resistance
    public float cameraTooLowSpeedLimiter = 5f; // this one needs to be resistive otherwise camera will dip into objects

    #endregion

    #region PrivateVariables

    private bool cameraMoving = false;
    private bool cameraRotating = false;
    private bool cameraZooming = false;

    private bool mouseOverGame = false;

    private Vector3 mousePositionAtRightClickDown;
    private Vector3 mousePositionAtMiddleClickDown;

    // inertia
    private Vector3 inertiaPositionDelta;
    private Vector3 inertiaRotationDelta;
    private float inertiaFovDelta;

    private Vector3 restrictionCenterPoint, viewCenterPoint;

    private bool toggleCenterPointFocus = false;
    private bool centeringOnPlayer = false;

    private GameSession gameSession;
    private Region region;

    #endregion

    void Start()
    {
        Camera.main.depthTextureMode = DepthTextureMode.Depth;

        transform.position = getCameraPositionPlayerCentered();

        gameSession = GameObject.FindGameObjectWithTag(StaticGameDefs.GameSessionTag).GetComponent<GameSession>();
        region = gameSession.getRegion();

        restrictionCenterPoint = new Vector3(0, 0, 0); // GameControl.gameSession.humanPlayer.getPos();
        viewCenterPoint = new Vector3(0, 0, 0);

        mousePositionAtRightClickDown = Input.mousePosition;
        mousePositionAtMiddleClickDown = Input.mousePosition;

        inertiaPositionDelta = Vector3.zero;
        inertiaRotationDelta = Vector3.zero;
        inertiaFovDelta = 0;
    }

    void Update()
    {
        region = gameSession.getRegion();

        cameraMoving = false;
        cameraRotating = false;
        cameraZooming = false;

        if (toggleCenterPointFocus && !centeringOnPlayer)
        {
            toggleCenterPointFocus = false;
            centeringOnPlayer = true;
            StartCoroutine(startCenteringOnPlayer());
        }

        Vector3 cameraPos = this.transform.position,
            cameraDir = this.transform.forward;

        cameraPos.y = 0;
        cameraDir.y = 0;

        viewCenterPoint = cameraPos + cameraDir * viewCenterOffset;

        checkInputConfiguration();

        float modifier = KeyActiveChecker.isActive(GameControlsManager.cameraSpeedModifier) ? cameraSpeedModifierMultiplier : 1f;

        Vector3 positionDelta = processCameraMovement() * modifier;
        Vector3 rotationDelta = processCameraRotation() * modifier;
        float fovDelta = processCameraZoom() * modifier;

        processCameraDeltas(positionDelta, rotationDelta, fovDelta);

        RestrictCamera();
    }

    private void checkInputConfiguration()
    {
        mouseOverGame = false;
        // mouse cursor position check
        if (Input.mousePosition.x >= 0 &&
            Input.mousePosition.y >= 0 &&
            Input.mousePosition.x <= Screen.width &&
            Input.mousePosition.y <= Screen.height)
        {
            mouseOverGame = true;
        }

        // on right click
        if (KeyActiveChecker.isActive(GameControlsManager.rightClickDown))
        {
            mousePositionAtRightClickDown = Input.mousePosition;
        }

        // on middle click
        if (KeyActiveChecker.isActive(GameControlsManager.middleClickDown))
        {
            mousePositionAtMiddleClickDown = Input.mousePosition;
        }
    }

    // keyboard and edge scrolling
    private Vector3 processCameraMovement()
    {
        Vector3 positionDelta = Vector3.zero;

        Vector3 mouseDelta = Input.mousePosition - mousePositionAtMiddleClickDown;
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        forward.y = 0;
        right.y = 0;

        if (KeyActiveChecker.isActive(GameControlsManager.cameraForward) ||
            (allowEdgeScrolling && Input.mousePosition.y >= Screen.height - edgeScrollDetectBorderThickness))
        {
            positionDelta += forward;
        }
        if (KeyActiveChecker.isActive(GameControlsManager.cameraBack) ||
            (allowEdgeScrolling && Input.mousePosition.y <= edgeScrollDetectBorderThickness))
        {
            positionDelta -= forward;
        }
        if (KeyActiveChecker.isActive(GameControlsManager.cameraLeft) ||
            (allowEdgeScrolling && Input.mousePosition.x <= edgeScrollDetectBorderThickness))
        {
            positionDelta -= right;
        }
        if (KeyActiveChecker.isActive(GameControlsManager.cameraRight) ||
            (allowEdgeScrolling && Input.mousePosition.x >= Screen.width - edgeScrollDetectBorderThickness))
        {
            positionDelta += right;
        }
        if (KeyActiveChecker.isActive(GameControlsManager.cameraDown))
        {
            positionDelta += Vector3.down;
        }
        if (KeyActiveChecker.isActive(GameControlsManager.cameraUp))
        {
            positionDelta += Vector3.up;
        }

        // scrolling with mouse
        if (allowMouseTranslation && KeyActiveChecker.isActive(GameControlsManager.middleClick))
        {
            if (mouseOverGame)
            {
                Vector3 mouseTranslation = Vector3.zero;
                mouseTranslation += right * mouseDelta.x / Screen.width;
                mouseTranslation += forward * mouseDelta.y / Screen.height;

                positionDelta += mouseTranslation * mouseTranslationSensitivityModifier;
            }
        }

        positionDelta *= scrollingSensitivityModifier * globalSensitivity * Time.deltaTime;

        if (Vector3.zero != positionDelta)
            cameraMoving = true;

        return positionDelta;
    }

    private Vector3 processCameraRotation()
    {
        Vector3 rotation = Vector3.zero;

        if (allowMouseRotation && KeyActiveChecker.isActive(GameControlsManager.rightClick)) // right mouse
        {
            if (mouseOverGame)
            {

                Vector3 mouseDelta = Input.mousePosition - mousePositionAtRightClickDown;

                rotation += Vector3.up * mouseDelta.x / Screen.width; // horizontal
                rotation += Vector3.left * mouseDelta.y / Screen.height; // vertical

                rotation *= mouseRotationSensitivityModifier * globalSensitivity * Time.deltaTime;

                if (Vector3.zero != rotation)
                    cameraRotating = true;
            }
        }

        return rotation;
    }

    private float processCameraZoom()
    {
        float fovDelta = 0;

        if (allowCameraZoom)
        {
            if (mouseOverGame)
            {
                // camera zoom via FOV change
                fovDelta = Input.mouseScrollDelta.y * cameraZoomSensitivityModifier;

                if (fovDelta != 0)
                    cameraZooming = true;
            }
        }

        return fovDelta;
    }

    private void processCameraDeltas(Vector3 positionDelta, Vector3 rotationDelta, float fovDelta)
    {
        if (allowCameraInertia)
        {
            inertiaPositionDelta = inertiaPositionDelta * inertiaDecay + positionDelta * (1f - inertiaDecay);
            inertiaRotationDelta = inertiaRotationDelta * inertiaDecay + rotationDelta * (1f - inertiaDecay);
            inertiaFovDelta = inertiaFovDelta * inertiaDecay + fovDelta * (1f - inertiaDecay);
        }

        // apply position delta
        transform.Translate(inertiaPositionDelta, Space.World);

        // apply rotation delta
        transform.localEulerAngles += inertiaRotationDelta;

        // apply zoom delta
        Camera.main.fieldOfView -= inertiaFovDelta;
    }

    private IEnumerator startCenteringOnPlayer(float centeredThreshold = 1f)
    {
        Vector3 vectorToPlayer = getCameraPositionPlayerCentered() - transform.position;
        while (vectorToPlayer.magnitude > centeredThreshold)
        {
            // move cam towards player
            transform.position += vectorToPlayer.normalized * vectorToPlayer.magnitude * viewCenterOnPlayerLimiterInertia;
            // update distance to player
            vectorToPlayer = getCameraPositionPlayerCentered() - transform.position;
            yield return null;
        }
        centeringOnPlayer = false;
    }

    private void RestrictCamera()
    {
        // check if camera is out of bounds 
        Vector3 posRelative = transform.position - restrictionCenterPoint;
        if (posRelative.x > cameraLimitDistance)
        {
            transform.position -= new Vector3(posRelative.x - cameraLimitDistance, 0, 0);
        }
        else if (posRelative.x < -cameraLimitDistance)
        {
            transform.position -= new Vector3(posRelative.x + cameraLimitDistance, 0, 0);
        }
        if (posRelative.z > cameraLimitDistance)
        {
            transform.position -= new Vector3(0, 0, posRelative.z - cameraLimitDistance);
        }
        else if (posRelative.z < -cameraLimitDistance)
        {
            transform.position -= new Vector3(0, 0, posRelative.z + cameraLimitDistance);
        }

        // adjust camera height based on terrain
        float waterLevel = 0; // GameControl.gameSession.mapGenerator.getRegion().getWaterLevelElevation();
        float offsetAboveWater = transform.position.y - (waterLevel) - minCameraToGroundDistance;
        if (offsetAboveWater < 0)
        { // camera too low based on water elevation
            transform.position -= new Vector3(0, offsetAboveWater, 0) * limiterInertia * cameraTooLowSpeedLimiter;
        }
        try
        {
            Vector3 tileBelow = region.getTileAt(transform.position).pos;

            float offsetAboveFloor = transform.position.y - (tileBelow.y) - minCameraToGroundDistance;
            float offsetBelowCeiling = tileBelow.y + maxCameraToGroundDistance - (transform.position.y);

            if (offsetAboveFloor < 0)
            { // camera too low based on tile height
                transform.position -= new Vector3(0, offsetAboveFloor, 0) * limiterInertia * cameraTooLowSpeedLimiter;
            }
            else if (offsetBelowCeiling < 0)
            { // camera too high 
                transform.position += new Vector3(0, offsetBelowCeiling, 0) * limiterInertia * cameraTooHighSpeedLimiter;
            }
        }
        catch (NullReferenceException e)
        {
            // do nothing
        }

        // restrict rotation
        Vector3 rotation = transform.localEulerAngles;
        rotation.x = Mathf.Clamp(rotation.x, cameraVerticalAngleMin, cameraVerticalAngleMax);
        rotation.z = 0;
        transform.localEulerAngles = rotation;

        // restrict fov
        Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, cameraFovMin, cameraFovMax);
    }

    public Vector3 getCameraPositionPlayerCentered()
    {
        return /*GameControl.gameSession.humanPlayer.getPos()*/ new Vector3(0, 0, 0) - transform.forward * viewCenterOnPlayerOffset;
    }
}