using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Utilities.Events;

using Common;

namespace EntitySelection
{
    public class Selectable : MonoBehaviour, ISelectable, IIdentifiable
    {
        public SelectionListener selectionListener { get; private set; }

        public GameObject selectionIndicator;

        public bool isSelected { get; private set; }

        private List<ISelectable> UiElements;

        private void Start()
        {
            UiElements = new List<ISelectable>();

            selectionListener = new SelectionListener(this.gameObject);

            isSelected = true;
            Deselect();

            SelectionManager.AddSelectable(this);
        }

        public void Select()
        {
            if (!isSelected)
            {
                selectionIndicator.SetActive(true);
                foreach (var uiElement in UiElements)
                    uiElement.Select();
            }
            isSelected = true;
        }

        public void Deselect()
        {
            if (isSelected)
            {
                selectionIndicator.SetActive(false);
                foreach (var uiElement in UiElements)
                    uiElement.Deselect();
            }
            isSelected = false;
        }

        public void AddUiElement(ISelectable uiElement)
        {
            AddUiElements(new List<ISelectable>() { uiElement });
        }
        
        public void AddUiElements(List<ISelectable> uiElements)
        {
            foreach (var uiElement in uiElements)
                this.UiElements.Add(uiElement);
        }

        public int GetId()
        {
            return this.selectionIndicator.GetInstanceID();
        }

        public void OnDestroy()
        {
            SelectionManager.RemoveSelectable(this);
        }
    }
}

