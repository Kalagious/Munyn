using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Interactivity;
using System;
using Avalonia.Controls.Shapes;
using Munyn.ViewModels;

namespace Munyn.Views;

public partial class NodeCanvasBaseView : Canvas // Note: XAML root is UserControl, but this class is Canvas. Consider aligning.
{
    private readonly ScaleTransform _zoom = new(1, 1);
    private readonly TranslateTransform _pan = new(0, 0);
    private readonly TransformGroup _transform = new();

    // For Panning
    private bool _isPanning;
    private Point _panLastPoint;

    // For Zooming
    private const double ZoomSpeed = 1.1;
    private readonly Grid _canvasHost; // Reference to the clipping Grid
    private readonly Canvas _nodeCanvasBase;

    public Action<PointerEventArgs> MainViewModelHandleMouseMoved { get; internal set; }
    public Action<PointerReleasedEventArgs> MainViewModelHandleMouseReleased { get; internal set; }


    public NodeCanvasBaseView()
    {
        InitializeComponent();

        _transform.Children.Add(_pan);
        _transform.Children.Add(_zoom);
        NodeCanvasBase.RenderTransform = _transform;

        // Get reference to CanvasHost
        _canvasHost = this.FindControl<Grid>("CanvasHost") ?? throw new Exception("CanvasHost not found");
        _nodeCanvasBase = this.FindControl<Canvas>("NodeCanvasBase") ?? throw new Exception("NodeCanvasBase not found");


        _nodeCanvasBase.PointerWheelChanged += NodeCanvasBase_PointerWheelChanged;
        _nodeCanvasBase.PointerPressed += NodeCanvasBase_PointerPressed;
        _nodeCanvasBase.PointerMoved += OnPointerMoved; // Existing handler, will be modified
        _nodeCanvasBase.PointerReleased += OnPointerReleased; // Existing handler, will be modified
        
        this.AttachedToVisualTree += OnAttachedToVisualTree;
    }


    private void OnAttachedToVisualTree(object? sender, Avalonia.VisualTreeAttachmentEventArgs e)
    {
        if (this.DataContext is MainViewModel mainViewModel)
        {
            MainViewModelHandleMouseMoved = mainViewModel.HandlePointerMoved;
            MainViewModelHandleMouseReleased = mainViewModel.OnEndConnectionDragFromNode;
            mainViewModel.NodeCanvasBase = _nodeCanvasBase;
        }
        NodeCanvasBase.LayoutUpdated += OnLayoutUpdated;
    }

    private void NodeCanvasBase_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsRightButtonPressed)
        {
            if (_canvasHost == null) return;

            _isPanning = true;
            _panLastPoint = e.GetPosition(_canvasHost); // Use position relative to CanvasHost for panning
            e.Pointer.Capture(_canvasHost);
            e.Handled = true;
        }
    }

    private void NodeCanvasBase_PointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        if (_canvasHost == null || NodeCanvasBase == null) return;

        var point = e.GetPosition(NodeCanvasBase); // Mouse position relative to the NodeCanvasBase
        var pointToHost = e.GetPosition(_canvasHost); // Mouse position relative to the CanvasHost (viewport)

        double scale = e.Delta.Y > 0 ? ZoomSpeed : 1 / ZoomSpeed;

        double oldScaleX = _zoom.ScaleX;
        double oldScaleY = _zoom.ScaleY;

        _zoom.ScaleX *= scale;
        _zoom.ScaleY *= scale;

        // Adjust pan to keep the point under the mouse stationary relative to the viewport
        // The pan adjustment is the difference in the mouse position's mapping to canvas coordinates
        // before and after the zoom, scaled by the new zoom level.
        _pan.X = pointToHost.X - (point.X * _zoom.ScaleX);
        _pan.Y = pointToHost.Y - (point.Y * _zoom.ScaleY);

        ConstrainView(); // To be implemented in Step 4
        e.Handled = true;
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_isPanning && e.Pointer.Captured == _canvasHost)
        {
            if (_canvasHost == null) return;

            var currentPoint = e.GetPosition(_canvasHost);
            var delta = currentPoint - _panLastPoint;
            _panLastPoint = currentPoint;

            _pan.X += delta.X;
            _pan.Y += delta.Y;

            ConstrainView(); // To be implemented in Step 4
            e.Handled = true;
        }
        else
        {
            MainViewModelHandleMouseMoved?.Invoke(e);
        }
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_isPanning && e.Pointer.Captured == _canvasHost)
        {
            _isPanning = false;
            e.Pointer.Capture(null);
            e.Handled = true;
        }
        else
        {
            MainViewModelHandleMouseReleased?.Invoke(e);
        }
    }

    private void ConstrainView()
    {
        if (NodeCanvasBase == null || _canvasHost == null || _canvasHost.Bounds.Width <= 0 || _canvasHost.Bounds.Height <= 0)
        {
            return; // Not ready to constrain yet
        }

        var canvasHostBounds = _canvasHost.Bounds;
        // NodeCanvasBase.Bounds is the original, untransformed size (e.g., 3000x3000)
        var nodeCanvasOriginalBounds = NodeCanvasBase.Bounds;

        if (nodeCanvasOriginalBounds.Width <= 0 || nodeCanvasOriginalBounds.Height <= 0)
        {
            return; // Original canvas size is not valid
        }

        // 1. Zoom Constraints: Prevent zooming out too much
        double minScaleX = canvasHostBounds.Width / nodeCanvasOriginalBounds.Width;
        if (_zoom.ScaleX < minScaleX)
        {
            _zoom.ScaleX = minScaleX;
        }

        double minScaleY = canvasHostBounds.Height / nodeCanvasOriginalBounds.Height;
        if (_zoom.ScaleY < minScaleY)
        {
            _zoom.ScaleY = minScaleY;
        }

        // Optional: Max zoom constraint (e.g., _zoom.ScaleX > 2.0) can be added here if needed.

        // Calculate effective width and height of the NodeCanvasBase after zoom
        double effectiveWidth = nodeCanvasOriginalBounds.Width * _zoom.ScaleX;
        double effectiveHeight = nodeCanvasOriginalBounds.Height * _zoom.ScaleY;

        // 2. Pan Constraints
        // Left edge: _pan.X cannot be positive (content shifted right, showing background on the left)
        if (_pan.X > 0)
        {
            _pan.X = 0;
        }

        // Top edge: _pan.Y cannot be positive (content shifted down, showing background on the top)
        if (_pan.Y > 0)
        {
            _pan.Y = 0;
        }

        // Right edge: The right side of the canvas (_pan.X + effectiveWidth) should not be less than the host width.
        // This means _pan.X should not be less than canvasHostBounds.Width - effectiveWidth.
        double minPanX = canvasHostBounds.Width - effectiveWidth;
        if (effectiveWidth <= canvasHostBounds.Width) // If canvas is narrower than or same width as host
        {
            _pan.X = 0; // Center it or align left
        }
        else if (_pan.X < minPanX)
        {
            _pan.X = minPanX;
        }

        // Bottom edge: The bottom side of the canvas (_pan.Y + effectiveHeight) should not be less than the host height.
        // This means _pan.Y should not be less than canvasHostBounds.Height - effectiveHeight.
        double minPanY = canvasHostBounds.Height - effectiveHeight;
        if (effectiveHeight <= canvasHostBounds.Height) // If canvas is shorter than or same height as host
        {
            _pan.Y = 0; // Center it or align top
        }
        else if (_pan.Y < minPanY)
        {
            _pan.Y = minPanY;
        }
    }




    public void AddGridLines(double spacing = 50)
    {
        var width = NodeCanvasBase.Bounds.Width;
        var height = NodeCanvasBase.Bounds.Height;


        SolidColorBrush lineColor = new SolidColorBrush(Color.Parse("#202020"));
        for (double x = 0; x <= width; x += spacing)
        {
            var line = new Line
            {
                StartPoint = new Point(x, 0),
                EndPoint = new Point(x, height),
                Stroke = lineColor,
                StrokeThickness = 1,
                ZIndex = -10
            };
            NodeCanvasBase.Children.Add(line);
        }

        for (double y = 0; y <= height; y += spacing)
        {
            var line = new Line
            {
                StartPoint = new Point(0, y),
                EndPoint = new Point(width, y),
                Stroke = lineColor,
                StrokeThickness = 1,
                ZIndex = -10
            };
            NodeCanvasBase.Children.Add(line);
        }
    }
    private void OnLayoutUpdated(object? sender, EventArgs e)
    {
        if (NodeCanvasBase.Bounds.Width > 0 && NodeCanvasBase.Bounds.Height > 0)
        {
            NodeCanvasBase.LayoutUpdated -= OnLayoutUpdated; // Unsubscribe to avoid repeated calls
            AddGridLines();
        }
    }

}