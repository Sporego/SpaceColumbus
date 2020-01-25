using System;
using System.Collections;
using System.Collections.Generic;
using HeightMapGenerators;
using Noises;
using Regions;
using UnityEngine;

namespace SquareRegions
{
    public class SquareRegion : Region
    {
        public RegionGenConfig regionGenConfig;

        public SquareRegion(int seed,
            RegionGenConfig regionGenConfig,
            HeightMapConfig heightMapConfig,
            FastPerlinNoiseConfig noiseConfig
        ) : base(seed)
        {
            // ErosionConfig erosionConfig

            this.regionGenConfig = regionGenConfig;

            // compute required array dimensions
            this.gridRadius = computeGridRadius();

            noiseConfig.resolution = (int)(this.gridRadius * heightMapConfig.resolutionScale) + 1;
            this.heightMap = new HeightMap(seed, heightMapConfig, noiseConfig); //erosionConfig);

            this.tileSize = regionGenConfig.tileSize;

            computeTileCenterCoords();

            computeElevationParameters();

            Debug.Log("Generated square region.");
        }

        private void computeTileCenterCoords()
        {
            Tile[,] coords;

            int arraySize = 2 * gridRadius + 1;
            if (arraySize < 0)
                return;

            coords = new Tile[arraySize, arraySize];

            this.regionSize = tileSize * arraySize * 2;

            // loop over X and Y in hex cube coordinatess
            for (int X = -gridRadius; X <= gridRadius; X++)
            {
                for (int Y = -gridRadius; Y <= gridRadius; Y++)
                {
                    int i = X + gridRadius;
                    int j = Y + gridRadius;

                    Vector2 uv = new Vector2(i / 2f / gridRadius, j / 2f / gridRadius);

                    float y = this.regionGenConfig.maxElevation * this.heightMap.getNoiseValueUV(uv.x, uv.y); // get elevation from Noise 

                    // initialize tile
                    // compute tile pos in unity axis coordinates
                    float x = tileSize * X;
                    float z = tileSize * Y;
                    coords[i, j] = new Tile(new Vector3(x, y, z), i, j);
                }
            }

            this.tiles = coords;
        }

        // *** TILE POSITION COMPUTATIONS AND GETTERS *** //

        // unity coordinate pos to storage array index
        override
        public Tile getTileAt(Vector3 pos)
        {
            Vector2 index = regionWorldCoordToIndex(new Vector2(pos.x, pos.z));

            int i, j;
            i = (int)index.x + this.gridRadius;
            j = (int)index.y + this.gridRadius;

            if (i < 0 || j < 0 || i >= tiles.GetLength(0) || j >= tiles.GetLength(0))
            {
                return null;
            }

            return this.tiles[i, j];
        }

        // unity units coordinates
        override
        public List<Vector2Int> getNeighborDirections()
        {
            return new List<Vector2Int>(SquareDirections.Neighbors);
        }

        // unity coordinate system to square coords
        override
        protected Vector3 regionWorldCoordToIndex(float x, float y)
        {
            float i = (int)(Mathf.Floor(x / this.tileSize + 0.5f));
            float j = (int)(Mathf.Floor(y / this.tileSize + 0.5f));
            return new Vector3(i, j, 0);
        }

        // *** REGION SIZE COMPUTATIONS *** //
        override
        protected int computeGridRadius()
        {
            return (int)(Mathf.Floor(Mathf.Sqrt(this.regionGenConfig.numberOfTiles)) / 2) - 1;
        }
    }

    public static class SquareDirections
    {
        public static List<Vector2Int> Neighbors
        {
            get
            {
                return new List<Vector2Int>() {
                Top,
                TopRight,
                Right,
                BottomRight,
                Bottom,
                BottomLeft,
                Left,
                TopLeft
                };
            }

        }

        public static List<Vector2Int> NeighborsNoDiags
        {
            get
            {
                return new List<Vector2Int>() {
                Top,
                Right,
                Bottom,
                Left,
                };
            }
        }

        public static Vector2Int Top
        {
            get
            {
                return new Vector2Int(-1, 0);
            }
        }
        public static Vector2Int TopRight
        {
            get
            {
                return new Vector2Int(-1, +1);
            }
        }
        public static Vector2Int Right
        {
            get
            {
                return new Vector2Int(0, +1);
            }
        }
        public static Vector2Int BottomRight
        {
            get
            {
                return new Vector2Int(+1, +1);
            }
        }
        public static Vector2Int Bottom
        {
            get
            {
                return new Vector2Int(+1, 0);
            }
        }
        public static Vector2Int BottomLeft
        {
            get
            {
                return new Vector2Int(+1, -1);
            }
        }
        public static Vector2Int Left
        {
            get
            {
                return new Vector2Int(0, -1);
            }
        }
        public static Vector2Int TopLeft
        {
            get
            {
                return new Vector2Int(-1, -1);
            }
        }
    }
}