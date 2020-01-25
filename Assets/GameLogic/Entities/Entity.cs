using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

using EntitySelection;
using Brains;

using Players;

namespace Entities
{
    public abstract class EntityComponent
    {
        Entity entity;

        public EntityComponent(Entity entity)
        {
            this.entity = entity;
        }

        public abstract void Start();
        public abstract void ProcessTick();
    }

    public abstract class Entity : MonoBehaviour
    {
        public OwnershipInfo ownershipInfo;

        public bool canMove { get; protected set; }

        public enum EntityType : byte
        {
            Building,
            Agent
        }

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

        
    }
}