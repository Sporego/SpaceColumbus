using System.Text;
using System.Collections;
using System.Collections.Generic;

using TMPro;

using Entities;
using Entities.Bodies;
using Entities.Bodies.Health;
using Entities.Bodies.Damages;

using Utilities.Events;

using UI.Utils;

namespace UI.Menus
{
    public class UiFieldVitalsLog : UiTwoTextField
    {
        override public void Awake()
        {
            base.Awake();
        }

        public string HpSystemToRichString(HPSystem hpSystem)
        {
            return RichStrings.WithColor("[" + hpSystem.HpCurrent + "/" + hpSystem.HpBase + "]", DamageStates.DamageStateToColor(hpSystem.GetDamageState()));
        }

        public void Initialize(BodyPart bodyPart)
        {
            this.TextLeft.Text = bodyPart.NameCustom;
            this.TextRight.Text = HpSystemToRichString(bodyPart.hpSystem); // assume BodyPart has hpSystem

            TriggerUpdateLayoutSize();
        }
    }
}