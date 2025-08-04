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

        private async void Property_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var property = (NodePropertyCompromised)this.DataContext;
            if (property != null && property.CompromiseLevel.HasValue)
            {
                await TopLevel.GetTopLevel(this).Clipboard.SetTextAsync(property.CompromiseLevel.Value.ToString());
            }
        }
    }
}
