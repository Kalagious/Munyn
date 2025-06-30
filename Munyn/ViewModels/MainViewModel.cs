using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Munyn.Views.Misc;
using System.Collections.ObjectModel;

namespace Munyn.ViewModels;

public partial class MainViewModel : ViewModelBase
{

    public ContextBase? rootContext;
    public ContextBase? currentContext;

    public Canvas? nodeCanvas;


    [ObservableProperty]
    private bool _isRootContext = true;

    [ObservableProperty]
    private bool _isTrayOpen = true;

    [ObservableProperty]
    private string _currentContextName = "Null";

    [RelayCommand]
    private void ToggleTray()
    {
        IsTrayOpen = ! IsTrayOpen;
    }

    [RelayCommand]
    private void BackContext()
    {
        if (currentContext != null)
        {
            currentContext = currentContext.parentContext;
            RefreshContext();
            ContextPathList.RemoveAt(ContextPathList.Count - 1);
        }
    }

    [RelayCommand]
     private void NewHostDrag()
    {
        if (nodeCanvas != null)
        {
            currentContext.contextNodes.Add(new HostNodeViewModel("Mailing.htb", "(Linux) Ubuntu 20.04", 30, 50, currentContext, nodeCanvas, this));
            RefreshContext();
        }
        
    }

    [RelayCommand]
    private void NewUserDrag()
    {
        if (nodeCanvas != null)
        {
            currentContext.contextNodes.Add(new UserNodeViewModel("Jeff", "Admin", "Test details", 30, 50, nodeCanvas));
            RefreshContext();
        }
    }

    [ObservableProperty]
    private ObservableCollection<ViewModelBase> _visableNodes;

    [ObservableProperty]
    private ObservableCollection<ContextPathEntryViewModel> _contextPathList;
    public MainViewModel()
    {
        // Initialize the ObservableCollection
        VisableNodes = new ObservableCollection<ViewModelBase>();
        ContextPathList = new ObservableCollection<ContextPathEntryViewModel>();

        rootContext = new ContextBase();
        rootContext.contextName = "Root";
        

        EnterContext(rootContext);

    }

    public void RefreshContext()
    {
        CurrentContextName = currentContext.contextName;
        VisableNodes = currentContext.contextNodes;

        if (CurrentContextName == "Root")
            IsRootContext = true;
        else
            IsRootContext = false;

    }

    public void EnterContext(ContextBase context, bool addpath = true)
    {
        if (addpath)
            ContextPathList.Add(new ContextPathEntryViewModel(context, this));

        currentContext = context;
        RefreshContext();


    }


}
