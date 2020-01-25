using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Utilities.Events;

namespace EntitySelection
{
    public class Selectable : MonoBehaviour
    {
        public SelectionListener selectionListener { get; private set; }

        public GameObject selectionIndicator;

        public bool isSelected { get; private set; }

        private void Start()
        {
            selectionListener = new SelectionListener(this.gameObject);

            isSelected = false;

            selectionIndicator.SetActive(false);

            GameObject.FindGameObjectWithTag(StaticGameDefs.SelectionManagerTag).GetComponent<SelectionManager>().AddSelectable(this);
        }

        public void Select()
        {
            if (!isSelected)
                selectionIndicator.SetActive(true);
            isSelected = true;
        }

        public void Deselect()
        {
            if (isSelected)
                selectionIndicator.SetActive(false);
            isSelected = false;
        }
    }
}

