using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

static class TerrainBlur
{
    public static float[,] blur(float[,] terrainData, int kernelSize = 3)
    {
        if (kernelSize % 2 == 0) throw new Exception("kernelSize must be uneven");
        int kernel = (kernelSize - 1) / 2;
        int xDim = terrainData.GetLength(1);
        int yDim = terrainData.GetLength(0);
        float[,] blurredTerrain = new float[yDim, xDim];
        
        for(int y = 0; y < yDim; y++)
        {
            for (int x = 0; x < xDim; x++)
            {
                float blurred = 0;
                int yClampLow = Mathf.Clamp(y - kernel, 0, yDim - 1);
                int yClampHigh = Mathf.Clamp(y + kernel, 0, yDim - 1);
                int xClampLow = Mathf.Clamp(x - kernel, 0, xDim - 1);
                int xClampHigh = Mathf.Clamp(x + kernel, 0, xDim - 1);

                int numFields = (yClampHigh - yClampLow) * (xClampHigh - xClampLow);
                for (int i = yClampLow; i <= yClampHigh; i++)
                {
                    for (int j = xClampLow; j <= xClampHigh; j++)
                    {
                        blurred += terrainData[i, j] / numFields;
                    }
                }
                blurredTerrain[y, x] = blurred;
            }
        }
        return blurredTerrain;
    }
}

