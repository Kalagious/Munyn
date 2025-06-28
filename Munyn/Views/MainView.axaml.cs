using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Shapes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using Munyn.ViewModels;
using System;
using System.Reactive.Linq;

namespace Munyn.Views;

public partial class MainView : UserControl
{
    private Canvas? _nodeCanvas;
    public MainView()
    {
        InitializeComponent();
        this.Loaded += OnLoaded;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void OnLoaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var itemsControl = this.FindControl<ItemsControl>("HostNodeItemsControl");

        if (itemsControl == null)
        {
            System.Diagnostics.Debug.WriteLine("Error: ItemsControl 'NodeItemsControl' not found.");
            return;
        }


        if (itemsControl.Presenter is ItemsPresenter presenter)
        {

            if (presenter.Panel != null && DataContext is MainViewModel mainVm)
            {

                mainVm.nodeCanvas = (Canvas)presenter.Panel; ;
                _nodeCanvas = mainVm.nodeCanvas;
        
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Error: Canvas 'NodeCanvas' not found.");
                return;
            }
            

        }
        if (_nodeCanvas != null)
        {
            DrawCanvasExtras();
            _nodeCanvas.GetObservable(BoundsProperty)
        .Select(bounds => bounds.Size)
        .DistinctUntilChanged()
        .Throttle(TimeSpan.FromMilliseconds(100))
        .Subscribe(size =>
        {
            Dispatcher.UIThread.Post(() =>
            {
                DrawCanvasExtras();
            }); 
        });
        }

    }

    private void DrawCanvasExtras()
    {
        if (_nodeCanvas == null)
            return;

        // Clear old lines

        // Draw new gridlines
        AddGridLines(_nodeCanvas, spacing: 50, width: _nodeCanvas.Bounds.Width, height: _nodeCanvas.Bounds.Height);
    }
    public void AddGridLines(Canvas canvas, double spacing = 50, double width = 800, double height = 600)
    {

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
            canvas.Children.Add(line);
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
            canvas.Children.Add(line);
        }
    }
}
    