using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Munyn.ViewModels;
using System;



namespace Munyn.Views.Nodes;

public partial class NodeView : UserControl
{
    private bool _isDragging = false;
    private Point _startDragPoint;
    private const double DragThreshold = 5.0;

    private Canvas? _rootDrawingCanvas;


    public NodeView()
    {
        InitializeComponent();
        this.LayoutUpdated += NodeView_LayoutUpdated;
    }

    private void NodeView_LayoutUpdated(object? sender, EventArgs e)
    {
        if (DataContext is NodeBaseViewModel viewModel)
        {
            foreach (PathBaseViewModel path in viewModel.connectedPaths)
                path.RecalculatePathData();
        }
    }
    public void Node_PointerPressed(object sender, PointerPressedEventArgs e)
    {
        // Only start dragging with the left mouse button
        if (sender.GetType() == typeof(Munyn.Views.Nodes.NodeDetails.NodeDetailsView))
        {
            e.Handled = true;
            return;
        }
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            _startDragPoint = e.GetPosition(this);

            if (DataContext is NodeBaseViewModel nodeVm) // Cast DataContext to its ViewModel
            {
                var topLevel = TopLevel.GetTopLevel(this);
                if (topLevel?.DataContext is MainViewModel mainViewModelFromTopLevel)
                    if (mainViewModelFromTopLevel.PathTool)
                    {
                        var portElement = sender as Control;
                        if (portElement != null && nodeVm.parentCanvas != null)
                        {
                            // Calculate port position in canvas coordinates
                            Point portCenterInControl = new Point(portElement.Bounds.Width / 2, portElement.Bounds.Height / 2);
                            Point portCenterInNode = portElement.TranslatePoint(portCenterInControl, this) ?? portCenterInControl;
                            Point portCenterInCanvas = this.TranslatePoint(portCenterInNode, nodeVm.parentCanvas) ?? portCenterInNode;

                            // INVOKE THE ACTION on the Node's ViewModel, which the MainViewModel has subscribed to!
                            nodeVm.OnStartConnectionDragNode?.Invoke(nodeVm, portCenterInCanvas, e);
                        }
                    }
                    else
                    {
                        e.Pointer.Capture(this);
                        e.Handled = true;
                    }
            }
        }
    }

    public void Node_PointerMoved(object sender, PointerEventArgs e)
    {

        if (sender.GetType() == typeof(Munyn.Views.Nodes.NodeDetails.NodeDetailsView))
        {
            e.Handled = true;
            return;
        }

        if (DataContext is not NodeBaseViewModel viewModel) return;

        if (_rootDrawingCanvas == null)
        {
            _rootDrawingCanvas = viewModel.parentCanvas;
            if (_rootDrawingCanvas == null)
            {
                // Cannot perform drag operations without the root canvas
                return;
            }
        }

        Point currentPointerPosOnCanvas = e.GetPosition(_rootDrawingCanvas);

        if (!_isDragging && e.Pointer.Captured == this)
        {
            // _startDragPoint is the offset within the node where the pointer was pressed.
            // viewModel.X and viewModel.Y is the node's current top-left position on the canvas.
            Point initialPointerPosOnCanvasAtPress = new Point(viewModel.X + _startDragPoint.X, viewModel.Y + _startDragPoint.Y);

            // Calculate distance moved on the canvas
            Point delta = currentPointerPosOnCanvas - initialPointerPosOnCanvasAtPress;
            double distance = Math.Sqrt(delta.X * delta.X + delta.Y * delta.Y);
            if (distance > DragThreshold)
                _isDragging = true;

        }

        if (_isDragging && e.Pointer.Captured == this) // Ensure pointer is still captured by this control
        {
            // currentPointerPosOnCanvas is already up-to-date and relative to _rootDrawingCanvas.
            // _startDragPoint is the click offset within the node.
            double newX = currentPointerPosOnCanvas.X - _startDragPoint.X;
            double newY = currentPointerPosOnCanvas.Y - _startDragPoint.Y;

            // Clamping: Ensure node stays within canvas bounds
            newX = Math.Max(0, newX);
            newY = Math.Max(0, newY);

            // Use this.Bounds for the node's own dimensions.
            // Ensure Bounds are valid before using them in Min function.
            double nodeWidth = this.Bounds.Width;
            double nodeHeight = this.Bounds.Height;

            if (nodeWidth > 0)
            {
                newX = Math.Min(_rootDrawingCanvas.Bounds.Width - nodeWidth, newX);
            }
            else // Fallback if nodeWidth is not positive (e.g. not laid out yet)
            {
                newX = Math.Min(_rootDrawingCanvas.Bounds.Width, newX);
            }

            if (nodeHeight > 0)
            {
                newY = Math.Min(_rootDrawingCanvas.Bounds.Height - nodeHeight, newY);
            }
            else // Fallback if nodeHeight is not positive
            {
                newY = Math.Min(_rootDrawingCanvas.Bounds.Height, newY);
            }

            viewModel.X = newX;
            viewModel.Y = newY;
            //System.Diagnostics.Debug.WriteLine($"PointerMoved: newX={newX}, newY={newY}");

            foreach (PathBaseViewModel path in viewModel.connectedPaths)
                path.RecalculatePathData();

            e.Handled = true; // Mark as handled
        }
    }

    public void Node_PointerReleased(object sender, PointerReleasedEventArgs e)
    {
        if (DataContext is NodeBaseViewModel viewModel)
            if (viewModel.mainVM.isPannable)
            {
                viewModel.mainVM.isPannable = false;
                e.Handled = true;
                return;
            }

        if (sender.GetType() == typeof(Munyn.Views.Nodes.NodeDetails.NodeDetailsView))
        {
            e.Handled = true;
            return;
        }

        if (!_isDragging)
            if (DataContext is NodeBaseViewModel nodeVm) // Cast DataContext to its ViewModel
                nodeVm.OnClickedNode?.Invoke(nodeVm, e);


        _isDragging = false;
        e.Pointer.Capture(null); // Release mouse capture
        e.Handled = true; // Mark as handled
    }
}

