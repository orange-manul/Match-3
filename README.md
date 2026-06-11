# Match-3 Mobile Game (Unity)

A mobile Match-3 puzzle game developed in Unity, utilizing an object-oriented approach in C#.

---

## 📸 Gameplay

The core grid generation, tile swiping mechanics, and cascade board updates upon matching are fully implemented and demonstrated below:

### Current Board State:
![Game Board](./Assets/GameplayMechanics/Gameplay.gif)

---

## 🛠 Features Implemented in This Development Cycle

During this working session, the core architectural mechanics were successfully implemented and optimized:

* **New Event System Integration (Unity UI):** Input handling has been migrated from the legacy `Input.mousePosition` to modern, cross-platform event processing using `IPointerDownHandler` and `IPointerUpHandler` (`eventData.position`). This completely resolved incompatibility issues with the new Input System package and prepared the game for mobile touch controls.
* **UI Raycast Bug Fix:** Adjusted `Raycast Target` parameters across the hierarchy. Disabling the raycast target on the background panel (`GridParent`) prevents it from blocking input, allowing tiles to correctly register user swipes.
* **Full Match-3 Game Loop (Coroutines):** Implemented a centralized `BoardController` to manage the gameplay cycle. Added smooth tile swapping using `Vector2.Lerp`, match detection along both horizontal and vertical axes, and an automatic swap-revert mechanism if no match is formed.
* **Refactoring & Code Optimization:** Eliminated code duplication by moving tile instantiation and setup logic into a unified factory method, `CreateTileObject`. Rewrote the cascade grid replenishment algorithm (`RefillBoard`), ensuring new fruits spawn in a clean, dynamic top-down queue without overlapping issues.

---

## 🚀 Tech Stack
* **Engine:** Unity 6 (6000.3.9f1)
* **Language:** C#
* **Platform:** Mobile (Android / iOS)
* **Input System:** New Input System Package + Unity UI EventSystem
