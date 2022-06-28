using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using OsmSharp.Complete;
using System.Linq;
using OsmSharp.Streams;
using System.IO;


/**
 * @class OSM_Map_Generator
 * 
 * draws road from OSM data
 */
public class OSM_Map_Generator
{
    Scene scene;
    GameObject map;

    public SRTM_Reader srtm;
    public LocalizedMercatorProjection mercator;
    public TerrainHeightInfo terrainHeight;
    public bool useTerrainHeight = true; // use terrainHeight instead of SRTM


    // plot size for all 
    public float plotSize = 10f;
    // multiplier for buildings
    public float plotScaleBuilding = 0.2f;
    // multiplier for ways
    public float plotScaleWay = 1f;

    // only plot named streets 
    public bool plotOnlyNamedStreets = false;

    public bool plotOnlyStreetsWithHighwayTag = false;
    // plot with line renderer. if false plots with cubes
    public bool plotWithLineRenderer = true;

    public bool useHeightmap = true;

    public bool generateTerrain = true;
    public bool generateRoads = false;
    public bool newScene = false;

    public UnityEngine.Material matColorWhite;
    public UnityEngine.Material matColorBlue;
    public UnityEngine.Material matColorGreen;
    public UnityEngine.Material matColorGrey;

    public OSM_Map_Generator()
    {
        matColorWhite = AssetDatabase.LoadAssetAtPath<UnityEngine.Material>("Assets/Materials/ColorWhite.mat");
        matColorBlue = AssetDatabase.LoadAssetAtPath<UnityEngine.Material>("Assets/Materials/ColorBlue.mat");
        matColorGreen = AssetDatabase.LoadAssetAtPath<UnityEngine.Material>("Assets/Materials/ColorGreen.mat");
        matColorGrey = AssetDatabase.LoadAssetAtPath<UnityEngine.Material>("Assets/Materials/ColorGrey.mat");
    }

    public void PlotMap(string filePath)
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
        }
        PlotMap(completeWays);
    }

    public void PlotMap(CompleteWay[] ways)
    {
        map = new GameObject("map");
        foreach (CompleteWay way in ways)
        {
            PlotWay(way);
        }
    }

    private void PlotWay(CompleteWay way)
    {
        string name = way?.Tags?.GetValue("name");
        string highway = way?.Tags?.GetValue("highway");

        if (plotOnlyNamedStreets && (name == null || name.Length == 0)) return;
        if (plotOnlyStreetsWithHighwayTag && (highway == null || highway.Length == 0)) return;
        GameObject way_gobj = new GameObject(way?.Tags?.GetValue("name"));
        way_gobj.transform.parent = map.transform;

        List<GameObject> listOfNodes = new List<GameObject>();
        foreach (OsmSharp.Node node in way.Nodes)
        {
            if (node.Latitude is double && node.Longitude is double)
            {
                Vector3 coords = LatLonToCoords(node.Latitude ?? 0, node.Longitude ?? 0);
                GameObject wayNode = new GameObject();
                wayNode.transform.position = coords;
                wayNode.transform.parent = way_gobj.transform;
                
                listOfNodes.Add(wayNode);
            }
        }
        if(plotWithLineRenderer)
        {
            if (name != null && name.Length > 0) PlotNodesWithLineRenderer(listOfNodes.ToArray(), matColorBlue);
            else PlotNodesWithLineRenderer(listOfNodes.ToArray(), matColorGrey, plotScaleBuilding);
        } else
        {
            PlotNodesWithCubes(listOfNodes.ToArray());
        }
        
    }

    private Vector3 LatLonToCoords(double lat, double lon)
    {
        Vector3 coords = new Vector3();
        coords.x = (float) (mercator.lonToX(lon));
        // this isn't an error. in XYZ coordinate systems, Y is the height.
        coords.z = (float) (mercator.latToY(lat));

        if(useHeightmap)
        {
            if (useTerrainHeight) coords.y = terrainHeight.GetHeightAtCoords((int) coords.x, (int) coords.z);
            else coords.y = srtm.GetElevationAtSync(lat, lon);
        }
        return coords;
    }

    private void PlotNodesWithCubes(GameObject[] nodes)
    {
        foreach (GameObject gobj in nodes)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.localScale = new Vector3(plotSize, plotSize, plotSize);
            cube.transform.position = gobj.transform.position;
            cube.transform.parent = gobj.transform;
        }
    }

    private void PlotNodesWithLineRenderer(GameObject[] nodes, UnityEngine.Material material = null, float scale = 1f)
    {
        if (nodes.Length <= 2) return;
        if (material == null) material = matColorWhite;

        GameObject parent = nodes[0].transform.parent.gameObject;
        LineRenderer lineRenderer = parent.AddComponent<LineRenderer>();
        lineRenderer.positionCount = nodes.Length;
        lineRenderer.widthMultiplier = plotSize * scale;
        lineRenderer.material = material;

        for (int i = 0; i < nodes.Length; i++)
        {
            lineRenderer.SetPosition(i, nodes[i].transform.position);
        }
    }

}
