# Simple RPG — 소스 코드

Unity 6.3으로 만든 싱글 플레이 RPG의 **C# 소스 코드**입니다. 개인 프로젝트, 개발 기간 1개월.

전투 · 성장 · 인벤토리 · 적 AI · 퀘스트 · 시나리오 · 세이브를 구현했고,
많은 요소가 유기적으로 연결되면서도 유지보수가 가능한 구조를 만드는 것이 목표였습니다.

코드는 **2026년 4월 발표 시점 기준**입니다.

📄 **[케이스 스터디 — 설계 의도부터 회고까지](https://gmumdev.github.io/portfolio/projects/simple-rpg.html)**

---

## ⚠️ 이 레포지토리는 빌드되지 않습니다

**소스 코드 열람용**입니다. 씬, 프리팹, 3D 모델, 텍스처 등 에셋은 포함되어 있지 않습니다.

원본 프로젝트는 서드파티 에셋 패키지(Unity Asset Store 및 교육 과정 제공분)를 사용합니다.
해당 에셋들은 라이선스상 원본 파일 형태로 재배포할 수 없어, 코드만 분리해 공개합니다.

클론해도 Unity에서 열리지 않습니다. 코드를 읽기 위한 레포지토리입니다.

---

## 구조

188개 `.cs` 파일.

```
Scripts/
├─ Core/          (5)   EventBus, 기반 타입
├─ Interfaces/    (12)  시스템 간 계약
├─ Event/         (24)  EventBus로 주고받는 이벤트 클래스
├─ Managers/      (13)  Quest, Scenario, UI 등 매니저
├─ Domain/              도메인별 로직
│  ├─ Player/     (6)
│  │  └─ NewStateMachine/ (14)  커스텀 계층적 FSM
│  ├─ Enemy/      (15)  적 AI, 보스 패턴
│  ├─ Quest/      (7)   + Contexts/, Handler/{Condition,Reward}/
│  ├─ Scenario/   (7)   + Contexts/, Handler/
│  ├─ NPC/        (6)   퀘스트 제공, 상호작용
│  ├─ PooledEntities/(2)
│  └─ Gatherables/(1)   채집물
├─ Combat/        (12)  전투, 스킬, 패턴
├─ Systems/              UI 시스템
│  ├─ SubUISystems/(17) 인벤토리, 퀘스트 창 등
│  └─ UIStates/   (5)   Normal · Watching · Battle
├─ DataSOs/       (10)  ScriptableObject 데이터 정의
├─ Enum/          (6)
└─ Utils/         (9)
```

## 눈여겨볼 만한 곳

| 파일 · 폴더 | 내용 |
|---|---|
| `Core/EventBus.cs` | 이벤트를 대리자가 아닌 클래스로 다루는 static EventBus. 구독 시 `SubscriptionToken`을 반환해, 리스너를 람다로 작성해도 안전하게 해지할 수 있습니다. |
| `Domain/Player/NewStateMachine/` | `StateMachineBehaviour` 의존을 걷어낸 커스텀 계층적 FSM. `PlayerState_WaitForAnimStart`가 애니메이션 Tag 간 이동을 감지해 상태 전환을 동기화합니다. |
| `Domain/Quest/Handler/` | 조건(`Condition`)과 보상(`Reward`) Handler를 분리. 새 조건은 인터페이스 구현만으로 추가됩니다. |
| `Domain/Enemy/EnemyPattern_BurstingInstances.cs` | JobSystem으로 작은 보스 Mesh를 대량 렌더링하는 패턴. |
| `Event/` | 24개 이벤트 클래스. 발행자와 구독자가 서로를 모르는 구조입니다. |

위 설계에 이르기까지의 시행착오와 아쉬웠던 점은 [케이스 스터디](https://gmumdev.github.io/projects/simple-rpg.html)에 정리했습니다.

## 환경

- Unity `6000.3.10f1` (6.3) · URP
- 주요 패키지 — Addressables, Cinemachine, Input System, Timeline, AI Navigation
- 전체 목록은 [`manifest.json`](manifest.json) 참고
