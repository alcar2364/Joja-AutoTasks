# Section 15 — Configuration System #

## 15.1 Purpose ##

The configuration system manages user-adjustable settings that influence
system behavior without altering rule definitions or task identity.

Configuration allows players to control presentation preferences, enable
or disable system features, and tune optional behaviors while preserving
deterministic rule evaluation.

Configuration must not influence deterministic `TaskID` generation or
rule identity. See Section 3.3 and Section 3.11.

## 15.2 Configuration architecture ##

The system uses a two-layer configuration interface model.

1. Generic Mod Configuration Menu (GMCM)
2. Full configuration menu implemented using StardewUI

GMCM provides a minimal bootstrap interface for accessing configuration.
The StardewUI configuration menu provides the full configuration editor
for all mod settings.

GMCM must not be treated as the authoritative configuration editor. All
configuration values must ultimately be managed by the system described
in this section.

## 15.3 Configuration scope ##

Configuration settings control optional behavior across the following
domains:

```text
- HUD presentation
- Menu presentation
- Built-in generator enablement
- Debug and diagnostics features
- Optional system behavior
```

Configuration may enable or disable built-in systems, but must not modify
rule evaluation semantics or task identity fields defined by the rule engine.
See Section 7.2.

## 15.4 Configuration structure ##

Configuration values are stored in a single configuration object
persisted to disk.

Conceptual structure:

``` cs
public class ModConfig
{
```

public SystemConfig System { get; set; }
public HudConfig Hud { get; set; }
public MenuConfig Menu { get; set; }
public GeneratorConfig Generators { get; set; }
public DebugConfig Debug { get; set; }

```text

}

```

The configuration object is loaded during mod initialization and must
remain accessible to all subsystems requiring configuration values.

## 15.5 System configuration ##

System configuration controls global behavior of the mod.

Example fields include:

```text
- Enable mod
- Enable analytics
- Enable rule diagnostics
- Enable debug features
```

System configuration is the only configuration category that may be
directly exposed through GMCM.

## 15.6 GMCM integration ##

Generic Mod Configuration Menu provides a minimal access layer to the
configuration system.

GMCM must expose only a limited set of options:

  |        Setting       |                 Purpose                |
  | -------------------- |--------------------------------------- |
  | Enable Mod           | Global mod enable / disable            |
  | Open Configuration   | Opens the StardewUI configuration menu |
  | Reset HUD Position   | Resets HUD layout to defaults          |
  | Enable Debug Mode    | Enables optional diagnostics           |

The "Open Configuration" control must open the full configuration menu
implemented using StardewUI.

Conceptual behavior:

``` cs
Game1.activeClickableMenu = new TaskManagerConfigMenu();
```

GMCM must not expose the full configuration surface of the mod.

## 15.7 Full configuration menu ##

The primary configuration interface is implemented using StardewUI.

The configuration menu must provide editing capabilities for all
configuration domains defined in this section.

Responsibilities of the configuration menu:

```text
- Display configuration categories
- Allow editing of configuration values
- Validate configuration changes
- Persist configuration updates
```

The menu must write changes to the shared `ModConfig` instance used by
the runtime system.

## 15.8 HUD configuration ##

HUD configuration controls behavior and appearance of the in-game HUD
component described in Section 21.

Example configuration fields include:

```text
- HUD visibility toggle
- HUD screen position
- Maximum tasks displayed
- Show completed tasks
- Scroll behavior parameters
```

HUD configuration values affect presentation behavior only and must not
change task generation or rule evaluation.

## 15.9 Menu configuration ##

Menu configuration controls behavior of the task management menu
implemented using StardewUI.

Example configuration fields include:

```text
- Default sorting mode
- Default task filtering options
- Menu layout density
- Expanded task details by default
```

Menu configuration must affect presentation behavior only.

## 15.10 Generator configuration ##

Generator configuration allows players to enable or disable built-in
task generators described in Section 13.

Example structure:

``` cs
public class GeneratorConfig
{
```

public bool EnableCropTasks { get; set; }
public bool EnableAnimalTasks { get; set; }
public bool EnableMachineTasks { get; set; }
public bool EnableCalendarTasks { get; set; }

```text

}

```

Disabling a generator prevents the generator from producing new tasks
during evaluation cycles.

Existing tasks produced by disabled generators may remain until the next
reconciliation cycle defined by the generation engine. See Section 5.

## 15.11 Debug configuration ##

Debug configuration enables optional diagnostics used during development
or troubleshooting.

Example fields include:

```text
- Enable debug overlay
- Enable rule evaluation logging
- Enable generator diagnostics
- Enable command tracing
```

Debug features must be disabled by default during normal gameplay.

## 15.12 Configuration loading ##

Configuration must be loaded during mod initialization before any
subsystems begin evaluation.

Load order:

1. Load configuration file from disk.
2. Validate configuration structure.
3. Apply default values for missing fields.
4. Provide configuration instance to dependent subsystems.

Subsystems must treat configuration as read-only during runtime.

## 15.13 Configuration persistence ##

Configuration changes made by the player must be persisted immediately
after modification.

The persistence mechanism must ensure:

```text
- Configuration file integrity
- Backward compatibility across versions
- Preservation of unknown fields for forward compatibility
```

## 15.14 Configuration versioning ##

Configuration files must include a schema version to support migration
across mod updates.

Conceptual structure:

``` json
{
  "ConfigVersion": 1,
  "System": { },
  "Hud": { },
  "Menu": { },
  "Generators": { },
  "Debug": { }
}
```

**V1 note:** V1 ships with only `ConfigVersion: 1`. No migration steps are
defined for V1 because there is only one config version. The cross-reference to
Section 18 is for the migration pipeline pattern (how migrations are structured
and executed), not for any specific config migration steps. Future config
migrations will be added to Section 18 when `ConfigVersion` increments in a
later release.

Configuration migrations must preserve existing user preferences when
possible. Migration rules are defined in Section 18.

## 15.15 Configuration constraints ##

Configuration must follow these constraints:

1. Configuration must not affect deterministic task identity.
2. Configuration must not alter rule evaluation semantics.
3. Configuration should affect presentation and optional system

```text
behavior only.
```

1. Configuration must remain backward compatible when new settings are

```text
introduced.
```

These constraints ensure consistent task behavior across game sessions.
