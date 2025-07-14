using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Munyn.ViewModels.Nodes.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Munyn.ViewModels
{
    public partial class NodeBaseViewModel : ViewModelBase
    {
        public Guid Id { get; set; }

        [ObservableProperty]
        private string? _nodeName;

        [ObservableProperty]
        ObservableCollection<NodePropertyBasic> _properties = new ObservableCollection<NodePropertyBasic>();
        [ObservableProperty]
        ObservableCollection<NodePropertyBasic> _propertiesInNodeView = new ObservableCollection<NodePropertyBasic>();

        [ObservableProperty]
        private bool _isContext = false;

        [ObservableProperty]
        private double _x;
        [ObservableProperty]
        private double _y;

        [ObservableProperty]
        private bool _editName = false;

        [RelayCommand]
        private void ToggleEditName() { EditName = !EditName; }

        public MainViewModel _mainVM;
        public Canvas parentCanvas;

        public List<PathBaseViewModel> connectedPaths = new List<PathBaseViewModel>();

        public NodeBaseViewModel()
        {
            Id = Guid.NewGuid();
        }

        [ObservableProperty]
        IBrush _nodeTheme;

        public Action<NodeBaseViewModel, Point, PointerPressedEventArgs> OnStartConnectionDragNode { get; internal set; }
        public Action<NodeBaseViewModel, PointerReleasedEventArgs> OnClickedNode { get; internal set; }

        [RelayCommand]
        private void AddBlankProperty() {
        
                AddNodeProperty(new NodePropertyBasic("New Property", false, false, true, -1));
        }

        public void AddNodeProperty(NodePropertyBasic property)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));
            
            Properties.Add(property);
            GetGraphViewProperties();
        }

        public NodePropertyBasic GetNodePropertyFromName(string name)
        {
            foreach (var property in Properties)
                if (property.PropertyName == name) return property;
            return null;
        }

        public void GetGraphViewProperties()
        {
            PropertiesInNodeView.Clear();
            foreach (NodePropertyBasic property in Properties)
            {
                if (property.IsVisableOnGraphNode)
                {
                    PropertiesInNodeView.Add(property);
                }
            }
        }

        public LinearGradientBrush makeGradient(string color1, string color2)
        {
            LinearGradientBrush nodeGradient = new LinearGradientBrush();
            nodeGradient.GradientStops.Add(new GradientStop(Color.Parse(color1), 0.0));
            nodeGradient.GradientStops.Add(new GradientStop(Color.Parse(color2), 1.0));
            nodeGradient.StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative);
            nodeGradient.EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative);
            return nodeGradient;
        }
    }
}
