# Resumen para continuar

## Lo que hice yo (Equipos 3 y 4)

### Scripts creados
| Script | Qué hace |
|---|---|
| `GameManager.cs` | Director del juego: turnos, acciones, eventos |
| `CameraSwitcher.cs` | Mueve la cámara entre jugadores |
| `CardData.cs` | Datos de una carta (ScriptableObject) |
| `DeckBuilder.cs` | Construye el mazo completo (39 cartas) |
| `Deck.cs` / `DiscardPile.cs` | Mazo y descarte |
| `PlayerState.cs` | Estado de un jugador |
| `DuelController.cs` | Resuelve duelos |
| `DuelTypes.cs` | DuelRequest, DuelSubmission, DuelResult |
| `GamePhase.cs` | Fases del juego |
| `GameSettings.cs` | Configuración (ScriptableObject) |
| `UIManager.cs` | HUD, botones, mensajes, paneles |
| `CardSelectorUI.cs` | Raycast para seleccionar cartas |
| `DuelPanelUI.cs` | Panel de duelo |
| `MainMenuUI.cs` | Menú principal |

### Escenas
- **HUD.unity** — Canvas completo con todos los paneles montados y referencias conectadas
- **MainScene.unity** — escena principal del juego (del Equipo 1)

---

## Lo que falta hacer

### 1. Agregar GameManager a MainScene
- Crear GameObject vacío `GameManager` en MainScene
- Add Component: `GameManager` + `CameraSwitcher`
- Arrastrar Main Camera al campo Main Camera del CameraSwitcher
- Arrastrar `Assets/GameSettings.asset` al campo Settings del GameManager

### 2. Integrar HUD con MainScene
Agregar esta línea al final de `GameManager.Start()`:
```csharp
SceneManager.LoadScene("HUD", LoadSceneMode.Additive);
```
Y agregar al principio del archivo:
```csharp
using UnityEngine.SceneManagement;
```
También agregar HUD y MainScene en **File → Build Settings**.

### 3. Layer "Card" para las cartas
- Edit → Project Settings → Tags and Layers → agregar layer `Card`
- Seleccionar prefabs de carta → cambiar Layer a `Card`
- En objeto UI → CardSelectorUI → asignar `Card Layer Mask` al layer `Card`

### 4. Conectar botones de paneles
- Botón Continuar en `RoundEndPanel` → OnClick → UIManager → `OnRoundEndContinue()`
- Botón Reiniciar en `GameOverPanel` → OnClick → UIManager → `OnGameOverRestart()`

### 5. Probar
- Correr MainScene
- Consola debe mostrar reparto inicial (39 cartas, 2 jugadores)
- Probar botón Tomar, cambio de turno, mensajes

---

## Decisión pendiente
¿La penalización para cartas vulnerables cuál es?
- `MinusOneDefense` — defensor suma −1
- `PlusOneAttack` — atacante suma +1
- `SingleCardDefenseOnly` — defensor solo puede usar 1 carta ← **default actual**

Cambiar en el asset `Assets/GameSettings.asset` desde el Inspector.

---

## Planes completos
- `plan_equipo3.md` — lógica de juego
- `plan_equipo4.md` — UI
