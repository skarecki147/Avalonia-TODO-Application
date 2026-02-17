using Avalonia.Controls;
using Avalonia.Input;
using TodoApp.Shared.ViewModels;

namespace TodoApp.Shared.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        KeyDown += OnKeyDown;
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (DataContext is MainViewModel vm)
        {
            var ctrl = e.KeyModifiers.HasFlag(KeyModifiers.Control);
            vm.HandleKeyboardShortcut(e.Key.ToString(), ctrl);
        }
    }
}
