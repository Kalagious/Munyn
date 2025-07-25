using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using Munyn.ViewModels;
using System;
using System.Reactive.Linq;

namespace Munyn.Views;

public partial class MainView : UserControl
{
    private Canvas? _NodeCanvasBase;

    private bool isPannable;
    private Point panStart;
    private Size viewportSize;
    private StackPanel categoriesPanel;
    private Grid topPanel;

    public MainView()
    {
        InitializeComponent();

        categoriesPanel = this.GetControl<StackPanel>("NodeCategories");
        topPanel = this.GetControl<Grid>("TopBar");
        this.Loaded += OnLoaded;
        this.PointerMoved += MainView_OnPointerMoved;
        this.PointerReleased += MainView_OnPointerReleased;
        this.PointerPressed += OnPointerPressed;
        this.SizeChanged += OnSizeChanged;
    }

    private void OnSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        viewportSize = e.NewSize;
        DrawGridLines();
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

                mainVm.NodeCanvasBase = (Canvas)presenter.Panel; 
                _NodeCanvasBase = mainVm.NodeCanvasBase;
                DrawGridLines();

        
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Error: Canvas '_NodeCanvasBase' not found.");
                return;
            }
        }
    }

    private void DrawGridLines()
    {
        if (_NodeCanvasBase == null) return;

        // Remove existing grid lines
        for (int i = _NodeCanvasBase.Children.Count - 1; i >= 0; i--)
        {
            if (_NodeCanvasBase.Children[i] is Line)
            {
                _NodeCanvasBase.Children.RemoveAt(i);
            }
        }

        double gridSpacing = 20;
        var strokeColor = Color.Parse("#2f2c32");
        double strokeThickness = 1;

        for (double x = 0; x < _NodeCanvasBase.Bounds.Width; x += gridSpacing)
        {
            var line = new Line
            {
                StartPoint = new Point(x, 0),
                EndPoint = new Point(x, _NodeCanvasBase.Bounds.Height),
                Stroke = new SolidColorBrush(strokeColor),
                StrokeThickness = strokeThickness
            };
            _NodeCanvasBase.Children.Insert(0, line);
        }

        for (double y = 0; y < _NodeCanvasBase.Bounds.Height; y += gridSpacing)
        {
            var line = new Line
            {
                StartPoint = new Point(0, y),
                EndPoint = new Point(_NodeCanvasBase.Bounds.Width, y),
                Stroke = new SolidColorBrush(strokeColor),
                StrokeThickness = strokeThickness
            };
            _NodeCanvasBase.Children.Insert(0, line);
        }
    }
    
    private void MainView_OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (DataContext is MainViewModel mainVm)
        {
            if (isPannable)
            {
                var currentPosition = e.GetPosition(this);
                var delta = currentPosition - panStart;
                panStart = currentPosition;

                var transform = (TranslateTransform)_NodeCanvasBase.RenderTransform;
                var newX = transform.X + delta.X;
                var newY = transform.Y + delta.Y;

                // Constrain panning

                if (categoriesPanel != null)
                {
                    newX = Math.Min(newX, (_NodeCanvasBase.Width - viewportSize.Width + categoriesPanel.Bounds.Width) / 2);
                    newX = Math.Max(newX, (-_NodeCanvasBase.Width + viewportSize.Width - categoriesPanel.Bounds.Width) / 2);

                }

                if (topPanel != null)
                {
                    newY = Math.Min(newY, (_NodeCanvasBase.Height - viewportSize.Height + topPanel.Bounds.Height) / 2);
                    newY = Math.Max(newY, (-_NodeCanvasBase.Height + viewportSize.Height - topPanel.Bounds.Height) / 2);
                }
                transform.X = newX;
                transform.Y = newY;


                return;
            }
            mainVm.HandlePointerMoved(e);
        }
        e.Handled = true;
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsRightButtonPressed)
        {
            isPannable = true;
            panStart = e.GetPosition(this);
        }
    }

    private void MainView_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (isPannable)
        {
            isPannable = false;
            return;
        }

        if (DataContext is MainViewModel mainVm)
            mainVm.OnEndConnectionDragFromNode(e);

        e.Handled = true;
    }
}


