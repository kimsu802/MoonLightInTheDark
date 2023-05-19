using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Friend : MonoBehaviour {
    public RuntimeAnimatorController cry;   //구출 전 애니메이션
    public RuntimeAnimatorController move;  //구출 후 애니메이션
    public static bool isSafe = false;      //구출 상태
    private GameObject target;              //쫓아갈 대상(플레이어)
    public float moveSpeed = 5f;            //이동속도
    Animator animator;
    CharacterController characterController;

    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = cry;   //기본 애니메이션은 구출 전으로
        characterController = GetComponent<CharacterController>();
    }
	
	// Update is called once per frame
	void Update () {
        if(isSafe)
        {
            GetComponent<BoxCollider>().enabled = false;
            animator.runtimeAnimatorController = move;  //구출 후 애니메이션으로 전환
            FollowTarget();
        }
	}

    void FollowTarget()
    {
        target = GameObject.FindGameObjectWithTag("Player").gameObject;
        Vector3 distance = target.transform.position - this.transform.position;

        //캐릭터 컨트롤러를 이용한 이동
        Vector3 framePos = Vector3.MoveTowards(transform.position, target.transform.position, moveSpeed * Time.deltaTime);  //프레임 1당 이동할 거리량
        Vector3 frameDir = framePos - transform.position;   //이동할 거리를 프레임마다 갱신

        //플레이어와 동료 사이의 거리가 1이상 벌어지면
        if (distance.sqrMagnitude > 1)
        {
            characterController.Move(frameDir);     //플레이어를 향해 이동
            animator.SetFloat("Speed", characterController.velocity.magnitude);
        }
        transform.LookAt(target.transform);
    }
}
