using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

static class TerrainBlur
{
    public static float[,] blur(float[,] terrainData)
    {
        int xDim = terrainData.GetLength(1);
        int yDim = terrainData.GetLength(0);
        float[,] blurredTerrain = new float[yDim, xDim];

        for(int y = 1; y < yDim - 1; y++)
        {
            for (int x = 1; x < xDim - 1; x++)
            {
                float blurred = 0;
                blurred += terrainData[y, x];
                blurred += terrainData[y+1, x];
                blurred += terrainData[y, x+1];
                blurred += terrainData[y-1, x];
                blurred += terrainData[y, x-1];

                blurredTerrain[y, x] = (blurred / 5);
            }
        }


        return blurredTerrain;
    }
}

