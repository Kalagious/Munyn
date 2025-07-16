using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Munyn.ViewModels.Nodes.Properties;

namespace Munyn.Views.Nodes.NodeDetails
{
    public partial class IconSelectionView : Window
    {
        public IconSelectionView()
        {
            InitializeComponent();
            DataContext = new IconSelectionViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void SelectIcon(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.CommandParameter is string iconName)
            {
                Close(iconName);
            }
        }
    }
}
