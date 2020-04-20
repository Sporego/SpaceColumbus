using System.Collections;
using System.Collections.Generic;
using System.Text;

using UnityEngine;
using UnityEngine.AI;

using Brains;
using Brains.Movement;
using Brains.Attack;
using EntitySelection;

using Entities.Bodies;
using Entities.Bodies.Damages;
using Entities.Bodies.Health;

using Utilities.Events;

namespace Entities
{
    public class AgentChangedEvent : EntityChangeEvent
    {
        public AgentChangedEvent(Agent agent) : base(agent) { }
    }

    public class AgentEventGenerator : EventGenerator<AgentChangedEvent>, IEventListener<BodyPartChangedEvent>
    {
        private Agent agent;
        public AgentEventGenerator(Agent agent) : base() { this.agent = agent; }

        public bool OnEvent(BodyPartChangedEvent bodyChangedEvent)
        {
            // TODO: any processing on event

            this.Notify(new AgentChangedEvent(this.agent));

            return true;
        }
    }

    [RequireComponent(
        typeof(Selectable),
        typeof(NavMeshAgent)
     )]
    public class Agent : Entity, IEventGenerator<AgentChangedEvent>
    {
        public Body Body { get; private set; }

        override public string Name { get { return "Agent"; } }

        override public bool IsDamageable { get { return this.Body.IsDamageable; } }
        override public bool IsDamaged { get { return this.Body.IsDamaged; } }

        AgentBrain Brain;

        AgentEventGenerator AgentEventSystem;

        public void Awake()
        {
            this.entityType = EntityType.Agent;
        }

        override public void Start()
        {
            base.Start();

            this.Body = Body.HumanoidBody;

            Debug.Log("Agent with body:\n" + Body.toString());

            var moveBrain = new MoveBrain(this.GetComponent<NavMeshAgent>());
            var attackBrain = new AttackBrain();
            Brain = new AgentBrainModerate(this.gameObject, moveBrain, attackBrain);

            AgentEventSystem = new AgentEventGenerator(this);
            this.Body.AddListener(AgentEventSystem);
        }

        public void MoveTo(Vector3 destination)
        {
            this.Brain.MoveTo(destination);
        }

        public void Stop() {
            this.Brain.StopMoving();
        }

        void FixedUpdate()
        {
            if (UnityEngine.Random.value < 0.005f)
            {
                this.TakeDamage(new Damage(DamageType.Blunt, 5, 0.1f));
            }

            Brain.ProcessTick();
        }

        override public void TakeDamage(Damage damage)
        {
            Body.TakeDamage(damage);
        }

        override public EDamageState GetDamageState()
        {
            return Body.GetDamageState();
        }

        public void AddListener(IEventListener<AgentChangedEvent> eventListener)
        {
            this.AgentEventSystem.AddListener(eventListener);
        }

        public void Notify(AgentChangedEvent gameEvent)
        {
            // not intended to be called
            throw new System.NotImplementedException();
        }
    }
}

