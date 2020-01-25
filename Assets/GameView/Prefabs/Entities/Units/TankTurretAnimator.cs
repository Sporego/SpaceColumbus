using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankTurretAnimator : MonoBehaviour
{
    public GameObject AnimatedObject;
    public float speed;
    public Vector3 moveAmount;
    public float rotationCompletedThresh = 1f;

    private Vector3 rotationTarget;

    // TODO: Convert this to an extractor Animator System + components

    private bool animating;

    public void Start()
    {
        animating = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!animating)
        {
            animating = true;

            Vector2 rc = UnityEngine.Random.onUnitSphere;
            rotationTarget = new Vector3(rc.x, 0, rc.y);
        }
        else {
            Quaternion toTargetQuaternion = Quaternion.LookRotation(rotationTarget);

            float angleDeltaCrt = Vector3.Angle(rotationTarget, AnimatedObject.transform.forward);

            if (angleDeltaCrt < rotationCompletedThresh)
            {
                animating = false;
            }
            else
            {
                float lerpRatio = speed / (Mathf.Max(angleDeltaCrt, 0.001f)) * Time.deltaTime;
                lerpRatio = Mathf.Clamp(lerpRatio, 0f, 1f);

                // calculate the Quaternion for the rotation
                var rotTurret = Quaternion.Slerp(AnimatedObject.transform.rotation, toTargetQuaternion, lerpRatio);

                // Apply the rotation to turret (horizontal: left/right)
                AnimatedObject.transform.localRotation = rotTurret;
                AnimatedObject.transform.localEulerAngles = new Vector3(
                    0f, AnimatedObject.transform.localEulerAngles.y, 0f); // only rotate y

                //this.transform.rotation.SetEulerAngles(this.transform.rotation.eulerAngles + delta * Time.deltaTime);
            }
        }
    }
}
