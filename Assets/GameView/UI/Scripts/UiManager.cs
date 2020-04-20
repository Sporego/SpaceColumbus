using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Entities;

using Utilities.Events;

namespace UI.Menus
{
    public class AgentUiActive : UiEvent
    {
        public bool ActiveState { get; private set; }

        public AgentUiActive(bool state)
        {
            this.ActiveState = state;
        }
    }

    public class SelectedEntityEvent : UiEvent
    {
        public Entity entity;

        public SelectedEntityEvent(Entity entity) { this.entity = entity; }
    }

    public abstract class UiEvent : GameEvent
    {

    }

    [System.Serializable]
    public struct UiComponent
    {
        public GameObject Obj;
        public bool Active;
    }

    public class UiManager : MonoBehaviour, IEventListener<UiEvent>
    {
        public Canvas MainCanvas;

        //public UiComponent EntityUi;
        public UiComponent AgentVitalsUi;

        public GameObject VitalsMonitoring;
        UiVitalsLog vitalsMenu;

        // Start is called before the first frame update
        void Start()
        {
            vitalsMenu = VitalsMonitoring.GetComponent<UiVitalsLog>();

            OnEvent(new AgentUiActive(true)); // enable, to make sure that it can be disabled
            OnEvent(new AgentUiActive(false)); // disable
        }

        public bool OnEvent(UiEvent gameEvent)
        {
            if (gameEvent is AgentUiActive activeStateEvent)
            {
                if (this.AgentVitalsUi.Active != activeStateEvent.ActiveState)
                {
                    this.AgentVitalsUi.Active = activeStateEvent.ActiveState;
                    this.AgentVitalsUi.Obj.SetActive(activeStateEvent.ActiveState);
                }
            }
            else if (gameEvent is SelectedEntityEvent entityEvent)
            {
                if (entityEvent.entity is Agent agent)
                    this.vitalsMenu.SetObservedAgent(agent);
                else
                    Debug.Log("Selected something that isnt an agent.");
            }

            return true;
        }

    }

}
