---
title: Consider deterministic initial player positions
date: 2026-05-24
tags: [game-dev, networking, architecture]
type: pattern
project: DapperBanana/ASCII-Assault
---

When new players connect, the server sends their initial positions. Currently, these are presumably randomly assigned. For debugging, testing, and potential future features like replays or deterministic game states, consider using a seed-based or pre-defined set of initial positions. This would make the game more predictable and easier to reason about, especially when dealing with edge cases or bugs related to player interactions at the start of a match. A seed could be stored and replayed to recreate a specific game scenario.
