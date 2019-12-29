using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities.MeshTools;
using SquareRegions;

using Regions;

namespace RegionModelGenerators
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
    public abstract class RegionModelGenerator : MonoBehaviour
    {
        public void Start()
        {
            this.gameObject.GetComponent<MeshRenderer>().material =
                GameObject.FindGameObjectWithTag(StaticGameDefs.GameRootObjectTag).GetComponent<MaterialSettings>().MainRegionMaterial;
        }

        public abstract void InitializeMesh(Region region);
    }
}
