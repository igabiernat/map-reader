using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Xml;

public class MapReader : MonoBehaviour
{
    public string mapTxt;
    public static int divider = 10;
    public MeshGenerator meshGenerator;
    
    public List<double> oobMinLat;
    public List<double> oobMaxLat;
    public List<double> oobMinLon;
    public List<double> oobMaxLon;

    public double[] actualBounds;

    public Dictionary<long, MapNode> mapNodes;
    public List<OSMWay> ways;

    public OSMBounds bounds;

    public bool isReady = false;
    // Start is called before the first frame update
    void Start()
    {
        meshGenerator = meshGenerator.GetComponent<MeshGenerator>();
        mapNodes = new Dictionary<long, MapNode>();
        ways = new List<OSMWay>();
        var mapLoaded = Resources.Load<TextAsset>(mapTxt);
        XmlDocument mapXml = new XmlDocument();
        actualBounds = new double[4];
        mapXml.LoadXml(mapLoaded.text);

        GetNodes(mapXml.SelectNodes("/osm/node"));
        GetWays(mapXml.SelectNodes("/osm/way"));
        GetBounds();
        SetBounds(mapXml.SelectSingleNode("/osm/bounds"));
        //GetWays(mapXml.SelectNodes("/osm/way"));
        meshGenerator.OnMapLoaded(bounds);
        isReady = true;
    }


    void GetNodes(XmlNodeList nodes)
    {
        foreach (XmlNode mapNode in nodes)
        {
            MapNode node = new MapNode(mapNode, bounds);
            mapNodes[node.id] = node;
        }

        /*foreach (KeyValuePair<long, MapNode> entry in mapNodes)
        {
            if (entry.Value.latitude > bounds.maxLat)
                oobMaxLat.Add(entry.Value.latitude);
            if (entry.Value.latitude < bounds.minLat)
                oobMinLat.Add(entry.Value.latitude);
            if (entry.Value.longitude > bounds.maxLon)
                oobMaxLon.Add(entry.Value.longitude);
            if (entry.Value.longitude < bounds.minLon)
                oobMinLon.Add(entry.Value.longitude);
        }*/
    }
    void GetWays(XmlNodeList wayNodes)
    {
        foreach (XmlNode wayNode in wayNodes)
        {
            OSMWay way = new OSMWay(wayNode);
            ways.Add(way);
        }
    }

    void GetBounds()
    {
        double minLat = mapNodes[ways[0].childrenIDs[0]].latitude;
        double maxLat = mapNodes[ways[0].childrenIDs[0]].latitude;
        double minLon = mapNodes[ways[0].childrenIDs[0]].longitude;
        double maxLon = mapNodes[ways[0].childrenIDs[0]].longitude;
        foreach (var way in ways)
        {
            if (way.isBuilding)
            {
                for (int i = 1; i < way.childrenIDs.Count; i++)
                {
                    if (mapNodes[way.childrenIDs[i]].latitude < minLat)
                        minLat = mapNodes[way.childrenIDs[i]].latitude;
                    if (mapNodes[way.childrenIDs[i]].latitude > maxLat)
                        maxLat = mapNodes[way.childrenIDs[i]].latitude;
                    if (mapNodes[way.childrenIDs[i]].longitude < minLon)
                        minLon = mapNodes[way.childrenIDs[i]].longitude;
                    if (mapNodes[way.childrenIDs[i]].longitude > maxLon)
                        maxLon = mapNodes[way.childrenIDs[i]].longitude;
                }
            }
        }
        actualBounds[0] = minLat-0.001;
        actualBounds[1] = maxLat+0.001;
        actualBounds[2] = minLon-0.001;
        actualBounds[3] = maxLon+0.001;
    }
    void SetBounds(XmlNode boundsNode)
    {
        bounds = new OSMBounds(boundsNode, actualBounds); //todo change bounds from theoretical to actual
    }
    // Update is called once per frame
    void Update()
    {
        foreach (OSMWay way in ways)
        {
            if (way.isVisible)
            {
                Color c = Color.magenta;    //road
                if (way.isBoundary)
                {
                    c = Color.cyan;        //building
                }

                if (way.isRoad)
                    c = Color.green;

                for (int i = 1; i < way.childrenIDs.Count; i++)
                {
                    MapNode p1 = mapNodes[way.childrenIDs[i-1]];
                    MapNode p2 = mapNodes[way.childrenIDs[i]];

                    Vector3 v1 = new Vector3((float)p1.X, 40, (float)p1.Y) - bounds.centre;
                    Vector3 v2 = new Vector3((float)p2.X, 40, (float)p2.Y) - bounds.centre;
                    
                    Debug.DrawLine(v1/10, v2/10, c);
                }
            }
        }
    }
}
