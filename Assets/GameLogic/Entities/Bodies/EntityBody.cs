using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

using Utilities.Misc;

using Entities.Bodies.Damages;
using Entities.Bodies.Injuries;
using Entities.Bodies.Health;

namespace Entities.Bodies
{
    public abstract class Body : Named, IWithInjuryState
    {
        public BodyPartBase bodyPartBase;

        public abstract string Name { get; }

        public abstract EInjuryState GetInjuryState();
    }

    public abstract class BodyPart : Named, IWithInjuryState
    {
        public HPSystem hpSystem;

        public abstract string Name { get; }

        public EInjuryState InjuryState { get; private set; }

        public BodyPart(HPSystem hpSystem)
        {
            this.hpSystem = hpSystem;
        }

        public virtual EInjuryState GetInjuryState()
        {
            return InjuryState;
        }
    }

    // a body part that has other body parts attached to it
    public abstract class BodyPartBase : BodyPart
    {
        public List<BodyPart> bodyParts { get; private set; }

        public BodyPartBase(HPSystem hpSystem) : base(hpSystem)
        {
            bodyParts = new List<BodyPart>();
        }

        public BodyPart AddBodyPart(BodyPart bodyPart)
        {
            this.bodyParts.Add(bodyPart);
            return bodyPart;
        }

        override public EInjuryState GetInjuryState()
        {
            return GetInjuryState(this.bodyParts);
        }

        // new method for getting the overall injury state given all body parts
        public abstract EInjuryState GetInjuryState(List<BodyPart> bodyParts);

        //public EInjuryState GetInjuryState(List<BodyPart> bodyParts)
        //{
        //    EInjuryState globalInjuryState = EInjuryState.None;
        //    foreach (var bodyPart in this.bodyParts)
        //    {
        //        globalInjuryState = InjuryStates.GetWorstInjuryState(globalInjuryState, bodyPart.GetInjuryState());
        //    }
        //    return globalInjuryState;
        //}
    }
}

