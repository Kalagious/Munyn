using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Avalonia.Markup.Xaml;
using Munyn.ViewModels;
using System;
using DynamicData;
using Splat;
using System.Collections.Generic;
using System.Diagnostics;


namespace Munyn.Views.Nodes;

public partial class NodeBaseView : UserControl
{
    private bool _isDragging = false;
    private Point _startDragPoint;
    private const double DragThreshold = 5.0;

    private Canvas? _rootDrawingCanvas;

    public void Node_PointerPressed(object sender, PointerPressedEventArgs e)
    {
        // Only start dragging with the left mouse button
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
        Point currentCanvasPosition = e.GetPosition(_rootDrawingCanvas);


        if (!_isDragging && e.Pointer.Captured == (this))
        {
            double deltaX = currentCanvasPosition.X - _startDragPoint.X;
            double deltaY = currentCanvasPosition.Y - _startDragPoint.Y;
            double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            if (distance > DragThreshold)
                _isDragging = true;

        }

        if (_isDragging && DataContext is NodeBaseViewModel viewModel)
        {
            // Get the current position of the mouse relative to the Canvas
            // Find the parent Canvas

            if (_rootDrawingCanvas == null)
            {
                _rootDrawingCanvas = viewModel.parentCanvas;
                if (_rootDrawingCanvas == null) return;
            }


            // Calculate the new X and Y based on the mouse movement and initial click offset
            // Current mouse position on canvas - initial click offset relative to node
            double newX = currentCanvasPosition.X - _startDragPoint.X;
            double newY = currentCanvasPosition.Y - _startDragPoint.Y;

            // Optional: Clamp values to stay within canvas bounds (adjust as needed)
            newX = Math.Max(0, newX);
            newY = Math.Max(0, newY);
            newX = Math.Min(_rootDrawingCanvas.Bounds.Width - Bounds.Width, newX);
            newY = Math.Min(_rootDrawingCanvas.Bounds.Height - Bounds.Height, newY);


            // Update the ViewModel properties
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
        if (!_isDragging)
            if (DataContext is NodeBaseViewModel nodeVm) // Cast DataContext to its ViewModel
                nodeVm.OnClickedNode?.Invoke(nodeVm, e);


        _isDragging = false;
        e.Pointer.Capture(null); // Release mouse capture

        e.Handled = true; // Mark as handled
    }
}

