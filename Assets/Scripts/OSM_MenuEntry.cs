using UnityEditor;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OsmSharp.Streams;
using OsmSharp.Complete;
using OsmSharp.Geo.Streams.Features.Interpreted;

public class MenuTest : MonoBehaviour
{

    // Add a menu item named "Do Something" to MyMenu in the menu bar.
    [MenuItem("OSM/Generate Map from OSM data")]
    static void MenuItem()
    {
        Debug.Log("Checking if map data is available");
        GenerateMapData(@".\Assets\OSM\flein.osm");
        Debug.Log("Done!");
    }

    private static void GenerateMapData(string filePath)
    {
        CompleteWay[] completeWays;
        using (var fileStream = new FileInfo(filePath).OpenRead())
        {
            XmlOsmStreamSource source = new XmlOsmStreamSource(fileStream);
         // Get all building ways and nodes 
         var tmp_ways = from osmGeo in source
                            where osmGeo.Type == OsmSharp.OsmGeoType.Node ||
                            (osmGeo.Type == OsmSharp.OsmGeoType.Way)
                            select osmGeo;

            // Should filter before calling ToComplete() to reduce memory usage
            var completes = tmp_ways.ToComplete(); // Create Complete objects (for Ways gives them a list of Node objects)
            var ways = from osmGeo in completes
                       where osmGeo.Type == OsmSharp.OsmGeoType.Way
                       select osmGeo;
            completeWays = ways.Cast<CompleteWay>().ToArray();
            Debug.Log("Parsed osm file");
            Debug.Log("There are " + completeWays.Length + " ways");
        }

        OSM_Map_Generator generator = new OSM_Map_Generator();
        generator.PlotMap(completeWays);
    }


    // Add a menu item named "Do Something" to MyMenu in the menu bar.
    [MenuItem("SRTM/load data")]
    static void SRTM_MenuItem()
    {

        string filename = "Assets/SRTM";
        SRTM_Reader srtm = new SRTM_Reader(filename);

        Debug.Log(srtm.GetElevationAtSync(49.10094934f, 9.21450603f));
        Debug.Log(srtm.GetElevationAtSync(49.21411973f, 9.2254717465f));
        Debug.Log(srtm.GetElevationAtSync(49.10092f, 9.21411f));
        Debug.Log(srtm.GetElevationAtSync(49.1204219f, 9.2139712f));
    }


    // Add a menu item named "Do Something" to MyMenu in the menu bar.
    [MenuItem("OSM/generate Terrain")]
    static void GenerateTerrain()
    {
        
        float max_lat = 49.1117000f;
        // float max_lat = 49.1017000f;
        float min_lat = 49.0943000f;
        float min_lon = 9.1985000f;
        // float max_lon = 9.2060000f;
        float max_lon = 9.2260000f;

        Terrain_Generator terrainGen = new Terrain_Generator();
        terrainGen.max_lat = max_lat;
        terrainGen.min_lat = min_lat;
        terrainGen.min_lon = min_lon;
        terrainGen.max_lon = max_lon;

        terrainGen.srtm = new SRTM_Reader("Assets/SRTM");
        terrainGen.GenerateTerrain();
    }
}

