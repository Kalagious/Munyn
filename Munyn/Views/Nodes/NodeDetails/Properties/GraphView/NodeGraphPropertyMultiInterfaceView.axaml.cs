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
                        var mainViewModel = (this.VisualRoot as TopLevel)?.DataContext as Munyn.ViewModels.MainViewModel;
                        if (mainViewModel != null)
                        {
                            var position = e.GetPosition(this.VisualRoot as Visual);
                            mainViewModel.ShowNotification("Copied!", position.X, position.Y);
                        }
                        e.Handled = true;
                    }
                }
            }
        }
    }
}