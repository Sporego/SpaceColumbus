using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Utilities.Events;

using Entities.Materials;
using Entities.Bodies.Damages;

namespace Entities.Bodies.Health
{
    public class HPSystemEvent : GameEvent
    {
        public int HP;

        public HPSystemEvent(int HP) { this.HP = HP; }

        public HPSystemEvent(Damage damage, DamageResistance damageResistance)
        {
            // TODO
            this.HP = 1; 
        }
    }

    public class HPSystem : IEventListener
    {
        private int HP;

        public DamageResistance damageResistance { get; private set; }

        public HPSystem(int HP)
        {
            this.HP = HP;
            this.damageResistance = new DamageResistance(DamageType.Blunt, 0);
        }

        public void Notify(GameEvent gameEvent)
        {
            ApplyDamage(((HPSystemEvent)gameEvent).HP);
        }

        public void ApplyDamage(int HP)
        {
            this.HP -= HP;
        }
    }

    public static class HPSystemFactory
    {
        public static HPSystem GetHPSystem(int HP, EEntityMaterial material)
        {
            DamageResistance damageResistance;
            // TODO
            if (material == EEntityMaterial.Flesh)
            {

            }
            else if (material == EEntityMaterial.Metal)
            {

            }
            else if (material == EEntityMaterial.Wood)
            {

            }
            else
            {
                
            }

            return new HPSystem(HP);
        }
    }
}


