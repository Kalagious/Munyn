using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Interactivity;
using System;
using Avalonia.Controls.Shapes;

namespace Munyn.Views;

public partial class NodeCanvasView : Canvas
{
    private readonly ScaleTransform _zoom = new();
    private readonly TranslateTransform _pan = new();
    private readonly TransformGroup _transform = new();

    private Point _lastDrag;
    private bool _isDragging;

    public NodeCanvasView()
    {
        InitializeComponent();

        _transform.Children.Add(_zoom);
        _transform.Children.Add(_pan);
        NodeCanvas.RenderTransform = _transform;

        NodeCanvas.PointerWheelChanged += OnPointerWheelChanged;
        NodeCanvas.PointerPressed += OnPointerPressed;
        NodeCanvas.PointerMoved += OnPointerMoved;
        NodeCanvas.PointerReleased += OnPointerReleased;

        
        CanvasHost.GetObservable(BoundsProperty).Subscribe(bounds => ClampPan());


        AddGridLines(50);


    }

    private void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        const double zoomFactor = 1.1;
        var scale = e.Delta.Y > 0 ? zoomFactor : 1 / zoomFactor;

        var mouse = e.GetPosition(NodeCanvas);
        var worldX = (mouse.X - _pan.X) / _zoom.ScaleX;
        var worldY = (mouse.Y - _pan.Y) / _zoom.ScaleY;

        _zoom.ScaleX *= scale;
        _zoom.ScaleY *= scale;

        _pan.X = mouse.X - worldX * _zoom.ScaleX;
        _pan.Y = mouse.Y - worldY * _zoom.ScaleY;

        ClampZoom();
        ClampPan();
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            _isDragging = true;
            _lastDrag = e.GetPosition(this);
            NodeCanvas.Cursor = new Cursor(StandardCursorType.SizeAll);
        }
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_isDragging)
        {
            var current = e.GetPosition(this);
            var delta = current - _lastDrag;
            _pan.X += delta.X;
            _pan.Y += delta.Y;
            _lastDrag = current;

            ClampPan();
        }
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _isDragging = false;
        NodeCanvas.Cursor = new Cursor(StandardCursorType.Arrow);
    }

    private void ClampZoom()
    {
        const double minZoom = 0.1, maxZoom = 4.0;
        _zoom.ScaleX = Math.Clamp(_zoom.ScaleX, minZoom, maxZoom);
        _zoom.ScaleY = _zoom.ScaleX;
    }

    private void ClampPan()
    {
        if (NodeCanvas.Bounds.Width == 0 || CanvasHost.Bounds.Width == 0)
            return;

        double canvasW = NodeCanvas.Bounds.Width * _zoom.ScaleX;
        double canvasH = NodeCanvas.Bounds.Height * _zoom.ScaleY;
        double hostW = CanvasHost.Bounds.Width;
        double hostH = CanvasHost.Bounds.Height;

        double minX = Math.Min(0, hostW - canvasW);
        double minY = Math.Min(0, hostH - canvasH);

        _pan.X = Math.Clamp(_pan.X, minX, 0);
        _pan.Y = Math.Clamp(_pan.Y, minY, 0);

        AddGridLines(50);

    }


    public void AddGridLines(double spacing = 50)
    {
        var width = NodeCanvas.Bounds.Width;
        var height = NodeCanvas.Bounds.Height;


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
            NodeCanvas.Children.Add(line);
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
            NodeCanvas.Children.Add(line);
        }
    }
}