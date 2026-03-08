---
name: jat-build-debug-and-deployment-workflow
Description: Build variants, property flags, SMAPI discovery, build output locations, debug workflow, mod deployment, release packaging, troubleshooting. Use when: building and running the mod, debugging in-game, creating releases, or troubleshooting build issues.
argument-hint: "Describe the build task: Do you need to build for debugging, deploy to SMAPI, create a release package, or troubleshoot a build failure?"
target: vscode
---

# Build, Debug, and Deployment Workflow Skill

JAT is a Stardew Valley mod built with .NET and deployed via SMAPI. Understanding build variants, property flags, SMAPI mod discovery, and debug workflows ensures successful development and deployment cycles.

## 1. Build Variants Overview ##

**Three Build Targets:**

| Variant | Purpose | Deploys | Zips | Use When |
|---------|---------|---------|------|----------|
| **debug check-only** | Compilation validation | ❌ No | ❌ No | Quick syntax check before committing |
| **debug with deploy** | Development build | ✓ Yes (copies DLL to SMAPI mods) | ✓ Yes | Testing in-game during development |
| **release package** | Distribution build | ❌ No | ✓ Yes (creates .zip archive) | Creating release for distribution |

**Build Configuration:**
```bash
# Check-only (fastest; validates syntax)
dotnet build JojaAutoTasks.csproj -c Debug -p:EnableModDeploy=false -p:EnableModZip=false

# Debug deploy (copies to SMAPI)
dotnet build JojaAutoTasks.csproj -c Debug -p:EnableModDeploy=true -p:EnableModZip=true

# Release package
dotnet build JojaAutoTasks.csproj -c Release -p:EnableModDeploy=false -p:EnableModZip=true
```

**Property Flags:**
- `-p:EnableModDeploy=true`: Copy compiled DLL to SMAPI mods folder (enables auto-discovery)
- `-p:EnableModZip=true`: Create .zip archive of compiled mod (for distribution)
- `-c Debug`: Debug configuration (symbols included, optimizations minimal)
- `-c Release`: Release configuration (optimized, symbols optional)

## 2. Build Output Locations ##

**Output Structure:**
```
bin/
├── Debug/
│   └── net6.0/
│       ├── JojaAutoTasks.dll
│       ├── JojaAutoTasks.pdb
│       ├── manifest.json
│       └── [dependencies]
│
└── Release/
    └── net6.0/
        ├── JojaAutoTasks.dll
        ├── manifest.json
        └── [dependencies]

obj/
├── Debug/
│   └── net6.0/
│       └── [intermediate build files]
└── Release/
    └── net6.0/
        └── [intermediate build files]

// When EnableModDeploy=true:
mods/JojaAutoTasks/
├── JojaAutoTasks.dll
├── manifest.json
└── [assets if included]

// When EnableModZip=true:
releases/
└── JojaAutoTasks.v1.0.0.zip
    ├── JojaAutoTasks/
    │   ├── JojaAutoTasks.dll
    │   └── manifest.json
    └── [documentation]
```

**Discovering Output:**
```powershell
# Find compiled DLL (debug)
Get-Item bin/Debug/net6.0/JojaAutoTasks.dll

# Find SMAPI deployment location
$gamePath = (dotnet msbuild JojaAutoTasks.csproj -nologo -getProperty:GamePath).Trim()
Get-Item "$gamePath/Mods/JojaAutoTasks/"

# Find release .zip
Get-Item releases/JojaAutoTasks.*.zip | Sort-Object LastWriteTime -Descending | Select-Object -First 1
```

## 3. SMAPI Discovery and Launch ##

**GamePath Resolution:**
```csharp
// MSBuild resolves GamePath from registry or config
var gamePath = (dotnet msbuild JojaAutoTasks.csproj -nologo -getProperty:GamePath).Trim();

// Example output:
// C:\Program Files\Steam\steamapps\common\Stardew Valley

// Validate before use
if (!Directory.Exists(gamePath))
    throw new InvalidOperationException($"Game not found at: {gamePath}");

// Find SMAPI executable
var smapiExe = Path.Combine(gamePath, "StardewModdingAPI.exe");
if (!File.Exists(smapiExe))
    throw new InvalidOperationException($"SMAPI not found at: {smapiExe}");
```

**Launch Sequence:**
```powershell
# PowerShell task for (build and) launch
$gamePath = (dotnet msbuild JojaAutoTasks.csproj -nologo -getProperty:GamePath).Trim()
$smapiExe = Join-Path $gamePath 'StardewModdingAPI.exe'

# Run the game
Start-Process -FilePath $smapiExe -WorkingDirectory $gamePath

Write-Host "Started SMAPI at: $smapiExe"
```

**SMAPI Mod Discovery Process:**
1. **SMAPI starts** → scans `Mods/` folder
2. **Finds JojaAutoTasks folder** with manifest.json
3. **Loads JojaAutoTasks.dll** (must match game architecture: net6.0)
4. **Calls ModEntry.Entry()** → mod initializes
5. **Exits if load fails** → check error log

**manifest.json Validity:**
```json
{
    "Name": "Joja AutoTasks",
    "Author": "YourName",
    "Version": "1.0.0",
    "Description": "Automatic task management for Stardew Valley",
    "UniqueID": "author.JojaAutoTasks",
    "EntryDll": "JojaAutoTasks.dll",
    "MinimumApiVersion": "3.13.0"
}
```

## 4. Debug Workflow ##

**Step 1: Build with Deploy**
```bash
dotnet build JojaAutoTasks.csproj -c Debug -p:EnableModDeploy=true -p:EnableModZip=false
```
This copies DLL to `mods/JojaAutoTasks/` where SMAPI will discover it.

**Step 2: Launch with Debugger (VS Code)**
- Set breakpoints in code
- Debug → Start Debugging (F5)
- Game launches; debugger attached
- Breakpoints trigger when code runs

**Step 3: Inspect State**
- Hover over variables to view values
- Use Debug Console to evaluate expressions
- Set watches on complex objects
- Use conditional breakpoints (`Logger != null`)

**Step 4: Hot Reload (Not Supported)**
- ⚠️ SMAPI does **not** support hot reload
- Must close game → rebuild → relaunch
- Build time ~2 seconds; restart is faster than you'd expect

**Debug Configuration (tasks.json):**
```json
{
    "label": "build JojaAutoTasks",
    "type": "shell",
    "command": "$ErrorActionPreference = 'Stop'; dotnet build JojaAutoTasks.csproj -c Debug -p:EnableModDeploy=true -p:EnableModZip=true; [launch SMAPI]"
}
```

## 5. Deployment to SMAPI ##

**Auto-Deployment (Via Build Flag):**
```bash
# Build automatically copies to SMAPI mods folder
dotnet build -p:EnableModDeploy=true
# Result: mods/JojaAutoTasks/JojaAutoTasks.dll created
```

**Manual Deployment (If Needed):**
```powershell
# Copy DLL to SMAPI manually
$dll = "bin/Debug/net6.0/JojaAutoTasks.dll"
$modsPath = "C:\Program Files\Steam\steamapps\common\Stardew Valley\Mods\JojaAutoTasks\"

Copy-Item $dll $modsPath
```

**Verification:**
```powershell
# Verify SMAPI can discover mod
$modsPath = "C:\Program Files\Steam\steamapps\common\Stardew Valley\Mods"
Test-Path "$modsPath/JojaAutoTasks/JojaAutoTasks.dll"  # Should be True

# Check SMAPI log
$logPath = "C:\Program Files\Steam\steamapps\common\Stardew Valley\ErrorLogs\"
Get-Content "$logPath/SMAPI-latest.txt" | Select-String "JojaAutoTasks"
```

## 6. Release Packaging ##

**Build Release Package:**
```bash
dotnet build JojaAutoTasks.csproj -c Release -p:EnableModDeploy=false -p:EnableModZip=true
```

**Output Structure in .zip:**
```
JojaAutoTasks/
├── JojaAutoTasks.dll     (release build, optimized)
├── manifest.json
├── README.md              (optional but recommended)
├── LICENSE.txt            (if applicable)
└── [documentation files]
```

**Naming Convention:**
- `JojaAutoTasks.v1.0.0.zip` (version included)
- Place in `releases/` folder
- Include README with installation instructions

**Distribution Checklist:**
- [ ] Version number updated in manifest.json and AssemblyInfo
- [ ] CHANGELOG.md updated with new features/fixes
- [ ] README.md complete with installation + usage instructions
- [ ] LICENSE.txt included if required
- [ ] No debug symbols in release DLL (Release build removes them)
- [ ] All dependencies packaged or declared as mod requirements

## 7. Build Troubleshooting ##

**Error: "Could Not Find GamePath"**
```
Cause: Stardew Valley not installed or registry not set
Fix:   Ensure game is installed; check registry at:
       HKEY_CURRENT_USER\Software\ConcernedApe\Stardew Valley
       GamePath should point to install directory
```

**Error: "SMAPI Executable Not Found"**
```
Cause: SMAPI not installed in game directory
Fix:   Download SMAPI from smapi.io; extract to game folder
       Verify: C:\Path\To\Game\StardewModdingAPI.exe exists
```

**Error: "Assembly Version Mismatch"**
```
Cause: Built for wrong .NET target (e.g., net8.0 instead of net6.0)
Fix:   Check .csproj file; ensure <TargetFramework>net6.0</TargetFramework>
       Rebuild: dotnet build --force
```

**Error: "Mod Won't Load (SMAPI Log Shows Error)"**
```
Steps:
1. Check SMAPI log: C:\Path\To\Game\ErrorLogs\SMAPI-latest.txt
2. Look for "JojaAutoTasks" entries
3. Common causes:
   - Missing dependency (check manifest MinimumApiVersion)
   - Syntax error in ModEntry.Entry() (fix and rebuild)
   - File not copied to mods folder (rebuild with EnableModDeploy=true)
```

## 8. Testing In-Game ##

**Before Testing:**
- Build succeeds (compile errors fixed)
- DLL copied to mods folder
- SMAPI log shows mod loaded without errors

**During Testing:**
- Use SMAPI console (` key) to check for warnings
- Monitor log file for exceptions
- Test all critical paths (startup, day transition, saving)

**Debugging In-Game:**
```csharp
// Log diagnostic info
_logger.Log($"Current phase: {_coordinator.CurrentPhase}");
_logger.Log($"Tasks generated: {_stateStore.GetCurrentSnapshot().Tasks.Count}");

// Use console commands (if implemented)
// Example: 'jat dump-state' prints state to log
```

## Links ##
- [External Resources Instructions](../Instructions/external-resources.instructions.md)
