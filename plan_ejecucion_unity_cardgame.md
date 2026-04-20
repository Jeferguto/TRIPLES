# Plan de ejecución para juego de cartas en Unity (FPV, 2-4 jugadores en mesa)

## Objetivo principal
Desarrollar un juego de cartas en Unity con estilo de controles en primera persona (FPV) para 2 a 4 jugadores sentados alrededor de una mesa. El juego será por turnos y permitirá hacer clic en las cartas para interactuar.

## Resumen
Este plan divide el proyecto en fases claras y asigna trabajo para que cuatro personas puedan avanzar en paralelo. Incluye:
- Preparación del proyecto
- Desarrollo de la mesa y jugadores
- Lógica de cartas y turnos
- Interfaz e interacción
- Red multiplayer local o en línea
- Pruebas y ajustes

---

## Fase 0: Preparación inicial

### 0.1. Configuración del entorno
- Instalar Unity Hub y una versión de Unity recomendada (por ejemplo, Unity 2023 LTS o 2024 LTS).
- Crear un nuevo proyecto 3D en Unity.
- Configurar el control de versiones (Git) si no está hecho.
- Elegir assets gratis de cartas, mesa y elementos de UI. Ejemplos:
  - Texturas de cartas gratuitas en sitios como OpenGameArt, Kenney.nl, Unity Asset Store gratis.
  - Muebles y mesa 3D básicos.
- Crear carpetas en el proyecto: `Assets/Scripts`, `Assets/Scenes`, `Assets/Prefabs`, `Assets/Materials`, `Assets/Art`.

### 0.2. Requisitos y alcance
- Definir reglas básicas del juego de cartas.
- Decidir si será local (un solo PC con 2-4 jugadores) o multiplayer en red.
- Definir la experiencia del jugador: sentarse en una mesa, ver las cartas frente a sí, girar la cámara con el mouse.

---

## Fase 1: Prototipo de mesa y cámara FPV

### 1.1. Escena de mesa
- Crear un `Scene` llamada `MainScene`.
- Colocar la mesa y una superficie plana.
- Añadir iluminación básica y cámara.

### 1.2. Cámara y movimiento FPV
- Implementar un controlador de cámara estilo FPV con mouse para rotar y teclado para movimiento leve.
- Limitar movimiento para que el jugador se mantenga alrededor de la mesa.
- Hacer que la cámara se ubique en una posición inicial sobre la mesa, como si el jugador estuviera sentado.

### 1.3. Representación de los jugadores en la mesa
- Crear cuatro posiciones de jugador alrededor de la mesa.
- Mostrar un marcador o área de juego para cada jugador.
- Asignar una vista inicial para cada jugador si hay cambio de turno.

### Entregable de fase 1
- `MainScene` con mesa, cámara FPV funcional y posición de jugadores visibles.
- Script de control de cámara y esquema de posiciones.

---

## Fase 2: Modelado de cartas y tablero de juego

### 2.1. Prefab de carta
- Crear un `Prefab` de carta con `Mesh Renderer` o `Sprite`.
- Añadir varias cartas de ejemplo con colores, nombres y valores.
- Añadir colisiones (`BoxCollider`) para detectar clics.

### 2.2. Zona de mesa y zonas de jugador
- Definir zonas de baraja, mano y descarte para cada jugador.
- Posicionar visualmente estas zonas alrededor de la mesa.

### 2.3. Interacción básica de clic
- Hacer raycast desde la cámara para detectar cuando se hace clic en una carta.
- Cambiar el color o resaltar la carta seleccionada.

### Entregable de fase 2
- Prefab de carta interactivo.
- Zonas de juego visibles y clics detectados correctamente.

---

## Fase 3: Lógica de juego por turnos

### 3.1. Modelo de datos de cartas
- Crear clases/estructuras para representar cartas:
  - `CardData` con atributos como `id`, `nombre`, `tipo`, `valor`, `texto`.
  - `PlayerHand`, `Deck`, `DiscardPile`.

### 3.2. Mecánica de turno
- Crear un `GameManager` que controle:
  - Estado actual del juego (`TurnoJugador`, `Fase`, `Estado`).
  - Rotación de turnos entre jugadores.
  - Acciones permitidas por turno.

### 3.3. Interacción de cartas por turno
- Solo permitir seleccionar y jugar carta si es el turno del jugador activo.
- Añadir una acción simple: robar carta, jugar carta, descartar carta.

### 3.4. UI de turno
- Mostrar en pantalla quién es el jugador activo y el estado del turno.
- Añadir botones `Terminar Turno`, `Robar Carta`, `Mostrar Mano`.

### Entregable de fase 3
- Juego por turnos funcional con 2-4 jugadores.
- Interacción de cartas controlada por turno.

---

## Fase 4: Interfaz de usuario y feedback

### 4.1. HUD y tutorial visual
- Crear una UI clara con `Canvas`:
  - Indicador de jugador actual.
  - Contador de cartas.
  - Mensajes de acción.
- Añadir iconos y textos legibles.

### 4.2. Popups y animaciones
- Animar cartas al robar o jugar.
- Mostrar efectos simples cuando se selecciona una carta.

### 4.3. Menú principal y opciones
- Crear menú inicial con `Start Game`, `Opciones`, `Salir`.
- Permitir elegir número de jugadores y modo de juego.

### Entregable de fase 4
- UI funcional con retroalimentación clara.
- Menú básico para iniciar la partida.

---

## Fase 5: Multiplayer y sincronización (opcional avanzado)

### 5.1. Arquitectura multiplayer
- Evaluar opciones:
  - Local (pases de dispositivo / mismos controles) o
  - Red con UNet / Netcode for GameObjects / Photon.
- Definir si cada jugador usa una instancia o si todos comparten una sola.

### 5.2. Sincronización de cartas y estado
- Sincronizar posición de cartas, mano de cada jugador y turno activo.
- Asegurarse de que solo el jugador activo ejecute acciones.

### 5.3. Pruebas de red
- Probar conexión entre 2 a 4 clientes.
- Manejar reconexión y estados inconsistentes.

### Entregable de fase 5
- Versión multiplayer básica estable o esquema preparado para migrar a red.

---

## Fase 6: Pruebas, pulido y entrega

### 6.1. Pruebas de juego
- Jugar varias partidas completas.
- Revisar errores de selección, estado de turno y comportamiento de cartas.

### 6.2. Pulido visual
- Ajustar iluminación y materiales.
- Mejorar animaciones de cartas y transiciones.

### 6.3. Documentación
- Crear un documento corto con las reglas del juego y cómo ejecutar la escena.
- Documentar la arquitectura principal y los scripts clave.

---

## División de trabajo para 4 compañeros

### Equipo 1: Fundamentos de escena y cámara
- Crear escena `MainScene`.
- Implementar cámara FPV y movimiento alrededor de la mesa.
- Definir posiciones de jugador.
- Resultados: escena jugable y controles básicos.

### Equipo 2: Modelado de cartas y físicas
- Crear prefabs de carta y materiales.
- Definir zonas de mano, baraja y descarte.
- Añadir colisiones y detección de clics.
- Resultados: cartas interactivas y físicamente posicionadas.

### Equipo 3: Lógica de juego y turnos
- Programar `GameManager` y sistema de turnos.
- Implementar modelo de cartas y estado de los jugadores.
- Crear reglas básicas de acción.
- Resultados: flujo de juego por turnos funcional.

### Equipo 4: UI y experiencia de usuario
- Crear HUD, menú principal y paneles de información.
- Implementar feedback visual y mensajes de turno.
- Pulir interacción con el mouse y animaciones.
- Resultados: UI clara y accesible.

> Si se desea versión multiplayer, dividir nuevamente:
> - Uno en red / sincronización.
> - Otro en optimización de UI y lógica local.

---

## Detalle de tareas paralelas por sprint

### Sprint 1: Base del juego
- Escena y controles FPV.
- Prefab de cartas y zonas de juego.
- Estructura de datos de cartas.
- UI básica de estado.

### Sprint 2: Interacción y turnos
- Raycast y selección de carta.
- `GameManager` de turnos.
- Acciones de jugar y robar.
- Mensajes y botones.

### Sprint 3: Pulido y pruebas
- Animaciones y efectos de carta.
- Ajustes de iluminación y materiales.
- Resolución de bugs y pruebas multijugador local.
- Documentación y build final.

---

## Cómo usar este plan con otro agente IA

1. Instrucciones: dar al agente el archivo `plan_ejecucion_unity_cardgame.md`.
2. Pedirle que genere scripts para cada fase, comenzando por `Fase 1`.
3. Solicitar tareas concretas por componente:
   - `CameraController`, `CardPrefab`, `GameManager`, `UIManager`.
4. Usar el plan como `context` para pedir código, estructura de carpetas y nombres de scripts.

---

## Notas adicionales
- Usa `Assets/Scripts` como carpeta central para todos los scripts.
- Mantén los prefabs de cartas independientes de la lógica del juego.
- Empieza con una versión local antes de plantear multiplayer.
- Prioriza primero una experiencia funcional y luego el pulido visual.

---

## Primeros pasos recomendados
1. Crear el proyecto en Unity 3D.
2. Añadir una mesa simple y luz ambiente.
3. Implementar la cámara FPV y el control de giro con mouse.
4. Crear un prefab de carta con collider.
5. Construir la lógica de turnos básica.

¡Con esto tienes un plan organizado para avanzar fase por fase y repartir tareas entre cuatro personas!