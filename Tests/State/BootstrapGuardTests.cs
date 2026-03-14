using JojaAutoTasks.Domain.Identifiers;
using JojaAutoTasks.Domain.Tasks;
using JojaAutoTasks.State;
using JojaAutoTasks.State.Commands;
using Store = JojaAutoTasks.State.StateStore;

namespace JojaAutoTasks.Tests.StateStore;

public sealed class BootstrapGuardTests
{
    [Fact]
    public void Dispatch_WhenDebugPolicyAndNoTimeContext_ThrowsInvalidOperationException()
    {
        Store store = new();
        store.SetBootstrapGuardPolicy(BootstrapGuardPolicy.Debug);

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            store.Dispatch(CreateAddCommand(new TaskId("bootstrap_debug_throw"))));

        Assert.Contains("Projection called before time context was initialized.", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Dispatch_WhenReleasePolicyAndNoTimeContext_EmitsExactlyOneWarningPerSession()
    {
        Store store = new();
        List<string> warnings = new();

        store.SetBootstrapGuardPolicy(BootstrapGuardPolicy.Release);
        store.SetWarnAction(warnings.Add);

        store.Dispatch(CreateAddCommand(new TaskId("bootstrap_release_first")));
        store.Dispatch(CreateAddCommand(new TaskId("bootstrap_release_second")));

        Assert.Single(warnings);
    }

    [Fact]
    public void Dispatch_WhenDebugDiagnosticPolicyAndNoTimeContext_EmitsWarningOnEachViolatingCall()
    {
        Store store = new();
        List<string> warnings = new();

        store.SetBootstrapGuardPolicy(BootstrapGuardPolicy.DebugDiagnostic);
        store.SetWarnAction(warnings.Add);

        store.Dispatch(CreateAddCommand(new TaskId("bootstrap_diag_one")));
        store.Dispatch(CreateAddCommand(new TaskId("bootstrap_diag_two")));
        store.Dispatch(CreateAddCommand(new TaskId("bootstrap_diag_three")));

        Assert.Equal(3, warnings.Count);
    }

    [Fact]
    public void Dispatch_WhenReleasePolicyWarningDedupeResetsAfterNewSession_EmitsOneWarningPerSession()
    {
        Store store = new();
        List<string> warnings = new();

        store.SetBootstrapGuardPolicy(BootstrapGuardPolicy.Release);
        store.SetWarnAction(warnings.Add);

        store.Dispatch(CreateAddCommand(new TaskId("bootstrap_release_session_one")));

        store.OnReturnToTitle();
        store.OnSaveLoaded();

        store.Dispatch(CreateAddCommand(new TaskId("bootstrap_release_session_two")));

        Assert.Equal(2, warnings.Count);
    }

    [Fact]
    public void SetBootstrapGuardPolicy_AllThreePoliciesTestableInOneSuite()
    {
        Store store = new();

        store.SetBootstrapGuardPolicy(BootstrapGuardPolicy.Debug);
        Assert.Throws<InvalidOperationException>(() =>
            store.Dispatch(CreateAddCommand(new TaskId("bootstrap_switch_debug"))));

        List<string> releaseWarnings = new();
        store.SetBootstrapGuardPolicy(BootstrapGuardPolicy.Release);
        store.SetWarnAction(releaseWarnings.Add);
        store.OnReturnToTitle();
        store.OnSaveLoaded();
        store.Dispatch(CreateAddCommand(new TaskId("bootstrap_switch_release_one")));
        store.Dispatch(CreateAddCommand(new TaskId("bootstrap_switch_release_two")));
        Assert.Single(releaseWarnings);

        List<string> diagnosticWarnings = new();
        store.SetBootstrapGuardPolicy(BootstrapGuardPolicy.DebugDiagnostic);
        store.SetWarnAction(diagnosticWarnings.Add);
        store.OnReturnToTitle();
        store.OnSaveLoaded();
        store.Dispatch(CreateAddCommand(new TaskId("bootstrap_switch_diag_one")));
        store.Dispatch(CreateAddCommand(new TaskId("bootstrap_switch_diag_two")));
        Assert.Equal(2, diagnosticWarnings.Count);
    }

    private static AddOrUpdateTaskCommand CreateAddCommand(TaskId taskId)
    {
        return new AddOrUpdateTaskCommand(
            taskId: taskId,
            category: TaskCategory.Farming,
            sourceType: TaskSourceType.Manual,
            title: "Bootstrap Guard Task",
            description: null,
            progressCurrent: 0,
            progressMax: 1,
            creationDay: DayKeyFactory.Create(1, "Spring", 1),
            sourceIdentifier: "manual:test");
    }
}