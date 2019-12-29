
using Regions;
using UnityEngine;
using Utilities.MeshTools;

[RequireComponent (typeof (MeshFilter), typeof (MeshRenderer), typeof (MeshCollider))]
public abstract class RegionModelGenerator : MonoBehaviour {

    public void Start () {
        GameObject go = GameObject.FindGameObjectWithTag ("GameSession");
        Region region = ((GameSession) go.GetComponent (typeof (GameSession))).getRegion ();
        InitializeMesh (region);
    }

    public abstract void InitializeMesh(Region region);
}