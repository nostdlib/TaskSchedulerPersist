# TaskSchedulerPersist

.NET Framework 2.0 library that creates Windows Task Scheduler persistence via COM interop. Registers a scheduled task that triggers on user logon and session unlock.

## Features

- Creates scheduled tasks using raw COM interop (no `Microsoft.Win32.TaskScheduler` dependency)
- Triggers: user logon + session unlock (5-minute delay)
- Device identification via WMI BIOS UUID
- Registry Run key helper for alternative persistence
- Post-build base64 encoding of output DLL

## Project Structure

```
Program.cs                  Entry point, task creation logic
RegistryHelper.cs           Registry Run key + device serial number helpers
TaskScheduler/
  ComInterfaces.cs          COM interop interfaces for Task Scheduler
  Enums.cs                  Task Scheduler enumerations
  NativeMethods.cs          SYSTEMTIME native struct
Properties/
  AssemblyInfo.cs           Assembly metadata
```

## Build

Requires MSBuild / Visual Studio with .NET Framework 2.0 targeting pack.

```
msbuild TaskSchedulerPersist.csproj /p:Configuration=Release
```

The post-build step outputs a `.b64.txt` file containing the base64-encoded DLL.

## Configuration

In Release mode, the following placeholders are replaced at build/deploy time:

| Placeholder | Description |
|---|---|
| `%ExecutablePath%` | Path to the executable to persist |
| `%Arguments%` | Command-line arguments |
| `%WorkingDirectory%` | Working directory for the process |
