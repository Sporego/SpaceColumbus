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

[AddComponentMenu("Input-Control")]
public class GameControl : MonoBehaviour
{
    #region Configuration
    [Header("GUI configuration")]
    public bool showGUI = true;
    //public int guiMenuWidth = 200;
    //public int guiMenuHeight = 300;

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
    private bool isSelecting, startedSelection;
    private Vector2 mousePositionAtSelectionStart, mousePositionAtSelectionEnd;
    #endregion Unit Selection

    #endregion

    public class AgentSpawnerControlListener : KeyActiveEventListener
    {
        GameControl gameControl;

        public AgentSpawnerControlListener(GameControl gameControl) : base(GameControlsManager.leftClickDownDouble) { this.gameControl = gameControl; }

        override public void Notify(GameEvent gameEvent)
        {
            Debug.Log("Spawning agents!!!");
            gameControl.SpawnAgent();
        }
    }

    public class AgentMoveControlListener : KeyActiveEventListener
    {
        GameControl gameControl;

        public AgentMoveControlListener(GameControl gameControl) : base(GameControlsManager.rightClickDownDouble) { this.gameControl = gameControl; }

        override public void Notify(GameEvent gameEvent)
        {
            Debug.Log("Moving agents!!!");
            gameControl.MoveSelectedAgents();
        }
    }

    public class AgentMoveStopListener : KeyActiveEventListener
    {
        GameControl gameControl;

        public AgentMoveStopListener(GameControl gameControl) : base(GameControlsManager.agentStopHotkey) { this.gameControl = gameControl; }

        override public void Notify(GameEvent gameEvent)
        {
            Debug.Log("Stopping agents!!!");
            gameControl.StopSelectedAgents();
        }
    }

    void Start()
    {
        isSelecting = false;
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
            startedSelection = true;
            isSelecting = true;
            mousePositionAtSelectionStart = Input.mousePosition;
        }

        if (isSelecting && KeyActiveChecker.isActive(GameControlsManager.leftClick))
        {
            mousePositionAtSelectionEnd = Input.mousePosition;
        }

        // If we let go of the left mouse button, end selection
        if (KeyActiveChecker.isActive(GameControlsManager.leftClickUp))
        {
            isSelecting = false;
        }
    }

    void ProcessSelectionArea()
    {
        if (!isSelecting)
            return;

        if (startedSelection)
        {
            selectionManager.Deselect();
            startedSelection = false;
        }

        //SelectionCriteria selectionCriteria = new SelectionCriteria(true, true, false);

        selectionManager.UpdateSelection(mousePositionAtSelectionStart, mousePositionAtSelectionEnd);
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
                ;
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

            if (isSelecting)
            {
                // Create a rect from both mouse positions
                var rect = UIUtils.GetScreenRect(mousePositionAtSelectionStart, Input.mousePosition);
                UIUtils.DrawScreenRect(rect, guiColor);
                UIUtils.DrawScreenRectBorder(rect, guiBorderWidth, guiBorderColor);
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