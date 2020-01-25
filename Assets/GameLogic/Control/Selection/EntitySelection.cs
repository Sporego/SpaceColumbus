using UnityEngine;
using System.Collections;

using Utilities.Events;
using Players;

namespace EntitySelection
{
    public class SelectionCriteria
    {
        bool isAgent;
        bool isBuilding;
        bool isControlable;
        OwnershipInfo ownership;

        public SelectionCriteria(bool isAgent, bool isBuilding, bool isControlable, OwnershipInfo ownership)
        {
            this.isAgent = isAgent;
            this.isBuilding = isBuilding;
            this.isControlable = isControlable;
            this.ownership = ownership;
        }

        public bool isValidSelection(Selectable selectable)
        {
            // TODO implement this
            bool valid = true;
            valid &= isAgent == StaticGameDefs.IsAgent(selectable.gameObject);
            valid &= isBuilding == StaticGameDefs.IsBuilding(selectable.gameObject);
            //valid &= isControlable != selectable.gameObject.GetComponent<Owner>();

            return valid;
        }
    }

    public class SelectionEvent : GameEvent
    {
        public bool isSelected;

        public SelectionEvent(bool isSelected)
        {
            this.isSelected = isSelected;
        }
    }

    public class SelectionListener : IEventListener
    {
        public Selectable selectable { get; }

        public SelectionListener(GameObject gameObject)
        {
            this.selectable = gameObject.GetComponent<Selectable>();
        }

        public void Notify(GameEvent gameEvent)
        {
            if (((SelectionEvent)gameEvent).isSelected)
                selectable.Select();
            else
                selectable.Deselect();
        }
    }

}
