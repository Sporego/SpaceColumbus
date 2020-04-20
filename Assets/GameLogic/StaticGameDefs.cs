using UnityEngine;

using Entities;

public static class StaticGameDefs
{
    public static string GameControlTag = "GameController";
    public static string GameSessionTag = "GameSession";
    public static string MainCameraTag = "MainCamera";
    public static string ControlsRootTag = "ControlsRoot";
    public static string ViewablesTag = "Viewables";
    public static string GameRootTag = "GameRoot";
    public static string NavMeshRootTag = "NavMeshRoot";
    public static string AgentRootTag = "AgentRoot";
    public static string BuildingRootTag = "BuildingRoot";
    public static string NavMeshElementTag = "NavMeshElement";
    public static string UiManagerTag = "UiManager";
    public static string EventSystemTag = "EventSystem";

    public static string RegionViewObjectName = "RegionView";

    public static bool IsAgent(GameObject gameObject) {
        return !(gameObject.GetComponent<Agent>() is null);
    }

    public static bool IsStructure(GameObject gameObject)
    {
        return !(gameObject.GetComponent<Structure>() is null);
    }

    //public static bool IsOwnedByPlayer(GameObject gameObject, Player player)
    //{
    //    return !(gameObject.GetComponent<Building>() is null);
    //}
}
