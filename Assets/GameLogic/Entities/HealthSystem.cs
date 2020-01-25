using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Utilities.Events;

namespace Entities.HealthSystems
{
    public enum DamageType
    {
        Fire,
        Electric,
        Cutting,
        Blunt,
        Piercing
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

        public Damage() : this(DamageType.Piercing, 0.1f) { }
    }

    public class MultiDamage : Damage
    {
        public List<Damage> damages;

        public MultiDamage() { }
    }


    public class HealthSystemEvent : GameEvent
    {
        public float hpDelta;

        public HealthSystemEvent(float hpDelta) { this.hpDelta = hpDelta; }
    }

    public class HealthSystem : IEventListener
    {
        private float HP;

        public HealthSystem(float HP) { this.HP = HP; }

        public void Notify(GameEvent gameEvent)
        {
            this.HP += ((HealthSystemEvent)gameEvent).hpDelta;
        }
    }
}


