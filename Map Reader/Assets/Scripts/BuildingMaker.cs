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
    public Dictionary<string, GameObject> types;
    public GameObject chosenPrefab;
    
    public MeshCollider meshCollider;
    private float[] distances;
    private Vector3[] sections;
    private float floorHeight;
    private Random rnd = new Random();

    public List<Vector3> buildingVertices;

    IEnumerator Start()
    {
        map = GetComponent<MapReader>();
        while (!map.isReady)
            yield return null;
        foreach (var way in map.ways)
        {
            if (way.typeOfBuilding!=null && way.childrenIDs.Count>1)
            {
                MapNode firstNode = map.mapNodes[way.childrenIDs[0]];    //first point
                MapNode secondNode = map.mapNodes[way.childrenIDs[1]]; //second point
                Vector3 sum = Vector3.zero;
                buildingVertices = new List<Vector3>();
                buildingVertices.Add(new Vector3((float)firstNode.X-map.bounds.centre.x,0,(float)firstNode.Y-map.bounds.centre.z));

                ChoosePrefab(way);
                if (chosenPrefab == null) 
                    continue;
                /*if (chosenPrefab == housePrefab)
                    chosenPrefab.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                else if(chosenPrefab == policePrefab)
                    chosenPrefab.transform.localScale = new Vector3(0.12f, 0.12f, 0.06f);
                else
                    chosenPrefab.transform.localScale = new Vector3(0.12f,0.12f,0.12f);*/

                float minBuildingHeight = GetHeight((float) (firstNode.X- map.bounds.centre.x), (float) (firstNode.Y- map.bounds.centre.z));
                distances = new float[way.childrenIDs.Count-1];
                sections = new Vector3[way.childrenIDs.Count-1];
                
                for (int i = 1; i < way.childrenIDs.Count; i++)
                {
                    MapNode n1 = map.mapNodes[way.childrenIDs[i - 1]];
                    MapNode n2 = map.mapNodes[way.childrenIDs[i]];
                    
                    float x1 = (float) (n1.X - map.bounds.centre.x);
                    float z1 = (float) (n1.Y - map.bounds.centre.z);
                    float x2 = (float) (n2.X - map.bounds.centre.x);
                    float z2 = (float) (n2.Y - map.bounds.centre.z);
                    
                    Vector3 v1 = new Vector3(x1,0, z1);
                    Vector3 v2 = new Vector3(x2,0, z2);
                    
                    buildingVertices.Add(v2);

                    sum += v1;

                    float pointHeight = GetHeight(x2, z2);
                    if (pointHeight < minBuildingHeight)
                        minBuildingHeight = pointHeight;
                    
                    //float distance = Vector3.Distance(v1, v2);
                    //distances[i - 1] = distance;
                    sections[i - 1] = v2 - v1;
                    
                }

                float minX = buildingVertices.Min(vec => vec.x);
                //Vector3 minXvec = buildingVertices.First(vec => vec.x == minX);
                float maxX = buildingVertices.Max(vec => vec.x);
                //Vector3 maxXvec = buildingVertices.First(vec => vec.x == maxX);
                float minZ = buildingVertices.Min(vec => vec.z);
                //Vector3 minZvec = buildingVertices.First(vec => vec.z == minZ);
                float maxZ = buildingVertices.Max(vec => vec.z);
                //Vector3 maxZvec = buildingVertices.First(vec => vec.z == maxZ);
                
                
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
                
                Vector3 centre = sum / (way.childrenIDs.Count - 1);
                centre.y = minBuildingHeight;
                
                Vector3 rot = sections[0];
                Vector3 v = sections[0];
                Vector3 u = sections[1];

                
                for (int i = 1; i < sections.Length; i++)
                {
                    if (Vector3.Angle(sections[i - 1], sections[i]) > 89 && Vector3.Angle(sections[i - 1], sections[i])<91)
                    {
                        v = sections[i - 1];
                        u = sections[i];
                    }
                }
                
                if (Mathf.Abs(v.x / v.z)> 1)
                {
                    rot = u;
                }                
                else if (Mathf.Abs(u.x / u.z)> 1)
                {
                    rot = v;
                }

                GameObject go = Instantiate(chosenPrefab, centre, Quaternion.LookRotation(rot,Vector3.up));
                go.transform.localScale = new Vector3(go.transform.localScale.x*xDistances.Sum(),go.transform.localScale.y,go.transform.localScale.z*zDistances.Sum());
                //go.transform.localScale = new Vector3(go.transform.localScale.x*xDistances[0], go.transform.localScale.y*Mathf.Min(xDistances[0],zDistances[0]), go.transform.localScale.z*zDistances[0]);
                //go.transform.localScale = new Vector3(go.transform.localScale.x*(maxZvec-minXvec).magnitude,go.transform.localScale.y*Mathf.Min(xDistances[0],zDistances[0]),go.transform.localScale.x*());
                //go.transform.localScale = new Vector3(go.transform.localScale.x*(xDistances.Sum()/2), go.transform.localScale.y*20f, go.transform.localScale.z*zDistances.Sum()/2);
                //go.transform.localScale = new Vector3(go.transform.localScale.x*(maxX-minX), go.transform.localScale.y*Mathf.Min(xDistances[0],zDistances[0]), go.transform.localScale.x*(maxZ-minZ));
                Bounds modelBounds = go.GetComponent<MeshRenderer>().bounds;
                float scaleX = (maxX - minX) / modelBounds.size.x;
                float scaleZ = (maxZ - minZ) / modelBounds.size.z;
                
                go.transform.localScale = new Vector3(go.transform.localScale.x*scaleX,go.transform.localScale.y*20f, go.transform.localScale.z*scaleZ);
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
        try
        {
            chosenPrefab = Resources.Load<GameObject>("Buildings/" + way.typeOfBuilding);
        }
        catch(Exception e)
        {
            chosenPrefab = null;
        }

        /*
        if (way.typeOfBuilding.Equals("bungalow") || way.typeOfBuilding.Equals("bungalow") ||
            way.typeOfBuilding.Equals("cabin") || way.typeOfBuilding.Equals("detached") ||
            way.typeOfBuilding.Equals("farm") || way.typeOfBuilding.Equals("house") ||
            way.typeOfBuilding.Equals("semidetached_house"))
        {
            chosenPrefab = Resources.Load<GameObject>("Buildings/house");
        }*/
        if (way.typeOfBuilding.Equals("yes")||way.typeOfBuilding.Equals("apartments"))
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
        /*else if (way.typeOfBuilding.Equals("commercial") || way.typeOfBuilding.Equals("kiosk") ||
                  way.typeOfBuilding.Equals("retail") || way.typeOfBuilding.Equals("shop"))
        {
            chosenPrefab = shopPrefab;
        }*/

        /*if (way.typesOfBuilding.Contains("shop"))
            chosenPrefab = shopPrefab;
        /*else if (way.typeOfBuilding.Equals("bungalow") || way.typeOfBuilding.Equals("bungalow") ||
                 way.typeOfBuilding.Equals("cabin") || way.typeOfBuilding.Equals("detached") ||
                 way.typeOfBuilding.Equals("farm") || way.typeOfBuilding.Equals("house") ||
                 way.typeOfBuilding.Equals("semidetached_house"))
        {
            chosenPrefab = housePrefab;
        }#1#
        /*else if (way.typeOfBuilding.Equals("commercial") || way.typeOfBuilding.Equals("kiosk") ||
                 way.typeOfBuilding.Equals("retail") || way.typeOfBuilding.Equals("shop"))
        {
            chosenPrefab = shopPrefab;
        }#1#
        
        }else if (way.typesOfBuilding.Contains("apartments"))
        {
            chosenPrefab = apartmentsPrefab;
        }else if (way.typesOfBuilding.Contains("school"))
        {
            chosenPrefab = Resources.Load<GameObject>("Buildings/school");
        }
        else if (way.typesOfBuilding.Contains("yes")||way.typesOfBuilding.Contains("apartments"))
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
        else chosenPrefab = null;*/
    }
}
