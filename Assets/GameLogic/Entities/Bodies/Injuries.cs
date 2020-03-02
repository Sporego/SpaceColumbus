using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities.Bodies.Injuries
{
    public interface IWithInjuryState
    {
        EInjuryState GetInjuryState();
    }

    public enum EInjuryState : byte
    {
        None = 0,
        InjuryMinor = 1,
        InjuryMajor = 2,
        //InjuryImpaired,
        InjuryTerminal = 3
    }

    public static class InjuryStates
    {
        public static EInjuryState GetWorstInjuryState(EInjuryState s1, EInjuryState s2)
        {
            int higherInjury = Mathf.Max((int)s1, (int)s2);
            return (EInjuryState)higherInjury;
        }
    }
}