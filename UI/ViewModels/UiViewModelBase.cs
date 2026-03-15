using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace JojaAutoTasks.Ui.ViewModels;

internal abstract class UiViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
