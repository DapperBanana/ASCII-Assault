---
title: Configuration loading in SQL Handler
date: 2026-03-27
tags: [configuration, database, initialization]
type: pattern
project: DapperBanana/ASCII-Assault
---

The SQL Handler is now loading its configuration from `appsettings.json`. Centralizing configuration in a dedicated file is a good practice, as it promotes maintainability and allows for easy modification of database settings without recompiling the code. Verify that sensitive data like database passwords are not stored directly in `appsettings.json` in plain text, especially if the repository is public or shared. Consider using environment variables or a secrets management solution for such sensitive information. Also ensure that any connection strings using user credentials are created securely (e.g. using the `SqlConnectionStringBuilder` class to avoid vulnerabilities related to string concatenation).
