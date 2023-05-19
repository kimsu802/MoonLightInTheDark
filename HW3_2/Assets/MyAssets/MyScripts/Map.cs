using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {
    public GameObject cam;

	// Use this for initialization
	void Start () {
        //cam.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Instantiate(cam, new Vector3(0, 104.1f, 0.3f), Quaternion.Euler(90.0f,0,0));
            Destroy(this.gameObject);
        }
    }
}
