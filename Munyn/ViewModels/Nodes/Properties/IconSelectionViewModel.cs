using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Munyn.ViewModels.Nodes.Properties
{
    public partial class IconSelectionViewModel : ViewModelBase
    {
        [ObservableProperty]
        private IEnumerable<string> _icons;

        [ObservableProperty]
        private string _selectedIcon;

        public IconSelectionViewModel()
        {
            Icons = new List<string>();
            if (Application.Current != null)
            {
                Icons = Application.Current.Resources
                    .Where(r => r.Value is StreamGeometry)
                    .Select(r => r.Key.ToString())
                    .ToList();
            }
        }

        [RelayCommand]
        private void SelectIcon(string iconName)
        {
            SelectedIcon = iconName;
        }
    }

    public class StringToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string iconName)
            {
                if (Application.Current != null && Application.Current.Resources.TryGetResource(iconName, null, out var resource))
                {
                    return resource;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
