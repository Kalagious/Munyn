using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Munyn.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private bool _isTrayOpen = false;

    [RelayCommand]
    private void ToggleTray()
    {
        IsTrayOpen = ! IsTrayOpen;
    }


    [ObservableProperty]
    private ObservableCollection<HostNodeViewModel> _hostNodes;

    public void MainWindowViewModel()
    {
        // Initialize the ObservableCollection
        HostNodes = new ObservableCollection<HostNodeViewModel>();

        // Add some sample data to the list
        HostNodes.Add(new HostNodeViewModel("Server 1", "192.168.1.10", "Ubuntu", 100, 200));
        HostNodes.Add(new HostNodeViewModel("Router", "192.168.1.1", "Cisco IOS", 100, 400));
        HostNodes.Add(new HostNodeViewModel("Workstation 3", "192.168.1.25", "Windows", 200, 100));
        HostNodes.Add(new HostNodeViewModel("Printer", "192.168.1.50", "Linux", 200, 400));
    }
}
