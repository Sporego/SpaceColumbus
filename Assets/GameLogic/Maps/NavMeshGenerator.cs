using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Navigation
{
    public class NavMeshGenerator : MonoBehaviour
    {
        private static string NavMeshRootObjectTag = "NavMeshRoot";

        private GameSession gameSession = null;

        private GameObject navMeshRoot = null;

        private List<GameObject> navMeshElements = new List<GameObject>(); // TODO: generate only using nav mesh elements

        private bool navMeshInitialized = false;

        private void Awake()
        {
            if (gameSession == null)
            {
                gameSession = GameObject.FindGameObjectWithTag(StaticGameDefs.GameSessionTag).GetComponent<GameSession>();
            }
            if (navMeshRoot == null)
            {
                navMeshRoot = this.gameObject;
            }
        }

        public void BuildNavMesh()
        {
            int agentTypeCount = UnityEngine.AI.NavMesh.GetSettingsCount();
            if (agentTypeCount < 1) { return; }
            for (int i = 0; i < agentTypeCount; ++i)
            {
                NavMeshBuildSettings settings = UnityEngine.AI.NavMesh.GetSettingsByIndex(i);
                NavMeshSurface navMeshSurface = navMeshRoot.AddComponent<NavMeshSurface>();
                navMeshSurface.agentTypeID = settings.agentTypeID;

                NavMeshBuildSettings actualSettings = navMeshSurface.GetBuildSettings();
                navMeshSurface.useGeometry = NavMeshCollectGeometry.RenderMeshes; // or you can use RenderMeshes

                navMeshSurface.BuildNavMesh();
            }

            this.navMeshInitialized = true;
        }
    }
}
