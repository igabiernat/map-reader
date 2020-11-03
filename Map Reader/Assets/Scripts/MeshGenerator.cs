using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class MeshGenerator : MonoBehaviour
{
    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;

    //public MapReader mapReader;

    private int xSize;
    private int zSize;
    // Start is called before the first frame update
    public void OnMapLoaded(OSMBounds bounds)
    {
        mesh = new Mesh();
        //mapReader = mapReader.GetComponent<MapReader>();
        GetComponent<MeshFilter>().mesh = mesh;
        xSize = (int)bounds.terrainXsize;
        zSize = (int)bounds.terrainYsize;
        CreateShape(bounds);
        UpdateMesh();
    }

    void CreateShape(OSMBounds bounds)
    {        
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        
        for (int i=0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                vertices[i] = new Vector3((float)bounds.terrainX + x,0, (float)bounds.terrainY + z);
                i++;
            }
        }

        triangles = new int[xSize * zSize * 6];
        int vert = 0;
        int tris = 0;

        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }
    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

}
