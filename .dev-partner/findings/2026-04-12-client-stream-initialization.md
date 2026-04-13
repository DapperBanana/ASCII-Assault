---
title: Client stream initialized in ClientHandler constructor
date: 2026-04-12
tags: [design, networking, initialization]
type: pattern
project: DapperBanana/ASCII-Assault
---

The client stream is now initialized within the `ClientHandler` constructor. This ensures that the stream is properly set up when a new client connection is established. Initializing resources in the constructor provides a clear and predictable initialization sequence, reducing the risk of null reference exceptions or uninitialized state issues later in the client handling logic. It encapsulates the stream setup within the `ClientHandler`, improving code organization and maintainability.
