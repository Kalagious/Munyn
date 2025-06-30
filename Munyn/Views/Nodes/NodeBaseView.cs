using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Avalonia.Markup.Xaml;
using Munyn.ViewModels;
using System;


namespace Munyn.Views.Nodes;

public partial class NodeBaseView : UserControl
{
    private bool _isDragging = false;
    private Point _startDragPoint;

    private Canvas? _rootDrawingCanvas;

    public void Node_PointerPressed(object sender, PointerPressedEventArgs e)
    {
        // Only start dragging with the left mouse button
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            _isDragging = true;
            // Capture the mouse so we continue receiving events even if the pointer moves outside the control
            e.Pointer.Capture(this);
            // Store the starting position of the mouse relative to the HostNode
            _startDragPoint = e.GetPosition(this);
            e.Handled = true; // Mark as handled to prevent parent controls from also handling
        }
    }

    public void Node_PointerMoved(object sender, PointerEventArgs e)
    {
        if (_isDragging && DataContext is NodeBaseViewModel viewModel)
        {
            // Get the current position of the mouse relative to the Canvas
            // Find the parent Canvas

            if (_rootDrawingCanvas == null)
            {
                _rootDrawingCanvas = viewModel.parentCanvas;
                if (_rootDrawingCanvas == null) return;
            }

            Point currentCanvasPosition = e.GetPosition(_rootDrawingCanvas);

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

            e.Handled = true; // Mark as handled
        }
    }

    public void Node_PointerReleased(object sender, PointerReleasedEventArgs e)
    {
        if (_isDragging)
        {
            _isDragging = false;
            e.Pointer.Capture(null); // Release mouse capture
            e.Handled = true; // Mark as handled
        }
    }
}

