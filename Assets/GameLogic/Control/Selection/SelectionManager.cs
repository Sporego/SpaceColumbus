using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Utilities.Events;

namespace EntitySelection
{
    public static class SelectionManager
    {
        // TODO: convert this to static class

        #region Config
        public static float TimeBetweenSelectionUpdates = 0.05f; // in seconds, minimum allowed Period for updating selection
        #endregion Config

        public static List<SelectionListener> CurrentlySelectedListeners { get; private set; }
        public static List<GameObject> CurrentlySelectedGameObjects { get; private set; }

        private static Dictionary<int, SelectionListener> SelectionListeners;

        private static Vector3[] SelectionScreenCoords = new Vector3[2];

        private static float TimeSinceLastSelectionUpdate;

        private static GameObject MouseOverObject;

        public static bool Dirty { get; set; }

        public static void Initialize()
        {
            SelectionListeners = new Dictionary<int, SelectionListener>();
            TimeSinceLastSelectionUpdate = TimeBetweenSelectionUpdates;
            ProcessSelected();
        }

        public static void AddSelectable(Selectable selectable)
        {
            int id = selectable.GetId();

            if (!SelectionListeners.ContainsKey(id))
                SelectionListeners.Add(id, selectable.selectionListener);
        }

        public static void RemoveSelectable(Selectable selectable)
        {
            int id = selectable.GetId();

            if (SelectionListeners.ContainsKey(id))
                SelectionListeners.Remove(id);
        }

        //public List<SelectionListener> GetSelectedListeners() { return this.currentlySelectedListeners; }

        public static List<GameObject> GetSelectedObjects() { return CurrentlySelectedGameObjects; }

        //public List<GameObject> GetSelectedObjects(SelectionCriteria criteria)
        //{
        //    return this.currentlySelectedGameObjects;
        //}

        private static void ProcessSelected()
        {
            List<SelectionListener> selectedListeners = new List<SelectionListener>();
            List<GameObject> selectedObjects = new List<GameObject>();

            foreach (var selectionListener in SelectionListeners.Values)
            {
                var selectable = selectionListener.selectable;
                if (selectable.isSelected)
                {
                    selectedListeners.Add(selectionListener);
                    selectedObjects.Add(selectionListener.selectable.gameObject);
                }
            }

            CurrentlySelectedListeners = selectedListeners;
            CurrentlySelectedGameObjects = selectedObjects;
        }

        //public void SetDirty(bool dirty) { this.dirty = dirty; }
        //public void SetDirty() { SetDirty(true); }

        public static void DeselectAll()
        {
            MouseOverObject = null;

            foreach (var selectionListener in SelectionListeners.Values)
            {
                var selectable = selectionListener.selectable;
                if (selectable.isSelected)
                    selectable.Deselect();
            }

            ProcessSelected();
        }

        public static Selectable[] GetSelectables(GameObject gameObject)
        {
            if (gameObject == null)
                return new Selectable[] { };
            return gameObject?.GetComponentsInParent<Selectable>();
        }

        public static void Deselect(GameObject gameObject)
        {
            foreach (var selectable in GetSelectables(gameObject))
            {
                Deselect(selectable);
            }
        }

        public static void Deselect(Selectable selectable)
        {
            if (selectable.isSelected)
                selectable.Deselect();
        }

        public static void Select(Selectable selectable, SelectionCriteria selectionCriteria = null)
        {
            if (!selectable.isSelected && SelectionCriteria.isValidSelection(selectionCriteria, selectable))
                selectable.Select();
        }

        public static void Select(GameObject gameObject, SelectionCriteria selectionCriteria=null)
        {
            foreach (var selectable in GetSelectables(gameObject))
            {
                Select(selectable, selectionCriteria);
            }
        }

        public static void UpdateMouseSelection(GameObject mouseOverObject, SelectionCriteria selectionCriteria)
        {
            // TODO: optimize this
            //if (this.mouseOverObject == mouseOverObject)
            //    return;

            Deselect(MouseOverObject);
            MouseOverObject = mouseOverObject;
            Select(MouseOverObject, selectionCriteria);
        }

        public static void UpdateSelected(Vector3 s1, Vector3 s2, GameObject mouseOverObject, SelectionCriteria selectionCriteria = null)
        {
            if (Dirty || CheckDirty(s1, s2))
            {
                Dirty = false;

                // update controls vars
                TimeSinceLastSelectionUpdate = 0f;

                UpdateBoxSelection(s1, s2, selectionCriteria);
                UpdateMouseSelection(mouseOverObject, selectionCriteria);
                ProcessSelected();
            }
        }

        public static void UpdateBoxSelection(Vector3 s1, Vector3 s2, SelectionCriteria selectionCriteria = null)
        {
            foreach (var selectionListener in SelectionListeners.Values)
            {
                var selectable = selectionListener.selectable;

                if (selectionCriteria != null && SelectionCriteria.isValidSelection(selectionCriteria, selectable))
                {
                    Vector3 p = Camera.main.WorldToScreenPoint(selectable.transform.position);
                    Vector2 s1p = Vector2.Min(s1, s2);
                    Vector2 s2p = Vector2.Max(s1, s2);
                    bool selected = s1p.x <= p.x && p.x <= s2p.x && s1p.y <= p.y && p.y <= s2p.y;

                    // update and notify only selection changes
                    if (selected)
                        Select(selectable); // no need to check selectable again

                    else
                        Deselect(selectable);
                }
                else
                {
                    // not valid selection
                }
            }
        }

        private static bool CheckDirty(Vector3 s1, Vector3 s2)
        {
            bool dirty = false;

            TimeSinceLastSelectionUpdate += Time.deltaTime;

            List<Vector3> selectionScreenCoordsNew = new List<Vector3>() { s1, s2 };
            selectionScreenCoordsNew = selectionScreenCoordsNew.OrderBy(v => v.x).ToList();

            if (SelectionScreenCoords[0] != selectionScreenCoordsNew[0] || SelectionScreenCoords[1] != selectionScreenCoordsNew[1])
            {
                dirty = true;
                SelectionScreenCoords[0] = selectionScreenCoordsNew[0];
                SelectionScreenCoords[1] = selectionScreenCoordsNew[1];
            }

            // harder control
            dirty &= TimeSinceLastSelectionUpdate >= TimeBetweenSelectionUpdates;
            dirty |= TimeSinceLastSelectionUpdate >= TimeBetweenSelectionUpdates;

            return dirty;
        }
    }


}
