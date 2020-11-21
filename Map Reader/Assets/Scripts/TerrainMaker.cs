using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainMaker : MonoBehaviour
{
    public Material terrainMaterial;
    public MeshCollider mc;
    private MapReader map;

    IEnumerator Start()
    {
        map = GetComponent<MapReader>();
        while (!map.isReady)
        {
            yield return null;
        }

        foreach (var way in map.ways)
        {
            if (way.isTerrain)
            {
                GameObject go = new GameObject();

                Vector3 localOrigin = Vector3.zero;
                for (int i = 0; i < way.childrenIDs.Count; i++)
                {
                    localOrigin += map.mapNodes[way.childrenIDs[i]];
                }

                localOrigin /= way.childrenIDs.Count;
                float localOriginx = (localOrigin.x-map.bounds.centre.x) / MapReader.divider;
                float localOriginz = (localOrigin.z-map.bounds.centre.z) / MapReader.divider;
                float localOriginy = GetHeight(localOriginx, localOriginz);
                go.transform.position = new Vector3(localOriginx,localOriginy, localOriginz);
                
                Mesh mesh = go.AddComponent<MeshFilter>().mesh;
                MeshRenderer mr = go.AddComponent<MeshRenderer>();
                mr.material = terrainMaterial; //TODO: check what terrain and apply material

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

                    Vector3 n1 = new Vector3(x1, 0, z1);
                    Vector3 n2 = new Vector3(x2, 0, z2);

                    n1.y = GetHeight(localOrigin.x - map.bounds.centre.x + x1, localOrigin.z - map.bounds.centre.z + z1);
                    n2.y = GetHeight(localOrigin.x - map.bounds.centre.x + x2, localOrigin.z - map.bounds.centre.z + z2);

                    vectors.Add((localOrigin - map.bounds.centre)/(MapReader.divider*MapReader.divider));
                    vectors.Add(n1);
                    vectors.Add(n2);

                    uvs.Add(new Vector2(0, 0));
                    uvs.Add(new Vector2(1, 0));
                    uvs.Add(new Vector2(0, 1));

                    int idx1, idx2, idx3;
                    idx3 = vectors.Count - 1;
                    idx2 = vectors.Count - 2;
                    idx1 = vectors.Count - 3;

                    indices.Add(idx1);
                    indices.Add(idx2);
                    indices.Add(idx3);
                }

                mesh.vertices = vectors.ToArray();
                mesh.triangles = indices.ToArray();
                mesh.uv = uvs.ToArray();
                mesh.RecalculateNormals();
                mr.material = terrainMaterial;
            }
        }
    }

    float GetHeight(float x, float z)
    {
        float height = 0;
        RaycastHit hit;
        Ray ray = new Ray(new Vector3(x,MeshGenerator.maxHeight,z),Vector3.down );
        if (mc.Raycast(ray, out hit, Mathf.Infinity))
        {
            height = hit.point.y;
        }
        //Debug.DrawRay(new Vector3(x,height+0.01f, z),Vector3.up, Color.blue ,100f);
        return(height+0.005f);
    }
}