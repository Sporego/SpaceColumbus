using System;
using System.Collections.Generic;
using Noises;
using SquareRegions;
using UnityEngine;
using Utilities.Misc;

namespace HeightMapGenerators
{
    [System.Serializable]
    public class HeightMapConfig
    {
        private const float configEpsilon = 1e-3f;

        public int preset = -1;

        [Header("Height Map Post Processing Configuration")]

        // how uniform the overall terrain should be
        [Range(0, 1 - configEpsilon)]
        public float flattenLinearStrength = 0.25f;

        [Range(0, 1 - configEpsilon)]
        public float flattenLowsStrength = 0.5f;

        // how much to suppress high elevations
        [Range(0, 1 - configEpsilon)]
        public float flattenDampenStrength = 0.25f;

        // how much to flatten terrain with logarithmic clamping
        [Range(0, 1f)]
        public float logClampThreshold = 0.5f;

        [Range(configEpsilon, 10f)]
        public float logClampIntensity = 2f;

        // amplifiction parameter for rescaling height values; high amplifiaction pulls small values smaller, and make high values appear higher; exponential stretching
        [Range(1, 10)]
        public float amplification = 5;

        [Header("Craters configuration")]

        [Range(0, 1f)]
        public float craterImportance = 0.075f;

        [Range(0, 100f)]
        public float centralCraterDepth = 25f;

        [Range(0.1f, 1f)]
        public float centralCraterWidth = 0.5f;

        // how many craters to generate on the map
        [Range(1, 1000)]
        public int craterCount = 100;

        [Range(0, 1f)]
        public float craterRandomInfluence = 0.95f;

        [Range(0.001f, 1f)]
        public float craterSize = 0.85f;

        [Range(0.001f, 100f)]
        public float craterDepth = 45f;

        [Header("Map Gen Noise configuration")]

        // height map resolution depends on region size; this scaler can be used to increase the resolution
        [Range(configEpsilon, 10f)]
        public float resolutionScale = 1f;

        // number of discrete values for the noise height map
        [Range(0, 100)]
        public int heightSteps = 5;

        public HeightMapConfig() { }
    }

    [System.Serializable]
    public class ErosionConfig
    {
        public ComputeShader erosionShader;

        public bool applyErosion = true;

        // number of iterations of erosion computations
        [Range(0, 200000)]
        public int iterations = 50;

        // scales the influence of erosion linearly
        [Range(0, 1)]
        public float strength = 1f;

        // amount of water to deposit during erosion simulation: higher means more erosion; acts like a strength parameter; high values smoothen erossion
        [Range(0, 1)]
        public float waterAmount = 0.1f;

        // amount of water to lose per simulation iteration: water[time k + 1] = erosionWaterLoss * water[time k]
        [Range(0, 1)]
        public float waterLoss = 0.99f;

        // limits the influence of elevation difference on terrain movement; acts as maximum elevation difference after which terrain movement won't be affected
        [Range(0, 1)]
        public float waterVelocityElevationDiffRegularizer = 0.2f;

        // if elevation is in range [0-1], water will contribute to elevation in waterAmount / waterToElevationProportion
        // ex: elevation = 0.9, waterAmount = 0.2, combined elevation = 0.9 + 0.2 = 1.1 
        [Range(0, 1)]
        public float waterToElevationProportion = 0.05f;

        [Range(0, 1)]
        public float minTerrainMovementProportion = 0.01f;

        public ErosionConfig()
        {
        }
    }

    public class HeightMap
    {
        private HeightMapConfig config;
        private FastPerlinNoiseConfig noiseConfig;
        private ErosionConfig erosionConfig;

        private Noise noise;

        public int preset { get { return this.config.preset; } }
        public float flattenLinearStrength { get { return this.config.flattenLinearStrength; } }
        public float flattenLowsStrength { get { return this.config.flattenLowsStrength; } }
        public float flattenDampenStrength { get { return this.config.flattenDampenStrength; } }
        public float logClampThreshold { get { return this.config.logClampThreshold; } }
        public float logClampIntensity { get { return this.config.logClampIntensity; } }
        public float amplification { get { return this.config.amplification; } }
        public int heightSteps { get { return this.config.heightSteps; } }
        public float craterImportance{ get { return this.config.craterImportance; } }
        public float centralCraterDepth { get { return this.config.centralCraterDepth; } }
        public float centralCraterWidth { get { return this.config.centralCraterWidth; } }
        public float craterRandomInfluence { get { return this.config.craterRandomInfluence; } }
        public int craterCount { get { return this.config.craterCount; } }
        public float craterSize { get { return this.config.craterSize; } }
        public float craterDepth{ get { return this.config.craterDepth; } }

        public ComputeShader erosionShader { get { return this.erosionConfig.erosionShader; } }
        public int erosionIterations { get { return this.erosionConfig.iterations; } }
        public float erosionStrength { get { return this.erosionConfig.strength; } }
        public float erosionWaterAmount { get { return this.erosionConfig.waterAmount; } }
        public float erosionWaterLoss { get { return this.erosionConfig.waterLoss; } }
        public float erosionWaterVelocityElevationDiffRegularizer { get { return this.erosionConfig.waterVelocityElevationDiffRegularizer; } }
        public float erosionWaterToElevationProportion { get { return this.erosionConfig.waterToElevationProportion; } }
        public float erosionMinTerrainMovementProportion { get { return this.erosionConfig.minTerrainMovementProportion; } }

        public float getNoiseValueUV(float u, float v) { return this.noise.lerpNoiseValue(u, v); }

        public HeightMap(int seed, HeightMapConfig config, FastPerlinNoiseConfig noiseConfig, ErosionConfig erosionConfig)
        {
            this.config = config;
            this.noiseConfig = noiseConfig;
            //this.noise = new ZeroNoiseMap(noiseConfig.resolution, seed);
            this.noise = new FastPerlinNoise(seed, this.noiseConfig);
            this.erosionConfig = erosionConfig;
            modifyNoise();
        }

        public void modifyNoise()
        {
            float[,] elevations = this.noise.getNoiseValues();
            Erosion erosion = new Erosion();
            erosion.erosion = erosionShader;

            if (erosionConfig.applyErosion)
                erosion.Erode(elevations, erosionIterations);

            switch (preset)
            {
                case 0:
                    applyNormalizedHalfSphere(elevations);
                    break;
                case 1:
                    applyLogisticsFunctionToElevations(elevations);
                    break;
                case 2:
                    amplifyElevations(elevations, 2);
                    break;
                case 3:
                    break;
                default:
                    Debug.Log("Default noise map.");

                    createCraters(elevations);

                    amplifyElevations(elevations, amplification);
                    logarithmicClamp(elevations, logClampThreshold, logClampIntensity);
                    dampenElevations(elevations, flattenDampenStrength);
                    flattenLows(elevations);
                    flattenLinearToAverage(elevations);

                    break;
            }

            Tools.normalize(elevations);


            // computeErosion (elevations);

            //Tools.normalize(elevations, maxOnly: false, rescaleSmallMax: false);

            //normalizeToNElevationLevels(elevations, heightSteps);

            this.noise.setNoiseValues(elevations);
        }

        // while generating N craters, want the earlier craters to be smaller than the later
        // this function is a simple heuristic that gives an importance weight to the crater based on its generation number (i.e. first vs last crater)
        public float craterScaleModifier(int craterNum, int power = 5, float minImportance = 0.05f)
        {
            return minImportance + (1f - minImportance) * Mathf.Pow((craterNum + craterCount) / 2f / craterCount, power);
        }

        public float randomCraterDiameter(int craterNum, int craterCount)
        {
            return craterSize * (1f + craterRandomInfluence * (UnityEngine.Random.value * craterScaleModifier(craterNum, craterCount) - 1f));
        }

        public float randomCraterDepth(float craterDiam) {
            return craterDepth * craterDiam * craterImportance * (1f + craterRandomInfluence * (UnityEngine.Random.value - 1));
        }

        public float computeHeightModifier(float elevation, float elevationAtImpact, float val) {
            float heightModifierPower = 1f;
            float heightModifierInfluence = 0;
            float heightModifier = Mathf.Clamp(1f - heightModifierInfluence * Mathf.Pow(Mathf.Abs(elevation - elevationAtImpact) / (val == 0 ? 1: val), heightModifierPower), 0f, 1f) ;
            return heightModifier;
        }

        public void createCraters(float[,] elevations)
        {
            // central landing crater
            generateCrater(elevations, new Vector2(0.5f, 0.5f), centralCraterWidth, centralCraterDepth * craterImportance); ;

            // extra smaller craters
            for (int craterNum = 0; craterNum < craterCount; craterNum++)
            {
                Vector2 rCraterCenterUV = new Vector2(UnityEngine.Random.value, UnityEngine.Random.value);

                float rCraterDiam = randomCraterDiameter(craterNum, craterCount);
                float rCraterDepth = randomCraterDepth(rCraterDiam);

                generateCrater(elevations, rCraterCenterUV, rCraterDiam, rCraterDepth); ;
            }

            Tools.normalize(elevations);
        }

        public void generateCrater(float[,] elevations, Vector2 uvCenter, float craterDiam, float craterDepth, int minRadius = 1)
        {
            //Debug.Log("Generating crater at [" + uvCenter.x + ", " + uvCenter.y + "] with craterDiam " + craterDiam + " craterDepth " + craterDepth);

            int mapSize = elevations.GetLength(0);

            int cI = (int)(mapSize * uvCenter.x);
            int cJ = (int)(mapSize * uvCenter.y);

            int radius = (int)Mathf.Max(craterDiam / 2 * mapSize, minRadius);

            //Debug.Log("Placing at indices [" + cI + ", " + cJ + "] with radius " + radius);

            for (int i = cI - radius; i < cI + radius; i++)
            {
                for (int j = cJ - radius; j < cJ + radius; j++)
                {
                    float val = radius * radius - Mathf.Pow(i - cI, 2) - Mathf.Pow(j - cJ, 2);
                    val = (val > 0) ? Mathf.Sqrt(val) : 0;

                    try
                    {
                        val = val / radius * craterDepth * computeHeightModifier(elevations[i, j], elevations[cI, cJ], val);
                        elevations[i, j] -= val;
                    }
                    catch (IndexOutOfRangeException)
                    {
                        // do nothing if out of range
                    }
                }
            }
        }

        // intensity is how strong the sphere effect is
        // threshold is the maximum value on the sphere that will be applied
        public void applyNormalizedHalfSphere(float[,] elevations, float intensity = 1f, float size = -1, float sphereMaxValue = 1f, bool overwrite = false, bool orientation = false)
        {
            size = (size < 0) ? elevations.GetLength(0) / 2 : elevations.GetLength(0) / 2 * size;
            if (size <= 0)
                return;

            int diameter = (int)size;

            float radius = diameter / 2f;
            float[,] sphere = new float[diameter, diameter];
            for (int i = 0; i < diameter; i++)
            {
                for (int j = 0; j < diameter; j++)
                {
                    float val = Mathf.Pow(radius, 2) - Mathf.Pow(i - radius, 2) - Mathf.Pow(j - radius, 2);
                    val = (val > 0) ? val : 0;
                    sphere[i, j] = Mathf.Sqrt(val);
                }
            }
            for (int i = 0; i < diameter; i++)
            {
                for (int j = 0; j < diameter; j++)
                {
                    float val = Mathf.Clamp(sphere[i, j] / radius, 0, sphereMaxValue);
                    if (!orientation)
                        val = -val;
                    sphere[i, j] = val;
                }
            }

            Tools.mergeArrays(elevations, sphere, 1f, intensity, overwrite);
        }

        private void flattenLinearToAverage(float[,] elevations)
        {
            float[] minMaxAvg = Tools.computeMinMaxAvg(elevations);
            float avg = minMaxAvg[2];

            for (int i = 0; i < elevations.GetLength(0); i++)
            {
                for (int j = 0; j < elevations.GetLength(1); j++)
                {
                    elevations[i, j] += flattenLinearStrength * (avg - elevations[i, j]);
                }
            }
        }

        // flattens low elevations stronger than high elevations; crushes elevations to the min value
        private void flattenLows(float[,] elevations)
        {
            float[] minMaxAvg = Tools.computeMinMaxAvg(elevations);
            float min = minMaxAvg[0];
            float max = minMaxAvg[1];

            for (int i = 0; i < elevations.GetLength(0); i++)
            {
                for (int j = 0; j < elevations.GetLength(1); j++)
                {
                    float distToMin = elevations[i, j] - min;
                    float ratio = distToMin / (max - min);
                    elevations[i, j] += flattenLowsStrength * (min - (1f - ratio) * elevations[i, j]);
                }
            }
        }

        private void normalizeToNElevationLevels(float[,] elevations, int levels)
        {
            if (levels <= 1)
                return;

            for (int i = 0; i < elevations.GetLength(0); i++)
            {
                for (int j = 0; j < elevations.GetLength(1); j++)
                {
                    elevations[i, j] = ((float)Mathf.RoundToInt(elevations[i, j] * (levels - 1)));
                }
            }

            Tools.normalize(elevations);
        }

        // flattens terrain
        private void logarithmicClamp(float[,] elevations, float threshold, float intensity)
        {
            for (int i = 0; i < elevations.GetLength(0); i++)
            {
                for (int j = 0; j < elevations.GetLength(1); j++)
                {
                    if (elevations[i, j] > threshold)
                    {
                        elevations[i, j] = threshold + Mathf.Log(1 + intensity * (elevations[i, j] - threshold)) / intensity;
                    }
                }
            }
        }

        public float logisticsFunction(float value, float growth_rate = 5.0f)
        {
            return (float)(1.0 / (1 + Mathf.Exp(growth_rate / 2 + -growth_rate * value)));
        }

        // flattens the terrain
        public void applyLogisticsFunctionToElevations(float[,] elevations)
        {
            for (int i = 0; i < elevations.GetLength(0); i++)
            {
                for (int j = 0; j < elevations.GetLength(1); j++)
                {
                    elevations[i, j] = logisticsFunction(elevations[i, j]);
                }
            }
        }

        // results in elevation = (amplify_factor * elevation) ^ amplify_factor
        public void amplifyElevations(float[,] elevations, float amplifyFactor, float scaleFactor = 1f)
        {
            Tools.normalize(elevations);

            for (int i = 0; i < elevations.GetLength(0); i++)
            {
                for (int j = 0; j < elevations.GetLength(1); j++)
                {
                    elevations[i, j] = Mathf.Pow(scaleFactor * elevations[i, j], amplifyFactor);
                }
            }

            Tools.normalize(elevations);
        }

        public void dampenElevations(float[,] elevations, float strength = 1f)
        {
            for (int i = 0; i < elevations.GetLength(0); i++)
            {
                for (int j = 0; j < elevations.GetLength(1); j++)
                {
                    elevations[i, j] -= strength * (elevations[i, j] - Mathf.Log(1f + Mathf.Epsilon + elevations[i, j]));
                }
            }
        }

        public void convolutionFilter(float[,] elevations, float[,] weights)
        {
            for (int i = 1; i < elevations.GetLength(0) - 1; i++)
            {
                for (int j = 1; j < elevations.GetLength(1) - 1; j++)
                {
                    for (int ii = -1; ii < 2; ii++)
                    {
                        for (int jj = -1; jj < 2; jj++)
                        {
                            elevations[i, j] += weights[ii + 1, jj + 1] * elevations[i + ii, j + jj];
                        }
                    }
                    if (elevations[i, j] < 0)
                    {
                        elevations[i, j] = 0;
                    }
                    if (elevations[i, j] > 1)
                    {
                        elevations[i, j] = 1;
                    }
                }
            }
        }

        // *** EROSSION COMPUTATIONS *** //

        // private void computeErosion (float[, ] elevations) {
        //     if (!this.erosionConfig.applyErosion)
        //         return;

        //     float[, ] waterVolumes = new float[elevations.GetLength (0), elevations.GetLength (1)];

        //     Debug.Log ("Computing Erosion: elevations size " + elevations.GetLength (0) + " by " + elevations.GetLength (1));

        //     erosionDepositWaterRandom (waterVolumes, erosionWaterAmount, 0.2f, 4);

        //     int numIterations = erosionIterations;
        //     float terrainMovement = float.MaxValue;
        //     while (numIterations-- > 0) {
        //         terrainMovement = computeErosionIteration (elevations, waterVolumes);
        //         Debug.Log ("EROSION: Moved " + terrainMovement + ". Iterations left " + numIterations);

        //         if (terrainMovement < erosionMinTerrainMovementProportion) {
        //             Debug.Log ("Erosion loop finished after " + (erosionIterations - numIterations) + " iterations.");
        //         }
        //     }

        //     this.noise.setNoiseValues (elevations);
        // }

        //     public struct HeightMapTile {
        //         public int i;
        //         public int j;
        //         public float elevation;
        //         public HeightMapTile (int i, int j, float elevation) {
        //             this.i = i;
        //             this.j = j;
        //             this.elevation = elevation;
        //         }
        //     }

        //     private List<HeightMapTile> mapGetNeighborTiles (int i, int j) {
        //         float[, ] elevations = this.noise.getNoiseValues ();

        //         List<HeightMapTile> neighbors = new List<HeightMapTile> ();
        //         foreach (Vector2Int dir in SquareRegion.SquareUtilities.SquareNeighborsNoDiag) {
        //             try {
        //                 int ii = i + dir.x;
        //                 int jj = j + dir.y;
        //                 neighbors.Add (new HeightMapTile (ii, jj, elevations[ii, jj]));
        //             } catch (IndexOutOfRangeException e) {
        //                 // do nothing
        //             }
        //         }

        //         return neighbors;
        //     }

        //     private void checkWater (float[, ] waterVolumes) {
        //         float sum = 0;
        //         for (int i = 0; i < waterVolumes.GetLength (0); i++) {
        //             for (int j = 0; j < waterVolumes.GetLength (1); j++) {
        //                 sum += waterVolumes[i, j];
        //             }
        //         }

        //         Tools.computeMinMaxAvg (waterVolumes);
        //         Debug.Log ("Total water amount = " + sum);
        //     }

        //     private float computeErosionIteration (float[, ] elevations, float[, ] waterVolumes) {
        //         float minWaterThreshold = 1e-3f;
        //         float velocityElevationToProximityRatio = 0.95f;
        //         float velocityProximityInfluence = 0.25f;

        //         checkWater (waterVolumes);

        //         // deep copy of arrays
        //         float[, ] waterUpdated = new float[waterVolumes.GetLength (0), waterVolumes.GetLength (1)];
        //         for (int i = 0; i < waterUpdated.GetLength (0); i++) {
        //             for (int j = 0; j < waterUpdated.GetLength (1); j++) {
        //                 waterUpdated[i, j] = waterVolumes[i, j];
        //             }
        //         }

        //         float totalTerrainMovement = 0;
        //         HeightMapTile current;
        //         for (int i = 0; i < elevations.GetLength (0); i++) {
        //             for (int j = 0; j < elevations.GetLength (1); j++) {
        //                 // do not do anything for small water amounts
        //                 if (waterVolumes[i, j] < minWaterThreshold) {
        //                     //Debug.Log("MIN WATER REACHED");
        //                     continue;
        //                 }

        //                 current = new HeightMapTile (i, j, elevations[i, j]);

        //                 // get tile neighbors
        //                 List<HeightMapTile> neighbors = mapGetNeighborTiles (i, j);

        //                 // sort in ascending order: lowest first
        //                 neighbors.Sort ((x, y) => x.elevation.CompareTo (y.elevation));

        //                 // compute elevation influenced 'velocity' vectors
        //                 List<float> elevationGradientWithWater = new List<float> ();
        //                 //List<float> elevationGradient = new List<float>();
        //                 foreach (HeightMapTile neighbor in neighbors) {
        //                     elevationGradientWithWater.Add ((current.elevation + waterVolumes[current.i, current.j] * erosionWaterToElevationProportion) -
        //                         (neighbor.elevation + waterVolumes[neighbor.i, neighbor.j] * erosionWaterToElevationProportion));
        //                     //elevationGradient.Add(current.elevation - neighbor.elevation);
        //                 }

        //                 float[] waterMovement = new float[neighbors.Count];
        //                 for (int k = 0; k < neighbors.Count; k++) {
        //                     if (elevationGradientWithWater[k] > 0) {
        //                         // get neighbor
        //                         HeightMapTile neighbor = neighbors[k];

        //                         // if no water left move on to next tile
        //                         if (waterUpdated[current.i, current.j] <= 0) {
        //                             waterUpdated[current.i, current.j] = 0;
        //                             break;
        //                         }

        //                         float waterVelocity;
        //                         // velocity from elevation
        //                         waterVelocity = 1f - Mathf.Exp (-Mathf.Abs (elevationGradientWithWater[k]) / erosionWaterVelocityElevationDiffRegularizer); // range [0,1]

        //                         // velocity from proximity
        //                         // do weighted sum based on constants
        //                         waterVelocity = waterVelocity * velocityElevationToProximityRatio + (1f - velocityElevationToProximityRatio) * velocityProximityInfluence;

        //                         float waterLossAmount = waterVelocity * waterUpdated[current.i, current.j];

        //                         //Debug.Log("velocity " + waterVelocity + "; waterLossAmount " + waterLossAmount + " from [" +  current.i + ", " + current.j +
        //                         //    "] to [" + neighbor.i + ", " + neighbor.j + "] using elev diff " + elevationGradient[k]);

        //                         waterMovement[k] = waterLossAmount;
        //                     }
        //                 }

        //                 // rescale water movement to account for movement to all neighbors
        //                 float waterMovementTotal = 0;
        //                 foreach (float f in waterMovement) {
        //                     waterMovementTotal += f;
        //                 }
        //                 if (waterMovementTotal > 0) {
        //                     // rescale
        //                     for (int k = 0; k < neighbors.Count; k++) {
        //                         waterMovement[k] *= waterMovement[k] / waterMovementTotal;
        //                     }

        //                     // check if want to move more water than is available
        //                     waterMovementTotal = 0;
        //                     foreach (float f in waterMovement) {
        //                         waterMovementTotal += f;
        //                     }
        //                     if (waterMovementTotal > waterUpdated[current.i, current.j]) {
        //                         float modifier = waterUpdated[current.i, current.j] / waterMovementTotal;
        //                         for (int k = 0; k < neighbors.Count; k++) {
        //                             waterMovement[k] *= modifier;
        //                         }
        //                     }
        //                 }

        //                 // update water amount and elevations
        //                 for (int k = 0; k < neighbors.Count; k++) {
        //                     HeightMapTile neighbor = neighbors[k];

        //                     // remove water from current
        //                     waterUpdated[current.i, current.j] -= waterMovement[k];

        //                     // add water to neighbor
        //                     waterUpdated[neighbor.i, neighbor.j] += waterMovement[k];

        //                     // compute terrain elevation adjustment
        //                     float terrainMovement = erosionStrength * elevationGradientWithWater[k] * (waterMovement[k] / erosionWaterAmount);

        //                     //Debug.Log("terrainMovement " + terrainMovement + " from [" + current.i + ", " + current.j +
        //                     //    "] to [" + neighbor.i + ", " + neighbor.j + "]");

        //                     // adjust elevations
        //                     elevations[current.i, current.j] -= terrainMovement;
        //                     elevations[neighbor.i, neighbor.j] += terrainMovement;

        //                     totalTerrainMovement += terrainMovement;
        //                 }
        //             }
        //         }

        //         // write back updated water volumes
        //         for (int i = 0; i < waterUpdated.GetLength (0); i++) {
        //             for (int j = 0; j < waterUpdated.GetLength (1); j++) {
        //                 waterVolumes[i, j] = waterUpdated[i, j];
        //             }
        //         }

        //         // simulate drying effect
        //         erosionRemoveWater (waterVolumes, erosionWaterLoss);

        //         return totalTerrainMovement;
        //     }

        //     private void erosionRemoveWater (float[, ] waterVolumes, float erosionWaterLoss) {
        //         for (int i = 0; i < waterVolumes.GetLength (0); i++) {
        //             for (int j = 0; j < waterVolumes.GetLength (1); j++) {
        //                 waterVolumes[i, j] *= erosionWaterLoss;
        //             }
        //         }
        //     }

        //     private void erosionDepositWater (float[, ] waterVolumes, float waterAmount) {
        //         for (int i = 0; i < waterVolumes.GetLength (0); i++) {
        //             for (int j = 0; j < waterVolumes.GetLength (1); j++) {
        //                 waterVolumes[i, j] = waterAmount;
        //             }
        //         }
        //     }

        //     private void erosionDepositWaterRandom (float[, ] waterVolumes, float waterAmount, float probability, int radius) {
        //         probability = Mathf.Clamp (probability, 0f, 1f);

        //         for (int i = 0; i < waterVolumes.GetLength (0); i++) {
        //             for (int j = 0; j < waterVolumes.GetLength (1); j++) {
        //                 waterVolumes[i, j] = 0;
        //             }
        //         }
        //         for (int i = 0; i < waterVolumes.GetLength (0); i++) {
        //             for (int j = 0; j < waterVolumes.GetLength (1); j++) {
        //                 if (UnityEngine.Random.Range (0f, 1f) < probability) {
        //                     for (int ii = i - radius; ii < i + radius; ii++) {
        //                         for (int jj = j - radius; jj < j + radius; jj++) {
        //                             try {
        //                                 waterVolumes[ii, jj] += UnityEngine.Random.Range (waterAmount * probability, waterAmount);
        //                             } catch (NullReferenceException e) {
        //                                 // do nothing
        //                             } catch (IndexOutOfRangeException e) {
        //                                 // do nothing
        //                             }
        //                         }
        //                     }
        //                 }
        //             }
        //         }
        //         for (int i = 0; i < waterVolumes.GetLength (0); i++) {
        //             for (int j = 0; j < waterVolumes.GetLength (1); j++) {
        //                 if (waterVolumes[i, j] > waterAmount) {
        //                     waterVolumes[i, j] = waterAmount;
        //                 }
        //             }
        //         }
        //     }
    }
}