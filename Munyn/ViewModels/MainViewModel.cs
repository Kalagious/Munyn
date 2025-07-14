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
using Munyn.ViewModels.Data;
using Munyn.ViewModels.Nodes.Properties;
using Newtonsoft.Json;
using System.IO;
using Avalonia.Platform.Storage;
using System.Threading.Tasks;
using System;

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
            NodeBaseViewModel newNode = new HostNodeViewModel("Mailing.htb", 30, 50, currentContext, NodeCanvasBase, this);
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
            NodeBaseViewModel newNode = new UserNodeViewModel("Jeff",  30, 50, NodeCanvasBase);
            newNode.OnStartConnectionDragNode = OnStartConnectionDragFromNode;
            newNode.OnClickedNode = OnClickedNode;

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
            newNode.OnClickedNode = OnClickedNode;


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
            newNode.OnClickedNode = OnClickedNode;

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
                //((PathBaseViewModel)node).RecalculatePathData(); // Original synchronous call
            }
        }

        // After updating VisableNodes and allowing the UI to potentially process it,
        // schedule a subsequent operation to recalculate paths.
        // This gives the layout system a chance to measure and arrange node views.
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            foreach (ViewModelBase node in VisableNodes)
            {
                if (node is PathBaseViewModel pathVM)
                {
                    pathVM.RecalculatePathData();
                }
            }
        }, Avalonia.Threading.DispatcherPriority.Background); // Use Background priority to run after layout
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

    [RelayCommand]
    private async void Save()
    {
        await SaveAsync();
    }

    [RelayCommand]
    private async void Load()
    {
        await LoadAsync();
    }

    private async Task SaveAsync()
    {
        var topLevel = TopLevel.GetTopLevel(NodeCanvasBase);
        if (topLevel == null) return;

        var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save Munyn Graph",
            FileTypeChoices = new[]
            {
                new FilePickerFileType("JSON") { Patterns = new[] { "*.json" } }
            }
        });

        if (file is not null)
        {
            var saveData = new ContextDto();
            BuildDtoFromContext(rootContext, saveData);

            string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
            await using var stream = await file.OpenWriteAsync();
            await using var writer = new StreamWriter(stream);
            await writer.WriteAsync(json);
        }
    }

    private void BuildDtoFromContext(ContextBase context, ContextDto dto)
    {
        dto.Id = context.Id.ToString();
        dto.NodeName = context.NodeName;
        dto.NodeType = context.GetType().Name;
        dto.X = context.X;
        dto.Y = context.Y;
        dto.Properties = context.Properties.Select(p => new NodePropertyDto
        {
            PropertyName = p.PropertyName,
            IsVisableOnGraphNode = p.IsVisableOnGraphNode,
            Value = p.PropertyValue
        }).ToList();

        if (context.NodeTheme is LinearGradientBrush contextBrush)
        {
            dto.ThemeColor1 = contextBrush.GradientStops[0].Color.ToString();
            dto.ThemeColor2 = contextBrush.GradientStops[1].Color.ToString();
        }

        dto.Nodes = new List<NodeDto>();
        dto.Paths = new List<PathDto>();
        dto.ChildrenContexts = new List<ContextDto>();

        foreach (var node in context.contextNodes)
        {
            if (node is ContextBase childContext)
            {
                var childDto = new ContextDto();
                BuildDtoFromContext(childContext, childDto);
                dto.ChildrenContexts.Add(childDto);
            }
            else if (node is PathBaseViewModel path)
            {
                dto.Paths.Add(new PathDto
                {
                    StartNodeId = path.StartNode.Id.ToString(),
                    EndNodeId = path.EndNode.Id.ToString()
                });
            }
            else if (node is NodeBaseViewModel vm)
            {
                var nodeDto = new NodeDto
                {
                    Id = vm.Id.ToString(),
                    NodeType = vm.GetType().Name,
                    NodeName = vm.NodeName,
                    X = vm.X,
                    Y = vm.Y,
                    Properties = vm.Properties.Select(p => new NodePropertyDto
                    {
                        PropertyName = p.PropertyName,
                        IsVisableOnGraphNode = p.IsVisableOnGraphNode,
                        Value = p.PropertyValue
                    }).ToList()
                };

                if (vm.NodeTheme is LinearGradientBrush brush)
                {
                    nodeDto.ThemeColor1 = brush.GradientStops[0].Color.ToString();
                    nodeDto.ThemeColor2 = brush.GradientStops[1].Color.ToString();
                }
                dto.Nodes.Add(nodeDto);
            }
        }
    }

    private async Task LoadAsync()
    {
        var topLevel = TopLevel.GetTopLevel(NodeCanvasBase);
        if (topLevel == null) return;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Load Munyn Graph",
            AllowMultiple = false,
            FileTypeFilter = new[] { new FilePickerFileType("JSON") { Patterns = new[] { "*.json" } } }
        });

        if (files.Count > 0)
        {
            var file = files[0];
            await using var stream = await file.OpenReadAsync();
            using var reader = new StreamReader(stream);
            var json = await reader.ReadToEndAsync();

            var saveData = JsonConvert.DeserializeObject<ContextDto>(json);

            rootContext = new ContextBase();
            var nodeMap = new Dictionary<string, NodeBaseViewModel>();
            var allPaths = new List<PathDto>();
            BuildContextFromDto(saveData, rootContext, nodeMap, allPaths);

            EnterContext(rootContext);
            RefreshContext();

            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                foreach (var pathDto in allPaths)
                {
                    if (nodeMap.TryGetValue(pathDto.StartNodeId, out var startNode) && nodeMap.TryGetValue(pathDto.EndNodeId, out var endNode))
                    {
                        var path = new PathBaseViewModel(startNode, new Avalonia.Point(endNode.X, endNode.Y), this)
                        {
                            EndNode = endNode
                        };
                        path._mainVm = this;
                        startNode.connectedPaths.Add(path);
                        endNode.connectedPaths.Add(path);
                        // Find the context that the path belongs to and add it
                        var pathContext = FindContextForNode(rootContext, startNode);
                        pathContext?.contextNodes.Add(path);
                        path.RecalculatePathData();
                    }
                }
            }, Avalonia.Threading.DispatcherPriority.Background);
        }
    }

    private ContextBase? FindContextForNode(ContextBase context, NodeBaseViewModel node)
    {
        if (context.contextNodes.Contains(node))
        {
            return context;
        }

        foreach (var childContext in context.contextNodes.OfType<ContextBase>())
        {
            var foundContext = FindContextForNode(childContext, node);
            if (foundContext != null)
            {
                return foundContext;
            }
        }

        return null;
    }

    private void BuildContextFromDto(ContextDto dto, ContextBase context, Dictionary<string, NodeBaseViewModel> nodeMap, List<PathDto> allPaths)
    {
        context.Id = Guid.Parse(dto.Id);
        context.contextName = dto.NodeName;
        context.NodeName = dto.NodeName;
        context.X = dto.X;
        context.Y = dto.Y;
        context._mainVM = this;

        foreach (var propDto in dto.Properties)
        {
            var existingProp = context.GetNodePropertyFromName(propDto.PropertyName);
            if (existingProp != null)
            {
                existingProp.PropertyValue = propDto.Value;
            }
            else
            {
                var newProp = new NodePropertyBasic(propDto.PropertyName, propDto.IsVisableOnGraphNode);
                newProp.PropertyValue = propDto.Value;
                context.AddNodeProperty(newProp);
            }
        }

        if (!string.IsNullOrEmpty(dto.ThemeColor1) && !string.IsNullOrEmpty(dto.ThemeColor2))
        {
            context.NodeTheme = context.makeGradient(dto.ThemeColor1, dto.ThemeColor2);
        }

        nodeMap[dto.Id] = context;

        foreach (var nodeDto in dto.Nodes)
        {
            NodeBaseViewModel newNode = null;
            switch (nodeDto.NodeType)
            {
                case nameof(HostNodeViewModel):
                    var hostNode = new HostNodeViewModel
                    {
                        NodeName = nodeDto.NodeName,
                        contextName = nodeDto.NodeName,
                        X = nodeDto.X,
                        Y = nodeDto.Y,
                        parentCanvas = NodeCanvasBase,
                        contextNodes = new ObservableCollection<ViewModelBase>(),
                        parentContext = context,
                        _mainVM = this
                    };
                    hostNode.InitializeProperties();
                    newNode = hostNode;
                    break;
                case nameof(NetworkNodeViewModel):
                    newNode = new NetworkNodeViewModel(nodeDto.NodeName, (float)nodeDto.X, (float)nodeDto.Y, context, NodeCanvasBase, this);
                    break;
                case nameof(UserNodeViewModel):
                    newNode = new UserNodeViewModel(nodeDto.NodeName, (float)nodeDto.X, (float)nodeDto.Y, NodeCanvasBase);
                    break;
                case nameof(ServiceNodeViewModel):
                    newNode = new ServiceNodeViewModel(nodeDto.NodeName, (float)nodeDto.X, (float)nodeDto.Y, NodeCanvasBase);
                    break;
                case nameof(AssetNodeViewModel):
                    newNode = new AssetNodeViewModel(nodeDto.NodeName, (float)nodeDto.X, (float)nodeDto.Y, NodeCanvasBase);
                    break;
            }

            if (newNode != null)
            {
                newNode.Id = Guid.Parse(nodeDto.Id);
                newNode.OnStartConnectionDragNode = OnStartConnectionDragFromNode;
                newNode.OnClickedNode = OnClickedNode;
                newNode._mainVM = this;

                foreach (var propDto in nodeDto.Properties)
                {
                    var existingProp = newNode.GetNodePropertyFromName(propDto.PropertyName);
                    if (existingProp != null)
                    {
                        existingProp.PropertyValue = propDto.Value;
                    }
                    else
                    {
                        var newProp = new NodePropertyBasic(propDto.PropertyName, propDto.IsVisableOnGraphNode);
                        newProp.PropertyValue = propDto.Value;
                        newNode.AddNodeProperty(newProp);
                    }
                }

                if (!string.IsNullOrEmpty(nodeDto.ThemeColor1) && !string.IsNullOrEmpty(nodeDto.ThemeColor2))
                {
                    newNode.NodeTheme = newNode.makeGradient(nodeDto.ThemeColor1, nodeDto.ThemeColor2);
                }

                context.contextNodes.Add(newNode);
                nodeMap[nodeDto.Id] = newNode;
            }
        }

        allPaths.AddRange(dto.Paths);

        foreach (var childContextDto in dto.ChildrenContexts)
        {
            if (childContextDto.NodeType == nameof(HostNodeViewModel))
            {
                var hostNode = new HostNodeViewModel
                {
                    NodeName = childContextDto.NodeName,
                    contextName = childContextDto.NodeName,
                    X = childContextDto.X,
                    Y = childContextDto.Y,
                    parentCanvas = NodeCanvasBase,
                    contextNodes = new ObservableCollection<ViewModelBase>(),
                    parentContext = context,
                    _mainVM = this,
                    OnClickedNode = OnClickedNode,
                    OnStartConnectionDragNode = OnStartConnectionDragFromNode
                };
                hostNode.InitializeProperties();
                BuildContextFromDto(childContextDto, hostNode, nodeMap, allPaths);
                context.contextNodes.Add(hostNode);
            }
            else
            {
                var childContext = new ContextBase { parentContext = context };
                BuildContextFromDto(childContextDto, childContext, nodeMap, allPaths);
                context.contextNodes.Add(childContext);
            }
        }
    }
}
