---
name: jat-identifier-determinism-patterns
Description: Determinism fundamentals, TaskID formula, RuleID stability, SubjectID mapping, DayKey consistency, identifier stability testing, anti-patterns. Use when: designing identifiers for tasks/rules/subjects, ensuring stable IDs across reloads, or testing determinism.
argument-hint: "Describe the identifier requirement: what needs to be uniquely and stably identified, and across what scenarios (reload, multi-pass generation)?"
target: vscode
---

# Identifier Determinism Patterns Skill

Task execution depends critically on stable identifiers: TaskID must hash identically across game reloads to match persisted completion states, RuleID must remain constant despite rule edits, SubjectID must map game entities consistently. Understanding determinism requirements and common identifier pitfalls is essential for preventing task duplication, lost progress, and stale completion references.

## 1. Determinism Fundamentals (Refresher) ##

**Core Rule:**
**Identical inputs must always produce identical identifiers, across reloads, game instances, and code versions.**

**Why It Matters:**
- TaskID hashing must match saved completion data (e.g., "completed task:abc123 on Spring-5")
- Across reload, regenerating same rule + subject + day must produce same TaskID
- Without determinism: tasks regenerate as "new" → duplicates, lost completion tracking

**Determinism Checklist (Quick):**
- [ ] Identifier computed from stable, serializable data only
- [ ] No DateTime.Now, random numbers, object references, hash codes
- [ ] No iteration over unordered collections (dictionaries, sets)
- [ ] No async operations or external service calls
- [ ] Test: run identifier generation 100 times → same result each time
- [ ] Test: after JSON serialization round-trip → same identifier

## 2. TaskID Formula (Deterministic Hashing) ##

**Standard TaskID Formula:**
```
TaskID = SHA256_Hash(UTF8_Encode(RuleID + "|" + SubjectID + "|" + DayKey))
```

Or simpler hash for demonstration:
```
TaskID = Hash(RuleID.ToString() + "|" + SubjectID.ToString() + "|" + DayKey.ToString())
```

**Implementation Pattern:**
```csharp
public sealed record TaskID
{
    public string Value { get; }

    private TaskID(string value)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Creates a stable TaskID from rule, subject, and day.
    /// Same inputs → identical TaskID (deterministic).
    /// </summary>
    public static TaskID Create(string ruleId, SubjectID subjectId, DayKey day)
    {
        // Ensure stable string representation
        var input = $"{ruleId}|{subjectId}|{day.ToIsoString()}";
        
        // Deterministic hash (not GUID, not random)
        using (var sha256 = System.Security.Cryptography.SHA256.Create())
        {
            var hash = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
            // Take first 16 bytes and encode as hex string (32 chars)
            var hashString = System.Convert.ToHexString(hash[..16]).ToLower();
            
            return new TaskID($"task:{hashString}");
        }
    }

    public override string ToString() => Value;

    public static TaskID Parse(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !value.StartsWith("task:"))
            throw new ArgumentException($"Invalid TaskID format: {value}");
        
        return new TaskID(value);
    }
}
```

**Why This Formula Works:**
- **RuleID** (stable): Identifies which rule/generator created the task
- **SubjectID** (stable): Identifies the subject (NPC name, location, etc.)
- **DayKey** (stable): Identifies the in-game date (not current date/time)
- **Hash**: Creates fixed-length ID without revealing formula (prevents collisions from pattern analysis)

**Collision Avoidance:**
- Different rules for same subject/day → different RuleID → different TaskID
- Same rule, different subjects → different SubjectID → different TaskID
- Same rule/subject, different days → different DayKey → different TaskID
- 256-bit hash → negligible collision probability

## 3. RuleID Stability for Rule Editing ##

**RuleID Must Not Change When Rule Is Edited:**
```csharp
// WRONG: Regenerate RuleID from rule content
var ruleId = Hash(rule.Name + rule.Description); // ❌ Changes if name edited!

// RIGHT: RuleID assigned once at creation, persisted, never regenerated
public sealed record Rule(
    RuleID Id,                    // Set at creation, never changes
    string Name,                  // Can be edited
    string Description = null,    // Can be edited
    RuleEvaluationOrder = 0      // Can be changed
);
```

**RuleID Creation and Persistence:**
```csharp
public sealed class RuleBuilder
{
    private Rule _rule = null!;

    public RuleBuilder WithId(RuleID id)
    {
        _rule = _rule with { Id = id };
        return this;
    }

    public RuleBuilder WithName(string name)
    {
        _rule = _rule with { Name = name };  // Name changes; ID stable
        return this;
    }

    public Rule Build()
    {
        if (_rule.Id == null)
            throw new InvalidOperationException("RuleID must be explicitly set");
        
        return _rule;
    }
}

// Usage:
var rule = new RuleBuilder()
    .WithId(RuleID.Parse("rule:daily-chores"))  // Assigned once
    .WithName("Daily Chores")                    // Can edit later
    .WithName("Morning Chores")                  // Edit again, ID unchanged
    .Build();
```

**Testing RuleID Stability:**
```csharp
[Test]
public void Rule_NameChanged_RuleIdRemainsSame()
{
    // Arrange
    var rule1 = new Rule(
        Id: RuleID.Parse("rule:1"),
        Name: "Morning Tasks"
    );

    // Act: Edit name
    var rule2 = rule1 with { Name: "Evening Tasks" };

    // Assert
    Assert.That(rule2.Id, Is.EqualTo(rule1.Id));
}
```

## 4. SubjectID Mapping (Game Entities → Stable IDs) ##

**SubjectID Represents What?**
- **NPC**: "npc:Abigail", "npc:Sebastian"
- **Location**: "location:Farm", "location:Forest"
- **Item**: "item:Cauliflower", "item:Gold Ore"
- **Group**: "group:AllVillagers", "group:RoommateBachelor"

**SubjectID Creation (Deterministic Mapping):**
```csharp
public sealed record SubjectID
{
    public string Value { get; }

    private SubjectID(string value) => Value = value;

    public static SubjectID ForNPC(string npcName)
    {
        if (string.IsNullOrWhiteSpace(npcName))
            throw new ArgumentException("NPC name required");
        
        // Normalize: trim, lowercase for comparison, but preserve original for ID
        var normalized = npcName.Trim();
        return new SubjectID($"npc:{normalized}");
    }

    public static SubjectID ForLocation(string locationName)
    {
        if (string.IsNullOrWhiteSpace(locationName))
            throw new ArgumentException("Location name required");
        
        var normalized = locationName.Trim();
        return new SubjectID($"location:{normalized}");
    }

    public static SubjectID Parse(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("SubjectID cannot be null/empty");
        
        var parts = value.Split(':');
        if (parts.Length != 2 || string.IsNullOrWhiteSpace(parts[1]))
            throw new ArgumentException($"Invalid SubjectID format: {value}");
        
        return new SubjectID(value);
    }

    public override string ToString() => Value;
}
```

**SubjectID Stability Checklist:**
- [ ] Uses game entity **name** or **key**, not object reference/hashcode
- [ ] Normalized consistently (case, whitespace, special chars)
- [ ] Same entity → always same SubjectID across reloads
- [ ] No dynamic list indices (e.g., "npc:0") that change with game updates

**Anti-Pattern: Object-Based Subject IDs:**
```csharp
// ❌ WRONG: Uses object hash (changes between loads)
var subjectId = new SubjectID(npcObject.GetHashCode().ToString());

// ❌ WRONG: Uses list index (changes if game updates NPC list)
var index = gameState.Characters.IndexOf(npc);
var subjectId = new SubjectID($"npc:{index}");

// ✓ RIGHT: Uses stable name property
var subjectId = new SubjectID($"npc:{npc.Name}");
```

## 5. DayKey Consistency (Date Representation and Ordering) ##

**DayKey Format (Deterministic):**
```csharp
public sealed record DayKey : IComparable<DayKey>
{
    public string Season { get; }    // "Spring", "Summer", "Fall", "Winter"
    public int Day { get; }          // 1–28

    private DayKey(string season, int day)
    {
        if (!ValidSeasons.Contains(season))
            throw new ArgumentException($"Invalid season: {season}");
        if (day < 1 || day > 28)
            throw new ArgumentException($"Day must be 1–28, got {day}");

        Season = season;
        Day = day;
    }

    /// <summary>
    /// Canonical ISO format: "Spring-1", "Summer-15", etc.
    /// Must be consistent across saves, game reloads, and different systems.
    /// </summary>
    public string ToIsoString() => $"{Season}-{Day:D2}";

    public static DayKey Parse(string isoString)
    {
        var parts = isoString.Split('-');
        if (parts.Length != 2 
            || !int.TryParse(parts[1], out var day)
            || !ValidSeasons.Contains(parts[0]))
        {
            throw new ArgumentException($"Invalid DayKey format: {isoString}");
        }

        return new DayKey(parts[0], day);
    }

    public int CompareTo(DayKey? other)
    {
        if (other == null) return 1;
        
        // Seasons ordered: Spring < Summer < Fall < Winter
        var seasonOrder = new Dictionary<string, int>
        {
            { "Spring", 0 }, { "Summer", 1 }, { "Fall", 2 }, { "Winter", 3 }
        };

        var seasonCmp = seasonOrder[Season].CompareTo(seasonOrder[other.Season]);
        if (seasonCmp != 0) return seasonCmp;

        return Day.CompareTo(other.Day);
    }

    public override string ToString() => ToIsoString();
    
    private static readonly HashSet<string> ValidSeasons = 
        new() { "Spring", "Summer", "Fall", "Winter" };
}
```

**DayKey Consistency Checklist:**
- [ ] Always use ISO format for serialization ("Spring-1", not "1/1" or "day 1")
- [ ] Never include year/absolute date (Stardew Valley repeats each year)
- [ ] Ordering deterministic (Spring < Summer < Fall < Winter, day 1-28)
- [ ] Test: Parse and serialize deterministically round-trips unchanged

## 6. Testing Identifier Stability ##

**Test: Simulate Reload (JSON Round-Trip):**
```csharp
[TestFixture]
public class IdentifierStabilityTests
{
    [Test]
    public void TaskID_AfterJsonRoundTrip_RemainsUnchanged()
    {
        // Arrange
        var original = TaskID.Create("rule:daily", new SubjectID("npc:Abigail"), 
                                     DayKey.Parse("Spring-5"));

        // Act: Serialize and deserialize
        var json = JsonConvert.SerializeObject(original.Value);
        var reloaded = JsonConvert.DeserializeObject<string>(json);
        var roundTrip = TaskID.Parse(reloaded!);

        // Assert
        Assert.That(roundTrip, Is.EqualTo(original));
    }

    [Test]
    public void DayKey_AfterJsonRoundTrip_RemainsUnchanged()
    {
        // Arrange
        var original = DayKey.Parse("Summer-15");

        // Act: Serialize
        var json = JsonConvert.SerializeObject(original.ToIsoString());
        var reloaded = JsonConvert.DeserializeObject<string>(json);

        // Assert
        Assert.That(reloaded, Is.EqualTo("Summer-15"));
    }

    [Test]
    public void SubjectID_WithSameName_ProducesSameId()
    {
        // Arrange
        var id1 = SubjectID.ForNPC("Abigail");
        var id2 = SubjectID.ForNPC("Abigail");

        // Act & Assert
        Assert.That(id2, Is.EqualTo(id1));
    }

    [Test]
    public void TaskID_MultiPass_ProducesSameId()
    {
        // Simulate multiple evaluation passes producing same task
        var rule = RuleID.Parse("rule:daily");
        var subject = SubjectID.ForNPC("Abigail");
        var day = DayKey.Parse("Spring-5");

        // Run 100 times
        var ids = Enumerable.Range(0, 100)
            .Select(_ => TaskID.Create(rule.ToString(), subject, day))
            .ToList();

        // All should be identical
        Assert.That(ids.Distinct(), Has.Count.EqualTo(1));
    }
}
```

**Test: Multi-Pass Evaluation (Determinism in Integration):**
```csharp
[Test]
public void TaskGeneration_TwoEvaluationPasses_ProducesIdenticalTasks()
{
    // Arrange
    var generator = new DailyChoresGenerator();
    var context = CreateTestContext(DayKey.Parse("Spring-1"));
    var subject = SubjectID.ForNPC("Abigail");

    // Act: First pass
    var tasks1 = generator.GenerateCandidateTasks(subject, context);

    // Act: Second pass (simulating reload)
    var tasks2 = generator.GenerateCandidateTasks(subject, context);

    // Assert: Identical task IDs in same order
    CollectionAssert.AreEqual(
        tasks1.OrderBy(t => t.Title).Select(t => t.TaskId),
        tasks2.OrderBy(t => t.Title).Select(t => t.TaskId)
    );
}
```

## 7. Anti-Patterns to Avoid ##

| Anti-Pattern | Why It Fails | Fix |
|--------------|-------------|-----|
| GUID for TaskID | Different each run, task "persists" as new each load | Use deterministic hash formula |
| DateTime.UtcNow as part of ID | Time varies, same rule produces different IDs | Use DayKey only, no time component |
| Object hash (GetHashCode) | Hash changes between .NET versions and runs | Use stable name/key properties |
| Unordered iteration (foreach dict) | Order varies, hash includes different data | Sort collections before hashing |
| List index as SubjectID ("npc:0") | Index changes when game updates NPC list | Use name property ("npc:Abigail") |
| RuleID from rule content | Rule edits change RuleID, breaking persisted links | Assign RuleID once, persist it |
| DayKey with DateTime.Now | Different every second, not matching saved state | Use constant date within season |
| Case-sensitive SubjectID | "NPC:Abigail" ≠ "npc:Abigail", inconsistent | Normalize case early, use consistently |

## 8. Identifier Stability Checklist ##

**For Each Identifier (TaskID, RuleID, SubjectID, DayKey):**

- [ ] **Is the formula documented?** (What inputs combine to make this ID?)
- [ ] **Is it deterministic?** (Same inputs → same ID always)
- [ ] **Will it survive a JSON round-trip?** (Serialize/deserialize unchanged)
- [ ] **Is it stable across game reloads?** (Reload same save → same IDs)
- [ ] **Are there tests verifying multi-pass stability?** (Run generation 100 times)
- [ ] **Are game entity references converted to stable names?** (Not hashes or indices)
- [ ] **Is version/migration considered?** (ID formula fixed, not version-dependent)

## Links ##
- [Backend Architecture Contract](../Contracts/BACKEND-ARCHITECTURE-CONTRACT.instructions.md)
- [Unit Testing Contract](../Contracts/UNIT-TESTING-CONTRACT.instructions.md)
- [Review and Verification Contract](../Contracts/REVIEW-AND-VERIFICATION-CONTRACT.instructions.md)
