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
    
    public double maxY;
    public double minY;
    public double maxX;
    public double minX;

    public double centreX;
    public double centreY;

    public double terrainX;
    public double terrainY;
    public double terrainXsize;
    public double terrainYsize;

    private TerrainData terrainData;

    public Vector3 centre;

    public OSMBounds(XmlNode node, double[] actualBounds)
    {
        /*maxLat = GetAttribute<float>("maxlat", node.Attributes);
        minLat = GetAttribute<float>("minlat", node.Attributes);
        maxLon = GetAttribute<float>("maxlon", node.Attributes);
        minLon = GetAttribute<float>("minlon", node.Attributes);

        maxX = MercatorProjection.lonToX(maxLon);
        minX = MercatorProjection.lonToX(minLon);
        maxY = MercatorProjection.latToY(maxLat);
        minY = MercatorProjection.latToY(minLat);

        centreX = (maxX + minX)/2;
        centreY = (maxY + minY)/2;

        centre = new Vector3((float)centreX,0, (float)centreY);*/

        /*maxLat = actualBounds[0];
        minLat = actualBounds[1];
        maxLon = actualBounds[2];
        minLon = actualBounds[3];

        maxX = MercatorProjection.lonToX(maxLon);
        minX = MercatorProjection.lonToX(minLon);
        maxY = MercatorProjection.latToY(maxLat);
        minY = MercatorProjection.latToY(minLat);

        centreX = (maxX + minX)/2;
        centreY = (maxY + minY)/2;

        centre = new Vector3((float)centreX,0, (float)centreY);*/
        
        /*maxLat = actualBounds[0];
        minLat = actualBounds[1];
        maxLon = actualBounds[2];
        minLon = actualBounds[3];

        maxX = MercatorProjection.lonToX(maxLon);
        minX = MercatorProjection.lonToX(minLon);
        maxY = MercatorProjection.latToY(maxLat);
        minY = MercatorProjection.latToY(minLat);

        centreX = (maxX + minX)/2;
        centreY = (maxY + minY)/2;

        centre = new Vector3((float)centreX,0, (float)centreY);*/

        minLat = actualBounds[0];
        maxLat = actualBounds[1];
        minLon = actualBounds[2];
        maxLon = actualBounds[3];
        
        minX = MercatorProjection.lonToX(minLon);
        maxX = MercatorProjection.lonToX(maxLon);
        minY = MercatorProjection.latToY(minLat);
        maxY = MercatorProjection.latToY(maxLat);
        
        centreX = (maxX + minX)/2;
        centreY = (maxY + minY)/2;

        centre = new Vector3((float)centreX,0, (float)centreY);       
        
        SetTerrainSize();
    }

    private void SetTerrainSize()
    {
        terrainXsize = (maxX - minX) / MapReader.divider;
        terrainYsize = (maxY - minY) / MapReader.divider;
        terrainX = 0 - ((maxX - minX) / (2*MapReader.divider));
        terrainY = 0 - ((maxY - minY) / (2*MapReader.divider));
    }
}
