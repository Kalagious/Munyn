using Avalonia;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Munyn.ViewModels.Nodes.Properties
{
    public partial class NodePropertyBasic : ObservableObject
    {

        [ObservableProperty] private string? _propertyName;
        [ObservableProperty] private string? _value;
        [ObservableProperty] private string? _propertyValue;
        [ObservableProperty] private StreamGeometry _icon;
        [ObservableProperty] private IBrush _iconColor;
        [ObservableProperty] private string _iconName;
        public string IconColorString;

        [ObservableProperty] private bool _isDefault;
        [ObservableProperty] private bool _isFavorited;
        [ObservableProperty] private bool _isEditable;
        [ObservableProperty] private bool _isVisableOnGraphNode;
        //[ObservableProperty] private int _propertyIndex; // Index in overall property list
        [ObservableProperty] private int _propertyIndexInNodeView; // Index for displaying properties specifically in nodes on the graph, -1 means doesnt matter

        [ObservableProperty] private bool _isIconSelectionOpen;
        public IconSelectionViewModel IconSelectionViewModel { get; set; }

        public NodePropertyBasic(string propertyName = "New Property", bool isDefault = false, bool isVisiableOnGraphNode = false, bool isEditable = true, int propertyIndexInNodeView = -1)
        {
            PropertyName = propertyName;
            PropertyIndexInNodeView = propertyIndexInNodeView;
            IsDefault = isDefault;
            IsEditable = isEditable;
            IsFavorited = false;
            IsVisableOnGraphNode = isVisiableOnGraphNode;
            IconName = "file-richtext-fill";
            IconSelectionViewModel = new IconSelectionViewModel();
            IconSelectionViewModel.PropertyChanged += IconSelectionViewModel_PropertyChanged;
            Refresh();
        }

        private void IconSelectionViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IconSelectionViewModel.SelectedIcon))
            {
                IconName = IconSelectionViewModel.SelectedIcon;
                Refresh();
            }
            else if (e.PropertyName == nameof(IconSelectionViewModel.IconColor))
            {
                IconColor = new SolidColorBrush(IconSelectionViewModel.IconColor);
                IconColorString = IconSelectionViewModel.IconColor.ToString();
            }
        }

        public void Refresh()
        {
            if (Application.Current.Resources.TryGetResource(IconName, null, out var resource))
            {
                Icon = (StreamGeometry)resource;
            }

            if (!string.IsNullOrEmpty(IconColorString))
            {
                if (Color.TryParse(IconColorString, out var color))
                {
                    IconColor = new SolidColorBrush(color);
                }
            }
            else
            {
                IconColor = new SolidColorBrush(Colors.Black);
            }
        }


    }
    public partial class NodePropertyList : NodePropertyBasic
    {
        [ObservableProperty]
        private List<NodePropertyBasic> _listContent;

        public NodePropertyList() : base() { }
    }

    public partial class NodePropertyText : NodePropertyBasic
    {
        [ObservableProperty]
        private string? _textContent;

        public NodePropertyText() : base() { }
    }

    public partial class NodePropertyCommand : NodePropertyBasic
    {
        [ObservableProperty]
        private string? _command;
        [ObservableProperty]
        private string? _description;

        public NodePropertyCommand() : base() { }
    }

    public partial class NodePropertyLink : NodePropertyBasic
    {
        [ObservableProperty]
        private string? _url;
        [ObservableProperty]
        private string? _displayText;

        public NodePropertyLink() : base() { }
    }

}
