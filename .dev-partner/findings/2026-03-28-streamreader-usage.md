---
title: Consider `using` statement for StreamReader/Writer
date: 2026-03-28
tags: [resource-management, csharp, best-practice]
type: pattern
project: DapperBanana/ASCII-Assault
---

The code initializes `StreamReader` and `StreamWriter` but doesn't wrap them in a `using` statement. This means the underlying stream might not be flushed or closed immediately when the `ClientHandler` thread exits or encounters an error. Using a `using` statement ensures that `Dispose()` is called, guaranteeing resources are released promptly. Can help prevent resource leaks, especially in long-running server applications handling many clients.
