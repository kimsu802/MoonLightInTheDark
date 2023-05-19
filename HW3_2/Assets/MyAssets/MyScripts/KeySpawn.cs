using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeySpawn : MonoBehaviour {
    public Transform[] points;
    public GameObject[] keys;

	// Use this for initialization
	void Start () {
        points = GameObject.Find("KeySpawn").GetComponentsInChildren<Transform>();
        CreateKey();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void CreateKey()
    {
        int rndP = Random.Range(1, points.Length);
        int rndK = Random.Range(0, keys.Length);
        Instantiate(keys[rndK], points[rndP].position, Quaternion.identity);
        Debug.Log(points[rndP].name);
        Debug.Log(keys[rndK].name);
    }
}
