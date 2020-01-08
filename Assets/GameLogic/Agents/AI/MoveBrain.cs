using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Brains
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class MoveBrain : MonoBehaviour
    {
        public float MinRemainingDistanceToDestination = 0.05f;

        NavMeshAgent navMeshAgent;

        // Start is called before the first frame update
        void Start()
        {
            this.navMeshAgent = this.GetComponent<NavMeshAgent>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetDestination(Vector3 destination) {
            if (this.navMeshAgent.isOnNavMesh)
                this.navMeshAgent.destination = destination;
        }

        public bool AtDestination()
        {
            return this.navMeshAgent.isOnNavMesh && this.navMeshAgent.remainingDistance <= MinRemainingDistanceToDestination;
        }
    }
}
