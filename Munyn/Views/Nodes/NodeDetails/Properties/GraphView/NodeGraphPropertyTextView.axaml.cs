using Avalonia.Controls;
using Avalonia.Input;
using Munyn.ViewModels.Nodes.Properties;

namespace Munyn.Views.Nodes.NodeDetails.Properties.GraphView
{
    public partial class NodeGraphPropertyTextView : UserControl
    {
        public NodeGraphPropertyTextView()
        {
            InitializeComponent();
        }

        private async void Property_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                var property = (NodePropertyText)this.DataContext;
                if (property != null && !string.IsNullOrEmpty(property.PropertyValue))
                {
                    await TopLevel.GetTopLevel(this).Clipboard.SetTextAsync(property.PropertyValue);
                    var mainViewModel = (this.VisualRoot as TopLevel)?.DataContext as Munyn.ViewModels.MainViewModel;
                    if (mainViewModel != null)
                    {
                        var position = e.GetPosition(this.VisualRoot);
                        mainViewModel.ShowNotification("Copied!", position.X, position.Y);
                    }
                    e.Handled = true;
                }
            }
        }
    }
}