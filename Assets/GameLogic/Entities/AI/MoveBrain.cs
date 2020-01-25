using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Brains.Movement
{
    [System.Serializable]
    public class MoveBrain
    {
        NavMeshAgent navMeshAgent;

        public float StuckDistanceThreshold = 0.05f; // minimal distance to destination to consider that destination is reached

        #region StuckConfig
        public float stuckTimeout = 2f; // in seconds

        public bool stuck = false;
        public Vector3 posAtStuck;
        public float timeSinceStuck = 0f;
        public float remainingDistance = 0;
        #endregion StuckConfig

        public Vector3 position { get { return this.navMeshAgent.nextPosition; } }

        // Start is called before the first frame update
        public MoveBrain(NavMeshAgent navMeshAgent)
        {
            this.posAtStuck = Vector3.positiveInfinity;
            this.navMeshAgent = navMeshAgent;
        }

        public bool SetDestination(Vector3 destination)
        {
            if (!this.navMeshAgent.isOnNavMesh)
                return false;

            NavMeshPath path = new NavMeshPath();
            bool success = this.navMeshAgent.SetDestination(destination);

            remainingDistance = this.navMeshAgent.remainingDistance;

            return success;
        }

        public void StopMoving()
        {
            if (!this.navMeshAgent.isOnNavMesh)
                return;

            float v = this.navMeshAgent.velocity.magnitude;
            float distToStop = v * v / this.navMeshAgent.acceleration / 2f;
            this.navMeshAgent.SetDestination(position + distToStop * this.navMeshAgent.velocity.normalized);
        }

        // TODO: make sure this works properly
        public void checkStuck()
        {
            bool stuck = false;
            Vector3 curPos = position;
            stuck |= (posAtStuck - curPos).magnitude < StuckDistanceThreshold;
            stuck |= (Mathf.Abs(remainingDistance - this.navMeshAgent.remainingDistance)) <= this.navMeshAgent.stoppingDistance;
            if (stuck && !this.stuck) // newly stuck; update position at stuck
                posAtStuck = curPos;
            this.stuck = stuck;
        }

        public bool AtDestination()
        {
            if (!this.navMeshAgent.isOnNavMesh)
                return true;

            checkStuck();
            if (this.stuck)
            {
                timeSinceStuck += Time.deltaTime;
                if (timeSinceStuck > stuckTimeout)
                    return true;
            }
            else
            {
                timeSinceStuck = 0f;
            }

            remainingDistance = this.navMeshAgent.remainingDistance;

            return this.navMeshAgent.remainingDistance <= this.navMeshAgent.stoppingDistance;
        }
    }
}
