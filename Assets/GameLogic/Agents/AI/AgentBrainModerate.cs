using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Brains
{
    public class AgentBrainModerate : AgentBrain
    {
        [Range(0, 1f)] public float NonIdleProbability = 0.05f;
        [Range(0, 1f)] public float IdleRoamingProbability = 0.5f;

        [Range(2f, 10f)] public float MaxIdleMoveMultiplier = 2f;
        [Range(0f, 100f)] public float MaxIdleMoveDistance = 10f;

        private static int MaxPathFindAttempts = 10;

        // Start is called before the first frame update
        new void Start()
        {
            base.Start();

            this.intelligence = Intelligence.Moderate;
            this.behaviourState = BehaviourState.Idle;
        }

        override public void MakeDecision()
        {
            float r1 = UnityEngine.Random.value;
            float r2 = UnityEngine.Random.value;
            float r3 = UnityEngine.Random.value;

            // Agent AI Final State Machine
            if (this.behaviourState == BehaviourState.Idle)
            {
                // transition to non-idle
                if (r1 < NonIdleProbability)
                {
                    // decide where to go
                    Vector3 crtPos = this.transform.position;
                    Vector3 destination = crtPos;
                    bool reachable = false;
                    int numAttempt = 0;
                    while (!reachable && numAttempt++ < MaxPathFindAttempts)
                    {
                        Vector2 rc = UnityEngine.Random.insideUnitCircle;

                        var movement = rc * MaxIdleMoveDistance;
                        destination = crtPos + new Vector3(movement.x, 0, movement.y);

                        NavMeshPath path = new NavMeshPath();
                        reachable = NavMesh.CalculatePath(crtPos, destination, NavMesh.AllAreas, path);
                    }

                    if (reachable)
                    {
                        this.moveBrain.SetDestination(destination);

                        if (r2 < IdleRoamingProbability)
                            this.behaviourState = BehaviourState.IdleRoaming;
                        else
                            this.behaviourState = BehaviourState.IdleAgressive;
                    }
                }
            }
            else if (this.behaviourState == BehaviourState.IdleRoaming)
            {
                if (this.moveBrain.AtDestination())
                    this.behaviourState = BehaviourState.Idle;
            }
            else if (this.behaviourState == BehaviourState.IdleAgressive)
            {
                this.behaviourState = BehaviourState.IdleRoaming; // placeholder
            }
            else if (this.behaviourState == BehaviourState.AttackMoving)
            {

            }
            else if (this.behaviourState == BehaviourState.AttackTargeting)
            {

            }
            else if (this.behaviourState == BehaviourState.AttackEngaging)
            {

            }
            else {
                this.behaviourState = BehaviourState.Idle;
            }
        }

        override public void Act()
        {

        }
    }
}
