using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using Brains;
using Brains.Movement;
using Brains.Attack;
using EntitySelection;

namespace Entities
{
    [RequireComponent(
        typeof(Selectable),
        typeof(NavMeshAgent)
     )]
    public class Agent : Entity
    {
        AgentBrain agentBrain;

        public void Awake()
        {
            this.entityType = EntityType.Agent;
        }

        void Start()
        {
            var moveBrain = new MoveBrain(this.GetComponent<NavMeshAgent>());
            var attackBrain = new AttackBrain();

            agentBrain = new AgentBrainModerate(this.gameObject, moveBrain, attackBrain);
        }

        public void MoveTo(Vector3 destination)
        {
            this.agentBrain.MoveTo(destination);
        }

        public void Stop() {
            this.agentBrain.StopMoving();
        }

        void FixedUpdate()
        {
            agentBrain.ProcessTick();
        }
    }
}

