using Entities.Bodies.Damages;
using Entities.Bodies.Injuries;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

namespace Entities
{
    [RequireComponent(typeof(NavMeshObstacle), typeof(BoxCollider))]
    public class Structure : Entity
    {
        override public string Name { get { return "Structure"; } }

        override public bool IsDamageable { get { return false; } }

        private void Awake()
        {
            this.entityType = EntityType.Building;
        }

        void Start()
        {
            gameObject.GetComponent<NavMeshObstacle>().size = gameObject.GetComponent<BoxCollider>().size;
        }

        void Update()
        {

        }

        public override EInjuryState GetInjuryState()
        {
            return EInjuryState.None;
        }

        public override void TakeDamage(Damage damage)
        {
            return;
        }
    }
}
