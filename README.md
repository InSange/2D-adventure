# 2D-adventure

유튜브 강의를 따라 하며 절차적 던전 생성을 공부한 Unity 학습용 저장소입니다.

- 인원: 1인
- 사용 기술: Unity / C#
- 참고한 강의: [Procedural Generation 재생목록](https://www.youtube.com/watch?v=-QOCX6SVFsk&list=PLcRSafycjWFenI87z7uZHFv6cUG2Tzu9v)

아래 경로는 모두 `adventure/Assets/Assets/` 아래를 가리킵니다.

## 코드와 정리 글

| 방식 | 코드 | 정리 |
|---|---|---|
| 미리 만든 방 프리팹을 출입구 방향에 맞춰 이어 붙이기 | `Scripts/Test2/test1/` | [블로그](https://sunwo777.tistory.com/17) |
| 랜덤 워크로 바닥 좌표를 만들고 타일로 그리기 | `Scripts/Test1/` | [블로그](https://sunwo777.tistory.com/25) |
| 복도를 먼저 뚫고 그 끝에 방을 붙이기 | `Scripts/Test1/` | [블로그](https://sunwo777.tistory.com/29) |
| 공간을 이진 분할(BSP)해 방을 배치하고 복도로 잇기 | `Scripts/Test1/` | [블로그](https://sunwo777.tistory.com/30) |

방 프리팹을 조합하는 방식은 방의 생김새를 직접 정할 수 있는 대신 조합이 한정됩니다.
나머지 셋은 좌표를 알고리즘으로 만들어 타일맵에 그리는 방식이고,
`Scripts/Test1/` 안에서 생성기만 갈아 끼웁니다.

## 절차적 생성 — `Scripts/Test1/`

| 파일 | 내용 |
|---|---|
| `ProceduralGenerationAlgorithms.cs` | 랜덤 워크, 랜덤 워크 복도, 이진 공간 분할 |
| `AbstractDungeonGenerator.cs` | 생성기 공통 추상 클래스 |
| `SimpleRandomWalkDungeonGenerator.cs` | 랜덤 워크 기반 생성기 |
| `CollidorFirstDungeonGenerator.cs` | 복도 우선 생성기 |
| `RoomFirstDungeonGenerator.cs` | BSP로 나눈 공간에 방을 넣고 복도로 연결 |
| `TilemapVisualizer.cs` | 좌표 집합을 타일맵에 그리기 |
| `WallGenerator.cs`, `WallTypesHelper.cs` | 바닥 좌표에서 벽 위치를 찾고 벽 타일 종류 판별 |
| `DungeonData.cs` | 생성 결과(방·복도·바닥 좌표) 보관 |
| `RoomDataExtractor.cs` | 방 정보를 뽑고 플레이어 방 기준으로 정렬 |
| `PropPlacementManager.cs`, `Prop.cs` | 구석·벽면 등 배치 조건에 맞는 자리를 찾아 오브젝트 스폰 |
| `AgentPlacer.cs` | 방 단위로 적·플레이어·동료 배치 |

생성기는 `AbstractDungeonGenerator`에서 시작해
`SimpleRandomWalkDungeonGenerator`를 거쳐
`CollidorFirst`와 `RoomFirst` 두 갈래로 갈라집니다.

`Prop`은 스크립터블 오브젝트로 두어 구석에 놓을지 어느 방향 벽에 붙일지를
데이터로 지정하고, `PropPlacementManager`가 그 조건에 맞는 자리를 찾습니다.
생성 파라미터도 `Data/DungeonData/SimpleRandomWalkSO.cs`로 에셋화해서
값을 바꿀 때 코드를 고치지 않게 했습니다.
`Editor/RandomDungeonGeneratorEditor.cs`는 인스펙터에서 바로 다시 생성할 수 있는
커스텀 에디터입니다.

## 던전 안 캐릭터 — `Scripts/Dungeon/`

생성한 맵 위에서 움직일 대상이 필요해 캐릭터와 AI도 붙여 봤습니다.

**AI** — 컨텍스트 기반 이동

| 파일 | 내용 |
|---|---|
| `ContextSolver.cs` | 8방향 `interest`·`danger` 배열을 모아 최종 이동 방향 결정 |
| `SteeringBehaviour.cs` | 방향 가중치를 채우는 행동의 추상 클래스 |
| `SeekBehaviour.cs` | 목표 쪽 방향의 `interest`를 올림 |
| `ObstacleAvoidanceBehaviour.cs` | 장애물 쪽 방향의 `danger`를 올림 |
| `Detector.cs` | 주변 탐지의 추상 클래스 |
| `TargetDetector.cs`, `FollowerTargetDetector.cs`, `ObstacleDetector.cs` | 목표·장애물 탐지 |
| `AIData.cs` | 탐지 결과 보관 |

여러 행동이 각자 방향별 점수를 채우고,
`ContextSolver`가 `interest`에서 `danger`를 빼서 가장 높은 방향으로 움직입니다.
길찾기 없이도 장애물을 피해 목표로 향하게 하는 방식입니다.

**조작과 상태** — `Scripts/Dungeon/Controll/`

`Agent`, `AgentMover`, `AgentAnimations`, `AnimationController`, `AnimationEventHelper`,
`PlayerInput`, `EnemyAI`, `FollwerAI`, `Health`, `WeaponParent`
