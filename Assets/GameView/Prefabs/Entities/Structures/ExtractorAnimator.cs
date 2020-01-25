using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtractorAnimator : MonoBehaviour
{
    public GameObject AnimatedObject;
    public float speed;
    public float moveAmount;
    private Vector3 pivot;

    // TODO: Convert this to an extractor Animator System + components
    void Update()
    {
        pivot = AnimatedObject.transform.parent.transform.position;
        AnimatedObject.transform.position = pivot + new Vector3(0, moveAmount * Mathf.Sin(Time.time * speed));
    }
}
