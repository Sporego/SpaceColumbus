using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using InputControls;
using Pathfinding;
using Regions;

using Utilities.Misc;
using Utilities.Events;
using EntitySelection;

using Entities;

using Players;

[AddComponentMenu("Input-Control")]
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

    //[Header("Indicators")]
    //#region SelectionIndicators
    //public GameObject mouseOverIndicator;
    //public GameObject selectionIndicator;
    //#endregion

    #region PrivateVariables

    private GameSession gameSession;
    private KeyPressManager keyPressManager;

    static GUIStyle guiStyle;

    private Vector3 mouseOverWorldPosition;

    #region Unit Selection
    private SelectionManager selectionManager;
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

        override public void Notify(GameEvent gameEvent)
        {
            Debug.Log("Spawning agents!!!");
            gameControl.SpawnAgent();
        }
    }

    public class AgentMoveControlListener : ControlListener
    {
        public AgentMoveControlListener(GameControl gameControl) : base(gameControl, GameControlsManager.rightClickDownDouble) { }

        override public void Notify(GameEvent gameEvent)
        {
            Debug.Log("Moving agents!!!");
            gameControl.MoveSelectedAgents();
        }
    }

    public class AgentMoveStopListener : ControlListener
    {
        public AgentMoveStopListener(GameControl gameControl) : base(gameControl, GameControlsManager.agentStopHotkey) { }

        override public void Notify(GameEvent gameEvent)
        {
            Debug.Log("Stopping agents!!!");
            gameControl.StopSelectedAgents();
        }
    }

    void Start()
    {
        isBoxSelecting = false;
        selectionManager = GameObject.FindGameObjectWithTag(StaticGameDefs.SelectionManagerTag).GetComponent<SelectionManager>();

        mouseOverWorldPosition = new Vector3();

        gameSession = (GameSession)GameObject.FindGameObjectWithTag(StaticGameDefs.GameSessionTag).GetComponent(typeof(GameSession));
        keyPressManager = this.GetComponent<KeyPressManager>();

        keyPressManager.AddKeyPressListener(new AgentSpawnerControlListener(this));
        keyPressManager.AddKeyPressListener(new AgentMoveControlListener(this));
        keyPressManager.AddKeyPressListener(new AgentMoveStopListener(this));

        // create GUI style
        guiStyle = new GUIStyle();
        guiStyle.alignment = TextAnchor.LowerLeft;
        guiStyle.normal.textColor = Tools.hexToColor("#153870");
    }

    public void SpawnAgent()
    {
        gameSession.SpawnSimpleAgent(mouseOverWorldPosition);
    }

    public void MoveSelectedAgents()
    {
        gameSession.MoveSelectedAgents(mouseOverWorldPosition);
    }

    public void StopSelectedAgents()
    {
        gameSession.StopSelectedAgents();
    }

    // TODO: implement this function to checked if mouse is over UI elements (don't check selection when over UI elements)
    bool mouseOverGameElements()
    {
        return true;
    }

    void GetSelectionArea()
    {
        if (!mouseOverGameElements())
            return;

        // If we press the left mouse button, save mouse location and begin selection
        if (KeyActiveChecker.isActive(GameControlsManager.leftClickDown))
        {
            startedBoxSelection = true;
            isBoxSelecting = true;
            mousePositionAtSelectionStart = Input.mousePosition;
        }

        if (isBoxSelecting && KeyActiveChecker.isActive(GameControlsManager.leftClick))
        {
            mousePositionAtSelectionEnd = Input.mousePosition;
        }

        // If we let go of the left mouse button, end selection
        if (KeyActiveChecker.isActive(GameControlsManager.leftClickUp))
        {
            isBoxSelecting = false;
        }
    }

    void ProcessSelectionArea()
    {
        // TODO LIST CRITERIA
        SelectionCriteria selectionCriteria = new SelectionCriteria(true, false, true, gameSession.currentPlayer.ownership.info);

        if (!isBoxSelecting)
        {
            if (selectionManager.GetSelectedObjects().Count == 0)
                selectionManager.UpdateMouseSelection(mouseOverObject, null);

            return;
        }

        if (startedBoxSelection)
        {
            selectionManager.DeselectAll();
            startedBoxSelection = false;
        }

        selectionManager.UpdateSelected(mousePositionAtSelectionStart, mousePositionAtSelectionEnd, mouseOverObject, selectionCriteria);
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
        UIUtils.DrawScreenRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
        // Left
        UIUtils.DrawScreenRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
        // Right
        UIUtils.DrawScreenRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
        // Bottom
        UIUtils.DrawScreenRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
    }

    void OnGUI()
    {
        if (showGUI)
        {
            if (isBoxSelecting)
            {
                // Create a rect from both mouse positions
                var rect = UIUtils.GetScreenRect(mousePositionAtSelectionStart, Input.mousePosition);
                UIUtils.DrawScreenRect(rect, guiColor);
                UIUtils.DrawScreenRectBorder(rect, guiBorderWidth, guiBorderColor);
            }

            var selectedObjects = selectionManager.GetSelectedObjects();
            int count = selectedObjects.Count;
            if (count > 0)
            {
                string info = "Selected " + count + ((count == 1) ? " entity.\n" : " entities.\n");

                if (selectedObjects.Count == 1)
                {
                    var selectedObject = selectedObjects[0];
                    var entity = selectedObject.GetComponent<Entity>();
                    if (entity != null)
                    {
                        info += "Entity: " + entity.Name + "\n";

                        if (entity.IsDamageable)
                            info += "Injury: " + entity.GetInjuryState() + "\n";
                        else
                            info += "Cannot be damaged.\n";

                        if (entity.GetType() == typeof(Agent))
                        {
                            Agent agent = entity as Agent;

                            info += agent.Body.GetHealthInfo();
                        }
                    }
                }
                else
                {
                }

                GUI.Box(new Rect(Screen.width - guiMenuWidth, Screen.height - guiMenuHeight, guiMenuWidth, guiMenuHeight), info);

            }


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