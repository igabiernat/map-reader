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

        maxLat = actualBounds[0];
        minLat = actualBounds[1];
        maxLon = actualBounds[2];
        minLon = actualBounds[3];

        maxX = MercatorProjection.lonToX(maxLon);
        minX = MercatorProjection.lonToX(minLon);
        maxY = MercatorProjection.latToY(maxLat);
        minY = MercatorProjection.latToY(minLat);

        centreX = (maxX + minX)/2;
        centreY = (maxY + minY)/2;

        centre = new Vector3((float)centreX,0, (float)centreY);
        
        SetTerrain();
    }

    private void SetTerrain()
    {
        terrainData = new TerrainData();
        terrainXsize = (maxX - minX) / MapReader.divider;
        terrainYsize = (maxY - minY) / MapReader.divider;
        terrainData.size = new Vector3((float)(terrainXsize),10, (float)(terrainYsize));
        //GameObject terrain = Terrain.CreateTerrainGameObject(terrainData);
        terrainX = 0 - (maxX - minX) / (2*MapReader.divider);
        terrainY = 0 - (maxY - minY) / (2*MapReader.divider);
        //terrain.transform.position = new Vector3((float)terrainX, 0, (float)terrainY);
        
    }
}
