using Avalonia;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Munyn.ViewModels.Nodes.Properties
{
    public partial class NodePropertyBasic : ObservableObject
    {

        [ObservableProperty] private string? _propertyName;
        [ObservableProperty] private string? _value;
        [ObservableProperty] private string? _propertyValue;
        [property: JsonIgnore]
        [ObservableProperty] private StreamGeometry _icon;
        [ObservableProperty] private IBrush _iconColor;
        [ObservableProperty] private string _iconName;

        [JsonIgnore]
        public NodeBaseViewModel ParentNode { get; set; }

        [ObservableProperty] private bool _isDefault;
        [ObservableProperty] private bool _isFavorited;
        [ObservableProperty] private bool _isEditable;
        [ObservableProperty] private bool _isVisableOnGraphNode;
        //[ObservableProperty] private int _propertyIndex; // Index in overall property list
        [ObservableProperty] private int _propertyIndexInNodeView; // Index for displaying properties specifically in nodes on the graph, -1 means doesnt matter

        [ObservableProperty] private bool _isIconSelectionOpen;
        public IconSelectionViewModel IconSelectionViewModel { get; set; }

        [RelayCommand]
        private void ToggleGraphVisability()
        {
            IsVisableOnGraphNode = !IsVisableOnGraphNode;
            ParentNode.GetGraphViewProperties();
        }


        public NodePropertyBasic() : this("New Property")
        {
        }

        public NodePropertyBasic(string propertyName = "New Property", bool isDefault = false, bool isVisiableOnGraphNode = false, bool isEditable = true)
        {
            PropertyName = propertyName;
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
            }
        }

        public void Refresh()
        {
            if (Application.Current.Resources.TryGetResource(IconName, null, out var resource))
            {
                Icon = (StreamGeometry)resource;
            }
            if (IconColor == null)
                IconColor = new SolidColorBrush(Colors.Black);
        }

        [RelayCommand]
        private void DeleteProperty()
        {
            ParentNode?.RemoveProperty(this);
        }
    }

    public partial class NodePropertyText : NodePropertyBasic
    {
        [ObservableProperty]
        private string? _textContent;

        public NodePropertyText() : base() { IconName = "text"; Refresh(); }
    }

    public partial class NodePropertyCommand : NodePropertyBasic
    {
        [ObservableProperty]
        private string? _command;
        [ObservableProperty]
        private string? _description;

        public NodePropertyCommand() : base() { IconName = "command"; Refresh(); }
    }
    
    public partial class NodePropertyInterface : NodePropertyBasic
    {
        [ObservableProperty]
        private string? _ip;
        [ObservableProperty]
        private string? _mac;

        public NodePropertyInterface() : base() { IconName = "sitemap-icon"; Refresh(); IsVisableOnGraphNode = true; }
    }

    public partial class NodePropertyMultiInterface : NodePropertyBasic
    {
        [ObservableProperty]
        private ObservableCollection<NodePropertyInterface> _interfaces;

        public NodePropertyMultiInterface() : base() { IconName = "sitemap-icon"; Refresh(); IsVisableOnGraphNode = true; }
}

public partial class NodePropertyLink : NodePropertyBasic
    {
        [ObservableProperty]
        private string? _url;
        [ObservableProperty]
        private string? _displayText;

        public NodePropertyLink() : base() { IconName = "link"; Refresh(); }
    }

    public partial class NodePropertyVulnerability : NodePropertyBasic
    {
        [ObservableProperty]
        private double _score;
        [ObservableProperty]
        private string? _location;
        [ObservableProperty]
        private string? _resource;
        [ObservableProperty]
        private string? _description;

        public NodePropertyVulnerability() : base() { IconName = "aim"; Refresh(); }
    }

    public partial class NodePropertyCompromised : NodePropertyBasic
    {
        [ObservableProperty]
        private double? _compromiseLevel = 10;
        
        public NodePropertyCompromised() : base() { IconName = "bolt"; Refresh(); }
    }

}
