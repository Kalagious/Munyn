using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Munyn.ViewModels;

public partial class MainViewModel : ViewModelBase
{

    public Canvas? nodeCanvas;

    [ObservableProperty]
    private bool _isTrayOpen = true;

    [RelayCommand]
    private void ToggleTray()
    {
        IsTrayOpen = ! IsTrayOpen;
    }

    [RelayCommand]
     private void NewHostDrag()
    {
        if (nodeCanvas != null)
            Nodes.Add(new HostNodeViewModel("New Host", "1.1.1.1", "Ubuntu", 0, 0, nodeCanvas));
    }

    [RelayCommand]
    private void NewUserDrag()
    {
        if (nodeCanvas != null)
            Nodes.Add(new UserNodeViewModel("Jeff", "Admin", "Test details", 0, 0, nodeCanvas));
    }

    [ObservableProperty]
    private ObservableCollection<ViewModelBase> _Nodes;

    public MainViewModel()
    {
        // Initialize the ObservableCollection
        Nodes = new ObservableCollection<ViewModelBase>();
    }
}
