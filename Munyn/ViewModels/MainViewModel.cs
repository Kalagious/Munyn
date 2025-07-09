using Avalonia.Controls;
using Avalonia;
using Avalonia.VisualTree;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using Munyn.Views.Misc;
using System.Collections.ObjectModel;
using System.Linq;
using System.Drawing;
using Avalonia.Input;
using Avalonia.Media;

namespace Munyn.ViewModels;

public partial class MainViewModel : ViewModelBase
{

    public ContextBase? rootContext;
    public ContextBase? currentContext;

    public Canvas? NodeCanvasBase;

    [ObservableProperty]
    private bool _pathTool = false;
    [ObservableProperty]
    private IBrush _pathToolBorderColor;

    public bool isDrawingPath = false;
    NodeBaseViewModel _connectionSourceNode;
    Avalonia.Point CurrentConnectionStartPoint;
    Avalonia.Point CurrentConnectionEndPoint;
    private PathBaseViewModel _currentPath;


    [ObservableProperty]
    private bool _isRootContext = true;

    [ObservableProperty]
    private bool _isTrayOpen = true;

    [ObservableProperty]
    private string _currentContextName = "Null";

    [ObservableProperty]
    private NodeBaseViewModel _selectedNode;

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
    private void TogglePathTool()
    {
        PathTool = ! PathTool;
        if (PathTool)
            PathToolBorderColor = new SolidColorBrush(Avalonia.Media.Color.FromArgb(255,242,242,242));
        else
            PathToolBorderColor = new SolidColorBrush(Avalonia.Media.Color.FromArgb(0,242,242,242));

        if (isDrawingPath)
        {
            currentContext.contextNodes.Remove(_currentPath);
            isDrawingPath = false;
            
        }
    }

    [RelayCommand]
    private void NewNetworkDrag()
    {
        if (NodeCanvasBase != null)
        {
            NodeBaseViewModel newNode = new NetworkNodeViewModel("10.10.14.0/24", 30, 50, currentContext, NodeCanvasBase, this);
            newNode.OnStartConnectionDragNode = OnStartConnectionDragFromNode;

            currentContext.contextNodes.Add(newNode);
            RefreshContext();
        }

    }

    [RelayCommand]
     private void NewHostDrag()
    {
        if (NodeCanvasBase != null)
        {
            NodeBaseViewModel newNode = new HostNodeViewModel("Mailing.htb", "(Linux) Ubuntu 20.04", 30, 50, currentContext, NodeCanvasBase, this);
            newNode.OnStartConnectionDragNode = OnStartConnectionDragFromNode;
            newNode.OnClickedNode = OnClickedNode;
            currentContext.contextNodes.Add(newNode);

            RefreshContext();
        }
        
    }

    [RelayCommand]
    private void NewUserDrag()
    {
        if (NodeCanvasBase != null)
        {
            NodeBaseViewModel newNode = new UserNodeViewModel("Jeff", "Admin", "Test details", 30, 50, NodeCanvasBase);
            newNode.OnStartConnectionDragNode = OnStartConnectionDragFromNode;

            currentContext.contextNodes.Add(newNode);
            RefreshContext();
        }
    }

    [RelayCommand]
    private void NewServiceDrag()
    {
        if (NodeCanvasBase != null)
        {
            NodeBaseViewModel newNode = new ServiceNodeViewModel("Apache Tomcat", 30, 50, NodeCanvasBase);
            newNode.OnStartConnectionDragNode = OnStartConnectionDragFromNode;
            

            currentContext.contextNodes.Add(newNode);
            RefreshContext();
        }

    }

    [RelayCommand]
    private void NewAssetDrag()
    {
        if (NodeCanvasBase != null)
        {

            NodeBaseViewModel newNode = new AssetNodeViewModel("id_rsa", 30, 50, NodeCanvasBase);
            newNode.OnStartConnectionDragNode = OnStartConnectionDragFromNode;

            currentContext.contextNodes.Add(newNode);
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

        foreach (ViewModelBase node in VisableNodes) {
            if (node is HostNodeViewModel)
            {
                ((HostNodeViewModel)node).Refresh();
            }
            if (node is PathBaseViewModel)
            {
                ((PathBaseViewModel)node).RecalculatePathData();
            }
        }


    }

    private void OnStartConnectionDragFromNode(NodeBaseViewModel sourceNode, Avalonia.Point portCanvasCoordinates, PointerPressedEventArgs e)
    {
        // This is where MainWindowViewModel handles starting the connection drag
        if (PathTool)
        {
            _connectionSourceNode = sourceNode;
            isDrawingPath = true;
            CurrentConnectionStartPoint = portCanvasCoordinates;
            CurrentConnectionEndPoint = portCanvasCoordinates;
            if (NodeCanvasBase != null)
                e.Pointer.Capture(NodeCanvasBase);

            _currentPath = new PathBaseViewModel(sourceNode, CurrentConnectionEndPoint, this);
            sourceNode.connectedPaths.Add(_currentPath);
            currentContext.contextNodes.Add(_currentPath);
        }
        e.Handled = true;
    }

    
    public void HandlePointerMoved(PointerEventArgs e)
    {
        if (isDrawingPath && e.Pointer.Captured == NodeCanvasBase)
        {
            Avalonia.Point currentCanvasPosition = e.GetPosition(NodeCanvasBase);

            _currentPath.EndPoint = currentCanvasPosition;
            _currentPath.RecalculatePathData();
        }
    }

    public void OnClickedNode(NodeBaseViewModel node, PointerReleasedEventArgs e)
    {
        SelectedNode = node;
    }


    public void OnEndConnectionDragFromNode(PointerReleasedEventArgs e)
    {
        if (isDrawingPath)
        {
            bool foundNode = false;
            Avalonia.Point releasePointInCanvas = e.GetPosition(NodeCanvasBase);
            var hitElement = NodeCanvasBase.GetVisualsAt(releasePointInCanvas);
            if (hitElement != null)
            {
                foreach (var element in hitElement)
                {
                    if (element.DataContext is PathBaseViewModel)
                        continue;

                    if (element.DataContext is NodeBaseViewModel hitNodeVm)
                    {
                        if (_connectionSourceNode != hitNodeVm)
                        {
                            _currentPath.EndNode = hitNodeVm;
                            hitNodeVm.connectedPaths.Add(_currentPath);
                            _currentPath.EndPoint = CurrentConnectionEndPoint;
                            _currentPath.RecalculatePathData();
                            foundNode = true;
                            break;
                        }
                    }
                }
            }
            
            if (!foundNode)
                currentContext.contextNodes.Remove(_currentPath);

            }
        e.Pointer.Capture(null);

        isDrawingPath = false;
        e.Handled = true;
    }

    public Avalonia.Controls.Control? FindNodeViewByViewModel(NodeBaseViewModel nodeViewModel)
    {
        if (NodeCanvasBase == null || nodeViewModel == null)
        {
            return null; // Cannot find view if canvas is not set or viewmodel is null
        }

        foreach (var child in NodeCanvasBase.Children)
        {
            if (child is Avalonia.Controls.Control nodeView && nodeView.DataContext == nodeViewModel)
            {
                return nodeView;
            }
        }
        return null;
    }
    public void EnterContext(ContextBase context)
    {
        currentContext = context;
        RefreshContext();
    }


}
