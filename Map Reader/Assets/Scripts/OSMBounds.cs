using System;
using System.Globalization;
using System.Xml;
using UnityEngine;

public class OSMBounds : OSMBase
{
    public double maxLat;
    public double minLat;
    public double maxLon;
    public double minLon;

    public double centreX;
    public double centreY;

    public Vector3 centre;

    public OSMBounds(XmlNode node)
    {
        maxLat = GetAttribute<float>("maxlat", node.Attributes);
        minLat = GetAttribute<float>("minlat", node.Attributes);
        maxLon = GetAttribute<float>("maxlon", node.Attributes);
        minLon = GetAttribute<float>("minlon", node.Attributes);

        centreX = (MercatorProjection.lonToX(maxLon) + MercatorProjection.lonToX(minLon))/2;
        centreY = (MercatorProjection.latToY(maxLat) + MercatorProjection.latToY(minLat))/2;

        centre = new Vector3((float)centreX,0, (float)centreY);

    }
}
