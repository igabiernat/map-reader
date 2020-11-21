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
    public bool isParking;
    public bool isFootway = false;
    public bool isRoad = false;
    public int floors = 1;
    public bool isTerrain;
    public bool isTree;
    public bool isWater;
    public string typeOfBuilding;
    public string typeOfAmenity;
    public string typeOfTerrain;
    public string typeOfBuildings;
    public string typeOfNatural;
    public string typeOfLeisure;
    public bool isRiver;
    
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
                string value = GetAttribute<string>("v", tag.Attributes);
                if (value.Equals("bungalow") || value.Equals("bungalow") ||
                    value.Equals("cabin") || value.Equals("detached") ||
                    value.Equals("farm") || value.Equals("house") ||
                    value.Equals("semidetached_house"))
                {
                    typeOfBuilding = "house";
                }
                else if (value.Equals("commercial") || value.Equals("kiosk") ||
                         value.Equals("retail") || value.Equals("shop"))
                {
                    typeOfBuilding = "shop";
                }
                else
                {
                    typeOfBuilding = value;
                }
                
                //isBuilding = true;
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
                string value = GetAttribute<string>("v", tag.Attributes);
                //isBuilding = true;
                if (value == "school" || value == "kindergarten" || value == "parking")
                {
                    typeOfTerrain = value;
                }
                else
                {
                    typeOfBuilding = value;
                }
                break;
            }
            else if (key == "shop")
            {
                typeOfBuilding = "shop";
                //isBuilding = true;
                break;
            }
            else if (key == "landuse")
            {
                typeOfTerrain = GetAttribute<string>("v", tag.Attributes);
            }
            else if (key == "waterway")
            {
                if (GetAttribute<string>("v", tag.Attributes) == "river")
                    isRiver = true;
            }
            else if (key == "natural")
            {
                string value = GetAttribute<string>("v", tag.Attributes);
                typeOfNatural = value;
            }
            else if (key == "leisure")
            {
                string value = GetAttribute<string>("v", tag.Attributes);
                if (value == "park")
                    typeOfTerrain = value;
            }
            
            /*if (key == "building:levels")
            {
                floors = Int32.Parse(GetAttribute<string>("v", tag.Attributes));
            }*/
        }
    }
}
