using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionBumpMapGenerator : MonoBehaviour
{
    Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        // Animate the Shininess value
        float snowAmount = Mathf.PingPong(Time.time, 1.0f);
        rend.material.SetFloat("_SnowAmount", snowAmount);
    }
}