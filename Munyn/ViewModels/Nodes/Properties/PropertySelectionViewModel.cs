using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace Munyn.ViewModels.Nodes.Properties
{
    public partial class PropertySelectionViewModel : ViewModelBase
    {
        [ObservableProperty]
        private ObservableCollection<PropertySelectionItem> _propertiesList = new ObservableCollection<PropertySelectionItem>();

        public NodeBaseViewModel selectedNode;

        public partial class PropertySelectionItem : ObservableObject
        {

            public Type PropertyType;

            [ObservableProperty]
            private string _propertyName;
            [ObservableProperty]
            private StreamGeometry _propertyIcon;
        }
        public PropertySelectionViewModel()
        {
            //Basic
            PropertiesList.Add(new PropertySelectionItem
            {
                PropertyType = typeof(NodePropertyBasic),
                PropertyName = "Basic",
                PropertyIcon = (StreamGeometry)Application.Current.Resources["file-richtext-fill"]
            });
            //Text
            PropertiesList.Add(new PropertySelectionItem
            {
                PropertyType = typeof(NodePropertyText),
                PropertyName = "Text",
                PropertyIcon = (StreamGeometry)Application.Current.Resources["text"]
            });
            //List
            PropertiesList.Add(new PropertySelectionItem
            {
                PropertyType = typeof(NodePropertyList),
                PropertyName = "List WIP",
                PropertyIcon = (StreamGeometry)Application.Current.Resources["list"]
            });
            //Command
            PropertiesList.Add(new PropertySelectionItem
            {
                PropertyType = typeof(NodePropertyCommand),
                PropertyName = "Command",
                PropertyIcon = (StreamGeometry)Application.Current.Resources["command"]
            });
            //Link
            PropertiesList.Add(new PropertySelectionItem
            {
                PropertyType = typeof(NodePropertyLink),
                PropertyName = "Link WIP",
                PropertyIcon = (StreamGeometry)Application.Current.Resources["link"]
            });            
            //Link
            PropertiesList.Add(new PropertySelectionItem
            {
                PropertyType = typeof(NodePropertyVulnerability),
                PropertyName = "Vulnerability",
                PropertyIcon = (StreamGeometry)Application.Current.Resources["aim"]
            });            
            //Link
            PropertiesList.Add(new PropertySelectionItem
            {
                PropertyType = typeof(NodePropertyCompromised),
                PropertyName = "Compromised",
                PropertyIcon = (StreamGeometry)Application.Current.Resources["skull"]
            });

        }

        [RelayCommand]
        private void SelectProperty(PropertySelectionItem selection)
        {
            NodePropertyBasic newProperty = Activator.CreateInstance(selection.PropertyType) as NodePropertyBasic;
            selectedNode.AddNodeProperty(newProperty);
            selectedNode.IsPropertySelectionOpen = false;
        }
    }
}
