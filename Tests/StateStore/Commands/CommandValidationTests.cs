using JojaAutoTasks.Domain.Identifiers;
using JojaAutoTasks.Domain.Tasks;
using JojaAutoTasks.State.Commands;

namespace JojaAutoTasks.Tests.State.Commands;

public sealed class CommandValidationTests
{
    [Fact]
    public void AddOrUpdateTaskCommand_Ctor_WhenTaskIdIsDefault_ThrowsArgumentException()
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(() =>
            new AddOrUpdateTaskCommand(
                taskId: default,
                category: TaskCategory.Animals,
                sourceType: TaskSourceType.BuiltIn,
                title: "Collect eggs",
                description: "From coop",
                progressCurrent: 0,
                progressMax: 1,
                creationDay: CreateDayKey(),
                sourceIdentifier: "BuiltIn:CollectEggs"));

        Assert.Equal("taskId", exception.ParamName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void AddOrUpdateTaskCommand_Ctor_WhenTitleIsEmptyOrWhitespace_ThrowsArgumentException(string title)
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(
            () => CreateValidAddOrUpdateCommand(title: title));

        Assert.Equal("title", exception.ParamName);
    }

    [Fact]
    public void AddOrUpdateTaskCommand_Ctor_WhenTitleIsNull_ThrowsArgumentException()
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(
            () => CreateValidAddOrUpdateCommand(title: null!));

        Assert.Equal("title", exception.ParamName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void AddOrUpdateTaskCommand_Ctor_WhenDescriptionIsWhitespace_ThrowsArgumentException(string description)
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(
            () => CreateValidAddOrUpdateCommand(description: description));

        Assert.Equal("description", exception.ParamName);
    }

    [Fact]
    public void AddOrUpdateTaskCommand_Ctor_WhenProgressCurrentIsNegative_ThrowsArgumentOutOfRangeException()
    {
        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => CreateValidAddOrUpdateCommand(progressCurrent: -1));

        Assert.Equal("progressCurrent", exception.ParamName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void AddOrUpdateTaskCommand_Ctor_WhenProgressMaxIsNotPositive_ThrowsArgumentOutOfRangeException(int progressMax)
    {
        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => CreateValidAddOrUpdateCommand(progressMax: progressMax));

        Assert.Equal("progressMax", exception.ParamName);
    }

    [Fact]
    public void AddOrUpdateTaskCommand_Ctor_WhenCreationDayIsDefault_ThrowsArgumentException()
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(() =>
            new AddOrUpdateTaskCommand(
                taskId: CreateTaskId(),
                category: TaskCategory.Animals,
                sourceType: TaskSourceType.BuiltIn,
                title: "Collect eggs",
                description: "From coop",
                progressCurrent: 0,
                progressMax: 1,
                creationDay: default,
                sourceIdentifier: "BuiltIn:CollectEggs"));

        Assert.Equal("creationDay", exception.ParamName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void AddOrUpdateTaskCommand_Ctor_WhenSourceIdentifierIsEmptyOrWhitespace_ThrowsArgumentException(string sourceIdentifier)
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(
            () => CreateValidAddOrUpdateCommand(sourceIdentifier: sourceIdentifier));

        Assert.Equal("sourceIdentifier", exception.ParamName);
    }

    [Fact]
    public void AddOrUpdateTaskCommand_Ctor_WithValidValues_SetsExpectedState()
    {
        TaskId taskId = new TaskId("BuiltIn_Farm_WaterCrops");
        DayKey creationDay = new DayKey("Year1-Spring1");
        TaskCategory category = TaskCategory.Farming;
        TaskSourceType sourceType = TaskSourceType.BuiltIn;
        string title = "Water crops";
        string? description = null;
        int progressCurrent = 0;
        int progressMax = 5;
        string sourceIdentifier = "BuiltIn:WaterCrops";

        AddOrUpdateTaskCommand command = new AddOrUpdateTaskCommand(
            taskId,
            category,
            sourceType,
            title,
            description,
            progressCurrent,
            progressMax,
            creationDay,
            sourceIdentifier);

        Assert.Equal(taskId, command.TaskId);
        Assert.Equal(category, command.Category);
        Assert.Equal(sourceType, command.SourceType);
        Assert.Equal(title, command.Title);
        Assert.Equal(description, command.Description);
        Assert.Equal(progressCurrent, command.ProgressCurrent);
        Assert.Equal(progressMax, command.ProgressMax);
        Assert.Equal(creationDay, command.CreationDay);
        Assert.Equal(sourceIdentifier, command.SourceIdentifier);
    }

    [Fact]
    public void CompleteTaskCommand_Ctor_WhenTaskIdIsDefault_ThrowsArgumentException()
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(
            () => new CompleteTaskCommand(default, CreateDayKey()));

        Assert.Equal("taskId", exception.ParamName);
    }

    [Fact]
    public void CompleteTaskCommand_Ctor_WhenCompletionDayIsDefault_ThrowsArgumentException()
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(
            () => new CompleteTaskCommand(CreateTaskId(), default));

        Assert.Equal("completionDay", exception.ParamName);
    }

    [Fact]
    public void CompleteTaskCommand_Ctor_WithValidValues_SetsExpectedState()
    {
        TaskId taskId = CreateTaskId();
        DayKey completionDay = CreateDayKey();

        CompleteTaskCommand command = new CompleteTaskCommand(taskId, completionDay);

        Assert.Equal(taskId, command.TaskId);
        Assert.Equal(completionDay, command.CompletionDay);
    }

    [Fact]
    public void UncompleteTaskCommand_Ctor_WhenTaskIdIsDefault_ThrowsArgumentException()
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(
            () => new UncompleteTaskCommand(default));

        Assert.Equal("taskId", exception.ParamName);
    }

    [Fact]
    public void UncompleteTaskCommand_Ctor_WithValidValue_SetsExpectedState()
    {
        TaskId taskId = CreateTaskId();

        UncompleteTaskCommand command = new UncompleteTaskCommand(taskId);

        Assert.Equal(taskId, command.TaskId);
    }

    [Fact]
    public void RemoveTaskCommand_Ctor_WhenTaskIdIsDefault_ThrowsArgumentException()
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(
            () => new RemoveTaskCommand(default));

        Assert.Equal("taskId", exception.ParamName);
    }

    [Fact]
    public void RemoveTaskCommand_Ctor_WithValidValue_SetsExpectedState()
    {
        TaskId taskId = CreateTaskId();

        RemoveTaskCommand command = new RemoveTaskCommand(taskId);

        Assert.Equal(taskId, command.TaskId);
    }

    [Fact]
    public void PinTaskCommand_Ctor_WhenTaskIdIsDefault_ThrowsArgumentException()
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(
            () => new PinTaskCommand(default));

        Assert.Equal("taskId", exception.ParamName);
    }

    [Fact]
    public void PinTaskCommand_Ctor_WithValidValue_SetsExpectedState()
    {
        TaskId taskId = CreateTaskId();

        PinTaskCommand command = new PinTaskCommand(taskId);

        Assert.Equal(taskId, command.TaskId);
    }

    [Fact]
    public void UnpinTaskCommand_Ctor_WhenTaskIdIsDefault_ThrowsArgumentException()
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(
            () => new UnpinTaskCommand(default));

        Assert.Equal("taskId", exception.ParamName);
    }

    [Fact]
    public void UnpinTaskCommand_Ctor_WithValidValue_SetsExpectedState()
    {
        TaskId taskId = CreateTaskId();

        UnpinTaskCommand command = new UnpinTaskCommand(taskId);

        Assert.Equal(taskId, command.TaskId);
    }

    private static AddOrUpdateTaskCommand CreateValidAddOrUpdateCommand(
        TaskId? taskId = null,
        string title = "Collect eggs",
        string? description = "From coop",
        int progressCurrent = 0,
        int progressMax = 1,
        DayKey? creationDay = null,
        string sourceIdentifier = "BuiltIn:CollectEggs")
    {
        return new AddOrUpdateTaskCommand(
            taskId ?? CreateTaskId(),
            TaskCategory.Animals,
            TaskSourceType.BuiltIn,
            title,
            description,
            progressCurrent,
            progressMax,
            creationDay ?? CreateDayKey(),
            sourceIdentifier);
    }

    private static TaskId CreateTaskId() => new TaskId("BuiltIn_TestTask");

    private static DayKey CreateDayKey() => new DayKey("Year1-Spring1");
}