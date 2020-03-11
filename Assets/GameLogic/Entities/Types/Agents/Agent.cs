using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using Brains;
using Brains.Movement;
using Brains.Attack;
using EntitySelection;

using Entities.Bodies;
using Entities.Bodies.Damages;
using Entities.Bodies.Health;
using Entities.Bodies.Injuries;

namespace Entities
{
    [RequireComponent(
        typeof(Selectable),
        typeof(NavMeshAgent)
     )]
    public class Agent : Entity
    {
        public Body Body { get; private set; }

        override public string Name { get { return "Agent"; } }

        override public bool IsDamageable { get { return this.Body.IsDamageable; } }

        AgentBrain agentBrain;

        public void Awake()
        {
            this.entityType = EntityType.Agent;
        }

        void Start()
        {
            this.Body = Body.HumanoidBody;

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

        override public void TakeDamage(Damage damage)
        {
            Body.TakeDamage(damage);
        }

        override public EInjuryState GetInjuryState()
        {
            return Body.GetInjuryState();
        }
    }
}

