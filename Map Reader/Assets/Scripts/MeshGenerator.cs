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
    public List<double> Xs = new List<double>();
    public List<double> Ys = new List<double>();
    public double[,] elevations;
    
    double height;

    public LocationsJSON locationsJson;

    private int xSize;
    private int zSize;
    // Start is called before the first frame update
    public void OnMapLoaded(OSMBounds bounds)
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        xSize = (int)Mathf.Floor((float)(bounds.terrainXsize/5));
        zSize = (int)Mathf.Floor((float)(bounds.terrainYsize/5));
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
                vertices[i] = new Vector3((float)bounds.terrainX + 5*x,(float)(elevations[x,z]-minHeight)/(MapReader.divider/2), (float)bounds.terrainY + 5*z);
                i++;
            }
        }
        /*vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                if (x % 2 == 1 && z % 2 != 1)
                    height = (elevations[(x - 1) / 2, z / 2] + elevations[(x + 1) / 2, z / 2]) / 2;
                if (x % 2 != 1 && z % 2 == 1)
                    height = (elevations[x / 2, (z - 1) / 2] + elevations[x / 2, (z + 1) / 2]) / 2;
                if (x % 2 == 1 && z % 2 == 1)
                    height = (elevations[(x - 1) / 2, (z - 1) / 2] + elevations[(x + 1) / 2, (z + 1) / 2]) / 2;
                if (x % 2 != 1 && z % 2 != 1)
                    height = elevations[x / 2, z / 2];
                vertices[i] = new Vector3((float) bounds.terrainX + 2.5f * x, (float) (height - 250) / 5,
                    (float) bounds.terrainY + 2.5f * z);
                i++;
            }
        }*/
        /*vertices = new Vector3[(xSize + 1) * (zSize*2 + 1)];
        for (int i=0, z = 0; z <= zSize*2; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                if (z % 2 == 1)
                    height = (elevations[x, (z - 1) / 2] + elevations[x, (z + 1) / 2])/2;
                else
                    height = elevations[x, z / 2];
                vertices[i] = new Vector3((float)bounds.terrainX + 5f*x,(float)(height-250)/5, (float)bounds.terrainY + 2.5f*z);
                i++;
            }
        }*/
        /*
        for (int z = 1; z <= zSize; z=z+2)
        {
            for (int x = 0; x <= xSize; x++)
            {
                double height = (elevations[x, z - 1] + elevations[x, z + 1])/2;
                vertices[z*xSize + x] = new Vector3((float)bounds.terrainX + 5*x,(float) (height-250)/MapReader.divider, (float)bounds.terrainY + 5*z);
            }
        }
        */

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
        double stepX = deltaX / bounds.terrainXsize;
        double stepY = deltaY / bounds.terrainYsize;
        
        for (int x = 0; x <= Math.Ceiling(bounds.terrainXsize); x=x+5)
        {
            Xs.Add(minX + stepX*x);
        }
        for (int y = 0; y <= Math.Ceiling(bounds.terrainYsize); y=y+5)
        {
            Ys.Add(minY + stepY*y);
        }
        locationsJson = new LocationsJSON(Xs, Ys);
        return (locationsJson.elevations);
    }

    void Interpolate(OSMBounds bounds)
    {
        elevations = GetElevation(bounds);
        //allElevations = new double[Xs.Count,Ys.Count];
        /*for (int y = 0; y < Ys.Count; y++)
        {
            for (int x = 0; x < Xs.Count; x++)
            {
                allElevations[x, y] = 0;
            }
        }

        for (int y = 0; y < (Ys.Count - (Ys.Count % 5)); y++)
        {
            for (int x = 0; x < (Xs.Count - (Xs.Count % 5)); x++)
            {
                if (x % 5 == 0 && y % 5 == 0)
                {
                    allElevations[x, y] = elevations[x / 5, y / 5];
                }
                else
                {
                    double previousValue = elevations[(x - (x % 5)) / 5, (y - (y % 5)) / 5];
                    double nextValue = elevations[(x + (5 - (x % 5))) / 5, (y + (5 - (y % 5))) / 5];
                    double delta = (nextValue - previousValue) / 5;
                    allElevations[x, y] = previousValue + (x % 5 * delta);
                }
            }
        }*/
    }
    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        MeshSmoothing.LaplacianFilter(mesh, 2);
        mesh.RecalculateNormals();
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

}
