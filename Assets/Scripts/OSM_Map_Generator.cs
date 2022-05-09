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
    double xOffset = 0f;
    double yOffset = 0f;

    public OSM_Map_Generator()
    {
    }

    public void PlotMap(CompleteWay[] ways)
    {
        Debug.Log("plotting the ways");
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        scene.name = "Map";
        // EditorSceneManager.OpenScene(scene.path);
        map = new GameObject("map");
        DetermineOffset(ways);
        Debug.Log(xOffset);
        Debug.Log(yOffset);


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
        PlotNodes(listOfNodes.ToArray());
    }

    private Vector3 LatLonToCoords(double lat, double lon)
    {
        Vector3 coords = new Vector3();
        coords.x = (float) (MercatorProjection.lonToX(lon) - xOffset);
        // this isn't an error. in XYZ coordinate systems, Y is the height.
        coords.z = (float) (MercatorProjection.latToY(lat) - yOffset);
        return coords;
    }

    private void PlotNodes(GameObject[] nodes)
    {
        foreach(GameObject gobj in nodes)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = gobj.transform.position;
            cube.transform.parent = gobj.transform;
        }
    }


}
