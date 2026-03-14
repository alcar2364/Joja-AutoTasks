using JojaAutoTasks.Domain.Identifiers;
using JojaAutoTasks.State.DayBoundary;
using JojaAutoTasks.State.Models;

namespace JojaAutoTasks.Tests.StateStore;

public sealed class DeadlineEvaluatorTests
{
    [Fact]
    public void Evaluate_WhenDueDateIsInFuture_DaysRemainingIsPositive()
    {
        DeadlineStoredFields stored = new(
            DayKeyFactory.Create(1, "Spring", 5),
            expiresAtTime: null);

        DeadlineFields result = DeadlineEvaluator.Evaluate(
            stored,
            DayKeyFactory.Create(1, "Spring", 3),
            currentTime: 600);

        Assert.Equal(2, result.DaysRemaining);
        Assert.True(result.DaysRemaining > 0);
    }

    [Fact]
    public void Evaluate_WhenDueDateIsToday_DaysRemainingIsZero()
    {
        DayKey currentDay = DayKeyFactory.Create(1, "Spring", 3);
        DeadlineStoredFields stored = new(currentDay, expiresAtTime: null);

        DeadlineFields result = DeadlineEvaluator.Evaluate(stored, currentDay, currentTime: 600);

        Assert.Equal(0, result.DaysRemaining);
    }

    [Fact]
    public void Evaluate_WhenDueDateIsInPast_DaysRemainingIsNegative()
    {
        DeadlineStoredFields stored = new(
            DayKeyFactory.Create(1, "Spring", 1),
            expiresAtTime: null);

        DeadlineFields result = DeadlineEvaluator.Evaluate(
            stored,
            DayKeyFactory.Create(1, "Spring", 3),
            currentTime: 600);

        Assert.Equal(-2, result.DaysRemaining);
        Assert.True(result.DaysRemaining < 0);
    }

    [Fact]
    public void Evaluate_WhenTodayIsBeforeDueDate_IsOverdueIsFalse()
    {
        DeadlineStoredFields stored = new(
            DayKeyFactory.Create(1, "Spring", 5),
            expiresAtTime: null);

        DeadlineFields result = DeadlineEvaluator.Evaluate(
            stored,
            DayKeyFactory.Create(1, "Spring", 3),
            currentTime: 600);

        Assert.False(result.IsOverdue);
    }

    [Fact]
    public void Evaluate_WhenTodayIsAfterDueDate_IsOverdueIsTrue()
    {
        DeadlineStoredFields stored = new(
            DayKeyFactory.Create(1, "Spring", 1),
            expiresAtTime: null);

        DeadlineFields result = DeadlineEvaluator.Evaluate(
            stored,
            DayKeyFactory.Create(1, "Spring", 3),
            currentTime: 600);

        Assert.True(result.IsOverdue);
    }

    [Fact]
    public void Evaluate_WhenTodayMatchesDueDate_IsOverdueIsFalse()
    {
        DayKey currentDay = DayKeyFactory.Create(1, "Spring", 3);
        DeadlineStoredFields stored = new(currentDay, expiresAtTime: null);

        DeadlineFields result = DeadlineEvaluator.Evaluate(stored, currentDay, currentTime: 600);

        Assert.False(result.IsOverdue);
    }

    [Fact]
    public void Evaluate_WhenExpiresAtTimeIsNull_IsWindowClosedIsFalse()
    {
        DayKey currentDay = DayKeyFactory.Create(1, "Spring", 3);
        DeadlineStoredFields stored = new(currentDay, expiresAtTime: null);

        DeadlineFields result = DeadlineEvaluator.Evaluate(stored, currentDay, currentTime: 2200);

        Assert.False(result.IsWindowClosed);
    }

    [Fact]
    public void Evaluate_WhenTodayIsNotDueDate_IsWindowClosedIsFalse()
    {
        DeadlineStoredFields stored = new(
            DayKeyFactory.Create(1, "Spring", 5),
            expiresAtTime: 2200);

        DeadlineFields result = DeadlineEvaluator.Evaluate(
            stored,
            DayKeyFactory.Create(1, "Spring", 3),
            currentTime: 2200);

        Assert.False(result.IsWindowClosed);
    }

    [Fact]
    public void Evaluate_WhenTodayIsDueDateAndCurrentTimeIsBeforeExpiry_IsWindowClosedIsFalse()
    {
        DayKey currentDay = DayKeyFactory.Create(1, "Spring", 3);
        DeadlineStoredFields stored = new(currentDay, expiresAtTime: 2200);

        DeadlineFields result = DeadlineEvaluator.Evaluate(stored, currentDay, currentTime: 1800);

        Assert.False(result.IsWindowClosed);
    }

    [Fact]
    public void Evaluate_WhenTodayIsDueDateAndCurrentTimeIsAtOrAfterExpiry_IsWindowClosedIsTrue()
    {
        DayKey currentDay = DayKeyFactory.Create(1, "Spring", 3);
        DeadlineStoredFields stored = new(currentDay, expiresAtTime: 2200);

        DeadlineFields result = DeadlineEvaluator.Evaluate(stored, currentDay, currentTime: 2200);

        Assert.True(result.IsWindowClosed);
    }
}