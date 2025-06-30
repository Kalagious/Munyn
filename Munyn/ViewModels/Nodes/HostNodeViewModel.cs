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
    public partial class HostNodeViewModel : ContextBase
    {
        
        [ObservableProperty]
        private string _HostName;

        [ObservableProperty]
        private string _HostOS;

        [ObservableProperty]
        private ObservableCollection<NetInterface> _netInterfaces;


        public class NetInterface
        {
            public string inf = "lo";
            public string ip = "127.0.0.1";
            public string mac = "12:34:56:78:90:ab";
        }

        [RelayCommand]
        private void EnterContextButton()
        {
            _mainVM.EnterContext(this);
        }
        public HostNodeViewModel(string name, string status, float x, float y, ContextBase parent, Canvas tmpParentCanvas, MainViewModel mainVM)
        {
            HostName = name;
            contextName = name;
            HostOS = status;
            X = x;
            Y = y;
            parentCanvas = tmpParentCanvas;
            contextNodes = new ObservableCollection<ViewModelBase>();
            _netInterfaces = new ObservableCollection<NetInterface>();
            _mainVM = mainVM;
            parentContext = parent;
        }

    }
}
