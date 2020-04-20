using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using Common;

using EntitySelection;
using Brains;

using Entities.Bodies.Damages;
using Entities.Bodies.Health;

using Players;

using Utilities.Events;

namespace Entities
{
    public enum EntityType : byte
    {
        Building,
        Agent
    }

    public class EntityChangeEvent : GameEvent
    {
        public Entity entity { get; private set; }

        public EntityChangeEvent(Entity entity) { this.entity = entity; }
    }
    
    public class EntityComponentChangedEvent : GameEvent
    {
        public EntityComponentChangedEvent() { }
    }

    public abstract class Entity : MonoBehaviour, INamed, IDamageable, IIdentifiable
    {
        public OwnershipInfo ownershipInfo;

        public abstract string Name { get; }
        public abstract bool IsDamageable { get ; }
        public abstract bool IsDamaged { get ; }

        public bool CanMove { get; protected set; }

        public EntityType entityType { get; protected set; }

        public virtual void Start()
        {
            EntityManager.RegisterEntity(this);

            CanMove = !(this.GetComponent<NavMeshAgent>() is null);
        }

        public abstract void TakeDamage(Damage damage);

        public abstract EDamageState GetDamageState();

        public int GetId()
        {
            return this.gameObject.GetInstanceID();
        }

        private void OnDestroy()
        {
            EntityManager.UnregisterEntity(this);
        }
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