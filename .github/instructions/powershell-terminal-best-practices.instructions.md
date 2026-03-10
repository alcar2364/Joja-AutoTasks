---
name: powershell-terminal-best-practices
description: "PowerShell command construction rules for run_in_terminal: quoting, escaping, file paths, complexity limits, and tool alternatives. Use when: constructing PowerShell commands for terminal execution."
---

# PowerShell Terminal Best Practices #

## Purpose ##

This instruction file provides rules for constructing valid, safe PowerShell commands when using the `run_in_terminal` tool in Windows environments.

## Source of Truth ##

1. User's explicit terminal command request
2. This instruction file (syntax and safety rules)
3. [PowerShell Quoting Rules (Microsoft Learn)](https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_quoting_rules)
4. [PowerShell Language Specification - Lexical Structure](https://learn.microsoft.com/powershell/scripting/lang-spec/chapter-02?view=powershell-7.5#23-tokens)

## Rule 1: Prefer Tools Over Terminal Commands ##

**Before issuing a terminal command, check if a better tool exists:**

| Task | ❌ Avoid Terminal Command | ✅ Use This Tool Instead |
|------|---------------------------|--------------------------|
| Read file content | `Get-Content`, `$lines = Get-Content ...` | `read_file` |
| List directory | `Get-ChildItem`, `ls`, `dir` | `list_dir` |
| Search code | `Select-String`, `findstr` | `grep_search`, `semantic_search` |
| Check file existence | `Test-Path` | `read_file` (will fail if missing) |
| Parse JSON | `ConvertFrom-Json` piped commands | `read_file` + parse in agent logic |
| Multi-file content inspection | Looped `Get-Content` | Multiple parallel `read_file` calls |

**Why this matters:**
- Tools return structured data (no parsing needed)
- Tools avoid quoting/escaping complexity
- Tools can run in parallel
- Tools don't depend on shell state or working directory

**Exception:** Use terminal commands for:
- Build/test/deploy operations (`dotnet build`, `dotnet test`)
- Process management (`Stop-Process`, `Start-Process`)
- Git operations (`git status`, `git diff`)
- Environment queries (`dotnet msbuild -getProperty:GamePath`)

## Rule 2: PowerShell Quoting Rules ##

### Single Quotes (Literal Strings) ###

Use single quotes when:
- String contains no variables
- String contains no escape sequences
- String is a literal path or value

```powershell
# ✅ Correct
'C:\Users\Robert\file.txt'
'No variable expansion here'
'Value with $symbols stays literal'

# ⚠️ Escaping single quotes inside single-quoted strings
'Don''t use single backslash'  # Double the single quote
```

### Double Quotes (Expandable Strings) ###

Use double quotes when:
- String contains variables that should expand
- String needs escape sequences (`\`n`, `\`t`, etc.)

```powershell
# ✅ Correct
"The value is $myVariable"
"Path: $env:USERPROFILE\Documents"
"Line 1`nLine 2"  # Backtick-n for newline

# ❌ Wrong - unescaped special characters
"The price is $23"  # $2 is interpreted as variable $2 followed by literal '3'

# ✅ Correct - escaped dollar sign
"The price is `$23"
```

### Backtick (`) as Escape Character ###

The backtick character (U+0060) is PowerShell's escape character:

| Sequence | Meaning |
|----------|---------|
| `` `$ `` | Literal dollar sign |
| `` `" `` | Literal double quote |
| `` ``` `` | Literal backtick |
| `` `n `` | Newline |
| `` `t `` | Tab |
| `` `r `` | Carriage return |
| `` `0 `` | Null character |

**Common mistake:**

```powershell
# ❌ Wrong - backslash doesn't escape in PowerShell
"\"quoted\""  # This does NOT work in PowerShell

# ✅ Correct - use backtick
"`"quoted`""
```

## Rule 3: File Paths with Spaces ##

Always quote file paths that might contain spaces:

```powershell
# ✅ Correct
Get-Content -Path 'C:\Users\Robert\OneDrive\Documents\Stardew Valley\file.txt'
Get-Content -Path "C:\Users\Robert\OneDrive\Documents\Stardew Valley\$fileName"

# ❌ Wrong - unquoted path with spaces
Get-Content -Path C:\Users\Robert\OneDrive\Documents\Stardew Valley\file.txt
```

**Best practice:** Use single quotes for literal paths, double quotes when paths contain variables.

## Rule 4: Command Complexity Threshold ##

**Maximum complexity for a single terminal command:**

| Metric | Limit | Rationale |
|--------|-------|-----------|
| Line length | 120 characters | Readability and validation |
| Piped commands | 3 stages | Avoid nested complexity |
| Variables | 5 per command | Reduce state tracking |
| Loops | 1 loop maximum | Use tools or scripts instead |
| Nested quotes | 2 levels maximum | Escaping gets error-prone |

**Example violation (from user's reported error):**

```powershell
# ❌ Too complex - multiple files, loops, nested variables, string concatenation
$ErrorActionPreference='Stop'; $file1='.github/...'; $targets1=39,41,42; 
$lines1=Get-Content -Path $file1; foreach($n in $targets1){ 
if($n -le $lines1.Length){ Write-Output ("$file1#$n: " + $lines1[$n-1]) } }
```

**Replacement strategy:**

```powershell
# ✅ Better - use read_file tool for each file + line range
# Call read_file in parallel instead of complex shell script
```

## Rule 5: Variable and String Concatenation ##

**Avoid complex string concatenation in terminal commands:**

```powershell
# ❌ Fragile - nested quotes, concatenation, escaping
Write-Output ("File: $file" + "#" + "$lineNum" + ": " + $lines[$lineNum - 1])

# ✅ Better - use string interpolation with double quotes
Write-Output "${file}#${lineNum}: $($lines[$lineNum - 1])"

# ✅ Best - use tools instead of terminal commands for this task
```

## Rule 6: Multi-Line Commands ##

For commands exceeding complexity limits, use backtick line continuation:

```powershell
# ✅ Acceptable multi-line PowerShell command
dotnet build JojaAutoTasks.csproj `
  -c Debug `
  -p:EnableModDeploy=false `
  -p:EnableModZip=false
```

**Rules for backtick continuation:**
- Backtick must be the LAST character on the line (no trailing spaces)
- No comments allowed after backtick on same line
- Continued line should start with logical indentation

## Rule 7: Error Handling ##

Always set `$ErrorActionPreference` for commands that must succeed:

```powershell
# ✅ Correct - fail fast on errors
$ErrorActionPreference = 'Stop'; dotnet build JojaAutoTasks.csproj

# ⚠️ Check exit codes for external executables
dotnet test ...; if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
```

## Rule 8: Security and Injection Prevention ##

**Never concatenate unsanitized user input into commands:**

```powershell
# ❌ DANGEROUS - injection risk
Invoke-Expression -Command "Get-Process -Id '$userInput'"

# ✅ Safe - use parameters and validation
if ($userInput -match '^\d+$') {
    Get-Process -Id ([int]$userInput)
}
```

**For dynamic string content requiring single-quote safety:**

```powershell
# Use .NET escape method
$safe = [System.Management.Automation.Language.CodeGeneration]::
    EscapeSingleQuotedStringContent($userInput)
Invoke-Expression -Command "Get-Process -Name '$safe'"
```

**Reference:** [Preventing Script Injection Attacks (Microsoft Learn)](https://learn.microsoft.com/powershell/scripting/security/preventing-script-injection?view=powershell-7.5#ways-to-guard-against-injection-attacks)

## Rule 9: Special Characters ##

**Characters requiring escaping in double-quoted strings:**

| Character | Escape With | Example |
|-----------|-------------|---------|
| `$` (dollar) | `` `$ `` | `"Price: `$50"` |
| `` ` `` (backtick) | ``` `` ``` | `` "Use backtick: ```" `` |
| `"` (double quote) | `` `" `` | `"She said `"Hi`""` |
| `@` (at sign) | `` `@ `` | `"Email: `@example.com"` |

**Characters NOT needing escaping in single-quoted strings:**

```powershell
# ✅ All literal except single quote itself
'$price is $50 here'       # Literal dollar signs
'Use backtick: `'          # Literal backtick
'Path: C:\Users\Robert'    # Literal backslashes
```

## Rule 10: Validation Checklist ##

Before issuing a PowerShell terminal command, verify:

- [ ] Is there a tool alternative? (`read_file`, `list_dir`, `grep_search`, etc.)
- [ ] Does the command exceed complexity threshold? (line length, loops, nesting)
- [ ] Are all file paths with spaces properly quoted?
- [ ] Are variables in double-quoted strings escaped if needed? (e.g., `` `$ ``)
- [ ] Are single quotes inside single-quoted strings doubled? (`''`)
- [ ] Are backticks inside double-quoted strings escaped? (``` `` ```)
- [ ] Is `$ErrorActionPreference` set for critical commands?
- [ ] Are backtick line continuations correctly placed? (last char, no trailing spaces)
- [ ] Is user/external input sanitized before inclusion?
- [ ] Can the command be tested in isolation before use?

## Common Failure Patterns and Fixes ##

### Pattern 1: Complex File Content Inspection ###

```powershell
# ❌ Reported user error - too complex
$file1='...'; $targets1=39,41,42; $lines1=Get-Content -Path $file1; 
foreach($n in $targets1){ Write-Output ("$file1#$n: " + $lines1[$n-1]) }

# ✅ Fix - use read_file tool in parallel
# Agent should call: read_file(file1, 39, 39), read_file(file1, 41, 41), etc.
```

### Pattern 2: Unescaped Variables in Strings ###

```powershell
# ❌ Wrong - $n expands but string concatenation breaks
Write-Output ("File: $file#$n: " + $lines[$n-1])

# ✅ Correct - escape or use subexpression
Write-Output "${file}#${n}: $($lines[$n - 1])"
```

### Pattern 3: Path Concatenation Without Quotes ###

```powershell
# ❌ Wrong - breaks on spaces
$path = C:\Users\Robert\OneDrive\Documents\file.txt

# ✅ Correct - quoted assignment
$path = 'C:\Users\Robert\OneDrive\Documents\file.txt'
```

### Pattern 4: Nested Quote Confusion ###

```powershell
# ❌ Wrong - quote mismatch
Write-Output "Value: "inner quotes" here"

# ✅ Correct - escape inner quotes
Write-Output "Value: `"inner quotes`" here"
```

## Agent Workflow ##

When a task requires terminal execution:

1. **Check tool alternatives first** (Rule 1)
2. **If terminal is needed:**
   - Validate against complexity threshold (Rule 4)
   - Choose correct quoting style (Rules 2, 3)
   - Validate special character escaping (Rules 6, 9)
3. **Run validation checklist** (Rule 10)
4. **Execute command with appropriate timeout**
5. **Check `$LASTEXITCODE` for external executables**

## Testing PowerShell Commands ##

Before issuing complex PowerShell commands via `run_in_terminal`, consider:

- Can you test the command syntax independently?
- Does the command have side effects that are safe to retry?
- Is the output deterministic and parseable?
- Will the command work from the cwd specified in the task?

## References ##

- [PowerShell Quoting Rules (Microsoft Learn)](https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_quoting_rules)
- [PowerShell Language Specification - Tokens and Escaping](https://learn.microsoft.com/powershell/scripting/lang-spec/chapter-02?view=powershell-7.5#23-tokens)
- [Preventing PowerShell Script Injection](https://learn.microsoft.com/powershell/scripting/security/preventing-script-injection?view=powershell-7.5)
- [Quoting Differences Between Scripting Languages (Azure CLI context)](https://learn.microsoft.com/cli/azure/use-azure-cli-successfully-quoting?view=azure-cli-latest)

---

**Last Updated:** 2026-03-10  
**Applies To:** All agents using `run_in_terminal` in Windows/PowerShell environments
