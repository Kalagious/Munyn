using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using Munyn.Views.Misc;
using System.Collections.ObjectModel;
using System.Linq;

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
        }
    }

    [RelayCommand]
    private void NewNetworkDrag()
    {
        if (nodeCanvas != null)
        {
            currentContext.contextNodes.Add(new NetworkNodeViewModel("10.10.14.0/24", 30, 50, currentContext, nodeCanvas, this));
            RefreshContext();
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

    [RelayCommand]
    private void NewServiceDrag()
    {
        if (nodeCanvas != null)
        {
            currentContext.contextNodes.Add(new ServiceNodeViewModel("Apache Webserver", 30, 50, nodeCanvas));
            RefreshContext();
        }

    }

    [RelayCommand]
    private void NewAssetDrag()
    {
        if (nodeCanvas != null)
        {
            //currentContext.contextNodes.Add(new HostNodeViewModel("Mailing.htb", "(Linux) Ubuntu 20.04", 30, 50, currentContext, nodeCanvas, this));
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

        if (currentContext != null)
        {
            ContextBase tmpContext = currentContext;
            ContextPathList.Clear();

            if (tmpContext.parentContext == null)
                ContextPathList.Add(new ContextPathEntryViewModel(currentContext, this));
            else
            {
                while (tmpContext.parentContext != null) 
                {
                    ContextPathList.Add(new ContextPathEntryViewModel(tmpContext, this));
                    tmpContext = tmpContext.parentContext;

                }
                ContextPathList.Add(new ContextPathEntryViewModel(rootContext, this));
            }

            IEnumerable<ContextPathEntryViewModel> reversedPath = ContextPathList.Reverse();
            ContextPathList = new ObservableCollection<ContextPathEntryViewModel>(reversedPath);
        }
        if (CurrentContextName == "Root")
            IsRootContext = true;
        else
            IsRootContext = false;

        foreach (NodeBaseViewModel node in VisableNodes) {
            if (node is HostNodeViewModel)
            {
                ((HostNodeViewModel)node).Refresh();
            }
        }

    }

    public void EnterContext(ContextBase context)
    {
        currentContext = context;
        RefreshContext();
    }


}
