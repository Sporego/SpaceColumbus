using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Utilities.Events;

using Entities.Materials;
using Entities.Bodies.Damages;

using UI.Utils;

namespace Entities.Bodies.Health
{
    public interface IDamageable
    {
        bool IsDamageable { get; }
        bool IsDamaged { get; }
        EDamageState GetDamageState();
        void TakeDamage(Damage damage);
    }

    public enum EDamageState : byte
    {
        None,
        Minor,
        Major,
        Critical,
        Terminal
    }

    public static class DamageStates
    {
        #region XmlDefs

        // names
        public const string NoneDamageStateName = "None";
        public const string MinorDamageStateName = "Minor";
        public const string MajorDamageStateName = "Major";
        public const string CriticalDamageStateName = "Critical";
        public const string TerminalDamageStateName = "Terminal";

        // thresholds are for equal or less than
        public const float MinorDamageStateTreshold = 0.99f;
        public const float MajorDamageStateTreshold = 0.7f;
        public const float CriticalDamageStateTreshold = 0.25f;
        public const float TerminalDamageStateTreshold = 0.05f;

        #endregion XmlDefs

        #region UI
        // TODO: refactor this region out of game logic

        // colors
        public static Color NoneDamageStateColor = new Color(0, 1, 0);
        public static Color MinorDamageStateColor = new Color(0.5f, 1, 0);
        public static Color MajorDamageStateColor = new Color(1, 1, 0);
        public static Color CriticalDamageStateColor = new Color(1, 0.5f, 0);
        public static Color TerminalDamageStateColor = new Color(1, 0, 0);

        public static Color DamageStateToColor(EDamageState damageState)
        {
            switch (damageState)
            {
                case EDamageState.Terminal:
                    return TerminalDamageStateColor;
                case EDamageState.Critical:
                    return CriticalDamageStateColor;
                case EDamageState.Major:
                    return MajorDamageStateColor;
                case EDamageState.Minor:
                    return MinorDamageStateColor;
                default:
                    return NoneDamageStateColor;
            }
        }

        public static string DamageStateToStr(EDamageState damageState)
        {
            switch (damageState)
            {
                case EDamageState.Terminal:
                    return TerminalDamageStateName;
                case EDamageState.Critical:
                    return CriticalDamageStateName;
                case EDamageState.Major:
                    return MajorDamageStateName;
                case EDamageState.Minor:
                    return MinorDamageStateName;
                default:
                    return NoneDamageStateName;
            }
        }

        public static string DamageStateToStrWithColor(EDamageState damageState)
        {
            return RichStrings.WithColor(DamageStateToStr(damageState), DamageStateToColor(damageState));
        }
        #endregion UI

        public static EDamageState GetWorstDamageState(EDamageState s1, EDamageState s2)
        {
            return (EDamageState)Mathf.Max((int)s1, (int)s2);
        }

        public static EDamageState HealthToDamageState(float health)
        {
            health = Mathf.Clamp(health, 0, 1);
            if (health <= TerminalDamageStateTreshold)
                return EDamageState.Terminal;
            else if (health <= CriticalDamageStateTreshold)
                return EDamageState.Critical;
            else if (health <= MajorDamageStateTreshold)
                return EDamageState.Major;
            else if (health <= MinorDamageStateTreshold)
                return EDamageState.Minor;
            else
                return EDamageState.None;
        }
    }

    public class HpSystemChangedEvent : EntityComponentChangedEvent
    {
        public readonly HPSystem hpSystem;
        public readonly List<Damage> damages;
        public readonly float healthDelta;

        public HpSystemChangedEvent(HPSystem hpSystem, List<Damage> damages, float healthDelta)
        {
            this.hpSystem = hpSystem;
            this.damages = damages;
            this.healthDelta = healthDelta;
        }
    }

    public class HPSystem : EventGenerator<HpSystemChangedEvent>, IDamageable
    {
        public bool IsDamageable { get { return true; } }

        public bool IsDamaged { get { return this.Health < 1f; } }

        public int HpBase { get; private set; }

        // always between 0 and 1
        public float Health { get; private set; }

        public int HpCurrent { get { return Mathf.RoundToInt(Health * HpBase); } }
        public int HpPrev { get; private set; }

        public string AsText { get { return "HP: [" + HpCurrent + "/" + HpBase + "]"; } }

        public List<DamageMultiplier> damageMultipliers { get; private set; }

        public HPSystem(int HpBase) : base()
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
            float healthDelta = totalDamage / HpBase;
            this.Health -= healthDelta;
            this.Health = Mathf.Clamp(this.Health, 0, 1);

            Notify(new HpSystemChangedEvent(this, damagesAfterModifier, healthDelta));
        }

        public EDamageState GetDamageState()
        {
            return DamageStates.HealthToDamageState(this.Health);
        }
    }
}
