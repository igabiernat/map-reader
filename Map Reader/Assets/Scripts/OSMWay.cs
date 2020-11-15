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
    public bool isRoad = false;
    
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
            if (key == "building")
            {
                isBuilding = GetAttribute<string>("v", tag.Attributes) == "yes";
            }
            if (key == "highway")
            {
                if (GetAttribute<string>("v", tag.Attributes) != "footway")
                    isRoad = true;
            }
        }
    }
}
