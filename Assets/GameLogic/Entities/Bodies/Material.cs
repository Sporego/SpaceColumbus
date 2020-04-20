using UnityEngine;
using System;
using System.Collections.Generic;

using Utilities.XmlReader;
using Utilities.Misc;

using Common;

using Entities;
using Entities.Bodies.Damages;


namespace Entities.Materials
{
    // TODO: could/should this be a ScriptableObject?
    public class EntityMaterial : INamed
    {
        #region XmlDefs
        private const string MaterialsXmlPath = "Assets/Defs/materials.xml";

        private const string RootField = "root";

        private const string HardnessField = "Hardness";
        private const string RestorationField = "Restoration";
        private const string FlamabilityField = "Flammability";
        private const string DamageMultipliersField = "DamageMultipliers";

        private static XmlReader MaterialXmlReader = new XmlReader(MaterialsXmlPath);
        #endregion XmlDefs

        public float Hardness { get; private set; }
        public float Restoration { get; private set; }
        public float Flamability { get; private set; }
        public List<DamageMultiplier> DamageMultipliers;

        public string Name { get; private set; }

        private EntityMaterial(string Name)
        {
            this.Name = Name;
            InitializeFromXml();
        }

        private void InitializeFromXml()
        {
            this.Hardness = MaterialXmlReader.getFloat(new List<string>() { RootField, this.Name, HardnessField });
            this.Restoration = MaterialXmlReader.getFloat(new List<string>() { RootField, this.Name, RestorationField });
            this.Flamability = MaterialXmlReader.getFloat(new List<string>() { RootField, this.Name, FlamabilityField });
            InitializeDamageMultipliersFromXml();
        }

        private void InitializeDamageMultipliersFromXml()
        {
            this.DamageMultipliers = new List<DamageMultiplier>();

            foreach (var damageType in Damage.DamageTypes)
            {
                try
                {
                    // try read damage type multipliers from xml file
                    float multiplier = MaterialXmlReader.getFloat(
                        new List<string>() { RootField, this.Name, DamageMultipliersField, Damage.DamageType2Str(damageType) }
                        );
                    this.DamageMultipliers.Add(new DamageMultiplier(damageType, multiplier));
                }
                catch (Exception e)
                {
                    this.DamageMultipliers.Add(new DamageMultiplier(damageType, 1f));
                }
            }
        }

        public static EntityMaterial GetMaterial(string name) { return new EntityMaterial(name); }
        public static EntityMaterial Flesh { get { return new EntityMaterial("Flesh"); } }
        public static EntityMaterial Bone { get { return new EntityMaterial("Bone"); } }
        public static EntityMaterial Steel { get { return new EntityMaterial("Steel"); } }
        public static EntityMaterial Plastic { get { return new EntityMaterial("Plastic"); } }
        public static EntityMaterial Wood { get { return new EntityMaterial("Wood"); } }
        public static EntityMaterial Stone { get { return new EntityMaterial("Stone"); } }
    }
}
