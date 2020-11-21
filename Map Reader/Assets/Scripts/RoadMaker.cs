using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class RoadMaker : MonoBehaviour
{
    public Material roadMaterial;
    public MeshCollider mc;
    private MapReader map;
    private float roadWidth;
    
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
            if (way.isRoad)
            {
                if (way.isFootway)
                    roadWidth = 1f;
                else
                    roadWidth = 3.5f;
                
                GameObject go = new GameObject();
                
                Mesh mesh =  go.AddComponent<MeshFilter>().mesh;
                MeshRenderer mr = go.AddComponent<MeshRenderer>();
                mr.material = roadMaterial;

                vectors = new List<Vector3>();
                normals = new List<Vector3>();
                uvs = new List<Vector2>();
                indices = new List<int>();

                for (int i = 1; i < way.childrenIDs.Count; i++)
                {
                    MapNode p1 = map.mapNodes[way.childrenIDs[i - 1]];
                    MapNode p2 = map.mapNodes[way.childrenIDs[i]];

                    float x1 = (float) (p1.X - map.bounds.centre.x);
                    float z1 = (float) (p1.Y - map.bounds.centre.z);
                    float x2 = (float) (p2.X - map.bounds.centre.x);
                    float z2 = (float) (p2.Y - map.bounds.centre.z);
                    
                    Vector3 n1 = new Vector3(x1,0,z1);
                    Vector3 n2 = new Vector3(x2,0,z2);

                    if (vectors.Count > 2)
                    {
                        Vector3 difffirst = (n2 - n1).normalized;
                        var crossfirst = Vector3.Cross(difffirst, Vector3.up) * roadWidth;

                        Vector3 v1first = n1 + crossfirst;
                        Vector3 v2first = n1 - crossfirst;

                        v1first.y = GetHeight(v1first.x, v1first.z)+0.8f;
                        v2first.y = GetHeight(v2first.x, v2first.z)+0.8f;

                        if (v1first.y < 0 || v2first.y < 0)
                            continue;
                        
                        Vector3 previousv3 = vectors[vectors.Count - 2];
                        Vector3 previousv4 = vectors[vectors.Count - 1];
                        
                        AddtoMesh(previousv3, previousv4, v1first, v2first);
                    }

                    float dist = Vector3.Distance(n1, n2);
                    int additionalvertices = 10 * (int)Mathf.Ceil(dist);
                    Vector3 step = (n2 - n1) / additionalvertices;
                    for (int j = 1; j <= additionalvertices; j++)
                    {
                        Vector3 s1 = n1 + (j-1)*step;
                        Vector3 s2 = n1 + j*step;
                        
                        Vector3 diff = (s2 - s1).normalized;
        
                        var cross = Vector3.Cross(diff, Vector3.up) * roadWidth;
        
                        // Create points that represent the width of the road
                        Vector3 v1 = s1 + cross;
                        Vector3 v2 = s1 - cross;
                        Vector3 v3 = s2 + cross;
                        Vector3 v4 = s2 - cross;

                        v1.y = GetHeight(v1.x, v1.z)+0.8f;
                        v2.y = GetHeight(v2.x, v2.z)+0.8f;
                        v3.y = GetHeight(v3.x, v3.z)+0.8f;
                        v4.y = GetHeight(v4.x, v4.z)+0.8f;

                        if (v1.y < 0 || v2.y < 0 || v3.y < 0 || v4.y < 0)
                            continue;
                    
                        AddtoMesh(v1,v2,v3,v4);
                    }
                }

                /*if ((vectors[0] - vectors[vectors.Count - 1]).magnitude < 5f && vectors.Count > 2)
                {
                    Vector3 fillGapv1 = vectors[vectors.Count - 1];
                    Vector3 fillGapv2 = vectors[vectors.Count - 2];
                    Vector3 fillGapv3 = vectors[0];
                    Vector3 fillGapv4 = vectors[1];
                    AddtoMesh(fillGapv1,fillGapv2,fillGapv3,fillGapv4);
                }*/

                mesh.vertices = vectors.ToArray();
                mesh.normals = normals.ToArray();
                mesh.triangles = indices.ToArray();
                mesh.uv = uvs.ToArray();
                mesh.RecalculateNormals();
                mr.material = roadMaterial;
            }
        }
    }
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

    void AddtoMesh(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
    {
        vectors.Add(v1);
        vectors.Add(v2);
        vectors.Add(v3);
        vectors.Add(v4);
                    
        int id1, id2, id3, id4;
        id4 = vectors.Count - 1;
        id3 = vectors.Count - 2;
        id2 = vectors.Count - 3;
        id1 = vectors.Count - 4;
                    
        indices.Add(id1);
        indices.Add(id3);
        indices.Add(id2);
        
        indices.Add(id3);
        indices.Add(id4);
        indices.Add(id2);
                
        uvs.Add(new Vector2(0, 0));
        uvs.Add(new Vector2(1, 0));
        uvs.Add(new Vector2(0, 1));
        uvs.Add(new Vector2(1, 1));
                
        normals.Add(Vector3.up);
        normals.Add(Vector3.up);
        normals.Add(Vector3.up);
        normals.Add(Vector3.up);
    }
}