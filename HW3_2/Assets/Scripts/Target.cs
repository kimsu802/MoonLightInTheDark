using UnityEngine;
using System.Collections;

// 타깃 클래스: Bullet 클래스를 포함한 GameObject가 닿으면 파괴, 모든 타깃이 파괴되면 스테이지를 종료
public class Target	: MonoBehaviour {
		
	public GameObject hitEffectPrefab = null;	// 히트 효과 프리팹
    private static int m_allTargetNum = 0;      // 스테이지에 있는 타깃 개수

    //	시작할 때 호출되는 함수: Start() 전에 실행, 일반적으로 변수 초기화 때 사용
    private void Awake() {
		// 타깃 개수를 기억
		m_allTargetNum++;
	}

    // Collider가 무언가에 닿으면 호출된 함수:
    // 자신의 GameObject에 Collider(IsTrigger를 ON으로 지정)와 Rigidbody를 적용한 상태에서만 호출
	private	void OnTriggerEnter(Collider hitCollider) {
		
		// 상대편의 GameObject를 얻음
		GameObject	hitObject	= hitCollider.gameObject;

        // 상대편이 총알인지 검사
		if (hitObject.GetComponent<Bullet>() == null) {
            // 총알이 아니면 무시
			return;
		}
				
		// 파괴된 오브젝트 처리

        // 히트 효과가 있는지 검사
		if (hitEffectPrefab != null) {
            // 자신과 같은 위치에서 히트 효과를 표현
			Instantiate( hitEffectPrefab, transform.position, transform.rotation);
		}

        // 스테이지가 종료됐는지 검사

        // 타깃의 총 개수에서 자신의 몫을 삭제
		m_allTargetNum--;

        // 만약 타깃 개수가 0이 되면 스테이지 종료
		if (m_allTargetNum <= 0) {
			//스테이지를 종료
			Game.SetStageClear();
		}

        // 이 GameObject를 Hierarchy에서 삭제
		Destroy( gameObject);
	}

}
