using System.Xml;
using System;
using System.Collections.Generic;

public class OSMWay : OSMBase
{
    public long id;
    public bool isVisible;
    public List<long> childrenIDs;
    public XmlNodeList childrenNodes;
    public bool isBuilding;
    
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
            isBuilding = true;
        else 
            isBuilding = false;
    }
}
