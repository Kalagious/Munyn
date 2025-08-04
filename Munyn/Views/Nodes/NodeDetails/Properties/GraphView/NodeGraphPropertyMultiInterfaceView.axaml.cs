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

        private async void Interface_Tapped(object? sender, Avalonia.Input.TappedEventArgs e)
        {
            if (e.Source is Control source && source.DataContext is NodePropertyInterface property)
            {
                if (property != null && !string.IsNullOrEmpty(property.Ip))
                {
                    await TopLevel.GetTopLevel(this).Clipboard.SetTextAsync(property.Ip);
                    e.Handled = true;
                }
            }
        }
    }
}