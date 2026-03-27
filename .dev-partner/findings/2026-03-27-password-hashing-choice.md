---
title: Password hashing algorithm selection
date: 2026-03-27
tags: [security, password hashing, cryptography]
type: learning
project: DapperBanana/ASCII-Assault
---

The developer added password hashing, which is great for security. However, the specific hashing algorithm wasn't mentioned in the commit message. It's important to use a modern, well-vetted algorithm like Argon2, bcrypt, or scrypt. Older algorithms like MD5 or SHA-1 are completely broken and should *never* be used for password hashing. Even SHA-256 or SHA-512 are not ideal *without* salting and key stretching. Explicitly defining the hashing algorithm and its configuration (salt length, iteration count) is crucial for future security audits and potential upgrades.
