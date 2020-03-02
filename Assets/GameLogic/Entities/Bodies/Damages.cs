using UnityEngine;
using UnityEditor;

using System.Collections.Generic;

namespace Entities.Bodies.Damages
{
    public enum DamageType : byte
    {
        Blunt,
        Piercing,
        Heat,
        Electric,
        Chemical,
        Psychological,
        EMP
    }

    public class Damage
    {
        DamageType damageType;

        public float amount { get; private set; }

        public Damage(DamageType damageType, float damage)
        {
            this.damageType = damageType;
            this.amount = damage;
        }

        public Damage() : this(DamageType.Blunt, 1f) { }
    }

    public class DamageResistance
    {
        Damage damage;

        public DamageResistance(DamageType damageType, float damageResistance)
        {
            this.damage = new Damage(damageType, damageResistance);
        }

        public DamageResistance(Damage damage)
        {
            this.damage = damage;
        }
    }

    public class MultiDamage : Damage
    {
        public List<Damage> damages;

        public MultiDamage(List<Damage> damages)
        {
            this.damages = damages;
        }
    }

}