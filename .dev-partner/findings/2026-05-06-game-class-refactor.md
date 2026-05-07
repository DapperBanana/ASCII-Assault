---
title: Encapsulating Game Logic in a Dedicated Class
date: 2026-05-06
tags: [design, refactoring, separation-of-concerns]
type: pattern
project: DapperBanana/ASCII-Assault
---

Moving game boundaries and related logic into a `Game` class promotes better separation of concerns. Instead of scattering game rules across different parts of the codebase (e.g., input handling, rendering), the `Game` class acts as a central authority. This makes the code easier to understand, test, and maintain. When new game rules or mechanics need to be added or existing ones modified, the changes are localized to the `Game` class, minimizing the risk of unintended side effects elsewhere in the system. This approach also facilitates unit testing of game logic independently of I/O or network components.
