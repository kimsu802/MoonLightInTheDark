using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendSpawn : MonoBehaviour {
    public Transform[] points;
    public GameObject friend;

    // Use this for initialization
    void Start()
    {
        points = GameObject.Find("FriendSpawn").GetComponentsInChildren<Transform>();
        CreateMap();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void CreateMap()
    {
        int rndP = Random.Range(1, points.Length);
        Instantiate(friend, points[rndP].position, Quaternion.identity);
        Debug.Log(points[rndP].name);
    }
}
