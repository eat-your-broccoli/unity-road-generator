using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class TerrainHeightInfo
{
    Terrain terrain;

    public TerrainHeightInfo() { }

    public TerrainHeightInfo(Terrain terrain)
    {
        this.setTerrain(terrain);
    }

    public void setTerrain(Terrain t)
    {
        this.terrain = t;
    }

    public float GetHeightAtCoords(int x, int y)
    {
        return terrain.terrainData.GetHeight(x, y);
    }
}

