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
                GameObject.FindGameObjectWithTag(StaticGameDefs.GameRootTag).GetComponent<MaterialSettings>().MainRegionMaterial;
            this.gameObject.tag = StaticGameDefs.NavMeshElementTag;
        }

        public abstract void InitializeMesh(Region region);
    }
}
