using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class RoadMaker : MonoBehaviour
{
    public Material roadMaterial;
    public MeshCollider mc;
    private MapReader map;
    public List<Vector3> ok  = new List<Vector3>();
    private float roadWidth;
    
    IEnumerator Start()
    {
        map = GetComponent<MapReader>();
        // Wait for the map to become ready
        while (!map.isReady)
        {
            yield return null;
        }

        // Iterate through the roads and build each one
        foreach (var way in map.ways)
        {
            if (way.isRoad)
            {
                if (way.isFootway)
                    roadWidth = 0.1f;
                else
                    roadWidth = 0.5f;
                
                GameObject go = new GameObject();
                
                Vector3 localOrigin = Vector3.zero;
                for (int i = 0; i < way.childrenIDs.Count; i++)
                {
                    localOrigin += map.mapNodes[way.childrenIDs[i]];
                }
                
                localOrigin /= way.childrenIDs.Count;
                //localOrigin -= map.bounds.centre;

                go.transform.position = (localOrigin-map.bounds.centre) / MapReader.divider;
                //go.transform.position = new Vector3(go.transform.position.x,GetHeight(go.transform.position.x, go.transform.position.z),go.transform.position.z);
                Mesh mesh =  go.AddComponent<MeshFilter>().mesh;
                MeshRenderer mr = go.AddComponent<MeshRenderer>();
                mr.material = roadMaterial;

                List<Vector3> vectors = new List<Vector3>();
                List<Vector3> normals = new List<Vector3>();
                List<Vector2> uvs = new List<Vector2>();
                List<int> indices = new List<int>();

                for (int i = 1; i < way.childrenIDs.Count; i++)
                {
                    MapNode p1 = map.mapNodes[way.childrenIDs[i - 1]];
                    MapNode p2 = map.mapNodes[way.childrenIDs[i]];

                    float x1 = (float) (p1.X - localOrigin.x) / MapReader.divider;
                    float z1 = (float) (p1.Y - localOrigin.z) / MapReader.divider;
                    float x2 = (float) (p2.X - localOrigin.x) / MapReader.divider;
                    float z2 = (float) (p2.Y - localOrigin.z) / MapReader.divider;
                    
                    Vector3 n1 = new Vector3(x1,0,z1);
                    Vector3 n2 = new Vector3(x2,0,z2);

                    if (vectors.Count > 2)
                    {
                        Vector3 difffirst = (n2 - n1).normalized;
                        var crossfirst = Vector3.Cross(difffirst, Vector3.up) * roadWidth;

                        Vector3 v1first = n1 + crossfirst;
                        Vector3 v2first = n1 - crossfirst;

                        v1first.y = GetHeight(localOrigin.x - map.bounds.centre.x + v1first.x,
                            localOrigin.z - map.bounds.centre.z + v1first.z);
                        v2first.y = GetHeight(localOrigin.x - map.bounds.centre.x + v2first.x,
                            localOrigin.z - map.bounds.centre.z + v2first.z);

                        Vector3 previousv3 = vectors[vectors.Count - 2];
                        Vector3 previousv4 = vectors[vectors.Count - 1];

                        vectors.Add(previousv3);
                        vectors.Add(previousv4);
                        vectors.Add(v1first);
                        vectors.Add(v2first);

                        uvs.Add(new Vector2(0, 0));
                        uvs.Add(new Vector2(1, 0));
                        uvs.Add(new Vector2(0, 1));
                        uvs.Add(new Vector2(1, 1));

                        int idx1f, idx2f, idx3f, idx4f;
                        idx4f = vectors.Count - 1;
                        idx3f = vectors.Count - 2;
                        idx2f = vectors.Count - 3;
                        idx1f = vectors.Count - 4;

                        // first triangle v1, v3, v2
                        indices.Add(idx1f);
                        indices.Add(idx3f);
                        indices.Add(idx2f);

                        indices.Add(idx3f);
                        indices.Add(idx4f);
                        indices.Add(idx2f);
                    }

                    float dist = Vector3.Distance(n1, n2);
                    /*int additionalvertices = 100;
                    if (dist > 2)
                        additionalvertices = 500;
                    if (dist > 5)
                        additionalvertices = 1000;*/
                    int additionalvertices = 100 * (int)Mathf.Ceil(dist);
                    Vector3 step = (n2 - n1) / additionalvertices;
                    for (int j = 1; j <= additionalvertices; j++)
                    {
                        Vector3 s1 = n1 + (j-1)*step;
                        Vector3 s2 = n1 + j*step;
                        //s1.y = GetHeight((localOrigin.x-map.bounds.centre.x + s1.x)/10, (localOrigin.z-map.bounds.centre.z + s1.z)/10) - go.transform.position.y;
                        //s2.y = GetHeight((localOrigin.x-map.bounds.centre.x + s2.x)/10, (localOrigin.z-map.bounds.centre.z + s2.z)/10) - go.transform.position.y;
                        
                        Vector3 diff = (s2 - s1).normalized;
        
                        // https://en.wikipedia.org/wiki/Lane
                        // According to the article, it's 3.7m in Canada
                        var cross = Vector3.Cross(diff, Vector3.up) * roadWidth;
        
                        // Create points that represent the width of the road
                        Vector3 v1 = s1 + cross;
                        Vector3 v2 = s1 - cross;
                        Vector3 v3 = s2 + cross;
                        Vector3 v4 = s2 - cross;

                        v1.y = GetHeight(localOrigin.x-map.bounds.centre.x + v1.x, localOrigin.z-map.bounds.centre.z + v1.z);
                        v2.y = GetHeight(localOrigin.x-map.bounds.centre.x + v2.x, localOrigin.z-map.bounds.centre.z + v2.z);
                        v3.y = GetHeight(localOrigin.x-map.bounds.centre.x + v3.x, localOrigin.z-map.bounds.centre.z + v3.z);
                        v4.y = GetHeight(localOrigin.x-map.bounds.centre.x + v4.x, localOrigin.z-map.bounds.centre.z + v4.z);
                    
                        vectors.Add(v1);
                        vectors.Add(v2);
                        vectors.Add(v3);
                        vectors.Add(v4);
        
                        uvs.Add(new Vector2(0, 0));
                        uvs.Add(new Vector2(1, 0));
                        uvs.Add(new Vector2(0, 1));
                        uvs.Add(new Vector2(1, 1));
        
                        int idx1, idx2, idx3, idx4;
                        idx4 = vectors.Count - 1;
                        idx3 = vectors.Count - 2;
                        idx2 = vectors.Count - 3;
                        idx1 = vectors.Count - 4;
        
                        // first triangle v1, v3, v2
                        indices.Add(idx1);
                        indices.Add(idx3);
                        indices.Add(idx2);
        
                        // second         v3, v4, v2
                        indices.Add(idx3);
                        indices.Add(idx4);
                        indices.Add(idx2);
                    }
                } 
                /*Vector3 s1 = (p1 - localOrigin) / MapReader.divider;
                    Vector3 s2 = (p2 - localOrigin) / MapReader.divider;

                    Vector3 diff = (s2 - s1).normalized;

                    // https://en.wikipedia.org/wiki/Lane
                    // According to the article, it's 3.7m in Canada
                    var cross = Vector3.Cross(diff, Vector3.up) * 0.5f;

                    // Create points that represent the width of the road
                    Vector3 v1 = s1 + cross;
                    Vector3 v2 = s1 - cross;
                    Vector3 v3 = s2 + cross;
                    Vector3 v4 = s2 - cross;

                    v1.y = GetHeight(v1.x, v1.z) + 0.1f;
                    v2.y = GetHeight(v2.x, v2.z) + 0.1f;
                    v3.y = GetHeight(v3.x, v3.z) + 0.1f;
                    v4.y = GetHeight(v4.x, v4.z) + 0.1f;

                    vectors.Add(v1);
                    vectors.Add(v2);
                    vectors.Add(v3);
                    vectors.Add(v4);

                    uvs.Add(new Vector2(0, 0));
                    uvs.Add(new Vector2(1, 0));
                    uvs.Add(new Vector2(0, 1));
                    uvs.Add(new Vector2(1, 1));

                    normals.Add(Vector3.up);
                    normals.Add(Vector3.up);
                    normals.Add(Vector3.up);
                    normals.Add(Vector3.up);

                    int idx1, idx2, idx3, idx4;
                    idx4 = vectors.Count - 1;
                    idx3 = vectors.Count - 2;
                    idx2 = vectors.Count - 3;
                    idx1 = vectors.Count - 4;

                    // first triangle v1, v3, v2
                    indices.Add(idx1);
                    indices.Add(idx3);
                    indices.Add(idx2);

                    // second         v3, v4, v2
                    indices.Add(idx3);
                    indices.Add(idx4);
                    indices.Add(idx2);*/


                //mesh.Clear();
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
        float height = 0;
        RaycastHit hit;
        Ray ray = new Ray(new Vector3(x/10,MeshGenerator.maxHeight,z/10),Vector3.down );
        if (mc.Raycast(ray, out hit, Mathf.Infinity))
        {
            height = hit.point.y;
            Debug.Log(height);
        }
        //Debug.DrawRay(new Vector3(x/10,height+0.01f, z/10),Vector3.up, Color.blue ,100f);
        return(height+0.01f);
    }
}