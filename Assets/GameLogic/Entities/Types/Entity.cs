using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

using EntitySelection;
using Brains;

using Entities.Bodies.Damages;
using Entities.Bodies.Injuries;

using Players;

namespace Entities
{
    public interface INamed
    {
        string Name { get; }
    }

    public enum EntityType : byte
    {
        Building,
        Agent
    }

    public abstract class Entity : MonoBehaviour, INamed, IDamageable, IWithInjuryState
    {
        public OwnershipInfo ownershipInfo;

        public abstract string Name { get; }
        public abstract bool IsDamageable { get ; }

        public bool canMove { get; protected set; }

        public EntityType entityType { get; protected set; }

        //public List<EntityComponent> components { get; private set; }

        // Start is called before the first frame update
        void Start()
        {
            canMove = !(this.GetComponent<NavMeshAgent>() is null);

            //foreach (var component in components)
            //    component.Start();
        }

        // Update is called once per frame
        void Update()
        {
            //foreach (var component in components)
            //    component.Update(this);
        }
        
        void FixedUpdate()
        {
            //foreach (var component in components)
            //    component.FixedUpdate(this);
        }

        public abstract void TakeDamage(Damage damage);

        public abstract EInjuryState GetInjuryState();
    }

    //public abstract class EntityComponent
    //{
    //    Entity entity;

    //    public EntityComponent(Entity entity)
    //    {
    //        this.entity = entity;
    //    }

    //    public abstract void Start();
    //    public abstract void ProcessTick();
    //}
}