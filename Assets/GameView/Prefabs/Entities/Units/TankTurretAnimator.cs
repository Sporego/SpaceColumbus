using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class TankTurretAnimator : MonoBehaviour
//{
//    public GameObject AnimatedObject;
//    public float speed;
//    public Vector3 moveAmount;
//    public float rotationCompletedThresh = 1f;

//    private Vector3 rotationTarget;

//    // TODO: Convert this to an extractor Animator System + components

//    public bool animating;

//    public void Start()
//    {
//        animating = false;
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        if (!animating)
//        {
//            animating = true;

//            float rc = UnityEngine.Random.value * 360 / 2 / Mathf.PI;
//            rotationTarget = new Vector3(Mathf.Cos(rc), 0, Mathf.Sin(rc));
//        }
//        else
//        {
//            Quaternion toTargetQuaternion = Quaternion.LookRotation(rotationTarget, Vector3.up);

//            float angleDeltaCrt = Vector3.Angle(rotationTarget, AnimatedObject.transform.forward);

//            if (angleDeltaCrt < rotationCompletedThresh)
//            {
//                animating = false;
//            }
//            else
//            {
//                float lerpRatio = speed / (Mathf.Max(angleDeltaCrt, 0.001f)) * Time.deltaTime;

//                // calculate the Quaternion for the rotation
//                var rotTurret = Quaternion.Slerp(AnimatedObject.transform.localRotation, toTargetQuaternion, lerpRatio);

//                // Apply the rotation to turret (horizontal: left/right)
//                AnimatedObject.transform.localRotation = rotTurret;

//                //this.transform.rotation.SetEulerAngles(this.transform.rotation.eulerAngles + delta * Time.deltaTime);
//            }
//        }
//    }
//}
