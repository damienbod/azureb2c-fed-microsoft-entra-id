﻿## Creating Migrations

### Console

dotnet ef migrations add init -c UserContext

### Powershell

Add-Migration "init" -context UserContext

## Running manually

Update-Database -context UserContext


**Links**

https://docs.microsoft.com/en-us/ef/core/miscellaneous/nullable-reference-types