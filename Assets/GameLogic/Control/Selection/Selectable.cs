using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Utilities.Events;

namespace EntitySelection
{
    public class Selectable : MonoBehaviour
    {
        public static SelectionManager selectionManager; // this is set by SelectionManager Awake()

        public SelectionListener selectionListener { get; private set; }

        public GameObject selectionIndicator;

        public bool isSelected { get; private set; }

        private void Start()
        {
            selectionListener = new SelectionListener(this.gameObject);

            isSelected = false;

            selectionIndicator.SetActive(false);

            selectionManager.AddSelectable(this);
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

