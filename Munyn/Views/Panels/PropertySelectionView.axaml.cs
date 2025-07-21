using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using Munyn.ViewModels;
using Munyn.ViewModels.Nodes.Properties;

namespace Munyn.Views.Panels
{
    public partial class PropertySelectionView : UserControl
    {
        public PropertySelectionView()
        {
            InitializeComponent();
            AttachedToVisualTree += PropertySelectionView_AttachedToVisualTree;
        }

        void PropertySelectionView_AttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
        {
            if (DataContext is PropertySelectionViewModel viewModel)
            {
                NodeBaseViewModel popupRootDataContext = (NodeBaseViewModel)TopLevel.GetTopLevel(this).DataContext;
                viewModel.selectedNode = popupRootDataContext;
            }
        }
    }
}
