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

        public static bool isValidSelection(SelectionCriteria criteria, Selectable selectable)
        {
            if (criteria is null)
                return true;

            bool valid = true;
            valid &= criteria.isAgent == StaticGameDefs.IsAgent(selectable.gameObject);
            valid &= criteria.isBuilding == StaticGameDefs.IsBuilding(selectable.gameObject);
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

    public class SelectionListener : IEventListener<SelectionEvent>
    {
        public Selectable selectable { get; }

        public SelectionListener(GameObject gameObject)
        {
            this.selectable = gameObject.GetComponent<Selectable>();
        }

        public void Notify(SelectionEvent selectionEvent)
        {
            if (selectionEvent.isSelected)
                selectable.Select();
            else
                selectable.Deselect();
        }
    }

}
