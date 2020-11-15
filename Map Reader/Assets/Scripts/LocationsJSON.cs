using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public class LocationsJSON
{
    private static readonly HttpClient client = new HttpClient();
    public string locationsJsonString;
    public double[,] elevations;
    public LocationsJSON(List<double> Xs, List<double> Ys)
    {
        /*elevations = new double[Xs.Count, Ys.Count];
        for (int x = 0; x < elevations.GetLength(0); x++)
        {
            for (int y = 0; y < elevations.GetLength(1); y++)
            {
                elevations[x, y] = 0;
            }
        }*/
        //elevations = new double[(int)Mathf.Ceil((Xs.Count/5)+1), (int)Mathf.Ceil((Ys.Count/5)+1)];
        elevations = new double[Xs.Count,Ys.Count];
        for (int x = 0; x < elevations.GetLength(0); x++)
        {
            for (int y = 0; y < elevations.GetLength(1); y++)
            {
                elevations[x, y] = 0;
            }
        }
        int j = 0;

        for (int y = 0; y < Ys.Count; y++)
        {
            AllLocations alllocations = new AllLocations()
            {
                locations = new List<Coordinates>()
            };
            for (int x = 0; x < Xs.Count; x++)
            {
                Coordinates coordinate = new Coordinates()
                {
                    latitude = Ys[y],
                    longitude = Xs[x]
                };
                alllocations.locations.Add(coordinate);
            }
            locationsJsonString = JsonConvert.SerializeObject(alllocations);
            var postData = new StringContent(locationsJsonString, Encoding.UTF8, "application/json");
            var response = client.PostAsync("http://192.168.163.129:8080/api/v1/lookup", postData).Result;
            var responseString = response.Content.ReadAsStringAsync().Result;
            ResultList resultList;
            resultList = JsonConvert.DeserializeObject<ResultList>(responseString);
            for (int i = 0; i < elevations.GetLength(0); i++)
            {
                elevations[i, j] = resultList.results[i].elevation;
            }
            j++;
        }
        /*foreach (double y in Ys)
        {
            AllLocations alllocations = new AllLocations()
            {
                locations = new List<Coordinates>()
            };
            foreach (double x in Xs)
            {
                Coordinates coordinate = new Coordinates()
                {
                    latitude = y,
                    longitude = x
                };
                alllocations.locations.Add(coordinate);
            }
            locationsJsonString = JsonConvert.SerializeObject(alllocations);
            var postData = new StringContent(locationsJsonString, Encoding.UTF8, "application/json");
            var response = client.PostAsync("http://172.18.105.193:8080/api/v1/lookup", postData).Result;
            var responseString = response.Content.ReadAsStringAsync().Result;
            ResultList resultList;
            resultList = JsonConvert.DeserializeObject<ResultList>(responseString);
            
            for (int i = 0; i < Xs.Count; i++)
            {
                elevations[i, j] = resultList.results[i].elevation;
            }
            j++;
        }*/
    }
}

public class Coordinates
{
    public double latitude { get; set; }
    public double longitude { get; set; }
}

public class Results
{
    public double latitude { get; set; }
    public double elevation { get; set; }
    
    public double longitude { get; set; }
}

public class ResultList
{
    public List<Results> results { get; set; }
}
public class AllLocations
{
    public List<Coordinates> locations { get; set; }
}

