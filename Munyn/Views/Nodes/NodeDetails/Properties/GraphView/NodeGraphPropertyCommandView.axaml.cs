using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Munyn.ViewModels.Nodes.Properties;

namespace Munyn.Views.Nodes.NodeDetails.Properties.GraphView
{
    public partial class NodeGraphPropertyCommandView : UserControl
    {
        public NodeGraphPropertyCommandView()
        {
            InitializeComponent();
        }

        private async void Property_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                var property = (NodePropertyCommand)this.DataContext;
                if (property != null && !string.IsNullOrEmpty(property.PropertyValue))
                {
                    await TopLevel.GetTopLevel(this).Clipboard.SetTextAsync(property.PropertyValue);
                    e.PreventGestureRecognition();
                    e.Handled = true;
                }
            }
        }
    }
}