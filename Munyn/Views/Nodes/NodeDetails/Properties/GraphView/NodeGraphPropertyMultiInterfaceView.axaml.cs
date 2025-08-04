using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Munyn.ViewModels.Nodes.Properties;

namespace Munyn.Views.Nodes.NodeDetails.Properties.GraphView
{
    public partial class NodeGraphPropertyMultiInterfaceView : UserControl
    {
        public NodeGraphPropertyMultiInterfaceView()
        {
            InitializeComponent();
        }

        private async void Interface_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                if (sender is Button button && button.DataContext is NodePropertyInterface property)
                {
                    if (property != null && !string.IsNullOrEmpty(property.Ip))
                    {
                        await TopLevel.GetTopLevel(this).Clipboard.SetTextAsync(property.Ip);
                        e.PreventGestureRecognition();
                        e.Handled = true;
                    }
                }
            }
        }
    }
}