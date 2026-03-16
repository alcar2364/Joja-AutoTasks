# Implementation Issues Archive

**Status**: Archive of resolved implementation issues  
**Last Updated**: 2026-03-16

| Issue | Legacy ID | Type | Summary | Created Phase | Priority | Resolution PR | Archived Date | Notes |
| ----- | --------- | ---- | ------- | ------------- | -------- | ------------- | ------------- | ----- |

<!-- markdownlint-disable MD013 -->

| 205 |  | Review follow-up | [ci] NuGet: Unauthorized cloud AI packages added to mod project (net6.0 incompatibility risk) |  | Medium |  | 2026-03-16 |  |
| 198 |  | Review follow-up | Commits [`7eda58e`](https://github.com/alcar2364/Joja-AutoTasks/commit/7eda58ef43b745d4e5ca71d70ff59c7b00c0a0f9) and [`23641cb`](https://github.com/alcar2364/Joja-AutoTasks/commit/23641cb8f82e567320cdaecc0e5774f1b8a26ae7) (2026-03-15) added `CommunityToolkit.Mvvm 8.4.0` to `JojaAutoTasks.csproj` and wired it into `UiViewModelBase` (derives from `ObservableObject`) and `HudViewModel` (`[ObservableProperty]` source generation). However, `JojaAutoTasks.csproj` has no `BundleExtraAssemblies` configuration. |  | Medium |  | 2026-03-16 |  |
| 195 | DEF-032 | Deferment | Address security issue #86 by replacing broad exception swallowing in `ConfigLoader` with deterministic, explicit error handling and logging behavior. | Phase 3 | Low |  | 2026-03-15 | Merged into #159. Retained only as historical traceability reference and not an active tracker. |
| 189 |  | Review follow-up | [ci] CI Infrastructure: `scripts/implementation_issues.py` missing — all three Implementation Issues sync workflows fail on every tri |  | Medium |  | 2026-03-15 |  |
| 107 | DEF-008 | Deferment | Wire ViewModels to subscribe to SnapshotChanged so snapshot updates propagate into UI state. | Phase 3 | Low |  | 2026-03-15 |  |
| - | DEF-030 | Deferment | Resolve StateStore namespace/type collision by renaming namespace root and folder structure. | Phase 3 | - | - | 2026-03-11 | Renamed namespace/folder usage to `JojaAutoTasks.State` and `State/`, removing `StateStore` class/namespace ambiguity. |
| - | DEF-003 | Deferment | Implement completion-marking runtime behavior once command/state transition path is in place. | Phase 2 | - | - | 2026-03-11 | Completion/uncompletion command flow and deterministic handler behavior were implemented and verified in Phase 3. |
| - | DEF-002 | Deferment | Implement manual task ID issuance and counter ownership in State Store command flow. | Phase 2 | - | - | 2026-03-11 | Manual task counter ownership and deterministic `ManualTask_{N}` issuance were implemented in the State Store flow. |

<!-- markdownlint-enable MD013 -->

## Summary

**Total Archived Implementation Issues**: 8

