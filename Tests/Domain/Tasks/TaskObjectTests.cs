using JojaAutoTasks.Domain.Identifiers;
using JojaAutoTasks.Domain.Tasks;
using TaskStatus = JojaAutoTasks.Domain.Tasks.TaskStatus;

namespace JojaAutoTasks.Tests.Domain.Tasks;

/// <summary>
/// Tests TaskObject constructor guards and legal state combinations.
/// </summary>
public class TaskObjectTests
{
    [Fact]
    public void Ctor_WhenIncompleteTaskInputsAreValid_CreatesInstance()
    {
        TaskObject sut = CreateTaskObject();

        Assert.Equal(TaskStatus.Incomplete, sut.Status);
        Assert.Null(sut.CompletionDay);
    }

    [Fact]
    public void Ctor_WhenCompletedTaskInputsAreValid_CreatesInstance()
    {
        DayKey completionDay = DayKeyFactory.Create(1, "Spring", 7);

        TaskObject sut = CreateTaskObject(
            status: TaskStatus.Completed,
            progressCurrent: 5,
            progressMax: 5,
            completionDay: completionDay);

        Assert.Equal(TaskStatus.Completed, sut.Status);
        Assert.Equal(completionDay, sut.CompletionDay);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Ctor_WhenTitleIsNullOrWhitespace_ThrowsArgumentException(string? title)
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(() =>
            CreateTaskObject(title: title));

        Assert.Equal("title", exception.ParamName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Ctor_WhenSourceIdentifierIsNullOrWhitespace_ThrowsArgumentException(string? sourceIdentifier)
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(() =>
            CreateTaskObject(sourceIdentifier: sourceIdentifier));

        Assert.Equal("sourceIdentifier", exception.ParamName);
    }

    [Fact]
    public void Ctor_WhenProgressCurrentIsNegative_ThrowsArgumentOutOfRangeException()
    {
        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            CreateTaskObject(progressCurrent: -1));

        Assert.Equal("progressCurrent", exception.ParamName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Ctor_WhenProgressMaxIsNotPositive_ThrowsArgumentOutOfRangeException(int progressMax)
    {
        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            CreateTaskObject(progressMax: progressMax));

        Assert.Equal("progressMax", exception.ParamName);
    }

    [Fact]
    public void Ctor_WhenStatusIsCompletedAndCompletionDayMissing_ThrowsArgumentException()
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(() =>
            CreateTaskObject(
                status: TaskStatus.Completed,
                progressCurrent: 1,
                progressMax: 1,
                completionDay: null));

        Assert.Equal("completionDay", exception.ParamName);
    }

    [Fact]
    public void Ctor_WhenStatusIsCompletedAndProgressCurrentIsLessThanProgressMax_ThrowsArgumentException()
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(() =>
            CreateTaskObject(
                status: TaskStatus.Completed,
                progressCurrent: 1,
                progressMax: 2,
                completionDay: DayKeyFactory.Create(1, "Spring", 7)));

        Assert.Equal("progressCurrent", exception.ParamName);
    }

    [Fact]
    public void Ctor_WhenStatusIsIncompleteAndCompletionDayProvided_ThrowsArgumentException()
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(() =>
            CreateTaskObject(completionDay: DayKeyFactory.Create(1, "Spring", 7)));

        Assert.Equal("completionDay", exception.ParamName);
    }

    private static TaskObject CreateTaskObject(
        TaskStatus status = TaskStatus.Incomplete,
        int progressCurrent = 0,
        int progressMax = 1,
        DayKey? completionDay = null,
        string? title = "Water Crops",
        string? sourceIdentifier = "BuiltIn.Foraging.Route")
    {
        DayKey creationDay = DayKeyFactory.Create(1, "Spring", 5);

        return new TaskObject(
            id: TaskIdFactory.CreateBuiltIn("ForageSweep", "forest_route", creationDay),
            category: TaskCategory.Farming,
            sourceType: TaskSourceType.BuiltIn,
            title: title!,
            description: "Keep crops hydrated.",
            status: status,
            progressCurrent: progressCurrent,
            progressMax: progressMax,
            creationDay: creationDay,
            completionDay: completionDay,
            sourceIdentifier: sourceIdentifier!);
    }
}
