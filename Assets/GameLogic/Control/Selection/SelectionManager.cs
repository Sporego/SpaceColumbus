using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Utilities.Events;

namespace EntitySelection
{
    public class SelectionManager : MonoBehaviour
    {
        // TODO: convert this to Singleton -> another local class with single instance; multiple SM MonoBs will still use the same SM

        #region Config
        public float timeBetweenSelectionUpdates = 0.05f; // in seconds, minimum allowed Period for updating selection
        #endregion Config

        private List<SelectionListener> currentlySelectedListeners { get; set; }
        private List<GameObject> currentlySelectedGameObjects { get; set; }

        private List<SelectionListener> selectionListeners = new List<SelectionListener>();

        private Vector3[] selectionScreenCoords = new Vector3[2];

        private float timeSinceLastSelectionUpdate;

        private GameObject mouseOverObject;

        public void Awake()
        {
            Selectable.selectionManager = this; // set the static def
        }

        public void Start()
        {
            timeSinceLastSelectionUpdate = timeBetweenSelectionUpdates;
            ProcessSelected();
            //SelectionManager.selectionPrefab = GameObject.FindGameObjectWithTag(StaticGameDefs.GameRootTag).GetComponent<PrefabManager>().SelectionPrefab;
        }

        public void AddSelectable(Selectable selectable)
        {
            var selectionListener = selectable.GetComponent<Selectable>().selectionListener;
            this.selectionListeners.Add(selectionListener);
        }

        public List<SelectionListener> GetSelectedListeners() { return this.currentlySelectedListeners; }

        public List<GameObject> GetSelectedObjects() { return this.currentlySelectedGameObjects; }

        public List<GameObject> GetSelectedObjects(SelectionCriteria criteria)
        {
            var objects = this.currentlySelectedGameObjects;

            return this.currentlySelectedGameObjects;
        }

        private void ProcessSelected()
        {
            List<SelectionListener> selectedListeners = new List<SelectionListener>();
            List<GameObject> selectedObjects = new List<GameObject>();

            foreach (var selectionListener in selectionListeners)
            {
                var selectable = selectionListener.selectable;
                if (selectable.isSelected)
                {
                    selectedListeners.Add(selectionListener);
                    selectedObjects.Add(selectionListener.selectable.gameObject);
                }

            }

            this.currentlySelectedListeners = selectedListeners;
            this.currentlySelectedGameObjects = selectedObjects;
        }

        //public void SetDirty(bool dirty) { this.dirty = dirty; }
        //public void SetDirty() { SetDirty(true); }

        public void DeselectAll()
        {
            foreach (var selectionListener in selectionListeners)
            {
                var selectable = selectionListener.selectable;
                if (selectable.isSelected)
                    selectable.Deselect();
            }
        }

        public Selectable[] GetSelectables(GameObject gameObject)
        {
            if (gameObject is null)
                return new Selectable[] { };
            return gameObject.GetComponentsInParent<Selectable>();
        }

        public void Deselect(GameObject gameObject)
        {
            foreach (var selectable in GetSelectables(gameObject))
            {
                if (selectable.isSelected)
                    selectable.Deselect();
            }
        }

        public void Select(GameObject gameObject, SelectionCriteria selectionCriteria=null)
        {
            foreach (var selectable in GetSelectables(gameObject))
            {
                if (!selectable.isSelected && SelectionCriteria.isValidSelection(selectionCriteria, selectable))
                    selectable.Select();
            }
        }

        public void UpdateMouseSelection(GameObject mouseOverObject, SelectionCriteria selectionCriteria)
        {
            if (this.mouseOverObject == mouseOverObject)
                return;

            Deselect(this.mouseOverObject);
            this.mouseOverObject = mouseOverObject;
            Select(this.mouseOverObject, selectionCriteria);
        }

        public void UpdateSelected(Vector3 s1, Vector3 s2, GameObject mouseOverObject, SelectionCriteria selectionCriteria = null)
        {
            Selectable[] selectables = mouseOverObject.GetComponentsInParent<Selectable>();
            foreach (var selectable in selectables)
            {
                if (SelectionCriteria.isValidSelection(selectionCriteria, selectable))
                    selectable.Select();
            }

            if (CheckDirty(s1, s2))
            {
                // update controls vars
                timeSinceLastSelectionUpdate = 0f;

                UpdateBoxSelection(s1, s2, selectionCriteria);
                ProcessSelected();
            }
        }

        public void UpdateBoxSelection(Vector3 s1, Vector3 s2, SelectionCriteria selectionCriteria = null)
        {
            for (int i = 0; i < selectionListeners.Count; i++)
            {
                var selectionListener = selectionListeners[i];
                var selectable = selectionListener.selectable;

                if (selectionCriteria != null && SelectionCriteria.isValidSelection(selectionCriteria, selectable))
                {
                    Vector3 p = Camera.main.WorldToScreenPoint(selectable.transform.position);
                    Vector2 s1p = Vector2.Min(s1, s2);
                    Vector2 s2p = Vector2.Max(s1, s2);
                    bool selected = s1p.x <= p.x && p.x <= s2p.x && s1p.y <= p.y && p.y <= s2p.y;

                    // update and notify only selection changes
                    if (selectable.isSelected != selected)
                    {
                        selectionListener.Notify(new SelectionEvent(selected));
                    }
                }
                else
                {
                    // not valid selection
                }
            }
        }

        private bool CheckDirty(Vector3 s1, Vector3 s2)
        {
            bool dirty = false;

            timeSinceLastSelectionUpdate += Time.deltaTime;

            List<Vector3> selectionScreenCoordsNew = new List<Vector3>() { s1, s2 };
            selectionScreenCoordsNew = selectionScreenCoordsNew.OrderBy(v => v.x).ToList();

            if (selectionScreenCoords[0] != selectionScreenCoordsNew[0] || selectionScreenCoords[1] != selectionScreenCoordsNew[1])
            {
                dirty = true;
                selectionScreenCoords[0] = selectionScreenCoordsNew[0];
                selectionScreenCoords[1] = selectionScreenCoordsNew[1];
            }

            dirty |= timeSinceLastSelectionUpdate >= timeBetweenSelectionUpdates;

            return dirty;
        }
    }
}
