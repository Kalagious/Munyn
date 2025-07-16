using System;
using Avalonia;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Munyn.ViewModels;

namespace Munyn.ViewModels
{
    public partial class ContextBase : NodeBaseViewModel
    {
        public ObservableCollection<ViewModelBase>? contextNodes;
        public String? contextName;
        public ContextBase? parentContext;

        [ObservableProperty]
        private StreamGeometry _contextIcon;
        

        [RelayCommand]
        private void EnterContextButton()
        {
            contextName = NodeName;
            _mainVM.EnterContext(this);
        }



        public ContextBase()
        {
            contextNodes = new ObservableCollection<ViewModelBase>();
            IsContext = true;
            ContextIcon = (StreamGeometry)Application.Current.Resources["sitemap-icon"];
        }

        public int GetNodeCountOfType<T>()
        {
            int count = 0;
            for (int i = 0; i < contextNodes.Count; i++)
            {
                if (contextNodes.ElementAt(i).GetType() == typeof(T)) count++;
            }
            return count;
        }
    }
}
