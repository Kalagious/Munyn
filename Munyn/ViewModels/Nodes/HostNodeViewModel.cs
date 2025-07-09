using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using Microsoft.VisualBasic;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using static SkiaSharp.HarfBuzz.SKShaper;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        
        public partial class NetInterface : ObservableObject
        {
            [ObservableProperty] private string _inf = "lo";
            [ObservableProperty] private string _ip = "127.0.0.1";
            [ObservableProperty] private string _mac = "12:34:56:78:90:ab";
            public NetInterface(string infIn, string ipIn, string macIn)
            {
                Inf = infIn;
                Ip = ipIn;
                Mac = macIn;
            }
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
            NetInterfaces.Add(new NetInterface("lo", "127.0.0.1", "12:34:56:78:90:ab"));
            NetInterfaces.Add(new NetInterface("eth0", "192.168.1.12", "12:34:56:78:90:ab"));
            NetInterfaces.Add(new NetInterface("tun0", "10.10.14.2", "12:34:56:78:90:ab"));

            _services = new ObservableCollection<ServiceListEntry>();
            _mainVM = mainVM;
            parentContext = parent;
        }

    }
}
