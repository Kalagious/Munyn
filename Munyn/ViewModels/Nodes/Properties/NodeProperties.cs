using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
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
        [ObservableProperty] private string? _propertyIcon;
        [ObservableProperty] private string? _icon;
        [ObservableProperty] private bool _isDefault;
        [ObservableProperty] private bool _isFavorited;
        [ObservableProperty] private bool _isEditable;
        [ObservableProperty] private bool _isVisableOnGraphNode;
        //[ObservableProperty] private int _propertyIndex; // Index in overall property list
        [ObservableProperty] private int _propertyIndexInNodeView; // Index for displaying properties specifically in nodes on the graph, -1 means doesnt matter

        public NodePropertyBasic(string propertyName = "New Property", bool isDefault = false, bool isVisiableOnGraphNode = false, bool isEditable = true, int propertyIndexInNodeView = -1)
        {
            PropertyName = propertyName;
            PropertyIndexInNodeView = propertyIndexInNodeView;
            IsDefault = isDefault;
            IsEditable = isEditable;
            IsFavorited = false;
            IsVisableOnGraphNode = isVisiableOnGraphNode;
            Icon = "file-richtext-fill";
        }


    }
    public partial class NodePropertyList : NodePropertyBasic
    {
        [ObservableProperty]
        private List<NodePropertyBasic> _listContent;
    }

    public partial class NodePropertyText : NodePropertyBasic
    {
        [ObservableProperty]
        private string? _textContent;
    }

    public enum ExecutableType
    {
        Bash,
        PowerShell
    }

    public partial class NodePropertyExecutable : NodePropertyBasic
    {
        [ObservableProperty]
        private string? _command;
        [ObservableProperty]
        private ExecutableType _type;
    }

    public partial class NodePropertyCommand : NodePropertyBasic
    {
        [ObservableProperty]
        private string? _command;
        [ObservableProperty]
        private string? _description;
    }

    public partial class NodePropertyLink : NodePropertyBasic
    {
        [ObservableProperty]
        private string? _url;
        [ObservableProperty]
        private string? _displayText;
    }

    public partial class NodePropertyCode : NodePropertyBasic
    {
        [ObservableProperty]
        private string? _code;
    }
}
