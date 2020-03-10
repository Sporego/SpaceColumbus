using UnityEngine;

using System;
using System.Collections.Generic;

using Utilities.Misc;
using Utilities.XmlReader;

using Entities.Bodies.Health;
using Entities.Bodies.Damages;
using Entities.Materials;

namespace Entities.Bodies
{
    public static class BodyPartFactory
    {
        #region XmlDefs

        private const string BodiesXmlPath = "Assets/Defs/bodyparts.xml";

        private const string RootField = "root";
        private const string ItemField = "item";
        private const string BaseStatsField = "BaseStats";
        private const string InclusionField = "Inclusion";
        private const string IsContainerField = "isContainer";
        private const string HpField = "hp";
        private const string SizeField = "size";
        private const string MaterialsField = "mat";
        private const string MaterialsNameField = "name";
        private const string MaterialsSizeField = "size";

        private static XmlReader BodyPartXmlReader = new XmlReader(BodiesXmlPath);
        #endregion XmlDefs

        #region BodiesGeneration
        public const string HumanoidBodyName = "Humanoid";
        public const string TorsoName = "Torso";
        public const string HeadName = "Head";

        // bodies
        public static Body HumanoidBody { get { return GetBody(HumanoidBodyName); } }

        // body parts
        public static BodyPart HumanoidTorso { get { return GetBodyPart(HumanoidBodyName, TorsoName); } }
        public static BodyPart HumanoidHead { get { return GetBodyPart(HumanoidBodyName, HeadName); } }
        #endregion BodiesGeneration

        public static Dictionary<string, Body> AvailableBodies = new Dictionary<string, Body>();
        public static Dictionary<string, Dictionary<string, BodyPart>> AvailableBodyParts = new Dictionary<string, Dictionary<string, BodyPart>>();
        public static bool IsInitialized = false;

        public static Body GetBody(string variant)
        {
            if (!IsInitialized)
                ReadBodyPartsFromXml();

            try
            {
                var body = AvailableBodies[variant];
                return body.Clone();
            }
            catch (KeyNotFoundException e)
            {
                LoggerDebug.LogE("Could not find body: " + variant);
                return null;
            }
        }


        public static BodyPart GetBodyPart(string variant, string name)
        {
            if (!IsInitialized)
                ReadBodyPartsFromXml();

            try
            {
                var bodyPart = AvailableBodyParts[variant][name];
                return bodyPart.Clone();
            }
            catch (KeyNotFoundException e)
            {
                LoggerDebug.LogE("Could not find body part: " + variant + "/" + name);
                return null;
            }
        }

        public static void ReadBodyPartsFromXml()
        {
            IsInitialized = true;

            AvailableBodyParts = new Dictionary<string, Dictionary<string, BodyPart>>();

            // STEP 1:
            // read names only
            List<string> bodyVariantsNames = BodyPartXmlReader.getChildren(new List<string>() { RootField, BaseStatsField });
            List<List<string>> bodyPartNamesPerVariant = new List<List<string>>();
            int countVariants = 0;
            foreach (var variantName in bodyVariantsNames)
            {
                List<string> bodyPartNames = BodyPartXmlReader.getChildren(new List<string>() { RootField, BaseStatsField, variantName });

                bodyPartNamesPerVariant.Add(new List<string>());
                foreach (var bodyPartName in bodyPartNames)
                {
                    //Console.WriteLine("Adding bodyPartName " + bodyPartName);
                    bodyPartNamesPerVariant[countVariants].Add(bodyPartName);
                }

                countVariants++;
            }

            // STEP 2:
            // setup BodyPart Dictionary
            for (int i = 0; i < bodyVariantsNames.Count; i++)
            {
                var variantName = bodyVariantsNames[i];

                // check if variant exists
                if (!AvailableBodyParts.ContainsKey(variantName))
                    AvailableBodyParts[variantName] = new Dictionary<string, BodyPart>();

                //Console.WriteLine("variantName: " + variantName);

                foreach (var bodyPartName in bodyPartNamesPerVariant[i])
                {
                    bool isContainer = BodyPartXmlReader.getFloat(new List<string>() { RootField, BaseStatsField, variantName, bodyPartName, IsContainerField }) == 1;
                    float hp = BodyPartXmlReader.getFloat(new List<string>() { RootField, BaseStatsField, variantName, bodyPartName, HpField });
                    float size = BodyPartXmlReader.getFloat(new List<string>() { RootField, BaseStatsField, variantName, bodyPartName, SizeField });

                    List<string> materialNames = BodyPartXmlReader.getStrings(
                        new List<string>() { RootField, BaseStatsField, variantName, bodyPartName, MaterialsField, ItemField, MaterialsNameField });
                    List<string> materialWeights = BodyPartXmlReader.getStrings(
                        new List<string>() { RootField, BaseStatsField, variantName, bodyPartName, MaterialsField, ItemField, MaterialsSizeField });

                    List<DamageMultiplier> multipliers = new List<DamageMultiplier>();
                    List<float> weights = new List<float>();
                    for (int j = 0; j < materialNames.Count; j++)
                    {
                        var materialName = materialNames[j];
                        float materialWeight = float.Parse(materialWeights[j]);
                        try
                        {
                            foreach (var mult in EntityMaterial.GetMaterial(materialName).DamageMultipliers)
                            {
                                multipliers.Add(mult);
                                weights.Add(materialWeight);
                            }
                        }
                        catch (Exception e) { /* do nothing */ }
                    }

                    multipliers = DamageMultiplier.Simplify(multipliers, weights);

                    HPSystem hpSystem = new HPSystem((int)hp, multipliers);

                    BodyPart bodyPart;
                    if (isContainer)
                    {
                        bodyPart = new BodyPartContainer(bodyPartName, size, hpSystem);
                    }
                    else
                    {
                        bodyPart = new BodyPart(bodyPartName, size, hpSystem);
                    }

                    // store bodypart
                    AvailableBodyParts[variantName][bodyPartName] = bodyPart;
                }
            }
            
            // STEP 3:
            // build bodies and add parts for containers
            List<string> variantNames = BodyPartXmlReader.getChildren(new List<string>() { RootField, InclusionField });
            foreach (var variantName in variantNames)
            {
                Body body = new Body(variantName);

                List<string> containerNames = BodyPartXmlReader.getChildren(new List<string>() { RootField, InclusionField, variantName });

                foreach (var bodyPartName in containerNames)
                {
                    //Debug.Log("INCLUSION FOR " + variantName + " " + bodyPartName);

                    var bodyPart = AvailableBodyParts[variantName][bodyPartName];

                    BodyPartContainer container;
                    if (bodyPart.GetType() == typeof(BodyPartContainer))
                    {
                        container = (BodyPartContainer)bodyPart;
                    }
                    else
                    {
                        container = new BodyPartContainer(bodyPart);
                    }

                    List<string> partsList = BodyPartXmlReader.getChildren(
                        new List<string>() { RootField, InclusionField, variantName, bodyPartName });

                    foreach (var partName in partsList)
                    {
                        List<string> customNames = BodyPartXmlReader.getStrings(
                            new List<string>() { RootField, InclusionField, variantName, bodyPartName, partName, ItemField });

                        if (customNames.Count == 0)
                            container.AddBodyPart(AvailableBodyParts[variantName][partName].Clone());
                        foreach (var customName in customNames)
                        {
                            BodyPart bp = AvailableBodyParts[variantName][partName].Clone();
                            bp.NameCustom = customName;
                            container.AddBodyPart(bp);
                        }
                    }

                    // write back
                    // TODO: check if this is necessary
                    AvailableBodyParts[variantName][bodyPartName] = container;

                    body.AddBodyParts(container);
                }

                AvailableBodies[variantName] = body;
            }
        }
    }
}
