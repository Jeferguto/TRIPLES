# Plan de ejecución — Equipo 3 (Lógica de juego y turnos)

## Contexto

Juego **Triples** en Unity: 2–4 jugadores alrededor de una mesa con cámara FPV, por turnos, con clics sobre cartas. Reglas completas en `Diseño Creativo - Reglas de Triples .md`.

### Composición del mazo (regla oficial)
- **15 especiales** = 5 tríos × 3 cartas iguales.
- **24 numéricas**: 4×V2, 4×V3, 4×V4, 4×V5, 3×V6, 3×V7, 2×V8.
- **Tokens**: 6 azules + 6 rojos (para escudos — máx 4 por jugador).

### Acciones de turno
- **Tomar**: roba del mazo. Numérica → mano (máx 5, si se pasa debe descartar). Especial → al frente (sin límite).
- **Proteger**: pone escudo sobre una carta (máx 4 escudos por jugador).
- **Atacar**: reta a un duelo por una carta del oponente.

### Reglas del duelo
- 0–3 cartas por jugador, del **mismo palo** o **mismo número**.
- Suma mayor gana. Empate → defensor.
- Atacante descarta **todas** sus cartas, gane o pierda.
- Defensor ganador con 1 carta: la conserva. Con 2+: descarta una.
- Defensor perdedor: descarta todas.
- Si la carta atacada tenía escudo y el atacante gana: se rompe el escudo, la carta queda **vulnerable** (no cambia de dueño en esta versión).
- Vulnerable: penalización a definir (enum configurable).

### Victoria
- Ronda: completar un **triple** (3 especiales iguales). Conserva el triple, rebaraja el resto.
- Partida: acumular **3 triples**.

---

## Estado actual del repo

- `CameraController.cs` — rotación FPV con límites (Equipo 1, listo).
- `PlayerPositionManager.cs` — 4 posiciones fijas alrededor de la mesa.
- `GameManager.cs` — solo cambia la cámara con teclas 1–4. **Sin lógica de juego aún.**
- `PointerController.cs` — puntero UI.
- `MainScene.unity`.
- No hay prefabs de carta, ni UI, ni modelo de datos.

---

## Principios de arquitectura

1. **Lógica headless**: todo el modelo debe ser testeable sin prefabs ni UI.
2. **Eventos públicos**: los Equipos 2 (visual) y 4 (UI) se suscriben, no llaman directo. Sin acoplamiento.
3. **Separación de concerns**: `GameManager` actual hace dos cosas (cámara + estado). Extraer cámara a `CameraSwitcher` y dejar `GameManager` puro.
4. **ScriptableObjects** para `CardData` (permite crear cartas desde el editor y pintarlas con arte del Equipo 2).

---

## Sprint 1 — Modelo de datos y estado

### 1.1 Refactor `GameManager`
- Crear `CameraSwitcher.cs` que reciba `PlayerPositionManager` y mueva la cámara.
- `GameManager` pasa a ser singleton de estado puro, sin referencias a `Camera`/`Transform`.
- Debe suscribirse: `OnPlayerTurnStarted → CameraSwitcher.SwitchTo(playerId)`.

### 1.2 `CardData.cs` (ScriptableObject)
```csharp
enum CardType { Number, Special }
enum Suit { Rojo, Azul, Verde, Amarillo }   // coordinar # palos con Equipo 2

[CreateAssetMenu]
class CardData : ScriptableObject {
    string id;
    CardType type;
    Suit suit;                // solo Number
    int value;                // 2–8 para Number, 0 para Special
    string specialSetId;      // "trio_A".."trio_E" para Special
    string displayName;
    Sprite artwork;           // lo llena Equipo 2
}
```

### 1.3 `DeckBuilder.cs`
- Construye el mazo completo según reglas (15 + 24 = 39 cartas).
- Fisher-Yates shuffle con `System.Random` inyectable (semilla para tests).
- Punto único de verdad del mazo.

### 1.4 `PlayerState.cs`
```
List<CardData> hand                  // máx 5 numéricas
List<CardData> specialsInFront
Dictionary<CardData,int> shields     // máx 4 totales
HashSet<CardData> vulnerable
List<string> completedTrios          // win condition

AddToHand / RemoveFromHand
AddSpecial / HasTrio(setId)
TotalShields()
```

### 1.5 `Deck.cs` y `DiscardPile.cs`
- Wrappers con `Draw()`, `Peek()`, `AddDiscard()`, `Shuffle()`.
- Evento `OnCountChanged`.

**Entregable S1**: test de consola que construye mazo, baraja, reparte 5 cartas a 2 jugadores e imprime el estado. Sin UI.

---

## Sprint 2 — Máquina de estados de turno

### 2.1 `TurnStateMachine.cs`
```
enum GamePhase { Setup, DetermineFirstPlayer, PlayerTurn, Duel, RoundEnd, GameOver }
enum TurnAction { None, Draw, Protect, Attack, EndTurn }
```

### 2.2 Eventos públicos del `GameManager`
- `event Action<int> OnPlayerTurnStarted`
- `event Action<int, CardData> OnCardDrawn`
- `event Action<int, CardData> OnShieldPlaced`
- `event Action<DuelRequest> OnDuelStarted`
- `event Action<DuelResult> OnDuelResolved`
- `event Action<int> OnRoundWon`
- `event Action<int> OnGameWon`

### 2.3 API de acciones (pública)
- `bool TryDraw(int playerId)`
- `bool TryProtect(int playerId, CardData target)`
- `bool TryAttack(int attackerId, int defenderId, CardData targetCard)`
- `void EndTurn()`

Cada `TryX` valida, devuelve bool y dispara evento. No lanza excepciones por estados inválidos.

**Entregable S2**: flujo de turno completo **sin duelo**. Logs de consola muestran transiciones. Tests unitarios de `PlayerState` (mano máxima, escudos máximos, rotación de turnos).

---

## Sprint 3 — Duelo + victoria

### 3.1 `DuelController.cs`
- `DuelRequest { attackerId, defenderId, targetCard }`
- `DuelSubmission { playerId, CardData[] cards }` (0–3 cartas).
- Validar: mismo palo **o** mismo número.
- Calcular suma, resolver (empate → defensor).
- Aplicar descartes exactos según reglas.
- Escudo + atacante gana → carta queda `vulnerable`.

### 3.2 Victoria
- Completar trío → `OnRoundWon`, mover trío a `completedTrios`, rebarajar resto, nueva ronda.
- 3 tríos → `OnGameWon`.

### 3.3 Penalización "vulnerable"
- `GameSettings` ScriptableObject con enum:
  - `MinusOneDefense` (−1 defensa)
  - `PlusOneAttack` (+1 atacante)
  - `SingleCardDefenseOnly` (solo 1 carta para defender)
- Hablar con el equipo para elegir default.

**Entregable S3**: partida completa jugable. Tests de duelo cubriendo: empate, atacante gana, defensor gana con 1 y con 2+, ataque a carta con escudo, ataque a vulnerable.

---

## Interfaces con otros equipos

| Equipo | Qué te consume | Qué necesitas de ellos |
|---|---|---|
| 1 (cámara) | `OnPlayerTurnStarted` → mueve cámara | Ya listo |
| 2 (cartas) | `CardData`, eventos de draw/shield | Prefab que acepte `CardData` + `SetVisualState(normal/shielded/vulnerable)` |
| 4 (UI) | Eventos de turno + getters | Botones que llamen `TryDraw/Protect/Attack` |

---

## Riesgos y decisiones pendientes

- **Palos**: las reglas los mencionan pero no los definen. Coordinar con Equipo 2 cuántos palos existen y cómo se reparten las numéricas.
- **Vulnerable**: regla explícitamente marcada "[DEFINIR PENALIZACIÓN]". Elegir default.
- **3–4 jugadores**: las reglas son para 2. Mantener 2 como MVP; documentar 3–4 como stretch goal.
- **"Tomar" inicial**: las reglas dicen que el de menos especiales empieza; en empate, duelo con 1 carta. Implementar `DetermineFirstPlayer` como fase dedicada.

---

## Orden de ejecución

1. Sprint 1 → modelo de datos + refactor cámara.
2. Sprint 2 → turnos sin duelo.
3. Sprint 3 → duelo + victoria.
4. Pulido, testing e integración con Equipos 2/4.
