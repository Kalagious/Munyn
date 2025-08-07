using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
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
        IBrush selectedBorderBrush;
        [ObservableProperty]
        private bool isSelected = false;

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

        public MainViewModel mainVM;
        public Canvas parentCanvas;
        public ContextBase parentContext;

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


        [ObservableProperty]
        private bool _isPropertySelectionOpen = false;

        [RelayCommand]
        private void OpenProperySelection()
        {
            IsPropertySelectionOpen = !IsPropertySelectionOpen;
        }


        [RelayCommand]
        private void DeleteNode()
        {
            if (mainVM != null)
            mainVM.DeleteNode(this);
        }


        public void SetSelected(bool selected)
        {
            IsSelected = selected;
            if (selected)
                SelectedBorderBrush = new SolidColorBrush(Color.Parse("#F2F2F2")); 
            else
                SelectedBorderBrush = new SolidColorBrush(Color.Parse("#00F2F2F2")); 

        }

        public void AddNodeProperty(NodePropertyBasic property)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));
            property.ParentNode = this;
            Properties.Add(property);
            GetGraphViewProperties();

            foreach (var path in connectedPaths)
            {
                path.UpdateCompromisedStatus();
            }
        }

        public void RemoveProperty(NodePropertyBasic property)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));
            Properties.Remove(property);
            GetGraphViewProperties();

            foreach (var path in connectedPaths)
            {
                path.UpdateCompromisedStatus();
            }
        }

        public NodePropertyBasic GetNodePropertyFromName(string name)
        {
            foreach (var property in Properties)
                if (property.PropertyName == name) return property;
            return null;
        }

        public void GetGraphViewProperties()
        {

            List<NodePropertyInterface> unGroupedInterfaces = new List<NodePropertyInterface>();
            NodePropertyMultiInterface existingMultiInterface = null;

            foreach (NodePropertyBasic property in Properties)
                if (property.GetType() == typeof(NodePropertyMultiInterface))
                    existingMultiInterface = property as NodePropertyMultiInterface;


            if (existingMultiInterface != null)
                existingMultiInterface.Interfaces.Clear();


            PropertiesInNodeView.Clear();
            foreach (NodePropertyBasic property in Properties)
            {
                if (property.IsVisableOnGraphNode)
                {
                    switch (property)
                    {
                        case NodePropertyInterface nodePropertyInterface:
                            if (existingMultiInterface != null)
                                existingMultiInterface.Interfaces.Add(property as NodePropertyInterface);
                            else
                                unGroupedInterfaces.Add(property as NodePropertyInterface);
                            break;

                        default:
                            PropertiesInNodeView.Add(property);
                            break;
                    }

                }
            }

            if (unGroupedInterfaces.Count() > 0)
            {
                if (existingMultiInterface != null)
                    existingMultiInterface.Interfaces.AddRange(unGroupedInterfaces);
                else
                {
                    Properties.Insert(0, new NodePropertyMultiInterface { PropertyName = "Interfaces", Interfaces = new ObservableCollection<NodePropertyInterface>(unGroupedInterfaces), IsVisableOnGraphNode = true });
                    PropertiesInNodeView.Insert(0,Properties[0]);

                }
            }
        }

        public void UpdatePropertiesFromDto(List<NodePropertyDto> propertyDtos)
        {
            Properties.Clear();
            foreach (var propertyDto in propertyDtos)
            {
                NodePropertyBasic newProperty = null;

                 if (propertyDto.PropertyType == "NodePropertyText")
                {
                    newProperty = new NodePropertyText
                    {
                        PropertyName = propertyDto.PropertyName,
                        TextContent = propertyDto.TextContent
                    };
                }
                else if (propertyDto.PropertyType == "NodePropertyCommand")
                {
                    newProperty = new NodePropertyCommand
                    {
                        PropertyName = propertyDto.PropertyName,
                        Command = propertyDto.Command,
                        Description = propertyDto.Description
                    };
                }
                else if (propertyDto.PropertyType == "NodePropertyLink")
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
                    IconName = property.IconName,
                };

               if (property is NodePropertyText textProperty)
                {
                    propertyDto.TextContent = textProperty.TextContent;
                }
                else if (property is NodePropertyCommand commandProperty)
                {
                    propertyDto.Command = commandProperty.Command;
                    propertyDto.Description = commandProperty.Description;
                }
                else if (property is NodePropertyLink linkProperty)
                {
                    propertyDto.Url = linkProperty.Url;
                    propertyDto.DisplayText = linkProperty.DisplayText;
                }
                else
                {
                    propertyDto.Value = property.Value;
                }
                propertyDto.PropertyType = property.GetType().Name;
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
