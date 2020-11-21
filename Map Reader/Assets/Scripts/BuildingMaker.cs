using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using Random = System.Random;

public class BuildingMaker : MonoBehaviour
{
    private MapReader map;
    public GameObject[] regularBuildingsPrefabs;
    public GameObject[] higherBuildingsPrefabs;
    public GameObject regularBuildingPrefab;
    public GameObject schoolPrefab;
    public GameObject housePrefab;
    public GameObject ospPrefab;
    public GameObject policePrefab;
    public GameObject shopPrefab;
    public GameObject bankPrefab;
    public GameObject churchPrefab;
    public GameObject gasStationPrefab;
    public GameObject hotelPrefab;
    public GameObject apartmentsPrefab;

    public GameObject chosenPrefab;
    
    public MeshCollider meshCollider;
    private float[] distances;
    private Vector3[] sections;
    private float floorHeight;
    Random rnd = new Random();

    IEnumerator Start()
    {
        floorHeight = 3.0f;
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

                ChoosePrefab(way);
                if (chosenPrefab == null) 
                    continue;
                chosenPrefab.transform.localScale = new Vector3(0.12f,0.12f,0.12f);

                float minBuildingHeight = GetHeight((float) (firstNode.X- map.bounds.centre.x) / MapReader.divider, (float) (firstNode.Y- map.bounds.centre.z) / MapReader.divider);
                distances = new float[way.childrenIDs.Count-1];
                sections = new Vector3[way.childrenIDs.Count-1];
                
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
                    
                    //float distance = Vector3.Distance(v1, v2);
                    //distances[i - 1] = distance;
                    sections[i - 1] = v2 - v1;
                    
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

                List<float> xDistances = new List<float>();
                List<float> zDistances = new List<float>();
                
                for (int i = 0; i < sections.Length; i++)
                {
                    if(Math.Abs(sections[i].x) > Math.Abs(sections[i].z))
                        xDistances.Add(sections[i].magnitude);
                    else
                        zDistances.Add(sections[i].magnitude);
                }

                xDistances.Sort();
                xDistances.Reverse();
                zDistances.Sort();
                zDistances.Reverse();
                
                /*float maxdistance = distances[0];
                int pos = 0;
                for (int i = 0; i < distances.Length; i++)
                {
                    if (distances[i] > maxdistance)
                    {
                        maxdistance = distances[i];
                        pos = i;
                    }
                }

                float secondmaxdistance;
                int pos2 = 0;
                
                if (pos != 0)
                    secondmaxdistance = distances[0];
                else
                    secondmaxdistance = distances[1];
                
                for (int i = 0; i < distances.Length; i++)
                {
                    if (i != pos && distances[i] > secondmaxdistance)
                    {
                        secondmaxdistance = distances[1];
                        pos2 = i;
                    }  
                }

                MapNode maxdist1 = map.mapNodes[way.childrenIDs[pos]];
                MapNode maxdist2 = map.mapNodes[way.childrenIDs[pos+1]];
                MapNode secondmaxdist1 = map.mapNodes[way.childrenIDs[pos2]];
                MapNode secondmaxdist2 = map.mapNodes[way.childrenIDs[pos2+1]];
                
                Vector3 maxB = new Vector3((float)(maxdist1.X - map.bounds.centre.x) / MapReader.divider, 0, (float)(maxdist1.Y - map.bounds.centre.z) / MapReader.divider);
                Vector3 maxA = new Vector3((float)(maxdist2.X - map.bounds.centre.x) / MapReader.divider, 0, (float)(maxdist2.Y - map.bounds.centre.z) / MapReader.divider);
                Vector3 secondmaxB = new Vector3((float)(secondmaxdist1.X - map.bounds.centre.x) / MapReader.divider, 0, (float)(secondmaxdist1.Y - map.bounds.centre.z) / MapReader.divider);
                Vector3 secondmaxA = new Vector3((float)(secondmaxdist2.X - map.bounds.centre.x) / MapReader.divider, 0, (float)(secondmaxdist2.Y - map.bounds.centre.z) / MapReader.divider);

                Vector3 maxDistanceVector = maxB - maxA;
                Vector3 secondmaxDistanceVector = secondmaxB - secondmaxA;

                float xScale = 1;
                float zScale = 1;
                if (maxDistanceVector.x > secondmaxDistanceVector.x && maxDistanceVector.x > maxDistanceVector.z)
                    xScale = maxDistanceVector.x;
                if (maxDistanceVector.x < secondmaxDistanceVector.x && secondmaxDistanceVector.x > secondmaxDistanceVector.z)
                    xScale = secondmaxDistanceVector.x;
                if (maxDistanceVector.z > secondmaxDistanceVector.z && maxDistanceVector.z > maxDistanceVector.x)
                    zScale = maxDistanceVector.z;
                if (maxDistanceVector.z < secondmaxDistanceVector.z && secondmaxDistanceVector.z > secondmaxDistanceVector.x)
                    zScale = secondmaxDistanceVector.x;*/
                
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
                //Array.Sort(distances);
                //Array.Reverse(distances);
                
                
                
                Vector3 centre = sum / (way.childrenIDs.Count - 1);
                centre.y = minBuildingHeight;
                
                /*float firstNodeX = (float)(firstNode.X - map.bounds.centre.x) / MapReader.divider;
                float firstNodeZ = (float)(firstNode.Y - map.bounds.centre.z) / MapReader.divider;                
                float secondNodeX = (float)(secondNode.X - map.bounds.centre.x) / MapReader.divider;
                float secondNodeZ = (float)(secondNode.Y - map.bounds.centre.z) / MapReader.divider;
                Vector3 A = new Vector3(firstNodeX,0, firstNodeZ);
                Vector3 B = new Vector3(secondNodeX, 0, secondNodeZ);*/
                Vector3 rot = sections[0];
                Vector3 v = sections[0];
                Vector3 u = sections[1];

                
                for (int i = 1; i < sections.Length; i++)
                {
                    if (Vector3.Angle(sections[i - 1], sections[i]) < 91 &&
                        Vector3.Angle(sections[i - 1], sections[i]) > 89)
                    {
                        v = sections[i - 1];
                        u = sections[i];
                    }
                }
                
                if (Mathf.Abs(v.x / v.z)> 1 && xDistances.Sum() / zDistances.Sum() > 1)
                {
                    rot = u;
                }                
                else if (Mathf.Abs(u.x / u.z)> 1 && xDistances.Sum() / zDistances.Sum() > 1)
                {
                    rot = v;
                }

                GameObject go = Instantiate(chosenPrefab, centre, Quaternion.LookRotation(rot,Vector3.up));
                //go.transform.localScale *= distances[1];
                go.transform.localScale = new Vector3(go.transform.localScale.x*xDistances[0], go.transform.localScale.y*floorHeight, go.transform.localScale.z*zDistances[0]);
                //v.y += 20;
                //u.y += 20;
                Debug.DrawRay(v,u,Color.white,100f);
                Vector3 angles = go.transform.rotation.eulerAngles;
                if (angles.y < 91 && angles.y > 85)
                {
                    angles.y += 90;
                    go.transform.rotation = Quaternion.Euler(angles);
                }
                if (angles.y < 122 && angles.y > 119)
                {
                    angles.y += 90;
                    go.transform.rotation = Quaternion.Euler(angles);
                }
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

    void ChoosePrefab(OSMWay way)
    {
        if (way.typeOfBuilding.Equals("shop"))
            chosenPrefab = shopPrefab;
        else if (way.typeOfBuilding.Equals("bungalow") || way.typeOfBuilding.Equals("bungalow") ||
                 way.typeOfBuilding.Equals("cabin") || way.typeOfBuilding.Equals("detached") ||
                 way.typeOfBuilding.Equals("farm") || way.typeOfBuilding.Equals("house") ||
                 way.typeOfBuilding.Equals("semidetached_house"))
        {
            chosenPrefab = housePrefab;
        }
        else if (way.typeOfBuilding.Equals("commercial") || way.typeOfBuilding.Equals("kiosk") ||
                 way.typeOfBuilding.Equals("retail") || way.typeOfBuilding.Equals("shop"))
        {
            chosenPrefab = shopPrefab;
        }
        else if (way.typeOfBuilding.Equals("fire_station"))
        {
            chosenPrefab = ospPrefab;
        }else if (way.typeOfBuilding.Equals("police"))
        {
            chosenPrefab = policePrefab;
        }else if (way.typeOfBuilding.Equals("bank"))
        {
            chosenPrefab = bankPrefab;
        }else if (way.typeOfBuilding.Equals("fuel"))
        {
            chosenPrefab = gasStationPrefab;
        }else if (way.typeOfBuilding.Equals("hotel"))
        {
            chosenPrefab = hotelPrefab;
        }else if (way.typeOfBuilding.Equals("apartments"))
        {
            chosenPrefab = apartmentsPrefab;
        }else if (way.typeOfBuilding.Equals("school"))
        {
            chosenPrefab = schoolPrefab;
        }
        else if (way.typeOfBuilding.Equals("yes")||way.typeOfBuilding.Equals("apartments"))
        {
            int index;
            if (way.floors > 2)
            {
                index = rnd.Next(0, 6);
                chosenPrefab = higherBuildingsPrefabs[index];
            }
            else
            {
                index = rnd.Next(0, 8);
                chosenPrefab = regularBuildingsPrefabs[index];
            }
        }
        else chosenPrefab = null;
    }
}
