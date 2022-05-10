using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using OsmSharp.Complete;
using System.Linq;

public class OSM_Map_Generator
{
    Scene scene;
    GameObject map;
    
    // coordinate offset in x direction
    double xOffset = 0f;
    // coordinate offset in y direction
    double yOffset = 0f;

    // plot size for all 
    public float plotSize = 10f;
    // multiplier for buildings
    public float plotScaleBuilding = 0.2f;
    // multiplier for ways
    public float plotScaleWay = 1f;

    // only plot named streets 
    public bool plotOnlyNamedStreets = true;

    public bool plotOnlyStreetsWithHighwayTag = true;
    // plot with line renderer. if false plots with cubes
    public bool plotWithLineRenderer = true;

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

    public void PlotMap(CompleteWay[] ways)
    {
        Debug.Log("plotting the ways");
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        scene.name = "Map";
        // EditorSceneManager.OpenScene(scene.path);
        map = new GameObject("map");
        DetermineOffset(ways);

        foreach (CompleteWay way in ways)
        {
            PlotWay(way);
        }
    }

    private void DetermineOffset(CompleteWay[] ways)
    {
        foreach (CompleteWay way in ways)
        {
            foreach (OsmSharp.Node node in way.Nodes)
            {
                if (node.Latitude.HasValue && node.Longitude.HasValue)
                {
                    xOffset = MercatorProjection.lonToX(node.Longitude ?? 0);
                    yOffset = MercatorProjection.latToY(node.Latitude ?? 0);
                    // stop when first node found
                    return;
                }
            }
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
        coords.x = (float) (MercatorProjection.lonToX(lon) - xOffset);
        // this isn't an error. in XYZ coordinate systems, Y is the height.
        coords.z = (float) (MercatorProjection.latToY(lat) - yOffset);
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
