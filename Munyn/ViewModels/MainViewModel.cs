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

    public bool isPannable;
    public bool isSelecting;

    public ContextBase? rootContext;
    public ContextBase? currentContext;

    public Canvas? NodeCanvasBase;

    [ObservableProperty]
    private Avalonia.Size _viewportSize;

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
    private MainViewModel _mainViewModelClone;

    [ObservableProperty]
    private string loadedFileName = "Untitled.Mun";

    [ObservableProperty]
    private bool _isRootContext = true;

    [ObservableProperty]
    private string _currentContextName = "Null";

    [ObservableProperty]
    private NodeBaseViewModel _selectedNode;

    [ObservableProperty]
    private bool _isNodeSelected = false;

    [ObservableProperty]
    private PathBaseViewModel _selectedPath;



    enum TrayStates
    {
        Closed,
        Basic,
        Attack,
        Paths,
        Extras
    }



    [ObservableProperty]
    private int _trayState = 1;

    [RelayCommand]
    private void SetBasicTray()
    {
        if (TrayState == (int)TrayStates.Basic)
        {
            TrayState = (int)TrayStates.Closed;
            return;
        }
        TrayState = (int)TrayStates.Basic;
    }    
    
    [RelayCommand]
    private void SetAttackTray()
    {
        if (TrayState == (int)TrayStates.Attack)
        {
            TrayState = (int)TrayStates.Closed;
            return;
        }
        TrayState = (int)TrayStates.Attack;
    }    
    
    [RelayCommand]
    private void SetExtrasTray()
    {
        if (TrayState == (int)TrayStates.Extras)
        {
            TrayState = (int)TrayStates.Closed;
            return;
        }
        TrayState = (int)TrayStates.Extras;
    }

    [RelayCommand]
    private void BackContext()
    {
        if (currentContext != null && currentContext.parentContext != null)
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

    private Avalonia.Point GetCenterOfViewport()
    {
        if (NodeCanvasBase == null || NodeCanvasBase.RenderTransform == null || NodeCanvasBase.Parent == null) return new Avalonia.Point(100, 100);

        var transform = (NodeCanvasBase.RenderTransform as TransformGroup).Children[1] as TranslateTransform;
        var scale = (NodeCanvasBase.RenderTransform as TransformGroup).Children[0] as ScaleTransform;

        if (transform != null)
        {
            Avalonia.Point temp = new Avalonia.Point((transform.X / -scale.ScaleX + NodeCanvasBase.Width/2) - 200, (transform.Y / -scale.ScaleY + NodeCanvasBase.Height/2) - 200);

            return temp;
        }

        return new Avalonia.Point(100, 100);
    }

    [RelayCommand]
    private void NewLinkDrag()
    {
        if (NodeCanvasBase != null)
        {
            var center = GetCenterOfViewport();
            NodeBaseViewModel newNode = new LinkNodeViewModel("NewLink", (float)center.X, (float)center.Y, currentContext, (NodeBaseViewModel)currentContext.contextNodes[0], NodeCanvasBase, this);
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
            var center = GetCenterOfViewport();
            NodeBaseViewModel newNode = new HostNodeViewModel("New Host", (float)center.X, (float)center.Y, currentContext, NodeCanvasBase, this);
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
            var center = GetCenterOfViewport();
            NodeBaseViewModel newNode = new UserNodeViewModel("New User",  (float)center.X, (float)center.Y, NodeCanvasBase, currentContext);
            newNode.OnStartConnectionDragNode = OnStartConnectionDragFromNode;
            newNode.OnClickedNode = OnClickedNode;
            newNode.mainVM = this;

            currentContext.contextNodes.Add(newNode);
            RefreshContext();
        }
    }

    [RelayCommand]
    private void NewServiceDrag()
    {
        if (NodeCanvasBase != null)
        {
            var center = GetCenterOfViewport();
            NodeBaseViewModel newNode = new ServiceNodeViewModel("New Service", (float)center.X, (float)center.Y, NodeCanvasBase, currentContext);
            newNode.OnStartConnectionDragNode = OnStartConnectionDragFromNode;
            newNode.OnClickedNode = OnClickedNode;
            newNode.mainVM = this;


            currentContext.contextNodes.Add(newNode);
            RefreshContext();
        }

    }

    [RelayCommand]
    private void NewAssetDrag()
    {
        if (NodeCanvasBase != null)
        {
            var center = GetCenterOfViewport();
            NodeBaseViewModel newNode = new AssetNodeViewModel("New Asset", (float)center.X, (float)center.Y, NodeCanvasBase, currentContext);
            newNode.OnStartConnectionDragNode = OnStartConnectionDragFromNode;
            newNode.OnClickedNode = OnClickedNode;
            newNode.mainVM = this;

            currentContext.contextNodes.Add(newNode);
            RefreshContext();
        }

    }

    [RelayCommand]
    private void NewReconDrag()
    {
        if (NodeCanvasBase != null)
        {
            var center = GetCenterOfViewport();
            NodeBaseViewModel newNode = new ReconNodeViewModel("New Recon", (float)center.X, (float)center.Y, NodeCanvasBase, currentContext);
            newNode.OnStartConnectionDragNode = OnStartConnectionDragFromNode;
            newNode.OnClickedNode = OnClickedNode;
            newNode.mainVM = this;

            currentContext.contextNodes.Add(newNode);
            RefreshContext();
        }

    }

    [RelayCommand]
    private void NewAttackDrag()
    {
        if (NodeCanvasBase != null)
        {
            var center = GetCenterOfViewport();
            NodeBaseViewModel newNode = new AttackNodeViewModel("New Attack", (float)center.X, (float)center.Y, NodeCanvasBase, currentContext);
            newNode.OnStartConnectionDragNode = OnStartConnectionDragFromNode;
            newNode.OnClickedNode = OnClickedNode;
            newNode.mainVM = this;

            currentContext.contextNodes.Add(newNode);
            RefreshContext();
        }

    }

    [RelayCommand]
    private void NewCheckpointDrag()
    {
        if (NodeCanvasBase != null)
        {
            var center = GetCenterOfViewport();
            NodeBaseViewModel newNode = new CheckpointNodeViewModel("New Checkpoint", (float)center.X, (float)center.Y, NodeCanvasBase, currentContext);
            newNode.OnStartConnectionDragNode = OnStartConnectionDragFromNode;
            newNode.OnClickedNode = OnClickedNode;
            newNode.mainVM = this;

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

        MainViewModelClone = this;
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
            _currentPath.OnClickedPath = OnClickedPath;
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
        if (SelectedPath != null)
        {
            SelectedPath.SetSelected(false);
            SelectedPath = null;
        }

        if (SelectedNode != null && SelectedNode == node)
        {
            node.SetSelected(false);
            SelectedNode = null;
            IsNodeSelected = false;

            return;
        }
        node.SetSelected(true);

        if (SelectedNode != null)
            SelectedNode.SetSelected(false);

        SelectedNode = node;
        IsNodeSelected = true;
    }

    public void OnClickedPath(PathBaseViewModel path, PointerPressedEventArgs e)
    {
        if (SelectedNode != null)
        {
            SelectedNode.SetSelected(false);
            SelectedNode = null;
            IsNodeSelected = false;
        }

        if (SelectedPath != null && SelectedPath == path)
        {
            path.SetSelected(false);
            SelectedPath = null;
            return;
        }
        path.SetSelected(true);

        if (SelectedPath != null)
            SelectedPath.SetSelected(false);

        SelectedPath = path;
        e.Handled = true;

    }

    [RelayCommand]
    public void DeleteSelectedNode()
    {
        if (SelectedNode != null)
        {
            DeleteNode(SelectedNode);
        }
    }

    [RelayCommand]
    public void DeleteSelectedPath()
    {
        if (SelectedPath != null)
        {
            var context = SelectedPath.StartNode.parentContext;
            if (context != null)
            {
                context.contextNodes.Remove(SelectedPath);
            }
            else
            {
                // Fallback or error handling
                currentContext.contextNodes.Remove(SelectedPath);
            }

            SelectedPath.StartNode.connectedPaths.Remove(SelectedPath);
            if (SelectedPath.EndNode != null)
                SelectedPath.EndNode.connectedPaths.Remove(SelectedPath);
            SelectedPath = null;
        }
    }

    [RelayCommand]
    public void DeleteNode(NodeBaseViewModel node)
    {
        if (node != null)
        {
            var context = node.parentContext;
            if (context == null)
            {
                // Fallback for nodes that might not have a parent context (like root)
                // or for safety.
                context = currentContext;
            }

            // Find all paths connected to the node within its own context
            var pathsToRemove = context.contextNodes
                .OfType<PathBaseViewModel>()
                .Where(p => p.StartNode == node || p.EndNode == node)
                .ToList();

            // Remove the paths from the context
            foreach (var path in pathsToRemove)
            {
                context.contextNodes.Remove(path);
            }

            // Remove the node itself from its context
            context.contextNodes.Remove(node);

            // If the deleted node was the selected node, clear the selection
            if (SelectedNode == node)
            {
                SelectedNode = null;
                IsNodeSelected = false;
            }
        }
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

    private IStorageFile? _lastSavedFile;

    [RelayCommand]
    private async Task SaveToLastFile()
    {
        if (_lastSavedFile is not null)
        {
            var saveData = new ContextDto();
            BuildDtoFromContext(rootContext, saveData);
            string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
            await using var stream = await _lastSavedFile.OpenWriteAsync();
            await using var writer = new StreamWriter(stream);
            await writer.WriteAsync(json);
        }
        else
        {
            await SaveAsAsync();
        }
    }

    [RelayCommand]
    private async void SaveAs()
    {
        await SaveAsAsync();
    }

    [RelayCommand]
    private async void Load()
    {
        await LoadAsync();
    }

    private async Task SaveAsAsync()
    {
        var topLevel = TopLevel.GetTopLevel(NodeCanvasBase);
        if (topLevel == null) return;

        var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save Munyn Graph",
            FileTypeChoices = new[]
            {
                new FilePickerFileType("MUN") { Patterns = new[] { "*.mun" } }
            }
        });

        if (file is not null)
        {
            _lastSavedFile = file;
            var saveData = new ContextDto();
            BuildDtoFromContext(rootContext, saveData);
            LoadedFileName = file.Name;
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
        dto.ContextName = context.contextName;
        dto.NodeType = context.GetType().Name;
        dto.X = context.X;
        dto.Y = context.Y;
        dto.Properties = context.Properties.Select(p => CreateDtoFromProperty(p)).ToList();

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
                    Properties = vm.Properties.Select(p => CreateDtoFromProperty(p)).ToList()
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
            FileTypeFilter = new[] { new FilePickerFileType("MUN") { Patterns = new[] { "*.mun" } } }
        });

        if (files.Count > 0)
        {
            var file = files[0];
            _lastSavedFile = file;
            LoadedFileName = file.Name;
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
                        path.OnClickedPath = OnClickedPath;
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
        if (string.IsNullOrEmpty(dto.NodeName))
            context.contextName = dto.ContextName;
        else
            context.contextName = dto.NodeName;
        context.NodeName = dto.NodeName;
        context.X = dto.X;
        context.Y = dto.Y;
        context.mainVM = this;

        context.Properties.Clear();
        foreach (var propDto in dto.Properties)
        {
            NodePropertyBasic newProp = CreatePropertyFromDto(propDto);
            context.AddNodeProperty(newProp);
        }
        context.GetGraphViewProperties();

        if (!string.IsNullOrEmpty(dto.ThemeColor1) && !string.IsNullOrEmpty(dto.ThemeColor2))
        {
            context.NodeTheme = context.makeTheme(dto.ThemeColor1, dto.ThemeColor2);
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
                        mainVM = this
                    };
                    hostNode.InitializeProperties();
                    newNode = hostNode;
                    break;
                case nameof(NetworkNodeViewModel):
                    newNode = new NetworkNodeViewModel(nodeDto.NodeName, (float)nodeDto.X, (float)nodeDto.Y, context, NodeCanvasBase, this);
                    break;
                case nameof(UserNodeViewModel):
                    newNode = new UserNodeViewModel(nodeDto.NodeName, (float)nodeDto.X, (float)nodeDto.Y, NodeCanvasBase, context);
                    break;
                case nameof(ServiceNodeViewModel):
                    newNode = new ServiceNodeViewModel(nodeDto.NodeName, (float)nodeDto.X, (float)nodeDto.Y, NodeCanvasBase, context);
                    break;
                case nameof(AssetNodeViewModel):
                    newNode = new AssetNodeViewModel(nodeDto.NodeName, (float)nodeDto.X, (float)nodeDto.Y, NodeCanvasBase, context);
                    break;
                case nameof(ReconNodeViewModel):
                    newNode = new ReconNodeViewModel(nodeDto.NodeName, (float)nodeDto.X, (float)nodeDto.Y, NodeCanvasBase, context);
                    break;
                case nameof(AttackNodeViewModel):
                    newNode = new AttackNodeViewModel(nodeDto.NodeName, (float)nodeDto.X, (float)nodeDto.Y, NodeCanvasBase, context);
                    break;
                case nameof(CheckpointNodeViewModel):
                    newNode = new CheckpointNodeViewModel(nodeDto.NodeName, (float)nodeDto.X, (float)nodeDto.Y, NodeCanvasBase, context);
                    break;
            }

            if (newNode != null)
            {
                newNode.Id = Guid.Parse(nodeDto.Id);
                newNode.OnStartConnectionDragNode = OnStartConnectionDragFromNode;
                newNode.OnClickedNode = OnClickedNode;
                newNode.mainVM = this;

                newNode.Properties.Clear();
                foreach (var propDto in nodeDto.Properties)
                {
                    NodePropertyBasic newProp = CreatePropertyFromDto(propDto);
                    newNode.AddNodeProperty(newProp);
                }

                if (!string.IsNullOrEmpty(nodeDto.ThemeColor1) && !string.IsNullOrEmpty(nodeDto.ThemeColor2))
                {
                    newNode.NodeTheme = newNode.makeTheme(nodeDto.ThemeColor1, nodeDto.ThemeColor2);
                }

                newNode.GetGraphViewProperties();

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
                    mainVM = this,
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


    private NodePropertyDto CreateDtoFromProperty(NodePropertyBasic property)
    {
        List<string> colors = new List<string>();
        if (property.IconColor is LinearGradientBrush brush)
        {
            foreach (var gradient in brush.GradientStops)
                colors.Add(gradient.Color.ToString());
        }
        var propDto = new NodePropertyDto
        {
            PropertyName = property.PropertyName,
            PropertyColors = colors,
            PropertyType = property.GetType().Name,
            IsVisableOnGraphNode = property.IsVisableOnGraphNode,
            Value = property.PropertyValue,
            IconName = property.IconName,
        };

        switch (property)
        {
            case NodePropertyText text:
                propDto.TextContent = text.TextContent;
                break;
            case NodePropertyCommand command:
                propDto.Command = command.Command;
                propDto.Description = command.Description;
                break;
            case NodePropertyInterface intrface:
                propDto.IP = intrface.Ip;
                propDto.MAC = intrface.Mac;
                break;
            case NodePropertyMultiInterface multiInterface:
                propDto.Interfaces = multiInterface.Interfaces;
                break;
            case NodePropertyLink link:
                propDto.Url = link.Url;
                propDto.DisplayText = link.DisplayText;
                break;
            case NodePropertyVulnerability vulnerability:
                propDto.Score = vulnerability.Score;
                propDto.Location = vulnerability.Location;
                propDto.Resource = vulnerability.Resource;
                propDto.Description = vulnerability.Description;
                break;
            case NodePropertyCompromised compromised:
                propDto.CompromiseLevel = compromised.CompromiseLevel;
                break;
        }

        return propDto;
    }


    private NodePropertyBasic CreatePropertyFromDto(NodePropertyDto dto)
    {
        NodePropertyBasic newProp = null;

        // Guess property type based on available fields in DTO
        if (dto.PropertyType == "NodePropertyMultiInterface")
        {
            newProp = new NodePropertyMultiInterface { Interfaces = dto.Interfaces };
        }
        else if (dto.PropertyType == "NodePropertyInterface")
        {
            newProp = new NodePropertyInterface { Ip = dto.IP, Mac = dto.MAC };
        }
        else if (dto.PropertyType == "NodePropertyCommand")
        {
            newProp = new NodePropertyCommand { Command = dto.Command, Description = dto.Description };
        }
        else if (dto.PropertyType == "NodePropertyLink")
        {
            newProp = new NodePropertyLink { Url = dto.Url, DisplayText = dto.DisplayText };
        }
        else if (dto.PropertyType == "NodePropertyVulnerability")
        {
            newProp = new NodePropertyVulnerability { Score = dto.Score.Value, Location = dto.Location, Resource = dto.Resource, Description = dto.Description };
        }
        else if (dto.PropertyType == "NodePropertyCompromised")
        {
            newProp = new NodePropertyCompromised { CompromiseLevel = dto.CompromiseLevel };
        }
        else if (dto.PropertyType == "NodePropertyText")
        {
            newProp = new NodePropertyText { TextContent = dto.TextContent };
        }
        else
        {
            newProp = new NodePropertyBasic();
        }

        newProp.PropertyName = dto.PropertyName;
        newProp.IsVisableOnGraphNode = dto.IsVisableOnGraphNode;
        newProp.PropertyValue = dto.Value;
        newProp.IconName = dto.IconName;
        newProp.Refresh();

        if (dto.PropertyColors.Count > 1)
        {
            LinearGradientBrush propertyColor = new LinearGradientBrush();
            propertyColor.StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative);
            propertyColor.EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative);

            double gradientStopOffset = 0.0;
            foreach (var color in dto.PropertyColors)
            {
                if (Avalonia.Media.Color.TryParse(color, out var parsedColor))
                {
                    propertyColor.GradientStops.Add(new GradientStop(parsedColor, gradientStopOffset));
                    gradientStopOffset += 1.0 / ((double)dto.PropertyColors.Count-1.0);
                }
            }
            newProp.IconColor = propertyColor;
        }



        return newProp;
    }
}
