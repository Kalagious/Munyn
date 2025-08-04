using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Munyn.ViewModels.Nodes.Properties;

namespace Munyn.Views.Nodes.NodeDetails.Properties.GraphView
{
    public partial class NodeGraphPropertyBasicView : UserControl
    {
        public NodeGraphPropertyBasicView()
        {
            InitializeComponent();
        }

        private async void Property_Tapped(object? sender, Avalonia.Input.TappedEventArgs e)
        {
            var property = (NodePropertyBasic)this.DataContext;
            if (property != null && !string.IsNullOrEmpty(property.PropertyValue))
            {
                await TopLevel.GetTopLevel(this).Clipboard.SetTextAsync(property.PropertyValue);
                e.Handled = true;
            }
        }
    }
}