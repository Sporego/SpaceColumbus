using System;
using System.Collections.Generic;
using System.Linq;
using Regions;
using UnityEngine;
using Utilities.MeshTools;
using SquareRegions;

public class SquareRegionView : RegionView
{
    public void Start()
    {
        base.Start();
        Debug.Log("Initialized Square Region View.");
    }

    override
    public void InitializeMesh(Region region)
    {
        Debug.Log("Initializing region Mesh...");

        // compute mesh parameters
        Dictionary<Vector3, int> verticesDict = new Dictionary<Vector3, int>();
        List<Vector3> normals = new List<Vector3>();

        List<Vector2> uvs = new List<Vector2>();

        List<int> tris = new List<int>();

        // copy vertice vectors
        Tile[,] tiles = region.getTiles();

        Dictionary<string, bool> trisDict = new Dictionary<string, bool>();

        float yTotal = 0;
        int trisCount = 0;
        int length = tiles.GetLength(0);
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < length; j++)
            {
                if (tiles[i, j] != null)
                {
                    // add 2 triangles per tile
                    // *  *
                    // *  *
                    // bottom right * is the current tile, will generate connections to 3 adjacent tiles

                    Vector2Int ind1, ind2;
                    for (int c = 0; c < 2; c++)
                    {
                        if (c == 0)
                        {
                            ind1 = SquareDirections.Top;
                            ind2 = SquareDirections.TopRight;
                        }
                        else
                        {
                            ind1 = SquareDirections.TopRight;
                            ind2 = SquareDirections.Right;
                        }

                        try
                        {
                            ind1 += new Vector2Int(i, j);
                            ind2 += new Vector2Int(i, j);

                            // compute string key for the triangle
                            int[] triInds = new int[] { (i * length + j), (ind1.x * length + ind1.y), (ind2.x * length + ind2.y) };
                            Array.Sort(triInds);
                            string triStringKey = "";
                            foreach (float f in triInds)
                            {
                                triStringKey += f + "-";
                            }

                            // add triangle only if it wasnt added already
                            if (!trisDict.ContainsKey(triStringKey))
                            {
                                trisDict.Add(triStringKey, true);

                                yTotal += tiles[i, j].coord.y + tiles[ind1.x, ind1.y].coord.y + tiles[ind2.x, ind2.y].coord.y;
                                List<Vector3> verticesLocal = new List<Vector3>();
                                verticesLocal.Add(tiles[i, j].coord.getPos());
                                verticesLocal.Add(tiles[ind1.x, ind1.y].coord.getPos());
                                verticesLocal.Add(tiles[ind2.x, ind2.y].coord.getPos());

                                List<Vector2> uvsLocal = new List<Vector2>();
                                uvsLocal.Add(region.coordUV(tiles[i, j].coord));
                                uvsLocal.Add(region.coordUV(tiles[ind1.x, ind1.y].coord));
                                uvsLocal.Add(region.coordUV(tiles[ind2.x, ind2.y].coord));

                                MeshTriAdder.addTriangle(new Vector3Int(0, 1, 2), verticesLocal, verticesDict, tris, normals, uvsLocal, uvs);
                                trisCount++;
                            }
                        }
                        catch (IndexOutOfRangeException e) { }
                        catch (NullReferenceException e)
                        {
                        }
                    }
                }
            }

            Debug.Log("Tiles with size " + length);
            Debug.Log("Built a mesh with " + verticesDict.Keys.Count + " vertices and " + trisCount + " triangles; total height " + yTotal + ".");

            Mesh mesh = new Mesh();

            mesh.Clear();
            mesh.subMeshCount = 2;

            mesh.SetVertices(verticesDict.Keys.ToList<Vector3>());

            mesh.SetTriangles(tris.ToArray(), 0);

            mesh.SetUVs(0, uvs);

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            // assign back to meshFilter
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            MeshCollider meshCollider = GetComponent<MeshCollider>();

            meshFilter.mesh = mesh;
            meshCollider.sharedMesh = meshFilter.sharedMesh;
        }
    }
}