---
name: persistence-safety-validator
description: "Validates persistence changes for minimal-persistence principle, versioning, migration safety, and reconstruction capability."
trigger: after-edit
applyTo: "{Domain/Persistence/**/*.cs,**/*Persistence*.cs,**/*Migration*.cs,Configuration/**/*.cs}"
---

# Persistence Safety Validator Hook #

**Trigger:** After editing persistence, migration, or configuration code
**Purpose:** Ensure persistence adheres to minimal persistence principle and migration safety

## Post-Edit Validation ##

After any edit to persistence-related code, validate:

### 1. Minimal Persistence Principle ###

**Rule:** Persist only what is **essential** to reconstruct canonical state. Derived values MUST be recomputed, not persisted.

**ALLOWED to persist:**
    * ✅ User-created rule definitions (Task Builder rules)
    * ✅ Manual task state (user-created tasks)
    * ✅ Task completion history (needed for "don't repeat" logic)
    * ✅ Configuration values set by user
    * ✅ Persistent flags/toggles that affect future behavior

**BLOCKED from persisting:**
    * ❌ Derived/computed values (can be recalculated from generator logic)
    * ❌ Transient caches (e.g., "tasks evaluated this frame")
    * ❌ UI state (scroll position, expanded/collapsed, sort order)
    * ❌ Snapshots or view models
    * ❌ Debug/diagnostic state
    * ❌ Redundant copies of data already persisted elsewhere

**Validation check:**
    For each new persisted field:
    Question: Can this value be recomputed from other persisted data + game state?
    If YES → ❌ DO NOT PERSIST (violates minimal persistence)
    If NO → ✅ OK to persist

### 2. Versioning Requirements ###

**Every persisted data structure MUST include version identifier.**

**Required patterns:**

    // ✅ ALLOWED: Versioned save data
    public class TaskHistorySaveData
    {
        public int Version { get; set; } = 1; // Explicit version
        public List<CompletedTask> CompletedTasks { get; set; }
    }

    // ✅ ALLOWED: Versioned config
    public class ModConfig
    {
        public string ConfigVersion { get; set; } = "1.0";
        // ... config fields
    }

    // ❌ BLOCKED: No version identifier
    public class TaskHistorySaveData
    {
        public List<CompletedTask> CompletedTasks { get; set; }
        // Missing: version field
    }

**If version field missing:**
    🚨 MISSING PERSISTENCE VERSION IDENTIFIER

    File: [path]
    Class: [class name]

    Issue: Persisted data structure lacks version identifier.
    Impact: Cannot safely migrate data when schema changes.

    Required fix: Add version field (int or string) to enable migrations.

**BLOCK persistence edit until version field added.**

### 3. Migration Safety ###

When changing persisted schema (adding/removing/renaming fields):

**Required:**
    * [ ] Version number incremented
    * [ ] Migration code written to handle old → new schema
    * [ ] Migration tested with sample old-version data
    * [ ] Fallback behavior defined for unrecognized versions
    * [ ] Migration documented in code comments

**Migration pattern:**

    // ✅ ALLOWED: Safe migration
    public static TaskHistorySaveData Migrate(JObject data)
    {
        int version = data["Version"]?.Value<int>() ?? 0;
        
        switch (version)
        {
            case 0: return MigrateV0ToV1(data);
            case 1: return data.ToObject<TaskHistorySaveData>();
            default:
                LogWarning($"Unknown version {version}, using defaults");
                return new TaskHistorySaveData(); // Safe fallback
        }
    }

**If migration code missing for schema change:**
    ⚠️ SCHEMA CHANGE WITHOUT MIGRATION

    File: [path]
    Change: [field added/removed/renamed]

    Issue: Schema changed but no migration code updated.
    Impact: Users with old save data will experience [data loss / errors / etc.].

    Required fix: Implement migration in [migration method name].

### 4. Reconstruction Capability ###

**Test:** Can the system fully reconstruct runtime state from persisted data?

**After persistence changes, verify:**
    * [ ] Load path exists and is tested
    * [ ] Loaded data can reconstruct State Store canonical state
    * [ ] Derived values are recomputed correctly on load
    * [ ] Evaluation engine produces expected results after reload

**If reconstruction is incomplete:**
    ⚠️ INCOMPLETE STATE RECONSTRUCTION

    Missing data needed to reconstruct:
    - [Specific canonical state not recoverable]

    Cause: [Explanation]
    Fix: Either persist [X] OR ensure [X] can be recomputed from [Y]

### 5. Safe Failure Behavior ###

Persistence code MUST handle failure gracefully:

**Required:**
    * Try-catch around file I/O operations
    * Fallback to defaults if load fails
    * Log errors with enough context to diagnose
    * Don't crash the game on corrupted save data

**Pattern detection:**

    // ❌ BLOCKED: Unsafe persistence (throws on failure)
    public void Save()
    {
        File.WriteAllText(path, JsonConvert.SerializeObject(data)); // No error handling
    }

    // ✅ ALLOWED: Safe persistence
    public void Save()
    {
        try
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(data));
        }
        catch (Exception ex)
        {
            ModLogger.Error($"Failed to save: {ex}");
            // Graceful degradation
        }
    }

### 6. Configuration vs Runtime State ###

**Configuration (ModConfig):**
    * User settings that persist across sessions
    * Should have UI for modification
    * Versioned and migrated

**Runtime State (save data):**
    * Canonical state needed to reconstruct task system
    * Not directly user-editable
    * Versioned and migrated

Don't conflate these. Keep them in separate files/structures.

### 7. Post-Validation Output ###

**If validation passes:**
    * Silent (no output)

**If violations found:**
    🔍 PERSISTENCE SAFETY VALIDATION RESULTS

    File: [path]

    Issues found:
    1. [Severity] [Issue description]
       Impact: [Consequences]
       Fix: [Required action]

    2. [Severity] [Issue description]
       Impact: [Consequences]
       Fix: [Required action]

    Overall: [PASS / NEEDS FIXES]

**Severity levels:**
    * 🚨 **CRITICAL**: Blocks merge (e.g., no version, unsafe failure)
    * ⚠️ **MAJOR**: Should fix before merge (e.g., missing migration)
    * ℹ️ **MINOR**: Improvement suggestion (e.g., redundant persist)

## Integration with Other Hooks ##

Works with:
    * **contract-auto-loader** (loads BACKEND-ARCHITECTURE-CONTRACT Section 1.3 on persistence)
    * **design-guide-context-augmenter** (loads Design Guide Section 09: Persistence Model, Section 18: Versioning)
    * **unit-test-coverage-enforcer** (flags missing migration tests)
