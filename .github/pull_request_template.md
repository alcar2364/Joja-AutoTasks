# Pull Request #

## Summary ##

Describe what changed and why.

## Scope ##

	* [ ] Scope is intentional and limited to the requested behavior/change.
	* [ ] Any out-of-scope follow-ups are explicitly listed below.

## Testing ##

> **⚠️ Testing is manual and must be performed by an admin before merge.**
> Automated tests are NOT run in CI. If this PR contains code changes, an admin must run the
> test suite locally and confirm all tests pass before approving.

If this PR contains code changes, an admin must verify the following before approving:

	* [ ] Admin has run `dotnet build JojaAutoTasks.csproj -c Debug -p:EnableModDeploy=false -p:EnableModZip=false` locally and the build succeeds.
	* [ ] Admin has run `dotnet test "Tests/JojaAutoTasks.Tests.csproj"` locally and all tests pass.
	* [ ] If `OnUpdateTicked` flow changed, admin verified tests cover deterministic
		tick throttling and the guard-block no-op path (no runtime access, no
		second dispatch in-window).
	* [ ] If lifecycle signal routing changed, admin verified lifecycle tests cover
		signal forwarding and signal-only `OnSaving` expectations.
	* [ ] If dispatcher routing changed, admin verified dispatcher tests still
		assert determinism, statelessness, and no-op dispatch contracts.
	* [ ] If `ModConfig.CurrentConfigVersion` or config migration logic changed,
		admin ran `dotnet test "Tests/JojaAutoTasks.Tests.csproj" --filter FullyQualifiedName~ConfigLoaderMigrationSafetyTests`.

If this PR contains **only documentation changes**, testing is not required.

## Risks / Notes ##

List risks, tradeoffs, and migration concerns (if any).

## Follow-ups ##

List optional follow-up tasks.
