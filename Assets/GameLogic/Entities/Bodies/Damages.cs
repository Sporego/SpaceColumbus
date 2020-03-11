using UnityEngine;
using UnityEditor;

using System.Collections.Generic;

using Utilities.Misc;

using Entities.Materials;

namespace Entities.Bodies.Damages
{
    public interface IDamageable
    {
        bool IsDamageable { get; }
        void TakeDamage(Damage damage);
    }

    public enum DamageType : byte
    {
        None,
        Blunt,
        Slashing,
        Piercing,
        Heat,
        Electric,
        Chemical,
        Psychological,
        EMP,
    }

    public class Damage
    {
        #region XmlDefs
        public const string DamageNoneName = "None";
        public const string DamageBluntName = "Blunt";
        public const string DamageSlashingName = "Slashing";
        public const string DamagePiercingName = "Piercing";
        public const string DamageHeatName = "Heat";
        public const string DamageElectricName = "Electric";
        public const string DamageChemicalName = "Chemical";
        public const string DamagePsychologicalName = "Psychological";
        public const string DamageEMPName = "EMP";
        #endregion XmlDefs

        #region StaticDefs
        private static List<DamageType> _DamageTypes = null;
        public static List<DamageType> DamageTypes
        {
            get
            {
                // this will be built on first call
                if (_DamageTypes is null)
                {
                    _DamageTypes = new List<DamageType>();
                    foreach (DamageType damageType in DamageType.GetValues(typeof(DamageType)))
                    {
                        if (damageType != DamageType.None)
                            _DamageTypes.Add(damageType);
                    }
                }

                return _DamageTypes;
            }
        }

        public static string DamageType2Str(DamageType damageType)
        {
            switch (damageType)
            {
                case DamageType.Blunt:
                    return DamageBluntName;
                case DamageType.Slashing:
                    return DamageSlashingName;
                case DamageType.Piercing:
                    return DamagePiercingName;
                case DamageType.Heat:
                    return DamageHeatName;
                case DamageType.Electric:
                    return DamageElectricName;
                case DamageType.Chemical:
                    return DamageChemicalName;
                case DamageType.Psychological:
                    return DamagePsychologicalName;
                case DamageType.EMP:
                    return DamageEMPName;
                default:
                    return DamageNoneName;
            }
        }

        public static DamageType DamageStr2Type(string damageType)
        {
            switch (damageType)
            {
                case DamageBluntName:
                    return DamageType.Blunt;
                case DamageSlashingName:
                    return DamageType.Slashing;
                case DamagePiercingName:
                    return DamageType.Piercing;
                case DamageHeatName:
                    return DamageType.Heat;
                case DamageElectricName:
                    return DamageType.Electric;
                case DamageChemicalName:
                    return DamageType.Chemical;
                case DamagePsychologicalName:
                    return DamageType.Psychological;
                case DamageEMPName:
                    return DamageType.EMP;
                default:
                    return DamageType.None;
            }
        }

        public static float GetTotalDamage(List<Damage> damages)
        {
            float totalDamage = 0f;
            foreach (var damage in damages)
                totalDamage += damage.amount;
            return totalDamage;
        }

        #endregion StaticDefs

        public DamageType damageType;
        public float amount;
        public float dispersion;

        public Damage(DamageType damageType, float amount, float dispersion=1f)
        {
            this.damageType = damageType;
            this.amount = amount;
            this.dispersion = Mathf.Clamp(dispersion, 0, 1);
        }

        public Damage(Damage damage) : this(damage.damageType, damage.amount, damage.dispersion) { }

        public float GetDamageAmountAfterMultiplier(DamageMultiplier damageMultiplier)
        {
            return GetDamageAmountAfterMultiplier(new List<DamageMultiplier>() { damageMultiplier });
        }

        public float GetDamageAmountAfterMultiplier(List<DamageMultiplier> multipliers)
        {
            float damageAmount = this.amount;
            foreach (var multiplier in multipliers)
                if (this.damageType == multiplier.damageType)
                    damageAmount *= multiplier.amount;
            return damageAmount;
        }

        public Damage SlashingDamage(float amount) { return new Damage(DamageType.Slashing, amount, 0.5f); }
        public Damage PiercingDamage(float amount) { return new Damage(DamageType.Piercing, amount, 0.1f); }
        public Damage BluntDamage(float amount) { return new Damage(DamageType.Blunt, amount, 0.25f); }
        public Damage ChemicalDamage(float amount) { return new Damage(DamageType.Chemical, amount, 0.75f); }
        public Damage ElectricDamage(float amount) { return new Damage(DamageType.Electric, amount, 1f); }
    }

    public class DamageMultiplier : Damage
    {
        // acts as a multiplier to damage
        // less than 1 implies resistance
        // higher than 1 implies weakness
        public DamageMultiplier(DamageType damageType, float amount) : base(damageType, amount) { }
        public DamageMultiplier(DamageMultiplier mult) : base(mult.damageType, mult.amount) { }

        #region StaticFunctions

        // if no weights are given, Multipliers get multiplied out
        // if weights are given, Multipliers are averaged given individual weights
        public static List<DamageMultiplier> Simplify(List<DamageMultiplier> multsIn, List<float> weightsIn = null)
        {
            // copy mults
            List<DamageMultiplier> mults = new List<DamageMultiplier>();
            foreach (var mult in multsIn)
                mults.Add(new DamageMultiplier(mult));

            // copy weights
            List<float> weights = new List<float>();
            if (weightsIn != null)
                weights.AddRange(weightsIn);

            int count = mults.Count;
            int simplified = 0;
            for (int i = 0; i < count - simplified; i++)
            {
                var m1 = mults[i];
                for (int j = 0; j < count - simplified; j++)
                {
                    if (i == j)
                        continue;

                    var m2 = mults[j];

                    if (m1.damageType == m2.damageType)
                    {
                        if (weightsIn == null)
                            m1.amount *= m2.amount;
                        else
                        {
                            m1.amount = m1.amount * weights[i] + m2.amount * weights[j];
                            weights[i] += weights[j];
                            m1.amount /= weights[i];
                            weights.RemoveAt(j);
                        }
                        mults.RemoveAt(j);
                        simplified++;
                    }
                }
            }

            return mults;
        }

        public static Damage GetDamageAfterMultiplier(Damage damage, DamageMultiplier multiplier)
        {
            return GetDamageAfterMultiplier(new List<Damage>() { damage }, multiplier)[0];
        }

        public static Damage GetDamageAfterMultiplier(Damage damage, List<DamageMultiplier> multipliers)
        {
            return GetDamageAfterMultiplier(new List<Damage>() { damage }, multipliers)[0];
        }

        public static List<Damage> GetDamageAfterMultiplier(List<Damage> damages, DamageMultiplier multiplier)
        {
            return GetDamageAfterMultiplier(damages, new List<DamageMultiplier>() { multiplier });
        }

        public static List<Damage> GetDamageAfterMultiplier(List<Damage> damages, List<DamageMultiplier> multipliers)
        {
            List<Damage> damagesAfterMultiplier = new List<Damage>();
            foreach (var damage in damages)
            {
                Damage damageAfterMultiplier = new Damage(damage);
                damage.GetDamageAmountAfterMultiplier(multipliers);

                foreach (var multiplier in multipliers)
                    damageAfterMultiplier.amount = damageAfterMultiplier.GetDamageAmountAfterMultiplier(multiplier);

                damagesAfterMultiplier.Add(damageAfterMultiplier);
            }
            return damagesAfterMultiplier;
        }
        #endregion StaticFunctions
    }
}