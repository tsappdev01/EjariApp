#!/bin/bash
# SessionStart hook for Claude Code on the web: installs the .NET SDK, starts a
# local SQL Server 2022 container and, when the ABP feed key is configured,
# restores packages. Safe to run repeatedly. Never writes secrets to the repo.
set -euo pipefail

if [ "${CLAUDE_CODE_REMOTE:-}" != "true" ]; then
  exit 0
fi

cd "${CLAUDE_PROJECT_DIR:-$(dirname "$0")/../..}"
ENV_FILE="${CLAUDE_ENV_FILE:-/dev/null}"
log() { echo "[session-start] $*" >&2; }

# --- .NET SDK -----------------------------------------------------------------
# The projects target net9.0. builds.dotnet.microsoft.com is usually blocked in
# this environment, so fall back to Ubuntu's .NET 10 SDK, which builds net9.0
# and runs it with roll-forward.
if ! command -v dotnet >/dev/null 2>&1; then
  if curl -fsSL --max-time 20 https://dot.net/v1/dotnet-install.sh -o /tmp/dotnet-install.sh 2>/dev/null \
     && bash /tmp/dotnet-install.sh --channel 9.0 --install-dir /usr/share/dotnet >/dev/null 2>&1; then
    ln -sf /usr/share/dotnet/dotnet /usr/local/bin/dotnet
    log ".NET 9 SDK installed"
  else
    log ".NET 9 download blocked; installing dotnet-sdk-10.0 from apt"
    apt-get update -qq >/dev/null 2>&1 || true
    DEBIAN_FRONTEND=noninteractive apt-get install -y -qq dotnet-sdk-10.0 >/dev/null
  fi
fi
{
  echo 'export DOTNET_CLI_TELEMETRY_OPTOUT=1'
  echo 'export DOTNET_NOLOGO=1'
  # Lets the net9.0 apps run on the .NET 10 runtime when 9 is not installed.
  echo 'export DOTNET_ROLL_FORWARD=Major'
} >> "$ENV_FILE"
log "dotnet $(dotnet --version)"

# --- ABP commercial feed --------------------------------------------------------
# The key goes in the user-level NuGet config, never in the repo. Set
# ABP_NUGET_API_KEY in the environment settings; nuget.abp.io must also be
# allowed by the network policy.
if [ -n "${ABP_NUGET_API_KEY:-}" ]; then
  dotnet nuget remove source ABP >/dev/null 2>&1 || true
  dotnet nuget add source "https://nuget.abp.io/${ABP_NUGET_API_KEY}/v3/index.json" --name ABP >/dev/null
  if dotnet restore DIP.sln >/tmp/dotnet-restore.log 2>&1; then
    log "dotnet restore succeeded"
  else
    log "dotnet restore FAILED - see /tmp/dotnet-restore.log (is nuget.abp.io allowed?)"
  fi
else
  log "ABP_NUGET_API_KEY not set: skipping restore (ABP commercial packages are required to build)"
fi

# --- Local SQL Server -----------------------------------------------------------
if command -v docker >/dev/null 2>&1; then
  if ! docker info >/dev/null 2>&1; then
    (nohup dockerd >/tmp/dockerd.log 2>&1 &)
    for _ in $(seq 1 30); do docker info >/dev/null 2>&1 && break; sleep 1; done
  fi
  # Throwaway container bound to localhost; the password is generated per machine.
  PASS_FILE="$HOME/.ejari-local-sql-password"
  [ -s "$PASS_FILE" ] || echo "Dev_$(head -c 18 /dev/urandom | base64 | tr -dc 'A-Za-z0-9')!1" > "$PASS_FILE"
  SA_PASSWORD="$(cat "$PASS_FILE")"
  if docker info >/dev/null 2>&1; then
    if ! docker container inspect sql >/dev/null 2>&1; then
      docker run -d --name sql -e ACCEPT_EULA=Y -e "MSSQL_SA_PASSWORD=$SA_PASSWORD" \
        -p 127.0.0.1:1433:1433 mcr.microsoft.com/mssql/server:2022-latest >/dev/null
    else
      docker start sql >/dev/null
    fi
    for _ in $(seq 1 60); do
      docker exec sql /opt/mssql-tools18/bin/sqlcmd -C -S localhost -U sa -P "$SA_PASSWORD" -Q "SELECT 1" >/dev/null 2>&1 && break
      sleep 2
    done
    {
      echo "export LOCAL_SQL_SA_PASSWORD='$SA_PASSWORD'"
      echo "export ConnectionStrings__Default='Server=localhost,1433;Database=Ejari_dip;User Id=sa;Password=$SA_PASSWORD;TrustServerCertificate=True;MultipleActiveResultSets=true'"
    } >> "$ENV_FILE"
    log "SQL Server 2022 running on localhost:1433 (container 'sql')"
  else
    log "docker daemon unavailable: SQL Server not started"
  fi
fi
