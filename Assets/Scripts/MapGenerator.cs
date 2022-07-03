using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

/**
 * @class MapGenerator
 * 
 * parent for all other generators
 */

public class MapGenerator
{
    LocalizedMercatorProjection mercator;
    SRTM_Reader srtm;
    Terrain_Generator terrainGen;
    OSM_Map_Generator osmGen;
    
    // osm map file
    public String osmPath = @".\Assets\OSM\flein_2.osm";

    // bounds of map
    public float max_lat = 49.1117000f;
    public float min_lat = 49.0943000f;
    public float min_lon = 9.1985000f;
    public float max_lon = 9.2260000f;

    // should a new scene be created?
    public bool newScene = false;
    // should terrain be generated?
    public bool generateTerrain = true;
    // should roads be generated?
    public bool generateRoads = false;
    // kernel sizes for blurring
    public int[] terrainBlurKernels;

    public void GenerateMap()
    {
        Scene scene;
        if (mercator == null)
        {
            mercator = new LocalizedMercatorProjection();
            mercator.x_offset = (float)MercatorProjection.lonToX(min_lon);
            mercator.y_offset = (float)MercatorProjection.latToY(min_lat);
        }
        if(srtm == null)
        {
            srtm = new SRTM_Reader("Assets/SRTM");
        }

        if(newScene == true)
        {
            scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            scene.name = "Map";
        } else
        {
            scene = SceneManager.GetActiveScene();
        }

        if (generateTerrain == true)
        {
            terrainGen = new Terrain_Generator();
            terrainGen.srtm = srtm;
            terrainGen.mercator = mercator;
            terrainGen.GenerateTerrain();
        }

        if(terrainBlurKernels != null && terrainBlurKernels.Length > 0)
        {
            // for each blur kernel, perform blurring
            var data = terrainGen.terrain.terrainData;
            var heights = data.GetHeights(0, 0, (int)data.size.x, (int)data.size.z);
            foreach ( int kernel in terrainBlurKernels)
            {
                heights = TerrainBlur.blur(heights, kernel);
            }
            terrainGen.terrain.terrainData.SetHeights(0, 0, heights);
        }

        if(generateRoads == true)
        {
            // wrap terrain in terrainHeightInfo
            TerrainHeightInfo terrainHeight = new TerrainHeightInfo(terrainGen.terrain);
            osmGen = new OSM_Map_Generator();
            osmGen.terrainHeight = terrainHeight;
            osmGen.useTerrainHeight = true;
            osmGen.mercator = mercator;
            osmGen.srtm = srtm;
            osmGen.PlotMap(osmPath);
        }
    }
}

