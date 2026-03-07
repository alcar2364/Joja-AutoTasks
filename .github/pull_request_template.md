# Pull Request #

## Summary ##

Describe what changed and why.

## Scope ##

	    * [ ] Scope is intentional and limited to the requested behavior/change.
	    * [ ] Any out-of-scope follow-ups are explicitly listed below.

## Testing ##

	    * [ ] I reviewed `JojaAutoTasks.Tests/README.md` and followed the test naming/coverage
	      conventions.
	    * [ ] I added or updated tests for the changed behavior.
	    * [ ] If `OnUpdateTicked` flow changed, I verified tests cover deterministic tick
	      throttling and the guard-block no-op path (no runtime access, no second dispatch
	      in-window).
	    * [ ] If lifecycle signal routing changed, I verified lifecycle tests cover signal forwarding and signal-only `OnSaving` expectations.
	    * [ ] If dispatcher routing changed, I verified dispatcher tests still assert determinism, statelessness, and no-op dispatch contracts.
	    * [ ] If `ModConfig.CurrentConfigVersion` or config migration logic changed, I updated
	      `ConfigLoaderMigrationSafetyTests` coverage.
	    * [ ] I ran `dotnet test "JojaAutoTasks.Tests\JojaAutoTasks.Tests.csproj"` locally.

## Risks / Notes ##

List risks, tradeoffs, and migration concerns (if any).

## Follow-ups ##

List optional follow-up tasks.
