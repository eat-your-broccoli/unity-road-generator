using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System;
public class Terrain_Generator
{
    public int depth = 10;
    public int width = 0;
    public int height = 300;
    public float scale = 30f;

    public Terrain terrain;
    public TerrainCollider terrainCollider;
    public Scene scene;

    public float max_lat = 0f;
    public float max_lon = 0f;
    public float min_lon = 0f;
    public float min_lat = 0f;

    LocalizedMercatorProjection mercator;
    public SRTM_Reader srtm;
    
    public Terrain_Generator(Scene scene) : this()
    {
        this.scene = scene;
    }

    public Terrain_Generator()
    {
        this.scene = SceneManager.GetActiveScene();
        GameObject gobj = new GameObject();
        gobj.name = "Terrain";
        this.terrain = gobj.AddComponent<Terrain>();
        this.terrain.terrainData = new TerrainData();
        this.terrain.materialTemplate = AssetDatabase.LoadAssetAtPath<UnityEngine.Material>("Assets/Materials/Terrain-Shader.mat");
        this.terrainCollider = gobj.AddComponent<TerrainCollider>();
        this.terrainCollider.terrainData = this.terrain.terrainData;
        this.mercator = new LocalizedMercatorProjection();
    }

    TerrainData ConfigureTerrainData(TerrainData terrainData)
    {
        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, height, depth);
        return terrainData;
    }

    public void GenerateTerrain()
    {
        mercator.x_offset = (float) MercatorProjection.lonToX(min_lon);
        mercator.y_offset = (float) MercatorProjection.latToY(min_lat);

        // determine size of map // lon: width, lat: depth
        this.depth = Math.Abs(Convert.ToInt32(mercator.latToY(max_lat) - mercator.latToY(min_lat)));
        this.width = Math.Abs(Convert.ToInt32(mercator.lonToX(max_lon) - mercator.lonToX(min_lon)));
        Debug.Log("height " + depth);
        Debug.Log("width " + width);
        this.terrain.terrainData = ConfigureTerrainData(terrain.terrainData);
        float[,] heights = GenerateTerrainData();
        Debug.Log(terrain.terrainData.size);
        Debug.Log(heights[20, 20]);
        terrain.terrainData.SetHeights(0, 0, heights);
    }

    float[,] GenerateTerrainData()
    {
        float[,] heights = new float[width, depth];
        for(int x = 0; x < (width); x += 1)
        {
            for (int y = 0; y < (depth); y += 1)
            {
                heights[x, y] = CalculateHeight(x, y);
            }
        }
        return heights;
    }

    float CalculateHeight(int x, int y)
    {
        // return Mathf.PerlinNoise((float)x / width * 100, (float)y / width * 100);
        return (float) ((float)srtm.GetElevationAtSync(mercator.yToLat(y), mercator.xToLon(x))) / (float) depth;
    }
}
