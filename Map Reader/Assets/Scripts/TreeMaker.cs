using System.Collections;
using System.Xml;
using UnityEngine;
using Random = System.Random;
public class TreeMaker : MonoBehaviour
{
    public MeshCollider mc;
    private MapReader map;
    private Random rnd = new Random();
    public GameObject[] trees;

    IEnumerator Start()
    {
        map = GetComponent<MapReader>();
        // Wait for the map to become ready
        while (!map.isReady)
        {
            yield return null;
        }

        foreach (var treeNode in map.trees)
        {
            Vector3 tree = Vector3.zero;
            tree += treeNode;
            tree -= map.bounds.centre;
            tree.y = GetHeight(tree.x, tree.z);

            GameObject go = Instantiate(trees[rnd.Next(0,trees.Length-1)],tree,Quaternion.LookRotation(tree,Vector3.up));
            go.transform.localScale *= 2;
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
        return(height);
    }
}
