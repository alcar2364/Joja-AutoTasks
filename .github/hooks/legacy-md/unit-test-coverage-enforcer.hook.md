---
name: unit-test-coverage-enforcer
description: "Flags missing critical test coverage: determinism, architecture boundaries, identifier stability, persistence reconstruction. Suggests test creation."
trigger: after-edit
applyTo: "{Domain,Events,Infrastructure,UI}/**/*.cs"
---

# Unit Test Coverage Enforcer Hook #

**Trigger:** After editing production code in Domain, Events, Infrastructure, or UI
**Purpose:** Identify critical test coverage gaps and suggest test creation

## Post-Edit Test Coverage Analysis ##

After production code edits, analyze what **critical test coverage** is needed.

### 1. Test Coverage Requirements by Subsystem ###

#### Domain/Identifiers/ ####
**Critical tests needed:**
    * [ ] Identifier stability across multiple evaluations (same inputs → same ID)
    * [ ] Identifier stability across simulated reload
    * [ ] No dependency on random/time-based values
    * [ ] DayKey correctness for date transitions

**Flag if missing:**
    ⚠️ IDENTIFIER TEST GAP

    Modified: Domain/Identifiers/[file]
    Missing tests:
    - Stability verification (multiple calls produce same ID)
    - Reload stability (ID unchanged after save/load simulation)

    Recommended: Add to Tests/Domain/Identifiers/[TestFile]

#### Domain/Tasks/ & Task Generation ####
**Critical tests needed:**
    * [ ] Task generation determinism (same conditions → same tasks)
    * [ ] Generated tasks have valid deterministic IDs
    * [ ] Rule evaluation produces expected results
    * [ ] Edge cases: empty inputs, invalid subjects, etc.

#### State Store / Commands / Reducers ####
**Critical tests needed:**
    * [ ] Command dispatch produces expected state changes
    * [ ] Reducers are pure (same command + state → same new state)
    * [ ] Snapshot reflects reducer changes
    * [ ] Boundary: only reducers can mutate canonical state

**Flag if missing:**
    ⚠️ STATE STORE BOUNDARY TEST GAP

    Modified: [State Store file]
    Missing tests:
    - Verify command → reducer → snapshot flow
    - Verify non-reducer code cannot mutate state

    Recommended: Add to Tests/Domain/StateStore/[TestFile]

#### Persistence / Migrations ####
**Critical tests needed:**
    * [ ] Save produces expected output
    * [ ] Load reconstructs canonical state correctly
    * [ ] Migration from old version → new version succeeds
    * [ ] Migration handles unknown/corrupted data gracefully
    * [ ] Derived values are recomputed, not loaded

**Flag if missing:**
    🚨 PERSISTENCE TEST GAP (CRITICAL)

    Modified: [Persistence file]
    Missing tests:
    - Save/load round-trip (state preserved)
    - Migration test (old schema → new schema)
    - Fallback behavior on load failure

    Recommended: Add to Tests/Configuration/ or Tests/Domain/Persistence/

#### UI / View Models ####
**Critical tests needed:**
    * [ ] View model consumes snapshot read-only (doesn't mutate)
    * [ ] View model dispatches commands for mutations
    * [ ] View model refreshes on snapshot-changed events
    * [ ] UI-local state separated from canonical state

### 2. Coverage Gap Severity ###

**🚨 CRITICAL gaps (block merge):**
    * Persistence save/load round-trip
    * Migration tests for schema changes
    * State Store boundary violations (UI mutating canonical state)
    * Identifier determinism for newly added ID types

**⚠️ MAJOR gaps (should fix before merge):**
    * Missing edge case tests (null inputs, empty collections)
    * Missing rule evaluation correctness tests
    * Missing command/reducer tests for new commands

**ℹ️ MINOR gaps (nice to have):**
    * Additional happy-path coverage
    * Performance tests
    * UI interaction tests

### 3. Test Suggestion Format ###

When coverage gap detected:

    📋 TEST COVERAGE RECOMMENDATION

    Modified file: [path]
    Change type: [new feature / bug fix / refactor]

    Suggested tests:
    1. [Test name/description]
       Purpose: [What this test protects against]
       Location: Tests/[suggested path]
       
    2. [Test name/description]
       Purpose: [What this test protects against]
       Location: Tests/[suggested path]

    Priority: [CRITICAL / MAJOR / MINOR]

    Would you like me to create these tests? [Y/n]
    Or handoff to UnitTestAgent? [Y/n]

### 4. Existing Test Review ###

If tests already exist for the modified code:

**Check:**
    * [ ] Do existing tests cover the new changes?
    * [ ] Are existing tests still valid after the change?
    * [ ] Did the change break any assumptions in tests?

**If existing tests need updates:**
    ⚠️ EXISTING TEST UPDATE NEEDED

    Modified: [production file]
    Affected tests: [test file paths]

    Issue: Tests may no longer cover new behavior or may have broken assumptions.

    Recommendation: Review and update affected tests before merge.

### 5. Determinism Test Pattern Templates ###

**For identifier stability:**
    [Fact]
    public void TaskID_SameInputs_ProducesSameID()
    {
        var ruleId = new RuleID("test-rule");
        var subjectId = new SubjectID("NPC:Abigail");
        var dayKey = DayKey.FromSeason(1, "spring", 1);

        var id1 = TaskID.FromRule(ruleId, subjectId, dayKey);
        var id2 = TaskID.FromRule(ruleId, subjectId, dayKey);

        Assert.Equal(id1, id2); // Must be deterministic
    }

**For State Store boundaries:**
    [Fact]
    public void StateStore_OnlyReducersCanMutateState()
    {
        var store = new StateStore();
        var snapshot = store.GetSnapshot();
        
        // Attempt to mutate snapshot (should have no effect on canonical state)
        snapshot.Tasks.Clear(); // Or verify this throws if immutable
        
        var newSnapshot = store.GetSnapshot();
        Assert.NotEmpty(newSnapshot.Tasks); // Canonical state unchanged
    }

**For persistence reconstruction:**
    [Fact]
    public void Persistence_RoundTrip_PreservesCanonicalState()
    {
        var original = CreateTestState();
        
        // Save
        var saved = persistence.Save(original);
        
        // Load
        var loaded = persistence.Load(saved);
        
        // Verify reconstruction
        Assert.Equal(original.Tasks.Count, loaded.Tasks.Count);
        Assert.Equal(original.Tasks[0].ID, loaded.Tasks[0].ID);
        // ... verify all canonical state preserved
    }

### 6. Integration with UnitTestAgent ###

When coverage gaps are significant:

    💡 HANDOFF SUGGESTION

    Detected [N] critical test coverage gaps.

    Recommend handoff to UnitTestAgent to:
    - Create missing tests
    - Review existing test adequacy
    - Ensure determinism/boundary/persistence coverage

    Proceed with handoff? [Y/n]

## Output Behavior ##

**Silent when:**
    * No coverage gaps detected
    * All critical areas already tested

**Output when:**
    * Critical gaps detected (ALWAYS flag)
    * Major gaps detected (flag unless user requested no tests)
    * Test creation suggested (offer to create or handoff)

## Integration with Other Hooks ##

Works with:
    * **identifier-validation** (triggers identifier test suggestions)
    * **state-mutation-guard** (triggers boundary test suggestions)
    * **persistence-safety-validator** (triggers migration test suggestions)
    * **contract-auto-loader** (loads UNIT-TESTING-CONTRACT for test quality rules)
