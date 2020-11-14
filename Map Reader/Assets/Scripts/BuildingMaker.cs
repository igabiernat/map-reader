using System.Collections;
using System;
using System.Security.Cryptography;
using UnityEngine;

public class BuildingMaker : MonoBehaviour
{
    private MapReader map;
    public GameObject buildingPrefab;
    public MeshCollider meshCollider;
    private float[] distances;

    IEnumerator Start()
    {
        map = GetComponent<MapReader>();
        while (!map.isReady)
            yield return null;
        foreach (var way in map.ways)
        {
            if (way.isBuilding && way.childrenIDs.Count>1)
            {
                MapNode firstNode = map.mapNodes[way.childrenIDs[0]];    //first point
                MapNode secondNode = map.mapNodes[way.childrenIDs[1]]; //second point
                Vector3 sum = Vector3.zero;
                
                float minBuildingHeight = GetHeight((float) (firstNode.X- map.bounds.centre.x) / MapReader.divider, (float) (firstNode.Y- map.bounds.centre.z) / MapReader.divider);
                distances = new float[way.childrenIDs.Count-1];
                
                for (int i = 1; i < way.childrenIDs.Count; i++)
                {
                    MapNode n1 = map.mapNodes[way.childrenIDs[i - 1]];
                    MapNode n2 = map.mapNodes[way.childrenIDs[i]];
                    
                    float x1 = (float) (n1.X - map.bounds.centre.x) / MapReader.divider;
                    float z1 = (float) (n1.Y - map.bounds.centre.z) / MapReader.divider;
                    float x2 = (float) (n2.X - map.bounds.centre.x) / MapReader.divider;
                    float z2 = (float) (n2.Y - map.bounds.centre.z) / MapReader.divider;
                    
                    Vector3 v1 = new Vector3(x1,0, z1);
                    Vector3 v2 = new Vector3(x2,0, z2);

                    sum += v1;

                    float pointHeight = GetHeight(x2, z2);
                    if (pointHeight < minBuildingHeight)
                        minBuildingHeight = pointHeight;
                    
                    float distance = Vector3.Distance(v1, v2);
                    distances[i - 1] = distance;
                    
                    /*
                    MapNode node = map.mapNodes[way.childrenIDs[i]];
                    if (node.X < minX)
                        minX = (float)node.X;
                    if (node.X > maxX)
                        maxX = (float)node.X;
                    if (node.Y < minY)
                        minY = (float)node.Y;
                    if (node.Y > maxY)
                        maxY = (float)node.Y;*/
                }

                /*minX = (minX - map.bounds.centre.x) / MapReader.divider;
                maxX = (maxX - map.bounds.centre.x) / MapReader.divider;
                minY = (minY - map.bounds.centre.z) / MapReader.divider;
                maxY = (maxY - map.bounds.centre.z) / MapReader.divider;
                
                float[] Ys = {minY, maxY};
                float[] Xs = {minX, maxX};
                float pointMinHeight = GetHeight(Xs[0], Ys[0]);
                for (int x = 0; x < 2; x++)
                {
                    for (int y = 0; y < 2; y++)
                    {
                        float pointY = GetHeight(Xs[x], Ys[y]);
                        if (pointY < pointMinHeight)
                            pointMinHeight = pointY;
                    }
                }*/
                
                /*Vector3 p1, p2, p3, p4;
                p1 = new Vector3(minX,pointMinHeight,minY);
                p2 = new Vector3(maxX,pointMinHeight,minY);
                p3 = new Vector3(maxX,pointMinHeight,maxY);
                p4 = new Vector3(minX,pointMinHeight,maxY);*/
                Array.Sort(distances);
                Array.Reverse(distances);
                
                Vector3 centre = sum / (way.childrenIDs.Count - 1);
                centre.y = minBuildingHeight;
                
                float firstNodeX = (float)(firstNode.X - map.bounds.centre.x) / MapReader.divider;
                float firstNodeZ = (float)(firstNode.Y - map.bounds.centre.z) / MapReader.divider;                
                float secondNodeX = (float)(secondNode.X - map.bounds.centre.x) / MapReader.divider;
                float secondNodeZ = (float)(secondNode.Y - map.bounds.centre.z) / MapReader.divider;
                Vector3 A = new Vector3(firstNodeX,0, firstNodeZ);
                Vector3 B = new Vector3(secondNodeX, 0, secondNodeZ);

                GameObject go = Instantiate(buildingPrefab, centre, Quaternion.LookRotation(B-A,Vector3.up));
                go.transform.localScale *= distances[1];
            }
        }
    }
    
    float GetHeight(float x, float z)
    {
        float height = 0;
        RaycastHit hit;
        Ray ray = new Ray(new Vector3(x,MeshGenerator.maxHeight,z),Vector3.down );
        if (meshCollider.Raycast(ray, out hit, 2.0f * MeshGenerator.maxHeight))
        {
            height = hit.point.y;
        }
        return(height);
    }
}
