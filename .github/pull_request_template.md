# Pull Request #

## Summary ##

Describe what changed and why.

## Scope ##

	* [ ] Scope is intentional and limited to the requested behavior/change.
	* [ ] Any out-of-scope follow-ups are explicitly listed below.

## Testing ##

		* [ ] I reviewed `Tests/README.md` and followed the test naming/coverage
			conventions.
		* [ ] I added or updated tests for the changed behavior.
		* [ ] If production files changed, I ran
			`dotnet build JojaAutoTasks.csproj -c Debug -p:EnableModDeploy=false -p:EnableModZip=false`
			to validate project/test isolation.
		* [ ] If `OnUpdateTicked` flow changed, I verified tests cover deterministic
			tick throttling and the guard-block no-op path (no runtime access, no
			second dispatch in-window).
		* [ ] If lifecycle signal routing changed, I verified lifecycle tests cover
			signal forwarding and signal-only `OnSaving` expectations.
		* [ ] If dispatcher routing changed, I verified dispatcher tests still
			assert determinism, statelessness, and no-op dispatch contracts.
		* [ ] If `ModConfig.CurrentConfigVersion` or config migration logic changed,
			I updated `ConfigLoaderMigrationSafetyTests` coverage.
		* [ ] I ran `dotnet test "Tests\JojaAutoTasks.Tests.csproj"` locally.
		* [ ] If I touched config normalization behavior, I ran
			`dotnet test "Tests\JojaAutoTasks.Tests.csproj" --filter FullyQualifiedName~ConfigLoaderMigrationSafetyTests`.

## Risks / Notes ##

List risks, tradeoffs, and migration concerns (if any).

## Follow-ups ##

List optional follow-up tasks.
