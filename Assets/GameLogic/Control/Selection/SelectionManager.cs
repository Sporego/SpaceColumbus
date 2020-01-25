using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Utilities.Events;

namespace EntitySelection
{
    public class SelectionManager : MonoBehaviour
    {
        #region Config
        public float timeBetweenSelectionUpdates = 0.05f; // in seconds, minimum allowed Period for updating selection
        #endregion Config

        private List<SelectionListener> currentlySelectedListeners { get; set; }
        private List<GameObject> currentlySelectedGameObjects { get; set; }

        private List<SelectionListener> selectionListeners = new List<SelectionListener>();
        private List<bool> isListenerSelected = new List<bool>();

        private bool dirty = true;
        private Vector3[] selectionScreenCoords = new Vector3[2];

        private float timeSinceLastSelectionUpdate;

        public void Start()
        {
            timeSinceLastSelectionUpdate = timeBetweenSelectionUpdates;
            UpdateSelected();
            //SelectionManager.selectionPrefab = GameObject.FindGameObjectWithTag(StaticGameDefs.GameRootTag).GetComponent<PrefabManager>().SelectionPrefab;
        }

        public void AddSelectable(Selectable selectable)
        {
            var selectionListener = selectable.GetComponent<Selectable>().selectionListener;
            this.selectionListeners.Add(selectionListener);
            this.isListenerSelected.Add(false);
        }

        public List<SelectionListener> GetSelectedListeners() { return this.currentlySelectedListeners; }

        public List<GameObject> GetSelectedObjects() { return this.currentlySelectedGameObjects; }

        public List<GameObject> GetSelectedObjects(SelectionCriteria criteria)
        {
            var objects = this.currentlySelectedGameObjects;

            return this.currentlySelectedGameObjects;
        }

        private void UpdateSelected()
        {
            List<SelectionListener> selectedListeners = new List<SelectionListener>();
            List<GameObject> selectedObjects = new List<GameObject>();

            for (int i = 0; i < selectionListeners.Count; i++)
            {
                if (isListenerSelected[i])
                {
                    var listener = selectionListeners[i];
                    selectedListeners.Add(listener);
                    selectedObjects.Add(listener.selectable.gameObject);
                }
            }

            this.currentlySelectedListeners = selectedListeners;
            this.currentlySelectedGameObjects = selectedObjects;
        }

        public void SetDirty(bool dirty) { this.dirty = dirty; }
        public void SetDirty() { SetDirty(true); }

        public void Deselect()
        {
            for (int i = 0; i < selectionListeners.Count; i++)
            {
                if (isListenerSelected[i])
                {
                    selectionListeners[i].Notify(new SelectionEvent(false));
                    isListenerSelected[i] = false;
                }
            }
        }

        public void UpdateSelection(Vector3 s1, Vector3 s2)
        {
            timeSinceLastSelectionUpdate += Time.deltaTime;

            CheckDirty(s1, s2);
            if (dirty)
            {
                // update controls vars
                dirty = false;
                timeSinceLastSelectionUpdate = 0f;

                for (int i = 0; i < selectionListeners.Count; i++)
                {
                    var selectionListener = selectionListeners[i];

                    Vector3 p = Camera.main.WorldToScreenPoint(selectionListener.selectable.transform.position);

                    // check if within selection
                    bool c1 = s1.x <= p.x && p.x <= s2.x;
                    bool c2 = s1.y <= p.y && p.y <= s2.y;
                    bool c3 = s1.x >= p.x && p.x >= s2.x;
                    bool c4 = s1.y >= p.y && p.y >= s2.y;
                    // (c1 && c2) || (c3 && c4) || (c3 && c2) || (c1 && c4) simplifies to
                    bool selected = (c1 || c3) && (c2 || c4);

                    // update and notify only selection changes
                    if (isListenerSelected[i] != selected)
                    {
                        selectionListener.Notify(new SelectionEvent(selected));
                        isListenerSelected[i] = selected;
                    }
                }

                UpdateSelected();
            }
        }

        private void CheckDirty(Vector3 s1, Vector3 s2)
        {
            List<Vector3> selectionScreenCoordsNew = new List<Vector3>() { s1, s2 };
            selectionScreenCoordsNew = selectionScreenCoordsNew.OrderBy(v => v.x).ToList();

            if (selectionScreenCoords[0] != selectionScreenCoordsNew[0] || selectionScreenCoords[1] != selectionScreenCoordsNew[1])
            {
                dirty = true;
                selectionScreenCoords[0] = selectionScreenCoordsNew[0];
                selectionScreenCoords[1] = selectionScreenCoordsNew[1];
            }

            dirty &= timeSinceLastSelectionUpdate >= timeBetweenSelectionUpdates;
        }
    }
}
