using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Analytics;

public class WaterMaker : MonoBehaviour
{
    public Material waterMaterial;
    public MeshCollider mc;
    private MapReader map;
    private List<Vector3> vectors;
    private List<Vector3> normals;
    private List<Vector2> uvs;
    private List<int> indices;

    IEnumerator Start()
    {
        map = GetComponent<MapReader>();
        while (!map.isReady)
        {
            yield return null;
        }

        foreach (var way in map.ways)
        {
            if (way.typeOfNatural == "water")
            {
                GameObject go = new GameObject();

                Vector3 localOrigin = Vector3.zero;

                for (int i = 1; i < way.childrenIDs.Count; i++)
                {
                    CheckIfOutsideBounds(map.mapNodes[way.childrenIDs[i]]);
                }

                for (int i = 0; i < way.childrenIDs.Count; i++)
                {
                    localOrigin += map.mapNodes[way.childrenIDs[i]];
                }

                localOrigin /= way.childrenIDs.Count;
                localOrigin -= map.bounds.centre;

                Mesh mesh = go.AddComponent<MeshFilter>().mesh;
                MeshRenderer mr = go.AddComponent<MeshRenderer>();

                //if (way.isParking)
                    //mr.material = parkingMaterial;
                //else

                vectors = new List<Vector3>();
                normals = new List<Vector3>();
                uvs = new List<Vector2>();
                indices = new List<int>();

                for (int i = 1; i < way.childrenIDs.Count; i++)
                {
                    float h = 0.5f;
                    if (way.typeOfTerrain == "parking")
                        h = 0.6f;
                    MapNode p1 = map.mapNodes[way.childrenIDs[i - 1]];
                    MapNode p2 = map.mapNodes[way.childrenIDs[i]];
                    
                    //CheckIfOutsideBounds(p1);
                    //CheckIfOutsideBounds(p2);

                    float x1 = (float) (p1.X - map.bounds.centre.x);
                    float z1 = (float) (p1.Y - map.bounds.centre.z);
                    float x2 = (float) (p2.X - map.bounds.centre.x);
                    float z2 = (float) (p2.Y - map.bounds.centre.z);

                    /*Vector3 n1 = CheckIfOutsideBounds(new Vector3(x1, 0, z1));
                    Vector3 n2 = CheckIfOutsideBounds(new Vector3(x2, 0, z2));
                    Vector3 n12 = CheckIfOutsideBounds((n2 + n1) / 2);*/

                    Vector3 n1 = new Vector3(x1, 0, z1);
                    Vector3 n2 = new Vector3(x2, 0, z2);
                    Vector3 n12 = (n2 + n1) / 2;

                    Vector3 stepn1 = (n1 - localOrigin) / 20;
                    Vector3 stepn2 = (n2 - localOrigin) / 20;
                    Vector3 stepn12 = (n12 - localOrigin) / 20;

                    Vector3 newv1 = localOrigin + stepn1;
                    Vector3 newv2 = localOrigin + stepn2;
                    Vector3 newv12 = localOrigin + stepn12;
                    
                    //First triangle from localOrigin 
                    localOrigin.y = GetHeight(localOrigin.x, localOrigin.z) + h;
                    newv1.y = GetHeight(newv1.x, newv1.z) + h;
                    newv12.y = GetHeight(newv12.x, newv12.z) + h;

                    //if (localOrigin.y < 0 || newv1.y < 0 || newv2.y < 0)
                        //continue;

                    AddtoMesh(localOrigin, newv1, newv12);

                    for (int j = 1; j <= 40; j++)
                    {
                        if (j % 2 == 1)
                        {
                            Vector3 prev1 = vectors[vectors.Count - 2]; //last v1
                            Vector3 prev2 = vectors[vectors.Count - 1]; //last v2
                            Vector3 thisone = prev1 + stepn1; //new v1
                            prev1.y = GetHeight(prev1.x, prev1.z) + h;
                            prev2.y = GetHeight(prev2.x, prev2.z) + h;
                            thisone.y = GetHeight(thisone.x, thisone.z) + h;
                            //if (prev1.y < 0 || prev2.y < 0 || thisone.y < 0)
                                //continue;
                            AddtoMesh(prev1, prev2, thisone);
                        }
                        else
                        {
                            Vector3 prev2 = vectors[vectors.Count - 2]; //last v2
                            Vector3 prev1 = vectors[vectors.Count - 1]; //last v1 aka newv1 from before
                            Vector3 thisone = prev2 + stepn12;
                            prev2.y = GetHeight(prev2.x, prev2.z) + h;
                            prev1.y = GetHeight(prev1.x, prev1.z) + h;
                            thisone.y = GetHeight(thisone.x, thisone.z) + h;
                            //if (prev1.y < 0 || prev2.y < 0 || thisone.y < 0)
                                //continue;
                            AddtoMesh(prev2, prev1, thisone);
                        }
                        
                    }
                    localOrigin.y = GetHeight(localOrigin.x, localOrigin.z) + h;
                    newv12.y = GetHeight(newv12.x, newv12.z) + h;
                    newv2.y = GetHeight(newv2.x, newv2.z) + h;
                    
                    //if (localOrigin.y < 0 || newv12.y < 0 || newv2.y < 0)
                        //continue;
                    
                    AddtoMesh(localOrigin, newv12, newv2);

                    for (int j = 1; j <= 40; j++)
                    {
                        if (j % 2 == 1)
                        {
                            Vector3 prev1 = vectors[vectors.Count - 2]; //last v1
                            Vector3 prev2 = vectors[vectors.Count - 1]; //last v2
                            Vector3 thisone = prev1 + stepn12; //new v1
                            prev1.y = GetHeight(prev1.x, prev1.z) + h;
                            prev2.y = GetHeight(prev2.x, prev2.z) + h;
                            thisone.y = GetHeight(thisone.x, thisone.z) + h;
                            //if (prev1.y < 0 || prev2.y < 0 || thisone.y < 0)
                                //continue;
                            AddtoMesh(prev1, prev2, thisone);
                        }
                        else
                        {
                            Vector3 prev2 = vectors[vectors.Count - 2]; //last v2
                            Vector3 prev1 = vectors[vectors.Count - 1]; //last v1 aka newv1 from before
                            Vector3 thisone = prev2 + stepn2;
                            prev2.y = GetHeight(prev2.x, prev2.z) + h;
                            prev1.y = GetHeight(prev1.x, prev1.z) + h;
                            thisone.y = GetHeight(thisone.x, thisone.z) + h;
                            //if (prev1.y < 0 || prev2.y < 0 || thisone.y < 0)
                                //continue;
                            AddtoMesh(prev2, prev1, thisone);
                        }
                    }
                }
                mesh.vertices = vectors.ToArray();
                mesh.triangles = indices.ToArray();
                //mesh.uv = uvs.ToArray();
                mesh.RecalculateNormals();
                mr.material = waterMaterial;
            }
        }
    }

    void AddtoMesh(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        vectors.Add(v1);
        vectors.Add(v2);
        vectors.Add(v3);
        vectors.Add(v1);
        vectors.Add(v2);
        vectors.Add(v3);
        indices.Add(vectors.Count - 2);
        indices.Add(vectors.Count - 3);
        indices.Add(vectors.Count - 1);
        indices.Add(vectors.Count - 6);
        indices.Add(vectors.Count - 5);
        indices.Add(vectors.Count - 4);
    }
    
    void CheckIfOutsideBounds(MapNode terrainNode)
    {
        if (terrainNode.X - map.bounds.maxX>= -5f)
            terrainNode.X = map.bounds.maxX-5f;
        if (terrainNode.Y - map.bounds.maxY>= -5f)
            terrainNode.Y = map.bounds.maxY-5f;
        if (terrainNode.X - map.bounds.minX<= 30f)
            terrainNode.X = map.bounds.minX+30f;
        if (terrainNode.Y - map.bounds.minY<= 30f)
            terrainNode.Y = map.bounds.minY+30f;
    }
    /*Vector3 CheckIfOutsideBounds(Vector3 v)
    {
        if (v.x >= map.bounds.terrainX+map.bounds.terrainXsize-5f)
            v.x = (float) (map.bounds.terrainX+map.bounds.terrainXsize)-10f;
        if (v.z >= map.bounds.terrainY+map.bounds.terrainYsize-5f)
            v.z = (float) (map.bounds.terrainY+map.bounds.terrainYsize)-10f;
        if (v.x <= map.bounds.terrainX+5f)
            v.x = (float) map.bounds.terrainX+10f;
        if (v.z <= map.bounds.terrainY+5f)
            v.z = (float) map.bounds.terrainY+10f;

        return v;
    }*/
    
    
    float GetHeight(float x, float z)
    {
        float height;
        RaycastHit hit;
        Ray ray = new Ray(new Vector3(x,MeshGenerator.maxHeight,z),Vector3.down);
        if (mc.Raycast(ray, out hit, Mathf.Infinity))
        {
            height = hit.point.y;
        }
        else
        {
            height = -2f;
        }
        return(height);
    }
}