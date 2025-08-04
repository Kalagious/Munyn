using Avalonia.Controls;
using Avalonia.Input;
using Munyn.ViewModels.Nodes.Properties;

namespace Munyn.Views.Nodes.NodeDetails
{
    public partial class NodeDetailsPropertyTextView : UserControl
    {
        public NodeDetailsPropertyTextView()
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
                }
            }
        }
    }
}