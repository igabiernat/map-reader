using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class MapReader : MonoBehaviour
{
    public string mapTxt;

    public Dictionary<long, MapNode> mapNodes;
    public List<OSMWay> ways;

    public OSMBounds bounds;
    // Start is called before the first frame update
    void Start()
    {
        mapNodes = new Dictionary<long, MapNode>();
        ways = new List<OSMWay>();
        var mapLoaded = Resources.Load<TextAsset>(mapTxt);
        XmlDocument mapXml = new XmlDocument();
        mapXml.LoadXml(mapLoaded.text);

        SetBounds(mapXml.SelectSingleNode("/osm/bounds"));
        GetNodes(mapXml.SelectNodes("/osm/node"));
        GetWays(mapXml.SelectNodes("/osm/way"));
    }

    void GetWays(XmlNodeList wayNodes)
    {
        foreach (XmlNode wayNode in wayNodes)
        {
            OSMWay way = new OSMWay(wayNode);
            ways.Add(way);
        }
    }

    void GetNodes(XmlNodeList nodes)
    {
        foreach (XmlNode mapNode in nodes)
        {
            MapNode node = new MapNode(mapNode);
            mapNodes[node.id] = node;
        }
    }
    
    void SetBounds(XmlNode boundsNode)
    {
        bounds = new OSMBounds(boundsNode);
    }
    // Update is called once per frame
    void Update()
    {
        foreach (OSMWay way in ways)
        {
            if (way.isVisible)
            {
                Color c = Color.magenta;    //road
                if (way.isBuilding)
                {
                    c = Color.cyan;        //building
                }

                for (int i = 1; i < way.childrenIDs.Count; i++)
                {
                    MapNode p1 = mapNodes[way.childrenIDs[i-1]];
                    MapNode p2 = mapNodes[way.childrenIDs[i]];

                    Vector3 v1 = new Vector3((float)p1.X, 0, (float)p1.Y) - bounds.centre;
                    Vector3 v2 = new Vector3((float)p2.X, 0, (float)p2.Y) - bounds.centre;
                    
                    Debug.DrawLine(v1, v2);
                }
            }
        }
    }
}
