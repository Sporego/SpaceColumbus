using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Entities.HealthSystems;

namespace Brains.Attack
{
    public class AttackVariant
    {
        public enum AttackType
        {
            Melee,
            Ranged
        }

        public AttackType attackType;

    }

    [System.Serializable]
    public class AttackBrain
    {
        public AttackVariant attackVariant;

        public AttackBrain()
        {

        }

        public float getAttackRange()
        {
            return 0;
        }

        public GameObject GetAttackTarget()
        {
            return null;
        }
    }
}
