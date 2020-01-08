using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using InputControls;
using Pathfinding;
using Regions;

using Utilities.Misc;

[AddComponentMenu("Input-Control")]
public class GameControl : MonoBehaviour
{
    #region Configuration
    [Header("GUI configuration")]
    public bool showGUI = true;
    public int guiMenuWidth = 200;
    public int guiMenuHeight = 300;

    #endregion

    [Header("Indicators")]
    #region SelectionIndicators
    public GameObject mouseOverIndicator;
    public GameObject selectionIndicator;
    #endregion

    #region PrivateVariables

    private GameSession gameSession;

    private KeyPressManager keyPressManager;

    static GUIStyle guiStyle;

    #endregion

    void Start()
    {
        gameSession = (GameSession)GameObject.FindGameObjectWithTag(StaticGameDefs.GameSessionTag).GetComponent(typeof(GameSession));
        keyPressManager = this.GetComponent<KeyPressManager>();


        //keyPressManager.AddKeyPressListener(keyInfo, keyActiveEventListener);

        //AstarPF = new AstarPathFinder(maxDepth: 50, maxCost: 1000, maxIncrementalCost: maxActionPoints);
        //DijsktraPF = new DijkstraPathFinder(maxDepth: maxActionPoints,
        //    maxCost: actionPoints,
        //    maxIncrementalCost: maxActionPoints
        //);

        //doublePressDetector = gameObject.GetComponent<DoubleKeyDetector>();

        // create GUI style
        guiStyle = new GUIStyle();
        guiStyle.alignment = TextAnchor.LowerLeft;
        guiStyle.normal.textColor = Tools.hexToColor("#153870");

        //mouseOverIndicator = Instantiate(mouseOverIndicator, transform);
        //selectionIndicator = Instantiate(selectionIndicator, transform);
    }

    void Update()
    {
        // update selection tile
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo))
        {
            GameObject hitObject = hitInfo.collider.transform.gameObject;
            if (hitObject == null)
            {
                // nothing to do
            }
            else
            {

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