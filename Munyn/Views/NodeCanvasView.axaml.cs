using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Interactivity;
using System;
using Avalonia.Controls.Shapes;
using Munyn.ViewModels;

namespace Munyn.Views;

public partial class NodeCanvasBaseView : Canvas
{
    private readonly ScaleTransform _zoom = new();
    private readonly TranslateTransform _pan = new();
    private readonly TransformGroup _transform = new();

    private Point _lastDrag;
    private bool _isDragging;
    public Action<PointerEventArgs> MainViewModelHandleMouseMoved { get; internal set; }
    public Action<PointerReleasedEventArgs> MainViewModelHandleMouseReleased { get; internal set; }


    public NodeCanvasBaseView()
    {
        InitializeComponent();

        NodeCanvasBase.PointerMoved += OnPointerMoved;
        NodeCanvasBase.PointerReleased += OnPointerReleased;
        
        this.AttachedToVisualTree += OnAttachedToVisualTree;
    }


    private void OnAttachedToVisualTree(object? sender, Avalonia.VisualTreeAttachmentEventArgs e)
    {
        // Get a reference to the MainWindowViewModel from the DataContext
        if (this.DataContext is MainViewModel mainViewModel)
        {
            MainViewModelHandleMouseMoved = mainViewModel.HandlePointerMoved;
            MainViewModelHandleMouseReleased = mainViewModel.OnEndConnectionDragFromNode;

            mainViewModel.NodeCanvasBase = NodeCanvasBase;
        }

        // Attach to LayoutUpdated to wait for the layout pass
        NodeCanvasBase.LayoutUpdated += OnLayoutUpdated;
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        MainViewModelHandleMouseMoved(e);
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        MainViewModelHandleMouseReleased(e);
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