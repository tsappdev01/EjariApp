# SQL CLR encryption assemblies (SAFE)

Source for the two CLR assemblies that `db/db.sql` registers in
`ProdPropertyManagementSystem`, rebuilt so they load with `PERMISSION_SET = SAFE`
instead of `UNSAFE`:

| Assembly | SQL functions | Used by |
|---|---|---|
| `EncryptionUrlHelper` | `dbo.EncryptUrl`, `dbo.DecryptUrl` | Persisted column `MaintainEORegistrations.EncryptedRefNo`, EO procedures |
| `EncryptionHelper` | `dbo.EncryptUrlSafe` | Procedures |

The output is byte-for-byte what the originals and the web app's
`src/DIP.Blazor/Shared/EncryptionHelper.cs` produce: AES-256-CBC, PKCS7, UTF-8,
URL-safe Base64 without padding. The only change is `AesManaged` (fully managed)
instead of `Aes.Create()`, which goes through the OS crypto provider and is why the
originals needed `UNSAFE`. SAFE assemblies also load on SQL Server on Linux, which
only accepts SAFE.

Namespaces, class names, method names and the assembly identity (`1.0.0.0`,
unsigned, .NET Framework 4.8) match the originals, so `db.sql`'s own
`CREATE FUNCTION` statements bind to these builds and `ALTER ASSEMBLY` can replace
the originals in place.

## The key is not in source

The build reads the key and IV from the environment and fails if either is missing
or the wrong length:

```bat
set CLR_AES_KEY=<base64 of 32 bytes>
set CLR_AES_IV=<base64 of 16 bytes>
dotnet build db\clr\DIP.Clr.sln -c Release
dotnet test  db\clr\DIP.Clr.sln -c Release
```

(PowerShell: `$env:CLR_AES_KEY = '...'`. Or pass `-p:ClrAesKey=... -p:ClrAesIV=...`.)

The compiled DLLs, the generated `obj/**/ClrKeys.g.cs` and the generated
`deploy-*.sql` all contain the key. `db/clr/.gitignore` keeps `bin/` and `obj/`
out of git; do not commit, email or attach any of them.

## Tests

`tests/Clr.Tests` compiles this code for .NET 9 together with the web app's
`EncryptionHelper.cs` (linked, unmodified) and checks that the database and the app
agree: same ciphertext, each decrypts the other's output, URL-safe, deterministic,
`NULL` in `NULL` out. If the app's helper changes, these tests say so.

## Deploying

Each build writes `<project>/bin/Release/net48/deploy-<assembly>.sql`. Run it in
the target database (SSMS, or `sqlcmd -d ProdPropertyManagementSystem -i ...`) as
a sysadmin. It is safe to re-run. It:

1. trusts the assembly's SHA-512 hash (`sp_add_trusted_assembly`), as
   `clr strict security` requires;
2. **creates** the assembly if absent, does nothing if this exact build is already
   installed, or **replaces** it with `ALTER ASSEMBLY ... SAFE, UNCHECKED DATA`;
3. after a replace, runs `DBCC CHECKTABLE ... WITH EXTENDED_LOGICAL_CHECKS` on every
   table whose persisted values depend on the assembly. That recomputes each
   `EncryptedRefNo` with the new binary. If any value differs (a wrong key or IV),
   it **puts the previous binary back** and fails with
   `stored values do not match the new assembly`;
4. creates the assembly's functions if they are absent (definitions in
   `<project>/functions.sql`, identical to `db.sql`).

Output to expect:

- A replace prints `Verifying [dbo].[MaintainEORegistrations]` and then
  `... replaced and verified.`
- A failed replace prints `Msg 2537 ... The record check (valid computed column)
  failed` for each mismatching row, then restores the previous binary and raises
  error 50000. Nothing is left changed.

### On production

- Back up the database first and run it in a maintenance window. Between the
  `ALTER` and the end of verification, the new binary is live. That window is the
  length of `CHECKTABLE` on `MaintainEORegistrations`, and if verification fails
  the old binary is back once the script ends.
- Once both assemblies are SAFE, whatever currently lets the UNSAFE originals load
  can go. That is either their trusted hashes (`sys.trusted_assemblies`), a
  signing certificate/login, or `TRUSTWORTHY ON` on the database
  (`SELECT is_trustworthy_on FROM sys.databases WHERE name = DB_NAME()`).
- This project reproduces the current key. Rotating the key changes every
  `EncryptedRefNo` and every link already sent. Rotation is a separate change: the
  persisted column has to be rebuilt, and the app has to accept old and new keys for
  a while.

Tested against SQL Server 2022 on Linux: create on an empty database, no-op re-runs,
replace over an existing build, and a wrong-key build detected and rolled back with
the stored values untouched.
