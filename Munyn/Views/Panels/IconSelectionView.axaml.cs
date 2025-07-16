using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Munyn.ViewModels.Nodes.Properties;

namespace Munyn.Views.Panels
{
    public partial class IconSelectionView : UserControl
    {
        public IconSelectionView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
