---
title: Server thread exception handling
date: 2026-04-09
tags: [c#, sockets, threading, exception]
type: gotcha
project: DapperBanana/ASCII-Assault
---

The main server thread now spawns `ClientHandler` threads. Unhandled exceptions within these client threads will crash the entire server. Implement robust exception handling *within* the `HandleClient` method to prevent a single client from bringing down the entire server. Log exceptions appropriately and consider sending an error message to the client before closing the connection. Wrap the `HandleClient` call in a `try...catch` block to achieve this.
