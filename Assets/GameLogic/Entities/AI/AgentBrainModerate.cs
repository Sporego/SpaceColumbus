using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using Brains.Movement;
using Brains.Attack;

namespace Brains
{
    [System.Serializable]
    public class AgentBrainModerate : AgentBrain
    {
        [Range(0, 1f)] public float NonIdleProbability = 0.005f;
        [Range(0, 1f)] public float IdleRoamingProbability = 0.5f;

        [Range(2f, 10f)] public float MaxIdleMoveMultiplier = 2f;
        [Range(0f, 100f)] public float MaxIdleMoveDistance = 10f;

        private static int MaxPathFindAttempts = 10;

        // Start is called before the first frame update
        public AgentBrainModerate(GameObject entityObject, MoveBrain moveBrain, AttackBrain attackBrain) : base(entityObject, moveBrain, attackBrain)
        {
            this.intelligence = Intelligence.Moderate;
            this.behaviourState = BehaviourState.Idle;
        }

        override protected void MakeDecision()
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
                    Vector3 crtPos = this.entityObject.transform.position;
                    bool success = false;
                    int numAttempt = 0;
                    while (!success && numAttempt++ < MaxPathFindAttempts)
                    {
                        var movement = UnityEngine.Random.insideUnitCircle * MaxIdleMoveDistance;
                        Vector3 destination = crtPos + new Vector3(movement.x, 0, movement.y);

                        success = this.moveBrain.SetDestination(destination);
                    }

                    this.behaviourState = BehaviourState.IdleRoaming;
                }
            }
            else if (this.behaviourState == BehaviourState.Moving)
            {
                if (this.moveBrain.AtDestination())
                    this.behaviourState = BehaviourState.Idle;
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
            else
            {
                this.behaviourState = BehaviourState.Idle;
            }
        }

        override public bool MoveTo(Vector3 destination)
        {
            bool success = this.moveBrain.SetDestination(destination);
            if (success)
                this.behaviourState = BehaviourState.Moving;
            return success;
        }

        override public void StopMoving()
        {
            this.moveBrain.StopMoving();
            this.behaviourState = BehaviourState.Idle;
        }

        override protected void Act()
        {

        }
    }
}
