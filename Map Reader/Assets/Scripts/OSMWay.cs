using System.Xml;
using System;
using System.Collections.Generic;

public class OSMWay : OSMBase
{
    public long id;
    public bool isVisible;
    public List<long> childrenIDs;
    public XmlNodeList childrenNodes;
    public bool isBoundary;
    public bool isBuilding;
    public bool isFootway = false;
    public bool isRoad = false;
    public int floors = 1;
    public string typeOfBuilding;
    public bool isTerrain;
    
    public OSMWay(XmlNode node)
    {
        childrenIDs = new List<long>();
        id = GetAttribute<long>("id", node.Attributes);
        isVisible = GetAttribute<bool>("visible", node.Attributes);

        XmlNodeList childrenNodes = node.SelectNodes("nd");

        foreach(XmlNode childrenNode in childrenNodes)
        {
            long refno = GetAttribute<long>("ref", childrenNode.Attributes);
            childrenIDs.Add(refno);
        }

        if (childrenIDs[0] == childrenIDs[childrenIDs.Count - 1]) 
            isBoundary = true;
        else 
            isBoundary = false;

        XmlNodeList tags = node.SelectNodes("tag");
        foreach (XmlNode tag in tags)
        {
            string key = GetAttribute<string>("k",tag.Attributes);
            if (key == "building:levels")
            {
                floors = Int32.Parse(GetAttribute<string>("v", tag.Attributes));
            }
            else if (key == "building")
            {
                typeOfBuilding = GetAttribute<string>("v", tag.Attributes);
                isBuilding = true;
            }
            else if (key == "highway")
            {
                isRoad = true;
                if (GetAttribute<string>("v", tag.Attributes) != "footway")
                    isFootway = false;
                else
                    isFootway = true;
            }
            else if (key == "amenity")
            {
                typeOfBuilding = GetAttribute<string>("v", tag.Attributes);
                isBuilding = true;
                if(typeOfBuilding=="school")
                    isBuilding = false;
                break;
            }
            else if (key == "shop")
            {
                typeOfBuilding = "shop";
                isBuilding = true;
                break;
            }
            else if (key == "landuse")
            {
                isTerrain = true;
            }
            /*if (key == "building:levels")
            {
                floors = Int32.Parse(GetAttribute<string>("v", tag.Attributes));
            }*/
        }
    }
}
