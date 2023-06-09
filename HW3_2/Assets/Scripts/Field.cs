using UnityEngine;
using System.Collections;

//	자동 미로 생성 프로그램: MAZE_LINE_X、MAZE_LINE_Y로 지정한 크기의 미로를 자동으로 생성
public class Field : MonoBehaviour {
	public GameObject m_blockObject	= null;		    // 미로를 구성하는 블록 오브젝트
	public GameObject m_playerObject = null;	    // 조작할 플레이어 캐릭터
	public GameObject m_goalObject = null;		    // 도착 지점 오브젝트
	public GameObject m_targetObject = null;	    // 파괴할 타겟 오브젝트
	public GameObject m_stageClearObject = null;    // 스테이지 종료 때 생성할 오브젝트
	public bool m_createAtOnce = true;			    // true이면 미로를 한꺼번에 생성한다. false이면 생성 과정을 보여줌
		
	// 스테이지 종류
	public enum	StageClear {
        Goal		// 도착 지점을 목표로 함
		,Target		// 타겟을 모두 부숨
	}
	
	public StageClear m_stageClear = StageClear.Goal; // 스테이지 종료의 종류
		
	// 검사 방향
	private	enum CheckDir {	//← ↑ → ↓ 순서
		Left		// 왼쪽
		,Up			// 위
		,Right		// 오른쪽
		,Down		// 아래
		,EnumMax	// 최대값	
		,None = -1
	}

	//검사 정보
	private	enum CheckData {
		X			// X축
		,Y			// Y축
		,EnumMax	// 최대값
	}
		
	private static readonly	int[][]	CHECK_DIR_LIST	= new int[ (int)CheckDir.EnumMax][] {	// 검사 방향
		//										 X		 Y
		 new int[ (int)CheckData.EnumMax] {		-1,		 0		}
		,new int[ (int)CheckData.EnumMax] {		 0,		-1		}
		,new int[ (int)CheckData.EnumMax] {		 1,		 0		}
		,new int[ (int)CheckData.EnumMax] {		 0,		 1		}
	};

	private static readonly	CheckDir[] REVERSE_DIR_LIST	= new CheckDir[(int)CheckDir.EnumMax] {	// 검사 방향의 반대 방향
		CheckDir.Right
		,CheckDir.Down
		,CheckDir.Left
		,CheckDir.Up
	};

	private static readonly	CheckDir[] CHECK_ORDER_LIST	= new CheckDir[ (int)CheckDir.EnumMax] { // 검사할 순서
		CheckDir.Up
		,CheckDir.Down
		,CheckDir.Left
		,CheckDir.Right
	};
		
	private	static readonly	int	MAZE_LINE_X	= 8; // 미로의 X통로 개수
	private	static readonly	int	MAZE_LINE_Y	= 8; // 미로의 Y통로 개수
	
	private	static readonly	int	MAZE_GRID_X	= ((MAZE_LINE_X	* 2) + 1);	// 미로의 X배열 개수
	private	static readonly	int	MAZE_GRID_Y	= ((MAZE_LINE_Y	* 2) + 1);	// 미로의 Y배열 개수
	private	static readonly	int EXEC_MAZE_COUNT_MAX	= (MAZE_LINE_X * MAZE_LINE_Y / 2);	// 블록을 하나씩 생성할 때 수행 회수
	private	static readonly	float MAZE_BLOCK_SCALE = 2.0f;				// 미로 스케일(블록 하나 만큼의 크기)
	private	static readonly	int	TARGET_NUM = 5;							// 파괴할 타겟 개수
		
	private	bool[][] m_mazeGrid = null;         // 미로 배열
	private	GameObject m_blockParent = null;    // 미로 블록을 기억할 부모
	private	int m_makeMazeCounter = 0;			// 블록을 하나씩 생성 할 때 사용하는 카운터
	private	bool m_stageClearedFlag = false;	// 스테이지 종료 오브젝트를 생성하면 true
		
	// 프로그램을 시작할 때 호출되는 함수
	private void Awake() {
		
		// 미로를 초기화한다
		InitializeMaze();
		
		// 미로를 한꺼번에 만들지 여부를 검사
		if (m_createAtOnce) {
			// 상하 좌우 가장자리에서 중심을 향해 가지를 뻗어 미로를 생성
			int		i;
			for ( i=0; i < EXEC_MAZE_COUNT_MAX; i++) {	// 상하 좌우 방향으로 검사하는 것이므로 반만 실행
				ExecMaze();
			}
			
			// 미로를 생성
			CreateMaze();
		}
		
		//플레이어를 생성한다
		CreatePlayer();
		
		//게임 종료의 종류에 따른 처리
		switch( m_stageClear) {
		case StageClear.Goal:
			//골을 생성한다
			CreateGoal();
			break;
		case StageClear.Target:
			//타겟을 생성한다
			CreateTarget();
			break;
		}
	}
	
	// 매 프레임마다 호출되는 함수
	private		void	Update() {
		
		//미로를 한꺼번에 만들지 않을 때의 처리
		if( false==m_createAtOnce) {
			
			//스페이스를 누르면 한 칸 진행
			if( Input.GetKeyDown( KeyCode.Space)) {
				//생성
				ExecMaze();
				
				//미로를 업데이트한다
				CreateMaze();
			}
		}
		
		//스테이지 종료 확인
		if( false==m_stageClearedFlag) {
			
			//스테이지를 클리어한 상태라면 플래그를 ON으로 지정한다
			if( Game.IsStageCleared()) {
				//스테이지 종료 표시를 생성한다
				CreateStageClear();
				
				//플래그를 ON으로 지정한다
				m_stageClearedFlag	= true;
			}
		}
	}
		
	// 미로를 초기화: 배열 변수를 초기화하여 외벽과 기둥을 만듬
	private		void	InitializeMaze() {
		
		//처음에 bool 배열을 만듭니다 (이것이 true일 때 블록을 배치한다)
		
		//C#에서 2차원 배열은 이렇게 선언합니다. 생김새가 조금 복잡합니다.
		m_mazeGrid			= new bool[		MAZE_GRID_X][];		//먼저 왼쪽 배열을 선언합니다
		
		//그리고 루프를 사용하여 오른쪽 배열을 선언합니다
		int		gridX;
		int		gridY;
        for ( gridX=0; gridX<MAZE_GRID_X; gridX++) {
			m_mazeGrid[ gridX]	= new bool[ MAZE_GRID_Y];
		}

        // 이렇게 하여 다음과 같은 방식으로 액세스할 수 있게 됩니다.
        // Debug.Log( "GRID["+	gridX	+"]["+	gridY	+"] = "+	m_mazeGrid[ gridX][ gridY]);
        
        //처음부터 블록을 놓도록 정해진 장소를 블록으로 채워 넣는다
        bool blockFlag;
		for( gridX=0; gridX<MAZE_GRID_X; gridX++) {
			for( gridY=0; gridY<MAZE_GRID_Y; gridY++) {
				//true일 때 이 위치는 블록을 놓아도 된다
				blockFlag	= false;
				
				// (0 == gridX):왼쪽 끝, (0 == gridY):위쪽 끝,  ((MAZE_GRID_X -1) == gridX):오른쪽 끝, ((MAZE_GRID_Y -1) == gridY)):아래쪽 끝
				if( (0==gridX)		||(0==gridY)		||((MAZE_GRID_X -1)==gridX)		||((MAZE_GRID_Y -1)==gridY)) {
					//상하 좌우의 가장자리 벽
					blockFlag	= true;
				} else
				if( (0==(gridX %2))	&&(0==(gridY %2))) {				//'%'는 잉여 연산자입니다. 나눈 나머지 값을 구하는 것입니다 (예 : 13 % 10 = 3)
					//X, Y가 모두 짝수 일 때는 기둥
					blockFlag	= true;
				}
				
				//값을 대입한다
				m_mazeGrid[ gridX][ gridY]		= blockFlag;
			}
		}
	}
		
	//	미로를 하나씩 생성한다
	private		void	ExecMaze() {
		
		//미로 생성이 완료됐다
		if( m_makeMazeCounter >= EXEC_MAZE_COUNT_MAX) {
			return;
		}
		
		//이번에 생성할 것은 이 번호 블록부터 검사를 시작한다
		int			 counter	= m_makeMazeCounter;
		//카운트+1
		m_makeMazeCounter++;
		
		
		//汎用変数
		int			lineMax;			//X와 Y 라인 수 중에서 큰 쪽을 입력한다
		int			start1, start2;		//검사 시작 위치
		
		int			gridX_A		= 0;
		int			gridY_A		= 0;
		int			gridX_B;
		int			gridY_B;
		int			gridX_C;
		int			gridY_C;
		
		CheckDir	checkDirNow;			//검사하는 방향
		CheckDir	checkDirNG;				//한 개 이전의 검사 방향
		
		
		//라인의 최대값을 얻는다
		lineMax		= Mathf.Max( MAZE_LINE_X, MAZE_LINE_Y);
		
		//검사 시작 위치 (블록 한 개씩 건너서 검사하므로 2를 곱한다)
		start1		= ((counter		/lineMax)		*2);
		start2		= ((counter		%lineMax)		*2);
		
		
		//상하 좌우 끝에서 한 개씩 가지를 뻗어 벽을 생성해간다
		int		i;
		for( i=0; i<(int)CheckDir.EnumMax; i++){
			
			//지금 검사하는 것은 이 방향이다
			checkDirNow		= CHECK_ORDER_LIST[ i];
			//어느 쪽 끝에서 어느 방향으로 가지를 늘릴지 정한다
			switch( checkDirNow) {
			case CheckDir.Left:
				//왼쪽으로 가지를 늘린다 (오른쪽 끝에서 시작)
				gridX_A	= ((MAZE_GRID_X -1) -start1);		//가로축은 1을 X에 넣는다
				gridY_A	= ((MAZE_GRID_Y -1) -start2);		//2는 Y축이다
				break;
			case CheckDir.Up:
				//위로 가지를 늘린다 (아래쪽 끝에서 시작)
				gridX_A	= ((MAZE_GRID_X -1) -start2);		//세로축은 2를 X에 넣는다
				gridY_A	= ((MAZE_GRID_Y -1) -start1);		//1은 Y축이다
				break;
			case CheckDir.Right:
				//오른쪽으로 가지를 늘린다(왼쪽 끝에서 시작)
				gridX_A	= (                     start1);
				gridY_A	= (                     start2);
				break;
			case CheckDir.Down:
				//아래쪽으로 가지를 늘린다 (위쪽 끝에서 시작)
				gridX_A	= (                     start2);
				gridY_A	= (                     start1);
				break;
			default:
				//default에 경고를 넣어두면 조기에 버그를 검출 할 수 있어 편리하다
				Debug.LogError( "존재하지 않는 방향("+ checkDirNow +")");
				//일단 의미 없는 값을 넣어둔다
				gridX_A	= -1;
				gridY_A	= -1;
				break;
			}
			// 장외 검사
			if(	(gridX_A < 0)		||(gridX_A >= MAZE_GRID_X)		||(gridY_A < 0)		||(gridY_A >= MAZE_GRID_Y)	) {
				// 여기에는 조사할 블록이 없다
				continue;
			}
			
			
			// 벽이 있는 기둥에 부딪힐 때까지 무한 루프
			for(;;) {
				// 체크할 기둥 위치 (시작 위치에서 두 개 옆에 있는 블록)
				gridX_B	= gridX_A	+(CHECK_DIR_LIST[ (int)checkDirNow][ (int)CheckData.X]	*2);
				gridY_B	= gridY_A	+(CHECK_DIR_LIST[ (int)checkDirNow][ (int)CheckData.Y]	*2);
				
				// 임의의 블록 주변을 살펴보고 다른 블록과 연결되어 있지 않은지 확인한다
				if( IsConnectedBlock( gridX_B, gridY_B)) {
					
					// 이미 무언가와 연결되어 있었으므로 작업을 중단한다
					break;
				}
				
				
				// 시작 위치와 체크 위치 사이의 위치에 블록을 넣는다
				gridX_C	= gridX_A	+CHECK_DIR_LIST[ (int)checkDirNow][ (int)CheckData.X];
				gridY_C	= gridY_A	+CHECK_DIR_LIST[ (int)checkDirNow][ (int)CheckData.Y];
				
				// 블록을 배치
				SetBlock( gridX_C, gridY_C, true);
				
				
				// 다음은 연결한 기둥부터 검색을 시작한다
				gridX_A	= gridX_B;
				gridY_A	= gridY_B;
				
				
				// 다음부터는 이쪽으로 오면 안된다
				checkDirNG		= REVERSE_DIR_LIST[ (int)checkDirNow];
				
				// 다음에 조사할 기둥을 무작위로 선택한다
				checkDirNow		= CHECK_ORDER_LIST[ Random.Range( 0, (int)CheckDir.EnumMax)];
				
				// 한 번 이전 위치로 되돌아가지 않도록 진행 방향을 검사한다
				if( checkDirNow==checkDirNG) {
					// 돌아가려고 하면 반대쪽을 향하게 한다
					checkDirNow		= REVERSE_DIR_LIST[ (int)checkDirNow];
				}
			}
			
		}
	}
		
	// 지정된 위치에 블록이 존재하는지 알아봄
	private		void	SetBlock( int gridX, int gridY, bool blockFlag) {
		m_mazeGrid[ gridX][ gridY]	= blockFlag;
	}
	
	// 지정된 위치에 블록이 존재하는지 알아봄
	// 블록이 존재하면 true를 반환
	private		bool	IsBlock( int gridX, int gridY) {
		return	m_mazeGrid[ gridX][ gridY];
	}
	
	// 지정된 블록의 상하좌우에 블록이 있는지 알아봄
	// 어떤 것에 연결된 경우는 true가 반환
	private		bool	IsConnectedBlock( int gridX, int gridY) {
		
		bool	connectedFlag	= false;	//어떤 것에 연결되어 있으면 true
		
		int		checkX;				// 검사할 X위치
		int		checkY;				// 검사할 Y위치
		
		// 주위를 한 번 둘러보며 확인
		int		i;
		for( i=0; i<(int)CheckDir.EnumMax; i++){
			// 조사할 블록의 위치
			checkX		= (gridX	+CHECK_DIR_LIST[ i][ (int)CheckData.X]);
			checkY		= (gridY	+CHECK_DIR_LIST[ i][ (int)CheckData.Y]);
			
			// 장외 검사
			if(	(checkX < 0)		||(checkX >= MAZE_GRID_X)		||(checkY < 0)		||(checkY >= MAZE_GRID_Y)	) {
				//여기에는 조사할 블록이 없음
				continue;
			}
			
			// 이미 블록이 존재하는지 검사
			if( IsBlock( checkX, checkY)) {
				//블록이 있었다
				connectedFlag	= true;
				//바로 종료한다
				break;
			}
		}
		
		return	connectedFlag;
	}
		
	//	미로를 Hierarchy에 생성
	private		void	CreateMaze() {
		
		// 이미 블록의 부모가 있으면 삭제
		if( m_blockParent) {
			// 삭제
			Destroy( m_blockParent);
			// null 넣어 둔다
			m_blockParent			= null;
		}
		
		
		// 블록의 부모를 만듬
		m_blockParent					= new GameObject();
		m_blockParent.name				= "BlockParent";
		m_blockParent.transform.parent	= transform;
		
		
		// 블록을 만듬
		GameObject	blockObject;		//우선 블록을 넣어 두는 변수
		Vector3		position;			//블록 생성 위치
		
		int	gridX;
		int	gridY;
		for( gridX=0; gridX<MAZE_GRID_X; gridX++) {
			
			for( gridY=0; gridY<MAZE_GRID_Y; gridY++) {
				
				// 블록인지 여부를 검사
				if( IsBlock( gridX, gridY)) {
					
					// 블록 생성 위치
					position			= new Vector3( gridX, 0, gridY)	*MAZE_BLOCK_SCALE;	// 유니티에서는 XZ 평면이 수평선임 (이 경우 왼쪽 아래에서 오른쪽 위로 진행한다)
					
					// blockObject:블록 생성 m_blockObject:복제 대상 position:생성 위치 Quaternion.identity:회전 (이번에는 사용하지 않는다)
					blockObject			= Instantiate(		m_blockObject,		position,		Quaternion.identity)		as GameObject;
					//이름을 변경
					blockObject.name	= "Block("+ gridX +","+ gridY +")";		//그리드의 위치를 기술한다
					
					// 로컬 스케일 (크기)을 변경
					blockObject.transform.localScale	= (Vector3.one	*MAZE_BLOCK_SCALE);		//Vector3.one は new Vector3( 1f, 1f, 1f) と同じ
					
					// 앞서 생성한 부모 아래에 넣음
					blockObject.transform.parent		= m_blockParent.transform;
				}
			}
		}
	}
	
	// 플레이어를 Hierarchy에 생성
	private		void	CreatePlayer() {
		//플레이어 만들기
		Instantiate(		m_playerObject,		new Vector3( 1, 0, 1) *MAZE_BLOCK_SCALE,		Quaternion.identity);
	}
	
	// 종료 지점을 Hierarchy에 생성
	private		void	CreateGoal() {
		// 종료 지점은 플레이어의 반대쪽 모서리에 배치
		Vector3	position	= new Vector3( (MAZE_GRID_X -2), 0, (MAZE_GRID_Y -2))	*MAZE_BLOCK_SCALE;
		// 종료 지점 만들기
		Instantiate(		m_goalObject,		position,		Quaternion.identity);
	}
	
	// 타깃을 Hierarchy에 생성
	private		void	CreateTarget() {
		Vector3	position;
		int	i;
		for( i=0; i<TARGET_NUM; i++) {
			// 타깃은 임의의 장소에 만듬
			position	= new Vector3( (Random.Range( 0, MAZE_LINE_X)	*2) +1, 0, (Random.Range( 0, MAZE_LINE_Y)	*2) +1)		*MAZE_BLOCK_SCALE;
			// 타깃을 만듬
			Instantiate(		m_targetObject,		position,		Quaternion.identity);
		}
	}
	
	
	// 스테이지 클리어 표시를 Hierarchy에 생성
	private		void	CreateStageClear() {
		// 스테이지 종료 오브젝트를 생성
		Instantiate(		m_stageClearObject,		Vector3.zero,		Quaternion.identity);
	}
	
	
	
	
	
}
