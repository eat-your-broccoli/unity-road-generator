using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System;

/**
 * @class Terrain_Generator
 * 
 * generates Terrain from SRTM_data
 */
public class Terrain_Generator
{
    public int depth = 4097;
    public int width = 4097;
    public int height = 300;

    public Terrain terrain;
    public TerrainCollider terrainCollider;
    public Scene scene;

    public LocalizedMercatorProjection mercator;
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

    public void GenerateTerrain()
    {
        ConfigureTerrainData();
        float[,] heights = GenerateTerrainData();
        terrain.terrainData.SetHeights(0, 0, heights);
    }

    void ConfigureTerrainData()
    {
        this.depth = 4097;
        this.width = 4097;
        this.height = 400;

        this.terrain.terrainData.heightmapResolution = width + 1;
        this.terrain.terrainData.size = new Vector3(width, height, depth);
    }

    float[,] GenerateTerrainData()
    {
        float[,] heights = new float[depth, width];
        for(int x = 0; x < (width); x += 1)
        {
            for (int y = 0; y < (depth); y += 1)
            {
                heights[y,x] = CalculateHeight(x, y);
            }
        }
        return heights;
    }

    float CalculateHeight(int x, int y)
    {
        float elevation = srtm.GetElevationAtSync(mercator.yToLat(y), mercator.xToLon(x));
        if (elevation >= height) return 1.0f;
        return elevation / height;
    }
}
