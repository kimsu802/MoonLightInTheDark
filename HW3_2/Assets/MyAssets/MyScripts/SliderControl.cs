using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderControl : MonoBehaviour {
    public static bool isDoor = false;      //문 앞에 서있는가
    public static bool isFriend = false;    //동료 근처에 서있는가
    public static bool getKey = false;      //열쇠를 얻었는가
    public Slider Dslider;      //문 슬라이더
    public Slider Fslider;      //동료 구출 슬라이더

    // Use this for initialization
    void Start () {
        
        //처음엔 화면에 표시되지 않게 초기화
        Dslider.gameObject.SetActive(false);
        Fslider.gameObject.SetActive(false);

        Dslider.value = 0.0f;
        Fslider.value = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
        SliderInteract();
    }

    void SliderInteract()
    {
        //문에 접근했을 때
        if (isDoor && getKey)
        {
            if (Input.GetMouseButton(0))
            {
                Dslider.gameObject.SetActive(true);
                Dslider.value += (Time.deltaTime / 5);
                //게이지가 다 차면
                if (Dslider.value >= 1)
                {
                    Player.isUnlock = true;
                    Dslider.gameObject.SetActive(false);
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                Dslider.value = 0.0f;
                Dslider.gameObject.SetActive(false);
            }
        } else {
            Dslider.value = 0f;
            Dslider.gameObject.SetActive(false);
        }

        //동료에게 접근했을 때
        if (isFriend)
        {
            if (Input.GetMouseButton(0))
            {
                Fslider.gameObject.SetActive(true);
                Fslider.value += (Time.deltaTime / 5);
                //게이지가 다 차면
                if (Fslider.value >= 1)
                {
                    Fslider.gameObject.SetActive(false);
                    Friend.isSafe = true;
                    isFriend = false;
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                Fslider.value = 0.0f;
                Fslider.gameObject.SetActive(false);
            }
        } else {
            Fslider.value = 0f;
            Fslider.gameObject.SetActive(false);
        }
    }
}
