---
title: Centralized movement calculation
date: 2026-05-22
tags: [abstraction, game-logic]
type: pattern
project: DapperBanana/ASCII-Assault
---

Moving the core movement logic into the `Game` class promotes a cleaner separation of concerns. The `ClientHandler` should primarily deal with I/O and command processing, while the `Game` class manages the game state and its rules. This makes the code more maintainable and testable. This also prevents duplicated logic if other game entities require similar movement calculation.
