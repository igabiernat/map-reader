using System;
using System.Globalization;
using System.Xml;

public class MapNode : OSMBase
{
    public long id;
    public double latitude;
    public double longitude;
    public double X;
    public double Y;

    public MapNode(XmlNode node)
    {
        latitude = GetAttribute<float>("lat", node.Attributes);
        id = GetAttribute<long>("id", node.Attributes);
        longitude = GetAttribute<float>("lon", node.Attributes);

        X = MercatorProjection.lonToX(longitude);
        Y = MercatorProjection.latToY(latitude);
    }


}
