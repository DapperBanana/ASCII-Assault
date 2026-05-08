---
title: Removed direct GameState broadcast to clients
date: 2026-05-07
tags: [networking, security, performance]
type: pattern
project: DapperBanana/ASCII-Assault
---

The commit history indicates an initial plan to broadcast the entire `GameState` directly to all clients. This approach was subsequently removed. Broadcasting the full game state can introduce security vulnerabilities by exposing internal game data to clients, which could be exploited for cheating or gaining an unfair advantage. Additionally, sending the entire game state every tick could lead to significant network overhead, especially as the game complexity and number of players increase. Instead, a more selective and controlled approach to data broadcasting should be used, where only the necessary information for each client is transmitted.
