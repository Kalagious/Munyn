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
        





        public NetworkNodeViewModel(string name, float x, float y, ContextBase parent, Canvas tmpParentCanvas, MainViewModel mainVM)
        {
            NodeName = name;
            contextName = name;
            X = x;
            Y = y;
            Width = 300;
            Height = 200;
            parentCanvas = tmpParentCanvas;
            contextNodes = new ObservableCollection<ViewModelBase>();

            base.mainVM = mainVM;
            parentContext = parent;
        }

    }
}
