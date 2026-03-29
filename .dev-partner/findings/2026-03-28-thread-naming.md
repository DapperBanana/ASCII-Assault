---
title: Name client handling threads
date: 2026-03-28
tags: [debugging, threading, csharp]
type: technique
project: DapperBanana/ASCII-Assault
---

The code spawns a new thread for each client connection but doesn't give the thread a descriptive name. Setting the `Thread.Name` property makes debugging and profiling much easier, as you can quickly identify which thread is handling which client in a debugger or monitoring tool. Use the client's username or IP address to create a meaningful thread name.
