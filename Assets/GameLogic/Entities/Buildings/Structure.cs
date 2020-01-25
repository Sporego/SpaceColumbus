using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

namespace Entities
{
    [RequireComponent(typeof(NavMeshObstacle), typeof(BoxCollider))]
    public class Structure : Entity
    {
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
    }

}



