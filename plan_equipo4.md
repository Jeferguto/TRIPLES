# Plan de ejecución — Equipo 4 (UI y experiencia de usuario)

## Contexto

Juego **Triples** en Unity con cámara FPV. La lógica de juego ya existe en `GameManager.cs` — el Equipo 4 solo conecta la UI a esa lógica mediante eventos y métodos públicos. No se toca lógica de juego.

---

## API disponible del GameManager (lo que puedes usar ya)

### Eventos (suscribirse en UIManager)
```csharp
GameManager.Instance.OnPlayerTurnStarted  += (int playerNumber) => { ... }
GameManager.Instance.OnCardDrawn          += (int playerId, CardData card) => { ... }
GameManager.Instance.OnShieldPlaced       += (int playerId, CardData card) => { ... }
GameManager.Instance.OnDuelStarted        += (DuelRequest req) => { ... }
GameManager.Instance.OnDuelResolved       += (DuelResult result) => { ... }
GameManager.Instance.OnRoundWon           += (int playerNumber) => { ... }
GameManager.Instance.OnGameWon            += (int playerNumber) => { ... }
```

### Métodos que llaman los botones
```csharp
GameManager.Instance.TryDraw(int playerId)                               // → bool
GameManager.Instance.TryProtect(int playerId, CardData target)            // → bool
GameManager.Instance.TryAttack(int attackerId, int defenderId, CardData t) // → bool
GameManager.Instance.EndTurn()
```

### Getters para mostrar en pantalla
```csharp
GameManager.Instance.CurrentPlayerNumber   // int
GameManager.Instance.Phase                 // GamePhase enum
GameManager.Instance.GetHandCount(int playerNumber)  // int
GameManager.Instance.GetHand(int playerNumber)       // List<CardData>
GameManager.Instance.GetTotalPlayers()               // int
```

---

## Estructura de Canvas recomendada

```
Canvas (Screen Space - Overlay)
├── HUD
│   ├── TurnPanel         ← "Turno del Jugador X"
│   ├── PhaseLabel        ← fase actual (debug, puede ocultarse en release)
│   ├── DeckCountLabel    ← "Mazo: N cartas"
│   └── PlayerHandCounts  ← contadores por jugador
├── ActionPanel           ← botones Tomar / Proteger / Atacar / Terminar turno
├── DuelPanel             ← aparece solo durante el duelo
│   ├── AttackerInfo
│   ├── DefenderInfo
│   └── ResolveButton
├── MessagePanel          ← mensajes flotantes ("¡Escudo roto!", "¡Triple completado!")
├── RoundEndPanel         ← aparece al ganar ronda (oculto por defecto)
└── GameOverPanel         ← aparece al ganar partida (oculto por defecto)
```

---

## Sprint 1 — HUD básico y botones de acción

### 1.1 UIManager.cs
- Singleton MonoBehaviour.
- Se suscribe a todos los eventos del `GameManager` en `OnEnable` y se desuscribe en `OnDisable`.
- Actualiza labels en pantalla cuando llegan los eventos.

### 1.2 TurnPanel
- Label "Turno del Jugador X" — se actualiza con `OnPlayerTurnStarted`.
- Label de fase actual (opcional, útil para debug).

### 1.3 ActionPanel (botones)
- **Tomar**: llama `GameManager.Instance.TryDraw(currentPlayer)`.
- **Terminar turno**: llama `GameManager.Instance.EndTurn()`.
- **Proteger** y **Atacar**: por ahora deshabilitados — se activan en Sprint 2 cuando el jugador seleccione una carta.
- Los botones solo son interactivos si `GameManager.Instance.Phase == GamePhase.PlayerTurn`.

### 1.4 DeckCountLabel
- Suscribirse a `Deck.OnCountChanged` o leer `GameManager` con un polling ligero en `Update`.

**Entregable S1**: HUD visible, botón Tomar funcional, turno cambia correctamente en pantalla.

---

## Sprint 2 — Selección de cartas y acciones avanzadas

### 2.1 Selección de carta con raycast
- `CardSelectorUI.cs`: en `Update`, hace raycast desde la cámara.
- Si el raycast golpea un `CardVisual`, lo resalta (`SetHighlight(true)`).
- Al hacer clic, guarda la carta seleccionada y habilita botones de **Proteger** y **Atacar**.

### 2.2 Flujo de Proteger
1. Jugador hace clic en su carta → se resalta.
2. Jugador pulsa botón **Proteger** → `TryProtect(currentPlayer, selectedCard)`.
3. Si devuelve `true`, mostrar mensaje "Escudo colocado".

### 2.3 Flujo de Atacar
1. Jugador hace clic en carta del oponente → se resalta.
2. Jugador pulsa botón **Atacar** → `TryAttack(attackerId, defenderId, selectedCard)`.
3. Se activa el `DuelPanel`.

### 2.4 DuelPanel
- Muestra atacante, defensor y carta en juego.
- Cada jugador selecciona 0–3 cartas de su mano para el duelo.
- Botón **Confirmar** → llama `GameManager.Instance.ResolveDuel(attackerSub, defenderSub, targetCard)`.
- Al recibir `OnDuelResolved`, oculta el panel y muestra resultado.

**Entregable S2**: flujo completo de turno con selección de carta, protección y ataque jugable.

---

## Sprint 3 — Feedback visual y paneles de fin

### 3.1 MessagePanel (mensajes flotantes)
- `ShowMessage(string text, float duration)` — aparece y desaparece con fade.
- Conectar a eventos:
  - `OnShieldPlaced` → "Escudo colocado en [carta]"
  - `OnDuelResolved` → "Ganó el Jugador X" / "Escudo roto"
  - `OnRoundWon` → "¡Jugador X completa un trío!"
  - `OnGameWon` → mostrar `GameOverPanel`

### 3.2 RoundEndPanel
- Aparece al `OnRoundWon`.
- Muestra quién ganó la ronda y cuántos tríos lleva.
- Botón **Continuar** → oculta el panel (la nueva ronda ya arrancó en el GameManager).

### 3.3 GameOverPanel
- Aparece al `OnGameWon`.
- Muestra "¡Jugador X gana!" y botón **Jugar de nuevo** (recarga la escena).

### 3.4 Menú principal (escena separada)
- Botón **Jugar** → carga `MainScene`.
- Selector de número de jugadores (2 por ahora).
- Botón **Salir**.

**Entregable S3**: experiencia completa desde menú hasta game over.

---

## Interfaces con otros equipos

| Equipo | Qué necesitas de ellos | Qué les das tú |
|---|---|---|
| 3 (lógica) | Ya listo — eventos y métodos públicos | Nada, tú consumes |
| 2 (cartas) | `CardVisual` con `SetHighlight` — ya existe | El raycast de selección llama `SetHighlight` |
| 1 (cámara) | Nada | Nada |

---

## Riesgos

- **DuelPanel con 2 jugadores en 1 pantalla**: en local, los dos jugadores ven la misma pantalla durante el duelo. Solución simple: un panel por turno ("Jugador 1, elige tus cartas" → confirmar → "Jugador 2, elige tus cartas" → confirmar → resolver).
- **Selección de carta en 3D con FPV**: el raycast debe ignorar objetos que no sean cartas. Usar LayerMask `"Card"`.

---

## Orden de ejecución

1. `UIManager.cs` + TurnPanel + botón Tomar → probar que el turno cambia en pantalla.
2. `CardSelectorUI.cs` + botones Proteger/Atacar.
3. `DuelPanel` completo.
4. `MessagePanel` + `RoundEndPanel` + `GameOverPanel`.
5. Menú principal.
