using System;
using System.Collections.Generic;
using HeightMapGenerators;
using UnityEngine;

namespace Regions
{
    public class Tile
    {
        public Vector3 pos;
        public int i;
        public int j;

        public Vector2Int index { get { return new Vector2Int(this.i, this.j); } }

        public Tile(Vector3 pos, int i, int j)
        {
            this.pos = pos;
            this.i = i;
            this.j = j;
        }

        public bool Equals(Tile tile) { return this.i == tile.i && this.j == tile.j; }
    }

    [System.Serializable] // for unity editor
    public class RegionGenConfig
    {
        private const int maxNumberOfTiles = 64000; // slightly less than ~2^16 -> unity's mesh vertex count limitation 

        [Range(1, maxNumberOfTiles)]
        public int numberOfTiles;

        [Range(0.001f, 100f)]
        public float tileSize;

        [Range(1, 250)]
        public int maxElevation;

        public RegionGenConfig() { }
    }

    public abstract class Region
    {
        protected int seed;

        protected HeightMap heightMap;

        protected float tileSize;
        protected int gridRadius;

        protected float regionSize;
        protected float center;

        //protected float waterLevelElevation;
        protected float minElevation, maxElevation, avgElevation;

        public Tile[,] tiles;
        protected abstract int computeGridRadius();

        protected Vector3 regionWorldCoordToIndex(Vector2 pos) { return regionWorldCoordToIndex(pos.x, pos.y); }
        protected Vector3 regionWorldCoordToIndex(Vector3 pos) { return regionWorldCoordToIndex(pos.x, pos.z); }
        protected abstract Vector3 regionWorldCoordToIndex(float x, float y);

        public abstract Tile getTileAt(Vector3 pos);
        public abstract List<Vector2Int> getNeighborDirections();

        public Region(int seed)
        {
            this.seed = seed;
        }

        public Tile[,] getTiles()
        {
            return this.tiles;
        }

        // unity units coordinates
        public List<Tile> getTileNeighbors(Vector3 tilePos)
        {
            return getTileNeighbors(regionWorldCoordToIndex(tilePos));
        }

        // unity units coordinates
        public List<Tile> getTileNeighbors(Vector2Int tileIndex)
        {
            return getTileNeighbors(tileIndex.x, tileIndex.y);
        }

        public float distanceBetweenTiles(Tile tile1, Tile tile2)
        {
            return (tile1.pos - tile2.pos).magnitude;
        }

        // array index coordinates
        public List<Tile> getTileNeighbors(int i, int j)
        {
            List<Tile> neighbors = new List<Tile>();
            foreach (Vector2Int dir in this.getNeighborDirections())
            {
                try
                {
                    Tile neighbor = this.tiles[i + dir.x, j + dir.y];
                    if (neighbor != null)
                        neighbors.Add(neighbor);
                }
                catch (IndexOutOfRangeException e)
                {
                    // nothing to do
                }
            }
            return neighbors;
        }

        public Vector2 pos2UV(Vector3 pos)
        {
            float u = pos.x / regionSize + 1f / 2f;
            float v = pos.z / regionSize + 1f / 2f;
            return new Vector2(u, v);
        }

        // *** ELEVATION PARAMETERS COMPUTATIONS *** //
        protected void computeElevationParameters()
        {
            this.minElevation = this.computeMinimumElevation();
            this.maxElevation = this.computeMaximumElevation();
            this.avgElevation = this.computeAverageElevation();
        }

        protected float computeAverageElevation()
        {
            double sum = 0;
            List<Vector3> positions = getTileVertices();
            foreach (Vector3 pos in positions)
            {
                sum += pos.y;
            }
            return (float)(sum / (positions.Count));
        }

        protected float computeMaximumElevation()
        {
            float max = -float.MaxValue;
            List<Vector3> positions = getTileVertices();
            foreach (Vector3 pos in positions)
            {
                if (max < pos.y)
                {
                    max = pos.y;
                }
            }
            return max;
        }

        protected float computeMinimumElevation()
        {
            float min = float.MaxValue;
            List<Vector3> positions = getTileVertices();
            foreach (Vector3 pos in positions)
            {
                if (min > pos.y)
                {
                    min = pos.y;
                }
            }
            return min;
        }

        // *** GETTERS AND SETTERS *** //

        public List<Vector3> getTileVertices()
        {
            List<Vector3> tilesList = new List<Vector3>();

            int length = tiles.GetLength(0);
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    if (tiles[i, j] != null)
                        tilesList.Add(tiles[i, j].pos);
                }
            }

            return tilesList;
        }
        //public int getMinimumElevation()
        //{
        //    return this.minElevation;
        //}
        //public int getMaximumElevation()
        //{
        //    return this.maxElevation;
        //}
        //public int getAverageElevation()
        //{
        //    return this.averageElevation;
        //}
        //public int getWaterLevelElevation()
        //{
        //    return this.waterLevelElevation;
        //}
        //public int getViewableSize()
        //{
        //    return this.regionConfig.regionSize;
        //}
        //public long getViewableSeed()
        //{
        //    return this.regionConfig.regionGenConfig.seed;
        //}
        //public int getMaxTileIndex()
        //{
        //    return this.tiles.GetLength(0);
        //}
    }
}