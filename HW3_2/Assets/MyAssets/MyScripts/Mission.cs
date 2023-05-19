using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mission : MonoBehaviour {
    public Text t1;     //첫번째 목표 텍스트
    public Text t2;     //두번째 목표 텍스트

	// Use this for initialization
	void Start () {
        t2.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		if(Friend.isSafe)
        {
            t1.gameObject.SetActive(false);
            t2.gameObject.SetActive(true);
        }
	}
}
