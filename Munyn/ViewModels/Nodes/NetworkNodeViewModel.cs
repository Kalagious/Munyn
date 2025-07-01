using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
    public partial class NetworkNodeViewModel : ContextBase
    {
        
        [ObservableProperty]
        private string _NetworkName;




        public NetworkNodeViewModel(string name, float x, float y, ContextBase parent, Canvas tmpParentCanvas, MainViewModel mainVM)
        {
            NetworkName = name;
            contextName = name;
            X = x;
            Y = y;
            parentCanvas = tmpParentCanvas;
            contextNodes = new ObservableCollection<ViewModelBase>();

            _mainVM = mainVM;
            parentContext = parent;
        }

    }
}
