using UnityEngine;
using System;
using System.Collections;

using Utilities.Events;
using Players;

namespace EntitySelection
{
    public interface ISelectable
    {
        void Select();
        void Deselect();
    }

    public class SelectionCriteria
    {
        public enum ECondition
        {
            And,
            Or
        }

        bool isAgent;
        bool isBuilding;
        bool isControlable;
        Func<bool, bool, bool> op;
        OwnershipInfo ownership;

        public SelectionCriteria(bool isAgent, bool isBuilding, bool isControlable, ECondition condition, OwnershipInfo ownership)
        {
            this.isAgent = isAgent;
            this.isBuilding = isBuilding;
            this.isControlable = isControlable;
            if (condition == ECondition.And) this.op = (a, b) => a & b; else this.op = (a, b) => a | b;
            this.ownership = ownership;
        }

        public static bool isValidSelection(SelectionCriteria criteria, Selectable selectable)
        {
            if (criteria is null)
                return true;

            bool valid = criteria.isAgent == StaticGameDefs.IsAgent(selectable.gameObject);
            valid = criteria.op(valid, criteria.isBuilding == StaticGameDefs.IsStructure(selectable.gameObject));
            //valid = criteria.op(valid, criteria.isControlable != selectable.gameObject.GetComponent<Owner>());

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

        public bool OnEvent(SelectionEvent selectionEvent)
        {
            if (selectionEvent.isSelected)
                selectable.Select();
            else
                selectable.Deselect();

            return true;
        }
    }

}
