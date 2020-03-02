using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using Utilities.MeshTools;
using Utilities.Misc;

//using Unwrapping;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CrystalGenerator : MonoBehaviour
{
    public static readonly int MinNumSides = 3;

    [Range(1, 10)] public int numCrystals = 5;
    [Range(3, 10)] public int numSides = 5;
    [Range(1, 20)] public int numCenters = 5;
    [Range(0.05f, 10)] public float crystalLength = 0.2f;
    [Range(0.05f, 1)] public float crystalWidth = 0.2f;
    public AnimationCurve crystalDensityCurve = AnimationCurve.Linear(0, 1, 0, 1);
    public AnimationCurve crystalSizeCurve = AnimationCurve.Linear(0, 1, 1, 0);
    [Range(0, 1)] public float randomness = 0.1f;

    public int seed = 0;
    public bool useRandomSeed = false;

    private System.Random randomGen;
    private float NextRandomFloat { get { return (float)randomGen.NextDouble(); } }

    public bool regenerate = true;

    // Start is called before the first frame update
    void Awake()
    {
        Debug.Log("Awaking Crystals");

        randomGen = new System.Random(useRandomSeed ? UnityEngine.Random.Range(0, int.MaxValue): seed);
        GenerateCrystal();
    }

    // Update is called once per frame
    void Update()
    {
        if (regenerate)
        {
            regenerate = false;
            GenerateCrystal();
        }
    }

    //public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    //{
    //    var dir = point - pivot; // get point direction relative to pivot
    //    dir = Quaternion.Euler(angles) * dir; // rotate it
    //    point = dir + pivot; // calculate rotated point
    //    return point; // return it
    //}

    public float evaluateDensityCurve(float ratio)
    {
        float offset = 1f / (numCenters + 2);
        return offset + crystalDensityCurve.Evaluate(ratio) * (1f - 2 * offset);
    }

    public void GenerateCrystal()
    {
        //Debug.Log("Generating crystal");

        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<int> tris = new List<int>();

        for (int lobe = 0; lobe < numCrystals; lobe++)
        {
            // compute lobe parameters given randomness
            int lobeNumCenters = Mathf.Max(1, (int) (numCenters - randomness * numCenters * NextRandomFloat));
            int lobeNumSides = Mathf.Max(MinNumSides, (int) (numCenters - randomness * numCenters * NextRandomFloat));
            float lobeScale = 1f - 0.75f * randomness * NextRandomFloat; // limit maximum randomness on scale
            float lobeWidth = lobeScale * crystalWidth * (1f - randomness + 2f * randomness * NextRandomFloat);
            float lobeLength = lobeScale * crystalLength * (1f - randomness + 2f * randomness * NextRandomFloat);
            float maxRotation = 2f * Mathf.PI / lobeNumSides;

            Vector3 lobeRoot = 0.25f * randomness * new Vector3(-1f + 2f * NextRandomFloat, 0, -1f + 2f * NextRandomFloat);

            //Debug.Log("Generating a crystal lobe " + lobe);

            Vector3 lobeDir = Samplers.sampleRandomCosineHemisphere(NextRandomFloat, NextRandomFloat);
            Quaternion rotation = Quaternion.LookRotation(lobeDir);

            List<Vector3> sides = new List<Vector3>();

            for (int j = 0; j < lobeNumCenters; j++)
            {
                float ratio = lobeNumCenters == 1 ? 0.5f: 1f * j / (lobeNumCenters - 1);

                Vector3 centerPos = lobeRoot + lobeLength * lobeDir * evaluateDensityCurve(ratio);

                for (int side = 0; side < lobeNumSides; side++)
                {
                    float angle = maxRotation * side;

                    Vector3 sideDir = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
                    //todo rotate to crystalDir
                    Vector3 sidePos = centerPos + lobeWidth * sideDir * crystalSizeCurve.Evaluate(ratio);

                    //sidePos = rotation * sidePos;

                    sides.Add(sidePos);
                }
            }
            
            Vector3 crystalTop = lobeRoot + lobeLength * lobeDir;

            //Debug.Log("ROOT: " + lobeRoot);
            //Debug.Log("SIDES: " + sides.Count);
            //foreach (var v in sides)
            //    Debug.Log(v);
            //Debug.Log("TOP: " + crystalTop);

            int index, s1, s2, s3;
            // generate triangles for the sides
            for (int j = 0; j < lobeNumCenters - 1; j++)
            {
                for (int side = 0; side < lobeNumSides; side++)
                {
                    index = j * lobeNumSides + side;

                    s1 = index;
                    bool b = (side + 1) % lobeNumSides == 0;

                    // need to generate 2 triangles per side
                    for (int tri = 0; tri < 2; tri++)
                    {
                        if (tri == 0) // triangle 1
                        {
                            s2 = b ? index + lobeNumSides : index + lobeNumSides;
                            s3 = b ? index + 1 : index + lobeNumSides + 1;
                        }
                        else // triangle 2
                        {
                            s2 = b ? index + 1 : index + lobeNumSides + 1;
                            s3 = b ? index - lobeNumSides + 1 : index + 1;
                        }

                        //Debug.Log("Indices" + s1 + " " + s2 + " " + s3);

                        List<Vector3> verticesLocal = new List<Vector3>() { sides[s1], sides[s2], sides[s3] };
                        MeshTriAdder.addTriangle(new Vector3Int(0, 1, 2), verticesLocal, vertices, tris, false);
                    }
                }
            }

            bool isBottom = true;
            Vector3 v1;
            // bottom and top triangles
            for (int c = 0; c < 2; c++)
            {
                if (isBottom)
                {
                    v1 = lobeRoot;
                    index = 0; // index is now used as an offset for the vertex indices
                }
                else {
                    v1 = crystalTop;
                    index = (lobeNumCenters - 1) * lobeNumSides;
                }

                for (int j = 0; j < lobeNumSides; j++)
                {
                    if (isBottom)
                    {
                        s2 = index + j;
                        s3 = (index + 1 + j) % lobeNumSides;

                    }
                    else
                    {
                        s2 = index + (1 + j) % lobeNumSides;
                        s3 = index + j;
                    }

                    List<Vector3> verticesLocal = new List<Vector3>() { v1, sides[s2], sides[s3] };
                    MeshTriAdder.addTriangle(new Vector3Int(0, 1, 2), verticesLocal, vertices, tris, false);
                }

                isBottom = false;
            }
        }

        Mesh mesh = new Mesh();

        mesh.Clear();
        mesh.subMeshCount = 2;

        mesh.SetVertices(vertices);

        mesh.SetTriangles(tris.ToArray(), 0);

        Vector2[] uvs = Unwrapping.GeneratePerTriangleUV(mesh);
        mesh.SetUVs(0, new List<Vector2>(uvs));

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        // assign back to meshFilter
        MeshFilter meshFilter = GetComponent<MeshFilter>();

        //mesh.Optimize();

        meshFilter.mesh = mesh;
    }
}
