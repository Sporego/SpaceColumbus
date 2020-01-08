using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Brains
{
    public abstract class AgentBrain : MonoBehaviour
    {
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
            AttackMoving,
            AttackTargeting,
            AttackEngaging,
        }

        protected MoveBrain moveBrain = null;
        protected AttackBrain attackBrain = null;

        protected Intelligence intelligence;
        protected BehaviourState behaviourState;

        public void Start()
        {
            moveBrain = this.GetComponent<MoveBrain>();
            attackBrain = this.GetComponent<AttackBrain>();
        }

        public abstract void MakeDecision();
        public abstract void Act();

        public void FixedUpdate()
        {
            MakeDecision();
            Act();
        }
    }

}
