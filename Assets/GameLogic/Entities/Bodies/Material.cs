using UnityEngine;
using System.Collections;

namespace Entities.Materials
{
    public enum EEntityMaterial
    {
        Flesh,
        Bone,
        Metal,
        Wood,
    }

    public class EntityMaterial : ScriptableObject
    {
        EEntityMaterial material;
        public float durability;
        public float hardness;
        public float restoration;
        public float flamability;

        public EntityMaterial(float durability, float hardness, float restoration, float flamability)
        {
            this.durability = durability;
            this.hardness = hardness;
            this.restoration = restoration;
            this.flamability = flamability;
        }
    }

    public class Flesh : EntityMaterial
    {
        public Flesh() : base(0.3f, 0.25f, 0.5f, 0.75f) { }
    }

    public class Metal : EntityMaterial
    {
        public Metal() : base(0.8f, 0.8f, 0.01f, 0.01f) { }
    }

    public class Wood : EntityMaterial
    {
        public Wood() : base(0.6f, 0.6f, 0.05f, 0.95f) { }
    }
}
