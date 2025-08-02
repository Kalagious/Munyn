using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Avalonia.Markup.Xaml;
using Munyn.ViewModels;
using Munyn.Views.Nodes;
using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Munyn.Views.Nodes.NodeDetails;

public partial class NodeDetailsView : NodeView
{
    public NodeDetailsView()
    {
        InitializeComponent();

    }


    public void Details_PointerPressed(object sender, PointerPressedEventArgs e)
    {
        return;
    }
    
    public void Details_PointerMoved(object sender, PointerEventArgs e)
    {
        return;
    }

    public void Details_PointerReleased(object sender, PointerReleasedEventArgs e)
    {
        return;
    }

} 