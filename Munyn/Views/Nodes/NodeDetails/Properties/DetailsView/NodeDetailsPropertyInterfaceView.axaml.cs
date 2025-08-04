using Avalonia.Controls;
using Avalonia.Input;
using Munyn.ViewModels.Nodes.Properties;

namespace Munyn.Views.Nodes.NodeDetails
{
    public partial class NodeDetailsPropertyInterfaceView : UserControl
    {
        public NodeDetailsPropertyInterfaceView()
        {
            InitializeComponent();
        }

        private async void Ip_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                var property = (NodePropertyInterface)this.DataContext;
                if (property != null && !string.IsNullOrEmpty(property.Ip))
                {
                    await TopLevel.GetTopLevel(this).Clipboard.SetTextAsync(property.Ip);
                }
            }
        }
    }
}