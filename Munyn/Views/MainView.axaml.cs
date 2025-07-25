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

    public MainView()
    {
        InitializeComponent();
        this.Loaded += OnLoaded;
        this.PointerMoved += MainView_OnPointerMoved;
        this.PointerReleased += MainView_OnPointerReleased;
        this.PointerPressed += OnPointerPressed;
        this.SizeChanged += OnSizeChanged;
    }

    private void OnSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        viewportSize = e.NewSize;
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

        
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Error: Canvas '_NodeCanvasBase' not found.");
                return;
            }
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
                newX = Math.Min(newX, 0);
                newY = Math.Min(newY, 0);
                newX = Math.Max(newX, viewportSize.Width - _NodeCanvasBase.Width);
                newY = Math.Max(newY, viewportSize.Height - _NodeCanvasBase.Height);

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


