# 🕹 Dungeon Shooter (Unity)

Unity 2021.3.0f1 기반 탑다운 슈팅 프로토타입입니다.  
플레이어 이동/사격, 몬스터 스폰·AI, 간단한 UI/사운드를 포함하며, 학습·성장 기록을 목적으로 공개합니다.

## 🎬 Scenes
`Assets/0.Scenes/`
- TitleScene.unity, LobbyScene.unity, StageScene_1.unity, StageScene_2.unity, BossStageScene.unity  
- StoreScene.unity, StoryScene.unity, LoadingScene.unity, GameOverScene.unity, EndingScene.unity, GridSceneBackUp.unity

## 📦 Packages (주요)
- 2D Feature, TextMeshPro, Timeline, UGUI, VisualScripting 등 (Packages/manifest.json 참조).

## 🛠 Tech
- Unity 2021.3.0f1
- C# (URP/2D 패키지 기반)

## ▶ 실행 방법
1. Unity Hub에서 `2021.3.0f1` 버전으로 열기.
2. `Assets/0.Scenes/TitleScene.unity` 또는 `LobbyScene.unity` 실행.
3. ▶ Play.

## 🚧 Known Issues (분석 기반)
- **NullReferenceException 발생 가능**:  
  - 여러 스크립트에서 `Find*`/`GetComponent` 사용 후 널 가드 없음.  
  - 해결: 인스펙터 참조 연결(SerializedField), `TryGetComponent` 패턴 적용.

- **리소스 로드 안정성**:  
  - `Resources.Load("문자열")` 경로 의존.  
  - 해결: 주소 지정(어드레서블) 또는 인스펙터 참조로 전환, 널 가드 추가.

- **충돌 처리 신뢰성**:  
  - 총알/몬스터 충돌 시 컴포넌트 존재 가정.  
  - 해결: `if (col.TryGetComponent(out EnemyManager en)) …`로 변경.

- **업데이트 루프 구조**:  
  - 일부 `Update()` 폴링/빈 루프.  
  - 해결: 이벤트·콜백 사용, 물리 관련은 `FixedUpdate()`로 이전.

- **싱글턴 초기화 순서**:  
  - `SoundManager` 등 첫 접근 이전에 씬 배치 필요.  
  - 해결: 부트스트랩 씬에서 `DontDestroyOnLoad` 오브젝트로 보장.
