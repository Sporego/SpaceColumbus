using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using UnityEngine;
using UnityEngine.EventSystems;

using Utilities.Misc;
using Utilities.Events;

using InputControls;
using Pathfinding;
using Regions;
using Entities;
using Players;
using EntitySelection;

using UI.Utils;
using UI.Menus;

public class GameControl : MonoBehaviour
{
    #region Configuration
    [Header("GUI configuration")]
    public bool showGUI = true;
    public int guiMenuWidth = 200;
    public int guiMenuHeight = 300;

    public Color guiColor = new Color(0.8f, 0.8f, 0.95f, 0.25f);
    public Color guiBorderColor = new Color(0.8f, 0.8f, 0.95f);
    public int guiBorderWidth = 2;
    #endregion

    private EventSystem eventSystem;

    //[Header("Indicators")]
    //#region SelectionIndicators
    //public GameObject mouseOverIndicator;
    //public GameObject selectionIndicator;
    //#endregion

    #region PrivateVariables

    private GameSession gameSession;
    private KeyPressManager keyPressManager;
    private UiManager uiManager;

    static GUIStyle guiStyle;

    private Vector3 mouseOverWorldPosition;

    #region Unit Selection
    private GameObject mouseOverObject;
    private bool isBoxSelecting, startedBoxSelection;
    private Vector2 mousePositionAtSelectionStart, mousePositionAtSelectionEnd;
    #endregion Unit Selection

    #endregion

    public abstract class ControlListener : KeyActiveEventListener
    {
        protected GameControl gameControl;

        public ControlListener(GameControl gameControl, KeyInfo keyInfo) : base(keyInfo)
        {
            this.gameControl = gameControl;
        }
    }

    public class AgentSpawnerControlListener : ControlListener
    {
        public AgentSpawnerControlListener(GameControl gameControl) : base(gameControl, GameControlsManager.leftClickDownDouble) { }

        override public bool OnEvent(GameEvent gameEvent)
        {
            Debug.Log("Spawning agents!!!");
            gameControl.SpawnAgent();

            return true;
        }
    }

    public class AgentMoveControlListener : ControlListener
    {
        public AgentMoveControlListener(GameControl gameControl) : base(gameControl, GameControlsManager.rightClickDownDouble) { }

        override public bool OnEvent(GameEvent gameEvent)
        {
            Debug.Log("Moving agents!!!");
            gameControl.MoveSelectedAgents();

            return true;
        }
    }

    public class AgentMoveStopListener : ControlListener
    {
        public AgentMoveStopListener(GameControl gameControl) : base(gameControl, GameControlsManager.agentStopHotkey) { }

        override public bool OnEvent(GameEvent gameEvent)
        {
            Debug.Log("Stopping agents!!!");
            gameControl.StopSelectedAgents();

            return true;
        }
    }

    void Start()
    {
        isBoxSelecting = false;

        mouseOverWorldPosition = new Vector3();

        gameSession = (GameSession)GameObject.FindGameObjectWithTag(StaticGameDefs.GameSessionTag).GetComponent(typeof(GameSession));
        keyPressManager = this.GetComponent<KeyPressManager>();

        keyPressManager.AddKeyPressListener(new AgentSpawnerControlListener(this));
        keyPressManager.AddKeyPressListener(new AgentMoveControlListener(this));
        keyPressManager.AddKeyPressListener(new AgentMoveStopListener(this));

        eventSystem = (EventSystem)GameObject.FindGameObjectWithTag(StaticGameDefs.EventSystemTag).GetComponent(typeof(EventSystem));
        uiManager = (UiManager)GameObject.FindGameObjectWithTag(StaticGameDefs.UiManagerTag).GetComponent(typeof(UiManager));

        // create GUI style
        guiStyle = new GUIStyle();
        guiStyle.alignment = TextAnchor.LowerLeft;
        guiStyle.normal.textColor = Tools.hexToColor("#153870");
    }

    public void SpawnAgent()
    {
        if (!IsMouseOverUi())
            gameSession.SpawnSimpleAgent(mouseOverWorldPosition);
    }

    public void MoveSelectedAgents()
    {
        if (!IsMouseOverUi())
            gameSession.MoveSelectedAgents(mouseOverWorldPosition);
    }

    public void StopSelectedAgents()
    {
        gameSession.StopSelectedAgents();
    }

    public bool IsMouseOverUi()
    {
        return eventSystem.IsPointerOverGameObject();
    }

    void GetSelectionArea()
    {
        // If the left mouse button is pressed, save mouse location and begin selection
        if (!IsMouseOverUi() && KeyActiveChecker.isActive(GameControlsManager.leftClickDown))
        {
            startedBoxSelection = true;
            isBoxSelecting = true;
            mousePositionAtSelectionStart = Input.mousePosition;
            SelectionManager.Dirty = true;
        }

        if (isBoxSelecting && KeyActiveChecker.isActive(GameControlsManager.leftClick))
        {
            mousePositionAtSelectionEnd = Input.mousePosition;
        }

        // If the left mouse button is released, end selection
        if (KeyActiveChecker.isActive(GameControlsManager.leftClickUp))
        {
            isBoxSelecting = false;
        }
    }

    void ProcessSelectionArea()
    {
        // placeholder selection
        SelectionCriteria selectionCriteria = new SelectionCriteria(
            true, false, true, 
            SelectionCriteria.ECondition.Or,
            gameSession.CurrentPlayer.ownership.info
            );

        // TODO ADD CRITERIA/SORTING of selected objects

        if (!isBoxSelecting)
        {
            var selectedObjects = SelectionManager.CurrentlySelectedGameObjects;

            try
            {
                if (selectedObjects.Count == 1)
                {
                    var selectedObject = selectedObjects[0];
                    var entity = selectedObject.GetComponent<Entity>();

                    if (entity != null)
                    {
                        uiManager.OnEvent(new SelectedEntityEvent(entity));

                        if (entity is Agent agent)
                        {
                            uiManager.OnEvent(new AgentUiActive(true));
                        }
                    }
                }
                else
                {
                    if (selectedObjects.Count == 0)
                    {
                        SelectionManager.UpdateMouseSelection(mouseOverObject, null);
                    }
                }
            }
            catch (MissingReferenceException e)
            {
                // if object gets destroyed, it may still be referenced here if selection manager doesnt update 'currently selected'
                Debug.Log("Warning: trying to inspect a destroyed object.");
            }

            return;
        }

        if (startedBoxSelection)
        {
            OnDeselectObjects();
        }
        
        SelectionManager.UpdateSelected(mousePositionAtSelectionStart, mousePositionAtSelectionEnd, mouseOverObject, selectionCriteria);
    }

    private void OnDeselectObjects()
    {
        uiManager.OnEvent(new AgentUiActive(false));

        SelectionManager.DeselectAll();

        startedBoxSelection = false;
    }

    private void FixedUpdate()
    {
        ProcessSelectionArea();
    }

    void Update()
    {
        GetSelectionArea();

        // update selection tile
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo))
        {
            mouseOverWorldPosition = hitInfo.point;
            //Debug.Log(mouseWorldPosition);
            GameObject hitObject = hitInfo.collider.transform.gameObject;
            if (hitObject == null)
            {
                // nothing to do
            }
            else
            {
                mouseOverObject = hitObject;
            }
        }

        //// right click
        //bool rightMouseClick = Input.GetMouseButtonDown(1);
        //if (pathfindKeyControl.isActivated())
        //{
        //    ChangeMoveMode();
        //}

        ///*PATHFINDING PART */
        //if (moveMode)
        //{
        //    DijsktraPF.maxDepth = maxActionPoints;
        //    DijsktraPF.maxCost = actionPoints;

        //    // draw move range only 
        //    // TODO optimize this to not recalculate path on every frame
        //    if (firstClickedTile != null)
        //    {
        //        DrawMoveRange();

        //        if (mouseOverTile != null)
        //        {
        //            DrawPathTo(mouseOverTile);
        //        }
        //    }

        //    // *** MOUSE CLICKS CONTROL PART *** //
        //    if (rightMouseClick && mouseOverTile != null)
        //    {
        //        if (selectionOrder)
        //        {
        //            firstClickedTile = mouseOverTile;
        //            // draw path using A* pathfinder (not Dijkstra) for faster performance
        //        }
        //        else
        //        {
        //            secondClickedTile = mouseOverTile;

        //            // check if right clicked same tile twice
        //            if (firstClickedTile.Equals(secondClickedTile))
        //            {
        //                //pathResult = GameControl.gameSession.playerAttemptMove(firstClickedTile, out attemptedMoveMessage, movePlayer: true);
        //                StartCoroutine(displayPath(pathResult));
        //                ChangeMoveMode();
        //            }
        //            else
        //            {
        //                // clicked another tile: overwrite first selection
        //                firstClickedTile = mouseOverTile;

        //                // flip selection order an extra time
        //                selectionOrder = !selectionOrder;
        //            }
        //        }
        //        // flip selection order
        //        selectionOrder = !selectionOrder;
        //    }
        //}
    }



    public static void DrawScreenRectBorder(Rect rect, float thickness, Color color)
    {
        // Top
        OnGuiUtil.DrawScreenRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
        // Left
        OnGuiUtil.DrawScreenRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
        // Right
        OnGuiUtil.DrawScreenRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
        // Bottom
        OnGuiUtil.DrawScreenRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
    }

    void OnGUI()
    {
        if (showGUI)
        {
            if (isBoxSelecting)
            {
                // Create a rect from both mouse positions
                var rect = OnGuiUtil.GetScreenRect(mousePositionAtSelectionStart, Input.mousePosition);
                OnGuiUtil.DrawScreenRect(rect, guiColor);
                OnGuiUtil.DrawScreenRectBorder(rect, guiBorderWidth, guiBorderColor);
            }

            //var selectedObjects = selectionManager.GetSelectedObjects();
            //int count = selectedObjects.Count;
            //if (count > 0)
            //{
            //    StringBuilder info = new StringBuilder();

            //    if (selectedObjects.Count == 1)
            //    {
            //        var selectedObject = selectedObjects[0];
            //        var entity = selectedObject.GetComponent<Entity>();
            //        if (entity != null)
            //        {
            //            info.Append("Entity: " + entity.Name + "\n");

            //            if (entity.IsDamageable)
            //                info.Append("Injury: " + entity.GetDamageState() + "\n");
            //            else
            //                info.Append("Cannot be damaged.\n");

            //            if (entity.GetType() == typeof(Agent))
            //            {
            //                Agent agent = entity as Agent;

            //                info.Append(agent.Body.GetHealthInfo());

            //                //uiManager.Notify(new AgentUiActive(true));
            //                //uiManager.Notify(new AgentChangedEvent(agent));
            //            }
            //        }
            //    }
            //    else
            //    {
            //        info.Append("Selected " + count + " entities.\n");
            //        uiManager.Notify(new AgentUiActive(false));
            //        // TODO: count objects and display how many of each kind
            //    }

            //    //GUI.Box(
            //    //    new Rect(Screen.width - guiMenuWidth, Screen.height - guiMenuHeight, guiMenuWidth, guiMenuHeight),
            //    //    info.ToString()
            //    //    );

            //}
            //else
            //{
            //    //uiManager.Notify(new AgentUiActive(false));
            //}


            //if (selectedTile != null)
            //{
            //    string currentSelection = "Selected " + selectedTile.pos;
            //    GUI.Box(new Rect(Screen.width - guiMenuWidth, Screen.height - guiMenuHeight, guiMenuWidth, guiMenuHeight), currentSelection);
            //}
            //if (firstClickedTile != null)
            //{
            //    string leftSelection = "First tile\n" + firstClickedTile.pos;
            //    GUI.Label(new Rect(0, Screen.height - guiMenuHeight, guiMenuWidth, guiMenuHeight), leftSelection, guiStyle);
            //}
            //if (secondClickedTile != null)
            //{
            //    string rightSelection = "Second tile\n" + secondClickedTile.pos;
            //    GUI.Label(new Rect(0, Screen.height - 2 * guiMenuHeight, guiMenuWidth, guiMenuHeight), rightSelection, guiStyle);
            //}
            //if (pathResult != null)
            //{
            //    string pathInfo = "Path cost:" + pathResult.pathCost;
            //    foreach (Tile tile in pathResult.getTilesOnPathStartFirst())
            //    {
            //        pathInfo += "\n" + tile.index;
            //    }
            //    GUI.Label(new Rect(guiMenuWidth, Screen.height - guiMenuHeight, guiMenuWidth, guiMenuHeight), pathInfo, guiStyle);
            //}
        }
    }
}