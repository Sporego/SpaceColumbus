using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Brains.Attack;
using Brains.Movement;

namespace Brains
{
    [System.Serializable]
    public abstract class AgentBrain
    {
        public GameObject entityObject { get; private set; }

        public enum Intelligence
        {
            Primitive,
            Low,
            Moderate,
            Advanced,
        }

        public enum BehaviourState
        {
            Idle,
            IdleRoaming,
            IdleAgressive,
            Moving,
            AttackMoving,
            AttackTargeting,
            AttackEngaging,
        }

        protected MoveBrain moveBrain = null;
        protected AttackBrain attackBrain = null;

        protected Intelligence intelligence;
        protected BehaviourState behaviourState;

        public AgentBrain(GameObject entityObject, MoveBrain moveBrain, AttackBrain attackBrain)
        {
            this.entityObject = entityObject;
            this.moveBrain = moveBrain;
            this.attackBrain = attackBrain;
        }
        public void ProcessTick()
        {
            MakeDecision();
            Act();
        }

        public abstract bool MoveTo(Vector3 destination);

        public abstract void StopMoving();

        protected abstract void MakeDecision();
        protected abstract void Act();
    }
}
