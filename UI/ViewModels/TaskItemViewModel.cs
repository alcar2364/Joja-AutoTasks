using CommunityToolkit.Mvvm.ComponentModel;
using JojaAutoTasks.Domain.Identifiers;
using JojaAutoTasks.Domain.Tasks;
using JojaAutoTasks.State.Models;
using TaskStatus = JojaAutoTasks.Domain.Tasks.TaskStatus;

namespace JojaAutoTasks.Ui.ViewModels;

internal sealed partial class TaskItemViewModel : UiViewModelBase
{
    [ObservableProperty]
    private TaskId _id;

    [ObservableProperty]
    private TaskCategory _category;

    [ObservableProperty]
    private TaskSourceType _sourceType;

    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private string? _description;

    [ObservableProperty]
    private string _sourceIdentifier;

    [ObservableProperty]
    private int _progressCurrent;

    [ObservableProperty]
    private int _progressMax;

    [ObservableProperty]
    private DayKey _creationDay;

    [ObservableProperty]
    private bool _isPinned;

    [ObservableProperty]
    private TaskStatus _status;

    [ObservableProperty]
    private DayKey? _completionDay;

    [ObservableProperty]
    private DeadlineFields? _deadlineFields;

    internal TaskItemViewModel(TaskView taskView)
    {
        Id = taskView.Id;
        Category = taskView.Category;
        SourceType = taskView.SourceType;
        Title = taskView.Title;
        Description = taskView.Description;
        SourceIdentifier = taskView.SourceIdentifier;
        ProgressCurrent = taskView.ProgressCurrent;
        ProgressMax = taskView.ProgressMax;
        CreationDay = taskView.CreationDay;
        IsPinned = taskView.IsPinned;
        Status = taskView.Status;
        CompletionDay = taskView.CompletionDay;
        DeadlineFields = taskView.DeadlineFields;
    }
}
