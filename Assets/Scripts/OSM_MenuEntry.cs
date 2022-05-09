using UnityEditor;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OsmSharp.Streams;
using OsmSharp.Complete;

public class MenuTest : MonoBehaviour
{

    // Add a menu item named "Do Something" to MyMenu in the menu bar.
    [MenuItem("OSM/Generate Map from OSM data")]
    static void MenuItem()
    {
        Debug.Log("Checking if map data is available");
        GenerateMapData(@".\Assets\OSM\test-map.osm");
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
}

