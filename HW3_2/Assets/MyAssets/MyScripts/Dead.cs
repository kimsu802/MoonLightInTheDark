using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Dead : MonoBehaviour {
    public GameObject abc = null;
    public GameObject bc = null;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        
	}

    void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.tag == "Monster")
        {
            AutoFade.LoadLevel("Fail", 0.5f, 0.5f, Color.black);

            abc.GetComponent<Player>().enabled = false;

            bc.GetComponent<Animator>().SetTrigger("Dead");
        }
    }
}

