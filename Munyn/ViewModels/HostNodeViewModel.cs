using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Munyn.ViewModels
{
    public partial class HostNodeViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _HostName;
        [ObservableProperty]
        private string _HostIP;
        [ObservableProperty]
        private string _HostOS;
        [ObservableProperty]
        private float _x;
        [ObservableProperty]
        private float _y;

        public HostNodeViewModel(string name, string ipAddress, string status, float x, float y)
        {
            HostName = name;
            HostIP = ipAddress;
            HostOS = status;
            _x = x;
            _y = y;
        }
    }
}
