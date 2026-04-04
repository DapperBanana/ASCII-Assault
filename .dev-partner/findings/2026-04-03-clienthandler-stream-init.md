---
title: Initialize client stream in constructor
date: 2026-04-03
tags: [design, c#, sockets]
type: pattern
project: DapperBanana/ASCII-Assault
---

Initializing the `NetworkStream` within the `ClientHandler` constructor ensures that the stream is immediately available when the client connection is established. This approach promotes better encapsulation and resource management by tying the stream's lifecycle directly to the `ClientHandler` instance. Doing so also avoids potential issues later if the stream weren't initialized before use by other handler methods.
