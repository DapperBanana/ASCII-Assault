---
title: Initialize NetworkStream in Constructor
date: 2026-04-20
tags: [c#, networkstream, resource-management]
type: pattern
project: DapperBanana/ASCII-Assault
---

Initializing the `NetworkStream` in the `ClientHandler` constructor ensures it's readily available when the client connection is established. This pattern promotes encapsulation by tying the stream's lifecycle directly to the `ClientHandler` instance. It also consolidates resource initialization in one place, making the code easier to understand and maintain. Properly disposing of the `NetworkStream` when the `ClientHandler` is disposed of (e.g., on client disconnect) will prevent resource leaks.
