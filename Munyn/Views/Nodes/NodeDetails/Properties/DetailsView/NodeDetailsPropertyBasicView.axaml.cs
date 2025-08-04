using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Munyn.ViewModels.Nodes.Properties;

namespace Munyn.Views.Nodes.NodeDetails;
public partial class NodeDetailsPropertyBasicView : UserControl
{
    public NodeDetailsPropertyBasicView()
    {
        InitializeComponent();
    }

    private async void Property_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            var property = (NodePropertyBasic)this.DataContext;
            if (property != null && !string.IsNullOrEmpty(property.PropertyValue))
            {
                await TopLevel.GetTopLevel(this).Clipboard.SetTextAsync(property.PropertyValue);
            }
        }
    }
}