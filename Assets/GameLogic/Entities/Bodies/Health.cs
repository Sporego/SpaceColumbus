using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Utilities.Events;

using Entities.Materials;
using Entities.Bodies.Damages;

namespace Entities.Bodies.Health
{
    public class HpSystemEvent : GameEvent
    {
        public readonly HPSystem hpSystem;
        public readonly List<Damage> damages;

        public HpSystemEvent(HPSystem hpSystem, List<Damage> damages)
        {
            this.hpSystem = hpSystem;
            this.damages = damages;
        }
    }

    public class HpSystemObserver : IEventListener<HpSystemEvent>
    {
        public void Notify(HpSystemEvent hpSystemEvent)
        {
            // TODO: update UI, etc

            StringBuilder sb = new StringBuilder();
            sb.Append("HpSystemEvent: " + hpSystemEvent.hpSystem.HpPrev + "->" + hpSystemEvent.hpSystem.HpCurrent + "HP:");
            foreach (var damage in hpSystemEvent.damages)
                sb.Append("\t" + Damage.DamageType2Str(damage.damageType) + " damage with " + damage.amount + " total amount;");
            Debug.Log("HpSystemEvent: " + sb.ToString());
        }
    }

    public class HPSystem : EventGenerator<HpSystemEvent>, IDamageable
    {
        private int HpBase;

        // always between 0 and 1
        public float Health { get; private set; }

        public int HpCurrent { get { return Mathf.RoundToInt(Health * HpBase); } }
        public int HpPrev { get; private set; }

        public List<DamageMultiplier> damageMultipliers { get; private set; }

        public HPSystem(int HpBase)
        {
            this.Health = 1f;
            this.HpBase = HpBase;
            this.HpPrev = HpBase;
            this.damageMultipliers = new List<DamageMultiplier>();
        }

        public HPSystem(int HpBase, List<DamageMultiplier> damageMultipliers) : this(HpBase)
        {
            foreach (var mult in damageMultipliers)
                this.damageMultipliers.Add(new DamageMultiplier(mult));
        }

        public HPSystem(int HpBase, DamageMultiplier damageMultiplier) : this(HpBase, new List<DamageMultiplier>() { damageMultiplier }) { }


        public HPSystem(HPSystem hpSystem) : this(hpSystem.HpBase, hpSystem.damageMultipliers)
        {
            this.HpPrev = hpSystem.HpPrev;
            this.Health = hpSystem.Health;
        }

        public void TakeDamage(Damage damage)
        {
            TakeDamage(new List<Damage>() { damage });
        }

        public void TakeDamage(List<Damage> damages)
        {
            this.HpPrev = HpCurrent;
            List<Damage> damagesAfterModifier = DamageMultiplier.GetDamageAfterMultiplier(damages, damageMultipliers);
            float totalDamage = Damage.GetTotalDamage(damagesAfterModifier);
            this.Health -= totalDamage / HpBase;
            OnEvent(new HpSystemEvent(this, damagesAfterModifier));
        }
    }
}


