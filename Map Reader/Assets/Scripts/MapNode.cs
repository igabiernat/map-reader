using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Xml;
using UnityEngine;

public class MapNode : OSMBase
{
    public long id;
    public double latitude;
    public double longitude;
    public double X;
    public double Y;

    public static implicit operator Vector3(MapNode node)
    {
        return new Vector3((float)node.X, 0, (float)node.Y);
    }

    public MapNode(XmlNode node, OSMBounds bounds)
    {
        latitude = GetAttribute<float>("lat", node.Attributes);
        id = GetAttribute<long>("id", node.Attributes);
        longitude = GetAttribute<float>("lon", node.Attributes);

        X = MercatorProjection.lonToX(longitude);
        Y = MercatorProjection.latToY(latitude);
        
    }


}
