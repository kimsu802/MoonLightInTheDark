using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSpawn : MonoBehaviour {
    public Transform[] points;
    public GameObject map;

    // Use this for initialization
    void Start()
    {
        points = GameObject.Find("MapSpawn").GetComponentsInChildren<Transform>();
        CreateMap();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void CreateMap()
    {
        int rndP = Random.Range(1, points.Length);
        Instantiate(map, points[rndP].position, Quaternion.identity);
        Debug.Log(points[rndP].name);
    }
}
