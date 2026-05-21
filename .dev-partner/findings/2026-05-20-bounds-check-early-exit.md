---
title: Bounds check early exit pattern
date: 2026-05-20
tags: [csharp, performance, defensive-programming]
type: pattern
project: DapperBanana/ASCII-Assault
---

The code uses an early-exit pattern for bounds checking. This involves checking if the new position is out of bounds *before* performing the movement logic. If the position is out of bounds, the function returns immediately. This avoids unnecessary calculations and potential issues that might arise from operating on out-of-bounds coordinates. This improves efficiency and reduces the risk of errors. It keeps the primary movement logic cleaner and easier to read, as it only executes when the new position is valid.
