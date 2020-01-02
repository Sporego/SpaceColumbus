using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using Noises;
using HeightMapGenerators;

using Regions;
using SquareRegions;
using Navigation;
using RegionModelGenerators;

using Utilities.Misc;

public class GameSession : MonoBehaviour
{
    [Range(1, 100000000)]
    public int seed = 0;

    public bool useRandomSeed;

    public bool drawGizmos = true;
    [Range(0.001f, 10)]
    public float gizmoSize = 1f;
    [Range(0, 100)]
    public int gizmoSkip = 0;

    public bool regenerate = false;

    public RegionGenConfig regionGenConfig;
    public HeightMapConfig heightMapConfig;
    public FastPerlinNoiseConfig noiseConfig;
    // public ErosionConfig erosionConfig;

    private Region region;

    public void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        Debug.Log("Initializing GameSession");

        this.seed = useRandomSeed ? UnityEngine.Random.Range(int.MinValue, int.MaxValue) : this.seed;

        Debug.Log("Initializing with seed " + this.seed);

        BuildRegion();
        BuildRegionView();
        BuildNavMeshes();
        SpawnSimpleAgent();
    }

    public void BuildRegion()
    {
        this.region = new SquareRegion(this.seed, regionGenConfig, heightMapConfig, noiseConfig); // erosionConfig);
    }

    public void BuildRegionView()
    {
        GameObject viewablesRoot = GameObject.FindGameObjectWithTag(StaticGameDefs.ViewablesTag);

        foreach (Transform t in viewablesRoot.transform)
        {
            GameObject.Destroy(t.gameObject);
        }

        GameObject regionView = new GameObject(StaticGameDefs.RegionViewObjectName);
        regionView.transform.parent = viewablesRoot.transform;

        SquareRegionModelGenerator squareRegionModelGenerator = regionView.AddComponent<RegionModelGenerators.SquareRegionModelGenerator>();
        squareRegionModelGenerator.InitializeMesh(this.region);
    }

    public void BuildNavMeshes()
    {
        GameObject navMeshRoot = GameObject.FindGameObjectWithTag(StaticGameDefs.NavMeshRootTag);
        NavMeshGenerator navMeshGenerator = navMeshRoot.GetComponent<NavMeshGenerator>();
        navMeshGenerator.BuildNavMesh();
    }

    public void SpawnSimpleAgent()
    {
        GameObject agentRoot = GameObject.FindGameObjectWithTag(StaticGameDefs.AgentRootTag);

        GameObject gameRoot = GameObject.FindGameObjectWithTag(StaticGameDefs.GameRootTag);
        GameObject agentPrefab = gameRoot.GetComponent<PrefabManager>().AgentPrefab;

        GameObject agent = GameObject.Instantiate(agentPrefab, agentRoot.transform);

        // set correct height as per game map
        Vector3 pos = agent.transform.position;
        pos.y = this.region.getTileAt(new Vector3()).pos.y;
        agent.transform.position = pos;

        // add nav mesh agent component
        BasicAgent basicAgent = agent.AddComponent<BasicAgent>();
    }

    public Region getRegion()
    {
        return this.region;
    }

    // Use this for initialization
    void Start()
    {
        Debug.Log("Starting game session.");
    }

    public void Update()
    {
        if (regenerate)
        {
            regenerate = false;
            Initialize();
        }
    }

    public Color hexToColor(string hex)
    {
        return Tools.hexToColor(hex);
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos)
        {
            return;
        }

        if (region != null)
        {
            // draw some floor
            Gizmos.color = hexToColor("#000000"); // black
            //Gizmos.DrawCube (new Vector3 (0, 0, 0), new Vector3 (10000, 0, 10000));

            // set color and draw gizmos
            //int water_level = gameSession.mapGenerator.getRegion().getWaterLevelElevation();
            int order = 0;
            Color c;
            foreach (Vector3 pos in region.getTileVertices())
            {
                if (gizmoSkip + 1 != 0)
                {
                    order = ++order % (gizmoSkip + 1);
                    if (order != 0)
                    {
                        continue;
                    }
                }
                //if (tile.getTileType() != null)
                {
                    int elevation = (int)pos.y /*- water_level*/ ;
                    //if (tile.getTileType().GetType() == typeof(WaterTileType))
                    //{
                    //    //Debug.Log("water: elevation " + elevation);
                    //    if (elevation > -5)
                    //    {
                    //        c = hexToColor("#C2D2E7");
                    //    }
                    //    else if (elevation > -10)
                    //    {
                    //        c = hexToColor("#54B3F0");
                    //    }
                    //    else if (elevation > -25)
                    //    {
                    //        c = hexToColor("#067DED");
                    //    }
                    //    else if (elevation > -50)
                    //    {
                    //        c = hexToColor("#005F95");
                    //    }
                    //    else
                    //        c = hexToColor("#004176");
                    //}
                    //else if (tile.getTileType().GetType() == typeof(LandTileType))
                    float heigh_scaling = 0.1f;
                    {
                        //Debug.Log("water: elevation " + elevation);
                        if (elevation < 0)
                            c = hexToColor("#696300");
                        else if (elevation < 10 * heigh_scaling)
                            c = hexToColor("#00C103");
                        else if (elevation < 20 * heigh_scaling)
                            c = hexToColor("#59FF00");
                        else if (elevation < 30 * heigh_scaling)
                            c = hexToColor("#F2FF00");
                        else if (elevation < 40 * heigh_scaling)
                            c = hexToColor("#FFBE00");
                        else if (elevation < 50 * heigh_scaling)
                            c = hexToColor("#FF8C00");
                        else if (elevation < 60 * heigh_scaling)
                            c = hexToColor("#FF6900");
                        else if (elevation < 70 * heigh_scaling)
                            c = hexToColor("#E74900");
                        else if (elevation < 80 * heigh_scaling)
                            c = hexToColor("#E10C00");
                        else if (elevation < 90 * heigh_scaling)
                            c = hexToColor("#971C00");
                        else if (elevation < 100 * heigh_scaling)
                            c = hexToColor("#C24340");
                        else if (elevation < 115 * heigh_scaling)
                            c = hexToColor("#B9818A");
                        else if (elevation < 130 * heigh_scaling)
                            c = hexToColor("#988E8B");
                        else if (elevation < 160 * heigh_scaling)
                            c = hexToColor("#AEB5BD");
                        else // default
                            c = hexToColor("#FFFFFF");
                    }
                    //else
                    //    c = new Color(0, 0, 0, 0);
                    Gizmos.color = c;
                    //if (elevation < 0) {
                    //    pos.y = water_level; // if it's water, draw elevation as equal to water_level
                    //}
                    Gizmos.DrawSphere(pos, gizmoSize);
                }
            }
        }
    }
}