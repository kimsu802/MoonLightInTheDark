using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            if (Input.GetButtonDown("Submit"))
                SceneManager.LoadScene("Main");
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.R))
                SceneManager.LoadScene("Title");
        }
	}
}
