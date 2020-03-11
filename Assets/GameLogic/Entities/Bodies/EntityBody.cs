using System;
using System.Text;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

using Utilities.Misc;

using Entities.Bodies.Damages;
using Entities.Bodies.Injuries;
using Entities.Bodies.Health;

namespace Entities.Bodies
{
    public class BodyPart : INamed, IWithInjuryState, IDamageable
    {
        public HPSystem hpSystem = null;

        public string Name { get; set; }

        public string NameCustom { get; set; }

        public virtual float Size { get; set; }

        public virtual bool IsDamageable { get { return this.hpSystem != null; } }

        public BodyPart(string name, float size=0)
        {
            this.Name = name;
            this.NameCustom = name;
            this.Size = size;
        }

        public BodyPart(string name, float size, HPSystem hpSystem) : this(name, size)
        {
            this.hpSystem = hpSystem;
        }

        public BodyPart(BodyPart bodyPart) : this(
            new string(bodyPart.Name.ToCharArray()),
            bodyPart.Size,
            (bodyPart.hpSystem == null) ? null : new HPSystem(bodyPart.hpSystem)
            )
        {
            this.NameCustom = bodyPart.NameCustom;
        }

        public virtual EInjuryState GetInjuryState()
        {
            return EInjuryState.None;
        }

        public virtual void TakeDamage(Damage damage)
        {
            if (IsDamageable)
                hpSystem.TakeDamage(damage);
        }

        public virtual BodyPart Clone()
        {
            return new BodyPart(this);
        }

        public virtual string GetHealthInfo()
        {
            if (this.hpSystem != null)
                return "HP: " + this.hpSystem.HpAsString;
            else
                return "";
        }
    }

    public class BodyPartContainer : BodyPart
    {
        public List<BodyPart> BodyParts { get; private set; }

        public BodyPartContainer(string name) : base(name)
        {
            this.BodyParts = new List<BodyPart>();
        }

        public BodyPartContainer(string name, float size, HPSystem hpSystem) : base(name, size, hpSystem)
        {
            this.BodyParts = new List<BodyPart>();
        }

        public BodyPartContainer(BodyPart bodyPart) : base(bodyPart)
        {
            this.BodyParts = new List<BodyPart>();
        }

        public BodyPartContainer(BodyPartContainer bodyPartContainer) : this(bodyPartContainer as BodyPart)
        {
            foreach (var bodyPart in bodyPartContainer.BodyParts) 
            {
                AddBodyPart(bodyPart.Clone());
            }
        }

        public void AddBodyPart(BodyPart bodyPart)
        {
            this.BodyParts.Add(bodyPart);
        }

        public void AddBodyParts(List<BodyPart> bodyParts)
        {
            foreach (var bodyPart in bodyParts)
                this.BodyParts.Add(bodyPart);
        }

        override public void TakeDamage(Damage damage)
        {
            if (IsDamageable)
                hpSystem.TakeDamage(damage);

            //TODO: distribute damage of contained bodyparts
            foreach (var bodyPart in BodyParts)
                bodyPart.TakeDamage(damage);
        }

        override public EInjuryState GetInjuryState()
        {
            EInjuryState worstInjury = EInjuryState.None;
            foreach (var bodyPart in this.BodyParts)
                worstInjury = InjuryStates.GetWorstInjuryState(worstInjury, bodyPart.GetInjuryState());
            return worstInjury;
        }

        override public BodyPart Clone()
        {
            return new BodyPartContainer(this);
        }

        override public string GetHealthInfo()
        {
            StringBuilder sb = new StringBuilder();

            if (this.hpSystem != null)
                sb.Append("HP: " + this.hpSystem.HpAsString + "\n");

            foreach (var bodyPart in this.BodyParts)
            {
                sb.Append(bodyPart.NameCustom + " " + bodyPart.GetHealthInfo() + "\n");
            }

            return sb.ToString();
        }
    }

    public class Body : BodyPartContainer, INamed, IWithInjuryState, IDamageable
    {
        override public bool IsDamageable {
            get
            {
                foreach (var bodyPart in BodyParts)
                    if (bodyPart.hpSystem != null)
                        return true;
                return false;
            }
        }

        public List<float> GetBodyPartSizes
        {
            get
            {
                List<float> sizes = new List<float>();
                foreach (var bodyPart in BodyParts)
                {
                    sizes.Add(bodyPart.Size);
                }
                return sizes;
            }
        }

        public Body(string name) : base(name) { }
        public Body() : this("Body") { }
        public static Body HumanoidBody { get { return BodyPartFactory.HumanoidBody; } }

        public Body(Body body) : this(body.Name)
        {
            foreach (var bodyPart in body.BodyParts)
                this.BodyParts.Add(bodyPart.Clone());
        }

        override public EInjuryState GetInjuryState()
        {
            EInjuryState worstInjury = EInjuryState.None;
            foreach (var bodyPart in this.BodyParts)
                worstInjury = InjuryStates.GetWorstInjuryState(worstInjury, bodyPart.GetInjuryState());
            return worstInjury;
        }

        override public void TakeDamage(Damage damage)
        {
            if (this.BodyParts.Count > 0)
            {
                if (this.BodyParts.Count == 1)
                    this.BodyParts[0].TakeDamage(damage);
                else
                {
                    Vector2Int indices = Samplers.SampleFromPdf(UnityEngine.Random.value, this.GetBodyPartSizes, damage.dispersion);

                    foreach (var bodyPart in this.BodyParts.ToArray().Slice(indices.x, indices.y))
                    {
                        bodyPart.TakeDamage(damage);
                    }
                }
            }
            else
                return;
        }

        override public BodyPart Clone()
        {
            return new Body(this);
        }
    }
}

