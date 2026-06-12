# Combat Framework — CLAUDE.md

Godot 4.7, C#, `[GlobalClass]`, `Resource`-based state machines.

---

## Estrutura de pastas relevante

```
source/
├── components/          — Nodes adicionados como filhos de SystemLogicComponents
├── constants/           — Strings de nomes de estados (LogicStateNames)
├── core/                — SystemLogicComponents (raiz de toda entidade)
├── data/                — CharacterStats (Resource)
├── handlers/            — Dispatchers estáticos (MovementHandler, PhysicsHandler)
├── interfaces/          — ISystemLogicContext, IHasAnimationTree, etc.
└── resources/states/
    ├── helpers/         — Implementações estáticas (CharacterBodyHelper, AnimationTreeHelper, etc.)
    ├── logic/           — LogicState base + estados concretos (Idle, Walk, Run, Attack)
    └── animation/       — AnimationState base + estados concretos (Locomotion, Airborne, Land, Attack)
```

---

## Arquitetura central

### Entidade

Toda entidade usa `SystemLogicComponents` como nó raiz. Ele implementa `ISystemLogicContext` e expõe componentes via `GetComponent<T>()` com cache.

```
SystemLogicComponents  (ISystemLogicContext)
├── LogicStateMachineComponent
├── AnimationStateMachineComponent
├── JumpComponent
├── HealthComponent
├── CameraComponent
├── AttackRotateModelComponent
└── [outros componentes]
```

**Pawn**: sempre `CharacterBody3D`. Suporte a `RigidBody3D` como pawn foi removido. RigidBody3D ainda existe como utilitário para objetos de cenário via `RigidBodyHelper`.

---

### Dois State Machines paralelos

| SM | Componente | Loop | Responsabilidade |
|---|---|---|---|
| Lógico | `LogicStateMachineComponent` | `_PhysicsProcess` | Física, input, gameplay |
| Animação | `AnimationStateMachineComponent` | `_Process` | Parâmetros do AnimationTree |

Comunicação: **unidirecional** — lógico pode chamar `context.AnimationStateMachineComponent?.ChangeState(...)`, animação nunca chama lógico.

---

### Handlers vs Helpers

- **Handlers** (`source/handlers/`) — estáticos, despacham para helpers. Ex: `MovementHandler.ApplyMovement(context, delta)`
- **Helpers** (`source/resources/states/helpers/`) — implementações específicas. Ex: `CharacterBodyHelper.ApplyMovement(...)`

`MovementHandler` e `PhysicsHandler` são os principais usados nos estados.  
`JumpHandler.cs` está **pendente de deleção** — lógica movida para `JumpComponent`.

---

### AnimationSnapshot

Struct imutável construída a cada `_Process` por `AnimationStateMachineComponent.BuildSnapshot()`. Estados de animação só recebem isso — nunca o contexto completo.

```csharp
public struct AnimationSnapshot
{
    public float   HorizontalSpeed;
    public float   NormalizedSpeed;    // 0..1 relativo ao RunSpeed
    public float   VerticalVelocity;
    public bool    IsGrounded;
    public bool    HasInput;
    public float   TimeInAir;
    public Vector3 RootMotionVelocity; // world space, câmera-relativo
}
```

`RootMotionVelocity` usa o forward da câmera (SpringArm) como basis. Fallback para `charBody.GlobalTransform.Basis`.  
Z é invertido no helper para corrigir convenção Blender (+Z) → Godot (-Z).

---

## Componentes

### LogicStateMachineComponent
- Sinal: `StateChanged(from, to)`
- Propriedade pública: `CurrentStateName`
- **Não** implementa `IHasAnimationTree` (foi removido — design errado)

### AnimationStateMachineComponent
- Sinal: `StateChanged(from, to)`
- Propriedades: `CurrentStateName`, `CurrentSnapshot`, `PlaybackPath`
- Export: `AnimationTree`, `PlaybackPath` (default `"parameters/playback"`), `InitialState`, `States[]`
- Implementa `IHasAnimationTree`

### JumpComponent
- Sinais: `Jumped`, `Landed`
- Lógica completa: press, hold, cut, JumpTravel (ação "jet")
- Up direction relativa ao planeta (`EntityProps.CurrentPlanet`), fallback para `UpDirection`

### HealthComponent
- Sinais: `DamageTaken(amount, source)`, `HealthChanged(current, max)`, `Died`, `Healed(amount)`, `Revived`, `IFramesStarted`, `IFramesEnded`
- Exports por grupo: Health, Defense, Invincibility, Regeneration
- `CalculateDamage`: flat defense → percent defense → MinDamage floor
- `SetInvincible(bool)`, `TakeDamage(float, Vector3)`, `Heal(float)`, `Revive(float?)`

### CameraComponent
- Contém `SpringArm`
- Usado por `CharacterBodyHelper` e `AnimationStateMachineComponent` para orientação de movimento

### AttackRotateModelComponent
- Exports: `Model`, `SpringArm`, `AttackTurnSpeed`, `InvertY`
- Só age quando `LogicStateMachineComponent.CurrentStateName == "attack"`
- Rotaciona model em direção ao forward da câmera

---

## Estados lógicos

Todos em `source/resources/states/logic/states/`, herdam de `LogicState`.

| Estado | Arquivo | Comportamento |
|---|---|---|
| `idle` | `LogicStateIdle` | Gravity + ApplyMovement (zera horizontal) + MoveAndSlide |
| `walk` | `LogicStateWalk` | Gravity + Movement + MoveAndSlide |
| `run` | `LogicStateRun` | Gravity + Movement + MoveAndSlide |
| `attack` | `LogicStateAttack` | Root motion ou movimento livre + InterruptibleBy |

`LogicStateAttack` exports:
- `FirstAttackAnimState` — nome do estado de animação inicial
- `UseRootMotion` — aplica `RootMotionVelocity` do snapshot
- `LockMovement` — trava input de movimento
- `InterruptibleBy[]` — estados que podem interromper o ataque

Interrupts implementados: `"jump"` (HandleInput), `"airborne"` (PhysicsUpdate).

Transição para ataque: `HandleInput` em Idle/Walk/Run verifica `@event.IsActionPressed("attack")`.

---

## Estados de animação

Todos em `source/resources/states/animation/`, herdam de `AnimationState`.

| Estado | Arquivo | Comportamento |
|---|---|---|
| `locomotion` | `AnimStateLocomotion` | BlendSpace1D por `NormalizedSpeed` com lerp suave |
| `airborne` | `AnimStateAirborne` | Blend por `VerticalVelocity` |
| `land` | `AnimStateLand` | Aguarda `AnimationFinish` → volta a locomotion |
| `attack1/2/3` | `AnimStateAttack` | `Travel()` + detecção de combo + `EndThreshold` |

`AnimStateAttack` exports:
- `NextComboState` — próximo ataque no combo (vazio = fim do combo)
- `ComboWindowStart` — 0..1 quando o input de combo começa a ser aceito
- `EndThreshold` — segundos antes do fim para detectar animação concluída (aumentar para animações longas)

---

## AnimationTreeHelper

Métodos principais:

```csharp
SetBlendPosition(IHasAnimationTree, path, value)
SetTreeCondition(IHasAnimationTree, path, bool)
Travel(IHasAnimationTree, stateName)          // usa playback.Travel()
AnimationFinish(IHasAnimationTree, threshold) // verifica pos >= length - threshold
GetNormalizedPlayPosition(IHasAnimationTree)  // 0..1
GetRootMotionVelocity(IHasAnimationTree, Basis, delta)
```

`PlaybackPath` vem de `IHasAnimationTree.PlaybackPath` — configurável no inspetor. Default: `"parameters/playback"`.

---

## CharacterStats (Resource)

Grupos no inspetor:

| Grupo | Propriedades |
|---|---|
| Movement | WalkSpeed, RunSpeed |
| Movement/Ground | Acceleration, Deceleration |
| Movement/Air | AirAcceleration, AirDeceleration |
| Health | MaxHealth |
| Jump | JumpForce, JumpHoldTime, CutJumpFactor |
| Physics | Gravity |

---

## Interfaces

```
ISystemLogicContext
  ├── IComponentOwner          → GetComponent<T>()
  ├── IHasStateMachineComponent
  └── IHasAnimationStateMachineComponent

IHasAnimationTree
  ├── AnimationTree { get; }
  └── PlaybackPath { get; }     // path para o playback no AnimationTree
```

---

## Constantes

`LogicStateNames`: `idle`, `walk`, `run`, `jump`, `walk_jump`, `travel`, `attack`, `airborne`, `hit`, `death`

---

## Pendente de deleção

Apagar manualmente pelo explorador de arquivos:
- `source/handlers/JumpHandler.cs`
- `source/components/ModelRotationComponent.cs`
- `source/components/RotateModelComponent.cs`

---

## Convenções

- Sempre compilar com `dotnet build` após mudanças
- SpringArm: cada componente que precisa tem `[Export] public SpringArm3D SpringArm`
- Nunca criar worktrees automaticamente — trabalhar direto no root
- Novos estados lógicos: herdar `LogicState`, adicionar nome em `LogicStateNames`
- Novos estados de animação: herdar `AnimationState`, usar `AnimationTreeHelper`
