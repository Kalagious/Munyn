using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
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
        private int _userCount = 0;
        [ObservableProperty]
        private int _serviceCount = 0;



        [ObservableProperty]
        private ObservableCollection<NetInterface> _netInterfaces;

        [ObservableProperty]
        private ObservableCollection<ServiceListEntry> _services;

        public class NetInterface
        {
            public string inf = "lo";
            public string ip = "127.0.0.1";
            public string mac = "12:34:56:78:90:ab";
        }

        public partial class ServiceListEntry : ObservableObject
        {
            [ObservableProperty]
            private string _serviceName = "None";

            [ObservableProperty]
            private string _serviceIconPath = "avares://Munyn/Assets/Icons/thunder-icon.svg";

            [ObservableProperty]
            private bool _isCompromised = false;

            public ServiceListEntry(string name, bool isCompromised = false) { 
                ServiceName = name;
                IsCompromised = isCompromised;
            }
        }

        public void Refresh()
        {
            UserCount = GetNodeCountOfType<UserNodeViewModel>();
            ServiceCount = GetNodeCountOfType<ServiceNodeViewModel>();
            Services.Clear();

            foreach (ViewModelBase node in contextNodes)
            {
                if (node.GetType() == typeof(ServiceNodeViewModel))
                    Services.Add(new ServiceListEntry(((ServiceNodeViewModel)node).ServiceName));
                
            }
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
            _services = new ObservableCollection<ServiceListEntry>();
            _mainVM = mainVM;
            parentContext = parent;
        }

    }
}
