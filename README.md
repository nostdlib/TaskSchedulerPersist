# TaskSchedulerPersist

> .NET Framework 2.0 library that establishes Windows Task Scheduler persistence via raw COM interop -- no external dependencies required.

![Language](https://img.shields.io/badge/language-C%23-239120?style=flat-square&logo=csharp)
![Framework](https://img.shields.io/badge/.NET_Framework-2.0-512BD4?style=flat-square&logo=dotnet)
![Platform](https://img.shields.io/badge/platform-Windows-0078D6?style=flat-square&logo=windows)
![License](https://img.shields.io/badge/license-MIT-green?style=flat-square)
![MITRE ATT&CK](https://img.shields.io/badge/MITRE_ATT%26CK-T1053.005-red?style=flat-square)

---

## Features

- **Zero external dependencies** -- uses raw COM interop to interface with the Windows Task Scheduler API directly, avoiding the `Microsoft.Win32.TaskScheduler` NuGet package
- **Dual trigger persistence** -- registers a scheduled task that fires on both user logon and session unlock with a configurable 5-minute delay
- **Machine-unique task naming** -- derives task names from the machine GUID (`HKLM\SOFTWARE\Microsoft\Cryptography\MachineGuid`) for per-device uniqueness
- **User-context execution** -- runs under the current user's interactive token (no elevated privileges required for registration)
- **Configurable payload** -- Release builds use placeholder tokens (`%ExecutablePath%`, `%Arguments%`, `%WorkingDirectory%`) for operator-defined payloads
- **Post-build Base64 output** -- automatically encodes the compiled DLL to a `.b64.txt` file for easy transport and ingestion
- **Static constructor entry point** -- task creation logic executes from a static constructor, enabling activation when the assembly is loaded (e.g., via `Assembly.Load`)

## Requirements

| Requirement | Details |
|---|---|
| **OS** | Windows 7 / Server 2008 R2 or later |
| **Runtime** | .NET Framework 2.0+ |
| **Build tools** | MSBuild 15.0+ or Visual Studio 2017+ |
| **Targeting pack** | .NET Framework 2.0 Targeting Pack |
| **Privileges** | Standard user (no admin required) |

## Usage

### Build

```bash
# Debug build (uses hardcoded test payload)
msbuild TaskSchedulerPersist.csproj /p:Configuration=Debug /p:Platform=AnyCPU

# Release build (uses placeholder tokens for operator configuration)
msbuild TaskSchedulerPersist.csproj /p:Configuration=Release /p:Platform=AnyCPU
```

The post-build step produces `TaskSchedulerPersist.b64.txt` alongside the compiled DLL containing the Base64-encoded assembly.

### Configuration

In Release mode, replace these placeholders in the source before building:

| Placeholder | Description |
|---|---|
| `%ExecutablePath%` | Full path to the executable to persist |
| `%Arguments%` | Command-line arguments passed to the executable |
| `%WorkingDirectory%` | Working directory for the spawned process |

### Project Structure

```
Program.cs                  Entry point and task creation logic
DeviceInfo.cs               Machine GUID retrieval for unique task naming
TaskScheduler/
  ComInterfaces.cs          COM interop interface definitions (ITaskService, ITaskFolder, etc.)
  Enums.cs                  Task Scheduler enumerations (trigger types, logon types, etc.)
  NativeMethods.cs          SYSTEMTIME native struct with DateTime conversion
Properties/
  AssemblyInfo.cs           Assembly metadata
```

## How It Works

1. **Assembly load triggers static constructor** -- `Program` has a `static Program()` constructor that calls `Main()`, so task registration happens the moment the type is loaded.

2. **Machine GUID retrieval** -- `DeviceInfo.GetUUID()` reads `MachineGuid` from `HKLM\SOFTWARE\Microsoft\Cryptography` and formats it as a 32-character hex string. This becomes the scheduled task name (prefixed with `_`).

3. **COM interop initialization** -- The code instantiates `ITaskService` via its COM class (`CLSID 0F87369F-...`) and connects to the local Task Scheduler service. All Task Scheduler interfaces are declared inline as COM-imported interfaces with the correct GUIDs and marshaling attributes.

4. **Task definition** -- A new task definition is created with these settings:
   - **Author**: current username
   - **Logon type**: interactive token
   - **Network required**: true
   - **Start when available**: true
   - **Multiple instances**: ignore new
   - **No execution time limit** (`PT0S`)
   - **Runs on battery power**

5. **Trigger registration** -- Two triggers are attached:
   - `SessionStateChange` (session unlock) with a 5-minute delay, scoped to the current user's SID
   - `Logon` with a 5-minute delay, scoped to the current user's SID

6. **Action registration** -- An `IExecAction` is created pointing to the configured executable, arguments, and working directory.

7. **Task registration** -- The task is registered under the root folder (`\`) using `TASK_CREATE_OR_UPDATE`, so re-running the library updates the existing task rather than failing.

### MITRE ATT&CK Mapping

| Technique | ID | Description |
|---|---|---|
| Scheduled Task/Job: Scheduled Task | [T1053.005](https://attack.mitre.org/techniques/T1053/005/) | Creates a scheduled task for persistence |
| Boot or Logon Autostart Execution | [T1547](https://attack.mitre.org/techniques/T1547/) | Triggers on user logon and session unlock |

## Detection

Defenders can detect this technique through the following methods:

### Event Logs
- **Microsoft-Windows-TaskScheduler/Operational** (Event ID 106) -- logged when a new scheduled task is registered
- **Security log** (Event ID 4698) -- audit event for scheduled task creation (requires "Audit Other Object Access Events" policy)

### Filesystem Artifacts
- Scheduled task XML definitions stored in `C:\Windows\System32\Tasks\` -- look for tasks with names matching GUID patterns (32 hex characters prefixed with `_`)

### Behavioral Indicators
- Tasks registered under the root `\` folder with no description or documentation
- Tasks authored by a standard user that execute on both logon and session unlock
- Tasks with `PT0S` execution time limit (infinite runtime)
- Tasks configured with `StartWhenAvailable = true` and `RunOnlyIfNetworkAvailable = true`

### Tooling
- **Autoruns** (Sysinternals) -- enumerate scheduled tasks alongside other persistence mechanisms
- **Sysmon** (Event ID 1) -- monitor process creation from Task Scheduler (`svchost.exe -k netsvcs -p -s Schedule`)
- **SIGMA rules** -- community detection rules for suspicious scheduled task creation patterns
- Review scheduled tasks via `schtasks /query /fo LIST /v` or PowerShell `Get-ScheduledTask`

## Disclaimer

This project is provided **strictly for authorized security testing, education, and research purposes**. It is intended to help security professionals understand Windows persistence mechanisms, develop detection capabilities, and improve defensive posture.

**Do not use this software for unauthorized access to systems you do not own or have explicit written permission to test.** Misuse of this tool may violate applicable laws and regulations.

By using this software, you accept full responsibility for ensuring your activities comply with all applicable laws, regulations, and organizational policies.

See [RESPONSIBLE_USE.md](RESPONSIBLE_USE.md) for the full responsible use policy.

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE) for details.
