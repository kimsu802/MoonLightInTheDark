using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour {
    public float scrollSpeed = 1.2f;
    private Material mat;

	// Use this for initialization
	void Start () {
        mat = GetComponent<Renderer>().material;
	}
	
	// Update is called once per frame
	void Update () {
        Vector2 newOffset = mat.mainTextureOffset;
        newOffset.Set(0, newOffset.y + (scrollSpeed * Time.deltaTime));
        mat.mainTextureOffset = newOffset;
	}
}
