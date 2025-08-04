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


    Rectangle selectionBox = new Rectangle()
    {
        Stroke = new SolidColorBrush(Color.Parse("#909090A0")),
        StrokeThickness = 1.5,
        Fill = new SolidColorBrush(Color.Parse("#90303040")),
        ZIndex = 10
    };


    private Point[] selectionPoints = new Point[2];
    private Point panStart;
    private Size viewportSize;
    private StackPanel categoriesPanel;
    private Grid topPanel;

    private TransformGroup transformGroup;
    private ScaleTransform scaleTransform;
    private TranslateTransform translateTransform;

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
        this.PointerWheelChanged += MainView_PointerWheelChanged;
        this.KeyDown += MainView_OnKeyDown;
    }

    private void OnSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        viewportSize = e.NewSize;
        if (DataContext is MainViewModel mainVm)
            mainVm.ViewportSize = e.NewSize;
        
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

                transformGroup = (TransformGroup)_NodeCanvasBase.RenderTransform;
                scaleTransform = (ScaleTransform)transformGroup.Children[0];
                translateTransform = (TranslateTransform)transformGroup.Children[1];
                translateTransform.X = 0;
                translateTransform.Y = 0;
                mainVm.NodeCanvasBase.Children.Add(selectionBox);

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

        double gridSpacing = 30;
        var strokeColor = Color.Parse("#303040");
        double strokeThickness = 1.5;

        for (double x = 0; x < _NodeCanvasBase.Bounds.Width; x += gridSpacing)
        {
            var line = new Line
            {
                StartPoint = new Point(x, 0),
                EndPoint = new Point(x, _NodeCanvasBase.Bounds.Height),
                Stroke = new SolidColorBrush(strokeColor),
                StrokeThickness = strokeThickness,
                ZIndex = -10
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
                StrokeThickness = strokeThickness,
                ZIndex = -10
            };
            _NodeCanvasBase.Children.Insert(0, line);
        }
    }
    
    private void MainView_OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (DataContext is MainViewModel mainVm)
        {
            if (((MainViewModel)DataContext).isPannable)
            {
                var currentPosition = e.GetPosition(this);
                var delta = currentPosition - panStart;
                panStart = currentPosition;

                var newX = translateTransform.X + delta.X;
                var newY = translateTransform.Y + delta.Y;

                // Constrain panning

                if (categoriesPanel != null)
                {
                    newX = Math.Min(newX, (_NodeCanvasBase.Width * scaleTransform.ScaleX - viewportSize.Width  + categoriesPanel.Bounds.Width) / 2);
                    newX = Math.Max(newX, (-_NodeCanvasBase.Width * scaleTransform.ScaleX + viewportSize.Width  - categoriesPanel.Bounds.Width) / 2);

                }

                if (topPanel != null)
                {
                    newY = Math.Min(newY, (_NodeCanvasBase.Height * scaleTransform.ScaleY - viewportSize.Height + topPanel.Bounds.Height) / 2);
                    newY = Math.Max(newY, (-_NodeCanvasBase.Height * scaleTransform.ScaleY + viewportSize.Height - topPanel.Bounds.Height) / 2);
                }
                translateTransform.X = newX;
                translateTransform.Y = newY;


                return;
            }
            else if (((MainViewModel)DataContext).isSelecting)
            {
                selectionPoints[1] = e.GetPosition(this);
                if (selectionPoints[1].X < selectionPoints[0].X)
                    Canvas.SetLeft(selectionBox, selectionPoints[1].X / scaleTransform.ScaleX);
                else
                    Canvas.SetLeft(selectionBox, selectionPoints[0].X / scaleTransform.ScaleX);


                if (selectionPoints[1].Y < selectionPoints[0].Y)
                    Canvas.SetTop(selectionBox, selectionPoints[1].Y / scaleTransform.ScaleY);
                else
                    Canvas.SetTop(selectionBox, selectionPoints[0].Y / scaleTransform.ScaleY);


                selectionBox.Width = Math.Abs((e.GetPosition(this).X - selectionPoints[0].X) / scaleTransform.ScaleX);
                selectionBox.Height = Math.Abs((e.GetPosition(this).Y - selectionPoints[0].Y) / scaleTransform.ScaleY);

            }


            mainVm.HandlePointerMoved(e);
        }
        e.Handled = true;
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (DataContext is MainViewModel mainView)
        {
            if (mainView.SelectedPath != null)
            {
                mainView.SelectedPath.SetSelected(false);
                mainView.SelectedPath = null;
            }
        }

        if (e.GetCurrentPoint(this).Properties.IsRightButtonPressed)
        {
            ((MainViewModel)DataContext).isPannable = true;
            panStart = e.GetPosition(this);
            
        }
        else if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            ((MainViewModel)DataContext).isSelecting = true;
            selectionPoints[0] = e.GetPosition(this);
        }
        e.Handled = true;
    }

    private void MainView_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (((MainViewModel)DataContext).isPannable)
        {
            ((MainViewModel)DataContext).isPannable = false;
            return;
        }

        if (((MainViewModel)DataContext).isSelecting)
        {
            ((MainViewModel)DataContext).isSelecting = false;
            selectionPoints[1] = e.GetPosition(this);
            selectionBox.Width = 0;
            selectionBox.Height = 0;
            return;
        }

        if (DataContext is MainViewModel mainVm)
            mainVm.OnEndConnectionDragFromNode(e);

        e.Handled = true;
    }

    private void MainView_PointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        if (_NodeCanvasBase == null) return;



        double zoomFactor = 1.08;
        double zoom = e.Delta.Y > 0 ? zoomFactor : 1 / zoomFactor;

        var newScale = scaleTransform.ScaleX * zoom;
        newScale = Math.Max(0.2, Math.Min(newScale, 2.0)); // Zoom constraints

        var position = e.GetPosition(this);
        var relativePosition = e.GetPosition(_NodeCanvasBase);

        var newX = (translateTransform.X / scaleTransform.ScaleX) * newScale;
        var newY = (translateTransform.Y / scaleTransform.ScaleY) * newScale;


        scaleTransform.ScaleX = newScale;
        scaleTransform.ScaleY = newScale;


        if (categoriesPanel != null)
        {
            newX = Math.Min(newX, (_NodeCanvasBase.Width * scaleTransform.ScaleX - viewportSize.Width + categoriesPanel.Bounds.Width) / 2);
            newX = Math.Max(newX, (-_NodeCanvasBase.Width * scaleTransform.ScaleX + viewportSize.Width - categoriesPanel.Bounds.Width) / 2);

        }

        if (topPanel != null)
        {
            newY = Math.Min(newY, (_NodeCanvasBase.Height * scaleTransform.ScaleY - viewportSize.Height + topPanel.Bounds.Height) / 2);
            newY = Math.Max(newY, (-_NodeCanvasBase.Height * scaleTransform.ScaleY + viewportSize.Height - topPanel.Bounds.Height) / 2);
        }


        translateTransform.X = newX;
        translateTransform.Y = newY;

        e.Handled = true;
    }

    private void MainView_OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Back)
        {
            if (DataContext is MainViewModel mainVm)
            {
                mainVm.DeleteSelectedPathCommand.Execute(null);
                mainVm.DeleteSelectedNodeCommand.Execute(null);
            }
        }
    }

    private void Path_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Path path && path.DataContext is PathBaseViewModel pathVm)
        {
            if (DataContext is MainViewModel mainVm)
            {
                mainVm.OnClickedPath(pathVm, e);
            }
        }
    }
}


