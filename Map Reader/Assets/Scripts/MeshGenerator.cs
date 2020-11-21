using System;
using System.Collections;
using System.Collections.Generic;
using mattatz.MeshSmoothingSystem;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Newtonsoft.Json;

public class MeshGenerator : MonoBehaviour
{
    public static float minHeight;
    public static float maxHeight;
    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;
    private List<double> Xs = new List<double>();
    private List<double> Ys = new List<double>();
    private double[,] elevations;
    private int optimizer = 50;
    private double xstep;
    private double zstep;
    public Material groundMaterial;
    private MeshRenderer meshRenderer;
    
    double height;

    public LocationsJSON locationsJson;

    private int xSize;
    private int zSize;
    // Start is called before the first frame update
    public void OnMapLoaded(OSMBounds bounds)
    {
        mesh = new Mesh();
        meshRenderer = GetComponent<MeshRenderer>();
        GetComponent<MeshFilter>().mesh = mesh;
        xSize = optimizer+1;
        zSize = optimizer+1;
        xstep = bounds.terrainXsize / optimizer;
        zstep = bounds.terrainYsize / optimizer;
        Interpolate(bounds);
        CreateShape(bounds);
        UpdateMesh();
    }

    void CreateShape(OSMBounds bounds)
    {        
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        FindMinMax(elevations);
        
        for (int i=0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                vertices[i] = new Vector3((float)(bounds.terrainX + xstep*x),(float)(elevations[x,z]-minHeight), (float)(bounds.terrainY + zstep*z));
                i++;
            }
        }

        triangles = new int[xSize* zSize * 6];
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

    void FindMinMax(double[,] arr)
    {
        double min = arr[0, 0];
        double max = arr[0, 0];
        for (int z = 0; z < arr.GetLength(1); z++)
        {
            for (int x = 0; x < arr.GetLength(0); x++)
            {
                if (arr[x, z] < min)
                    min = arr[x, z];
                if (arr[x, z] > max)
                    max = arr[x, z];
            }
        }
        minHeight = (float)min;
        maxHeight = (float)max;
    }
    double[,] GetElevation(OSMBounds bounds)
    {
        double maxX = bounds.maxLon;
        double minX = bounds.minLon;
        double maxY = bounds.maxLat;
        double minY = bounds.minLat;

        double deltaX = maxX - minX;
        double deltaY = maxY - minY;
        double stepX = deltaX / optimizer;
        double stepY = deltaY / optimizer;
        
        for (int x = 0; x <= optimizer+1; x++)
        {
            Xs.Add(minX + stepX*x);
        }
        for (int y = 0; y <= optimizer+1; y++)
        {
            Ys.Add(minY + stepY*y);
        }
        locationsJson = new LocationsJSON(Xs, Ys);
        return (locationsJson.elevations);
    }

    void Interpolate(OSMBounds bounds)
    {
        elevations = GetElevation(bounds);
    }
    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        MeshSmoothing.LaplacianFilter(mesh, 2);
        mesh.RecalculateNormals();
        GetComponent<MeshCollider>().sharedMesh = mesh;
        meshRenderer.material = groundMaterial;
    }

}
