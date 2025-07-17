using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Munyn.ViewModels.Data;
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
        private StreamGeometry _icon;
        public string IconName;

        [ObservableProperty]
        IBrush _nodeTheme;

        public Action<NodeBaseViewModel, Point, PointerPressedEventArgs> OnStartConnectionDragNode { get; internal set; }
        public Action<NodeBaseViewModel, PointerReleasedEventArgs> OnClickedNode { get; internal set; }

        [RelayCommand]
        private void AddBlankProperty()
        {
            AddNodeProperty(new NodePropertyBasic("New Property", false, false, true, -1));
        }

        [RelayCommand]
        private void AddListProperty()
        {
            AddNodeProperty(new NodePropertyList { PropertyName = "New List Property" });
        }

        [RelayCommand]
        private void AddTextProperty()
        {
            AddNodeProperty(new NodePropertyText { PropertyName = "New Text Property" });
        }


        [RelayCommand]
        private void AddLinkProperty()
        {
            AddNodeProperty(new NodePropertyLink { PropertyName = "New Link Property" });
        }


        [RelayCommand]
        private void AddCommandProperty()
        {
            AddNodeProperty(new NodePropertyCommand { PropertyName = "New Command Property" });
        }

        [RelayCommand]
        private void DeleteNode()
        {
            if (_mainVM != null)
            _mainVM.DeleteNode(this);
        }

        public void AddNodeProperty(NodePropertyBasic property)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));
            property.ParentNode = this;
            Properties.Add(property);
            GetGraphViewProperties();
        }

        public void RemoveProperty(NodePropertyBasic property)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));
            Properties.Remove(property);
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

        public void UpdatePropertiesFromDto(List<NodePropertyDto> propertyDtos)
        {
            Properties.Clear();
            foreach (var propertyDto in propertyDtos)
            {
                NodePropertyBasic newProperty = null;
                if (propertyDto.PropertyType == 1)
                {
                    newProperty = new NodePropertyList
                    {
                        PropertyName = propertyDto.PropertyName,
                        ListContent = new List<NodePropertyBasic>()
                    };
                    foreach (var innerPropertyDto in propertyDto.ListContent)
                    {
                        //This is not ideal, but it will work for now, it will not correctly load list properties within list properties
                        ((NodePropertyList)newProperty).ListContent.Add(new NodePropertyBasic
                        {
                            PropertyName = innerPropertyDto.PropertyName,
                            Value = innerPropertyDto.Value
                        });
                    }
                }
                else if (propertyDto.PropertyType == 2)
                {
                    newProperty = new NodePropertyText
                    {
                        PropertyName = propertyDto.PropertyName,
                        TextContent = propertyDto.TextContent
                    };
                }
                else if (propertyDto.PropertyType == 4)
                {
                    newProperty = new NodePropertyCommand
                    {
                        PropertyName = propertyDto.PropertyName,
                        Command = propertyDto.Command,
                        Description = propertyDto.Description
                    };
                }
                else if (propertyDto.PropertyType == 5)
                {
                    newProperty = new NodePropertyLink
                    {
                        PropertyName = propertyDto.PropertyName,
                        Url = propertyDto.Url,
                        DisplayText = propertyDto.DisplayText
                    };
                }
                else
                {
                    newProperty = new NodePropertyBasic
                    {
                        PropertyName = propertyDto.PropertyName,
                        Value = propertyDto.Value
                    };
                }

                if (newProperty != null)
                {
                    newProperty.IconName = propertyDto.IconName;
                    AddNodeProperty(newProperty);
                }
            }
        }

        public List<NodePropertyDto> GetPropertiesDto()
        {
            var propertyDtos = new List<NodePropertyDto>();
            foreach (var property in Properties)
            {
                var propertyDto = new NodePropertyDto
                {
                    PropertyName = property.PropertyName,
                    IsVisableOnGraphNode = property.IsVisableOnGraphNode,
                    IconName = property.IconName
                };

                if (property is NodePropertyList listProperty)
                {
                    propertyDto.PropertyType = 1;
                    propertyDto.ListContent = new List<NodePropertyDto>();
                    foreach (var innerProperty in listProperty.ListContent)
                    {
                        propertyDto.ListContent.Add(new NodePropertyDto
                        {
                            PropertyName = innerProperty.PropertyName,
                            Value = innerProperty.Value
                        });
                    }
                }
                else if (property is NodePropertyText textProperty)
                {
                    propertyDto.PropertyType = 2;
                    propertyDto.TextContent = textProperty.TextContent;
                }
                else if (property is NodePropertyCommand commandProperty)
                {
                    propertyDto.PropertyType = 4;
                    propertyDto.Command = commandProperty.Command;
                    propertyDto.Description = commandProperty.Description;
                }
                else if (property is NodePropertyLink linkProperty)
                {
                    propertyDto.PropertyType = 5;
                    propertyDto.Url = linkProperty.Url;
                    propertyDto.DisplayText = linkProperty.DisplayText;
                }
                else
                {
                    propertyDto.PropertyType = 0;
                    propertyDto.Value = property.Value;
                }
                propertyDtos.Add(propertyDto);
            }
            return propertyDtos;
        }


        public LinearGradientBrush makeTheme(string color1, string color2)
        {
            if (IconName != null)
                Icon = (StreamGeometry)Application.Current.Resources[IconName];


            LinearGradientBrush nodeGradient = new LinearGradientBrush();
            nodeGradient.GradientStops.Add(new GradientStop(Color.Parse(color1), 0.0));
            nodeGradient.GradientStops.Add(new GradientStop(Color.Parse(color2), 1.0));
            nodeGradient.StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative);
            nodeGradient.EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative);
            return nodeGradient;
        }
    }
}
