using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Munyn.ViewModels.Nodes.Properties;

namespace Munyn.Views.Nodes.NodeDetails.Properties.GraphView
{
    public partial class NodeGraphPropertyCompromisedView : UserControl
    {
        public NodeGraphPropertyCompromisedView()
        {
            InitializeComponent();
        }

        private async void Property_Tapped(object? sender, Avalonia.Input.TappedEventArgs e)
        {
            var property = (NodePropertyCompromised)this.DataContext;
            if (property != null && property.CompromiseLevel.HasValue)
            {
                await TopLevel.GetTopLevel(this).Clipboard.SetTextAsync(property.CompromiseLevel.Value.ToString());
                e.Handled = true;
            }
        }
    }
}
