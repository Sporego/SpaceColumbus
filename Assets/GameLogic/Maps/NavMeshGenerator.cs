using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Navigation
{
    public class NavMeshGenerator : MonoBehaviour
    {
        private GameObject navMeshRoot = null;

        private List<GameObject> navMeshElements = new List<GameObject>(); // TODO: generate only using nav mesh elements

        private bool navMeshInitialized = false;

        private void Awake()
        {
            if (navMeshRoot == null)
            {
                navMeshRoot = this.gameObject;
            }
        }

        public void BuildNavMesh()
        {
            Awake();

            // remove existing navMeshSurfaces
            foreach (NavMeshSurface navMeshSurface in navMeshRoot.GetComponents<NavMeshSurface>())
                Destroy(navMeshSurface);

            int agentTypeCount = UnityEngine.AI.NavMesh.GetSettingsCount();
            if (agentTypeCount < 1) { return; }
            for (int i = 0; i < agentTypeCount; ++i)
            {
                NavMeshBuildSettings settings = UnityEngine.AI.NavMesh.GetSettingsByIndex(i);
                NavMeshSurface navMeshSurface = navMeshRoot.AddComponent<NavMeshSurface>();
                navMeshSurface.agentTypeID = settings.agentTypeID;

                NavMeshBuildSettings actualSettings = navMeshSurface.GetBuildSettings();
                navMeshSurface.useGeometry = NavMeshCollectGeometry.RenderMeshes; // or you can use RenderMeshes
                //navMeshSurface.layerMask = true;

                // remove existing agents from the navmesh layermask
                navMeshSurface.layerMask -= LayerMask.GetMask("Agents");
                navMeshSurface.layerMask -= LayerMask.GetMask("Ignore Raycast");

                navMeshSurface.BuildNavMesh();
            }

            this.navMeshInitialized = true;
        }
    }
}
