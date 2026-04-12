---
title: Using Argon2 for password hashing
date: 2026-04-11
tags: [security, password, hashing]
type: pattern
project: DapperBanana/ASCII-Assault
---

The project now uses Argon2 for password hashing and verification. Argon2 is a modern key derivation function that is more resistant to GPU cracking attacks compared to older algorithms like bcrypt or SHA-based methods. This improves security. The `PasswordHelper` class likely encapsulates the Argon2 implementation, making it easy to use consistently across the application. This shows a proactive approach to security best practices.
