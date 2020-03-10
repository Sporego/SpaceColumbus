using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

using Entities.Bodies.Damages;
using Entities.Bodies.Injuries;
using Entities.Bodies.Health;
using System;

namespace Entities.Bodies
{
    public class BodyPart : INamed, IWithInjuryState, IDamageable
    {
        public HPSystem hpSystem = null;

        public string Name { get; set; }

        public string NameCustom { get; set; }

        public float Size { get; set; }

        public bool IsDamageable { get { return this.hpSystem != null; } }

        public BodyPart(string name, float size)
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
            ) { }

        public virtual EInjuryState GetInjuryState()
        {
            throw new System.NotImplementedException();
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
    }

    public class BodyPartContainer : BodyPart
    {
        public List<BodyPart> bodyParts { get; private set; }

        public BodyPartContainer(string name, float size, HPSystem hpSystem) : base(name, size, hpSystem)
        {
            this.bodyParts = new List<BodyPart>();
        }

        public BodyPartContainer(BodyPart container) : base(container)
        {
            this.bodyParts = new List<BodyPart>();
        }

        public void AddBodyPart(BodyPart bodyPart)
        {
            this.bodyParts.Add(bodyPart);
        }

        override public void TakeDamage(Damage damage)
        {
            if (IsDamageable)
                hpSystem.TakeDamage(damage);

            // TODO: distribute damage of contained bodyparts
            //foreach (var bodyPart in bodyParts)
            //    BodyPart.take
        }

        override public BodyPart Clone()
        {
            return new BodyPartContainer(this);
        }
    }

    public class Body : INamed, IWithInjuryState, IDamageable
    {
        List<BodyPart> bodyParts;

        public Body()
        {
            this.Name = "Body";
            this.bodyParts = new List<BodyPart>();
        }

        public Body(string name) : this()
        {
            this.Name = name;
        }

        public Body(Body body) : this(body.Name)
        {
            foreach (var bodyPart in body.bodyParts)
                this.bodyParts.Add(bodyPart.Clone());
        }

        public void AddBodyParts(BodyPart bodyPart)
        {
            AddBodyParts(new List<BodyPart>() { bodyPart });
        }

        public void AddBodyParts(List<BodyPart> bodyParts)
        {
            foreach (var bodyPart in bodyParts)
                this.bodyParts.Add(bodyPart);
        }

        public string Name { get; set; }

        public EInjuryState GetInjuryState()
        {
            throw new System.NotImplementedException();
        }

        public void TakeDamage(Damage damage)
        {
            throw new System.NotImplementedException();
        }
        public Body Clone()
        {
            return new Body(this);
        }

        public static Body HumanoidBody { get { return BodyPartFactory.HumanoidBody; } }
    }
}

