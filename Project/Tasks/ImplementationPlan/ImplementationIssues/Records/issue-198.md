---
issue_number: 198
legacy_id: ""
type: "Review follow-up"
title: "[ci] Build: CommunityToolkit.Mvvm missing BundleExtraAssemblies — mod will fail to load at runtime"
summary: "Commits [`7eda58e`](https://github.com/alcar2364/Joja-AutoTasks/commit/7eda58ef43b745d4e5ca71d70ff59c7b00c0a0f9) and [`23641cb`](https://github.com/alcar2364/Joja-AutoTasks/commit/23641cb8f82e567320cdaecc0e5774f1b8a26ae7) (2026-03-15) added `CommunityToolkit.Mvvm 8.4.0` to `JojaAutoTasks.csproj` and wired it into `UiViewModelBase` (derives from `ObservableObject`) and `HudViewModel` (`[ObservableProperty]` source generation). However, `JojaAutoTasks.csproj` has no `BundleExtraAssemblies` configuration.

**CI does not catch this** because the CI workflow runs with `-p:EnableModDeploy=false`, which skips the mod-folder copy step. The `dotnet build` compiles successfully. The failure surfaces only when the mod is actually deployed."
created_phase: ""
source: "GitHub issue"
scheduled_target: ""
status: "Resolved"
priority: "Medium"
github_url: "https://github.com/alcar2364/Joja-AutoTasks/issues/198"
resolution_pr: ""
created_by: "automation"
created_at: "2026-03-15T09:42:57Z"
updated_at: "2026-03-16T00:45:20+00:00"
sync_state: "github-synced"
notes: ""
---

# Implementation Issue Record

## Rationale And Context

## Impact

When the mod is built for deployment (`EnableModDeploy=true`) or zipped for release, SMAPI's `ModBuildConfig` will not copy `CommunityToolkit.Mvvm.dll` to the mod folder. At runtime, SMAPI will throw a `FileNotFoundException` on mod load:

```
System.IO.FileNotFoundException: Could not load file or assembly 'CommunityToolkit.Mvvm, Version=...'
```

`CommunityToolkit.Mvvm` has **runtime components** that are actively used:
- `UiViewModelBase : ObservableObject` — requires the runtime DLL at load time
- `HudViewModel` uses `[ObservableProperty]` — generated code calls into `ObservableObject` internals

## Implementation Notes

## Acceptance / Closing Criteria

## History / Resolution Notes
