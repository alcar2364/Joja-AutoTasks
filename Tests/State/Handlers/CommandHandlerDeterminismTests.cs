using JojaAutoTasks.Domain.Identifiers;
using JojaAutoTasks.Domain.Tasks;
using JojaAutoTasks.State;
using JojaAutoTasks.State.Commands;
using JojaAutoTasks.State.Handlers;
using JojaAutoTasks.State.Models;
using TaskStatus = JojaAutoTasks.Domain.Tasks.TaskStatus;

namespace JojaAutoTasks.Tests.StateStore.Handlers;

/// <summary>
/// Ensures command handlers are deterministic by asserting that identical command
/// sequences applied to independent state containers produce identical snapshots.
/// </summary>
public class CommandHandlerDeterminismTests
{
    [Fact]
    public void AddOrUpdateTaskCommandHandler_WhenApplyingSameManualUpdateSequence_ProducesSameSnapshot()
    {
        TaskId taskId = new("manual_det_add_update");
        AddOrUpdateTaskCommandHandler handler = new();

        AssertDeterministicSnapshot(sequenceState =>
        {
            handler.Handle(CreateManualAddOrUpdateCommand(taskId, "Initial title", "Initial description", 1), sequenceState);
            handler.Handle(CreateManualAddOrUpdateCommand(taskId, "Updated title", "Updated description", 3), sequenceState);
        });
    }

    [Fact]
    public void CompleteTaskCommandHandler_WhenApplyingSameSequence_ProducesSameSnapshot()
    {
        TaskId taskId = new("manual_det_complete");
        AddOrUpdateTaskCommandHandler addOrUpdateHandler = new();
        CompleteTaskCommandHandler completeHandler = new();

        AssertDeterministicSnapshot(sequenceState =>
        {
            addOrUpdateHandler.Handle(CreateManualAddOrUpdateCommand(taskId, "Complete me", null, 0), sequenceState);
            completeHandler.Handle(new CompleteTaskCommand(taskId, DayKeyFactory.Create(1, "Spring", 2)), sequenceState);
        });
    }

    [Fact]
    public void CompleteTaskCommandHandler_WhenIsPlayerInitiatedFalse_ProducesCorrectCompletedTransition()
    {
        TaskId taskId = new("manual_det_complete_player_false");
        DayKey completionDay = DayKeyFactory.Create(1, "Spring", 2);
        AddOrUpdateTaskCommandHandler addOrUpdateHandler = new();
        CompleteTaskCommandHandler completeHandler = new();

        AssertDeterministicSnapshot(sequenceState =>
        {
            addOrUpdateHandler.Handle(CreateManualAddOrUpdateCommand(taskId, "Complete me", null, 0), sequenceState);
            completeHandler.Handle(new CompleteTaskCommand(taskId, completionDay, isPlayerInitiated: false), sequenceState);
        });

        StateContainer state = new();
        addOrUpdateHandler.Handle(CreateManualAddOrUpdateCommand(taskId, "Complete me", null, 0), state);
        completeHandler.Handle(new CompleteTaskCommand(taskId, completionDay, isPlayerInitiated: false), state);

        TaskSnapshot snapshot = SnapshotProjector.Project(state, DayKeyFactory.Create(1, "Spring", 1), 600);
        TaskView view = Assert.Single(snapshot.TaskViews);
        Assert.Equal(TaskStatus.Completed, view.Status);
        Assert.Equal(completionDay, view.CompletionDay);
    }

    [Fact]
    public void CompleteTaskCommandHandler_WhenIsPlayerInitiatedTrue_ProducesSameCompletedTransition()
    {
        TaskId taskId = new("manual_det_complete_player_true");
        DayKey completionDay = DayKeyFactory.Create(1, "Spring", 2);
        AddOrUpdateTaskCommandHandler addOrUpdateHandler = new();
        CompleteTaskCommandHandler completeHandler = new();

        AssertDeterministicSnapshot(sequenceState =>
        {
            addOrUpdateHandler.Handle(CreateManualAddOrUpdateCommand(taskId, "Complete me", null, 0), sequenceState);
            completeHandler.Handle(new CompleteTaskCommand(taskId, completionDay, isPlayerInitiated: true), sequenceState);
        });

        StateContainer falseState = new();
        StateContainer trueState = new();

        addOrUpdateHandler.Handle(CreateManualAddOrUpdateCommand(taskId, "Complete me", null, 0), falseState);
        addOrUpdateHandler.Handle(CreateManualAddOrUpdateCommand(taskId, "Complete me", null, 0), trueState);

        completeHandler.Handle(new CompleteTaskCommand(taskId, completionDay, isPlayerInitiated: false), falseState);
        completeHandler.Handle(new CompleteTaskCommand(taskId, completionDay, isPlayerInitiated: true), trueState);

        TaskSnapshot falseSnapshot = SnapshotProjector.Project(falseState, DayKeyFactory.Create(1, "Spring", 1), 600);
        TaskSnapshot trueSnapshot = SnapshotProjector.Project(trueState, DayKeyFactory.Create(1, "Spring", 1), 600);

        TaskView falseView = Assert.Single(falseSnapshot.TaskViews);
        TaskView trueView = Assert.Single(trueSnapshot.TaskViews);

        Assert.Equal(TaskStatus.Completed, falseView.Status);
        Assert.Equal(TaskStatus.Completed, trueView.Status);
        Assert.Equal(completionDay, falseView.CompletionDay);
        Assert.Equal(completionDay, trueView.CompletionDay);
        Assert.Equal(ToComparableViews(falseSnapshot), ToComparableViews(trueSnapshot));
    }

    [Fact]
    public void UncompleteTaskCommandHandler_WhenApplyingSameSequence_ProducesSameSnapshot()
    {
        TaskId taskId = new("manual_det_uncomplete");
        AddOrUpdateTaskCommandHandler addOrUpdateHandler = new();
        CompleteTaskCommandHandler completeHandler = new();
        UncompleteTaskCommandHandler uncompleteHandler = new();

        AssertDeterministicSnapshot(sequenceState =>
        {
            addOrUpdateHandler.Handle(CreateManualAddOrUpdateCommand(taskId, "Uncomplete me", "desc", 0), sequenceState);
            completeHandler.Handle(new CompleteTaskCommand(taskId, DayKeyFactory.Create(1, "Spring", 3)), sequenceState);
            uncompleteHandler.Handle(new UncompleteTaskCommand(taskId), sequenceState);
        });
    }

    [Fact]
    public void RemoveTaskCommandHandler_WhenApplyingSameSequence_ProducesSameSnapshot()
    {
        TaskId taskId = new("manual_det_remove");
        AddOrUpdateTaskCommandHandler addOrUpdateHandler = new();
        RemoveTaskCommandHandler removeHandler = new();

        AssertDeterministicSnapshot(sequenceState =>
        {
            addOrUpdateHandler.Handle(CreateManualAddOrUpdateCommand(taskId, "Remove me", null, 0), sequenceState);
            removeHandler.Handle(new RemoveTaskCommand(taskId), sequenceState);
        });
    }

    [Fact]
    public void PinTaskCommandHandler_WhenApplyingSameSequence_ProducesSameSnapshot()
    {
        TaskId taskId = new("manual_det_pin");
        AddOrUpdateTaskCommandHandler addOrUpdateHandler = new();
        PinTaskCommandHandler pinHandler = new();

        AssertDeterministicSnapshot(sequenceState =>
        {
            addOrUpdateHandler.Handle(CreateManualAddOrUpdateCommand(taskId, "Pin me", null, 0), sequenceState);
            pinHandler.Handle(new PinTaskCommand(taskId), sequenceState);
        });
    }

    [Fact]
    public void UnpinTaskCommandHandler_WhenApplyingSameSequence_ProducesSameSnapshot()
    {
        TaskId taskId = new("manual_det_unpin");
        AddOrUpdateTaskCommandHandler addOrUpdateHandler = new();
        PinTaskCommandHandler pinHandler = new();
        UnpinTaskCommandHandler unpinHandler = new();

        AssertDeterministicSnapshot(sequenceState =>
        {
            addOrUpdateHandler.Handle(CreateManualAddOrUpdateCommand(taskId, "Unpin me", "desc", 0), sequenceState);
            pinHandler.Handle(new PinTaskCommand(taskId), sequenceState);
            unpinHandler.Handle(new UnpinTaskCommand(taskId), sequenceState);
        });
    }

    private static void AssertDeterministicSnapshot(Action<StateContainer> applyCommandSequence)
    {
        StateContainer leftState = new();
        StateContainer rightState = new();

        applyCommandSequence(leftState);
        applyCommandSequence(rightState);

        TaskSnapshot leftSnapshot = SnapshotProjector.Project(leftState, DayKeyFactory.Create(1, "Spring", 1), 600);
        TaskSnapshot rightSnapshot = SnapshotProjector.Project(rightState, DayKeyFactory.Create(1, "Spring", 1), 600);

        Assert.Equal(leftSnapshot.Version, rightSnapshot.Version);
        Assert.Equal(ToComparableViews(leftSnapshot), ToComparableViews(rightSnapshot));
    }

    private static IReadOnlyList<ComparableTaskView> ToComparableViews(TaskSnapshot snapshot)
    {
        return snapshot.TaskViews
            .Select(static view => new ComparableTaskView(
                view.Id,
                view.Category,
                view.SourceType,
                view.Title,
                view.Description,
                view.Status,
                view.ProgressCurrent,
                view.ProgressMax,
                view.CreationDay,
                view.CompletionDay,
                view.SourceIdentifier,
                view.IsPinned))
            .ToArray();
    }

    private static AddOrUpdateTaskCommand CreateManualAddOrUpdateCommand(
        TaskId taskId,
        string title,
        string? description,
        int progressCurrent)
    {
        return new AddOrUpdateTaskCommand(
            taskId: taskId,
            category: TaskCategory.Farming,
            sourceType: TaskSourceType.Manual,
            title: title,
            description: description,
            progressCurrent: progressCurrent,
            progressMax: 5,
            creationDay: DayKeyFactory.Create(1, "Spring", 1),
            sourceIdentifier: "manual:test");
    }

    private readonly record struct ComparableTaskView(
        TaskId Id,
        TaskCategory Category,
        TaskSourceType SourceType,
        string Title,
        string? Description,
        TaskStatus Status,
        int ProgressCurrent,
        int ProgressMax,
        DayKey CreationDay,
        DayKey? CompletionDay,
        string SourceIdentifier,
        bool IsPinned);
}
