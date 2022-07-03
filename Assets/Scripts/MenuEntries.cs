using UnityEditor;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OsmSharp.Streams;
using OsmSharp.Complete;
using OsmSharp.Geo.Streams.Features.Interpreted;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

/**
 * Menu entries in Unity Editor
 * 
 */ 
public class MenuEntries : MonoBehaviour
{

    // Add a menu item named "Do Something" to MyMenu in the menu bar.
    [MenuItem("OSM/Generate Map from OSM data")]
    static void MenuItem()
    {
        Debug.Log("Checking if map data is available");
        // GenerateMapData(@".\Assets\OSM\flein_2.osm");
        MapGenerator gen = new MapGenerator();

        gen.terrainBlurKernels = new int[] {3, 13 };
        gen.generateRoads = true;
        gen.generateTerrain = true;
        gen.osmPath = @".\Assets\OSM\flein_2.osm";
        gen.max_lat = 49.1117000f;
        gen.min_lat = 49.0943000f;
        gen.min_lon = 9.1985000f;
        gen.max_lon = 9.2260000f;
        gen.GenerateMap();
        Debug.Log("Done!");
    }
}

