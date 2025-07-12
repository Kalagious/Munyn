using Avalonia.Controls;
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
    public partial class ServiceNodeViewModel : NodeBaseViewModel
    {

        [ObservableProperty]
        private string _servicePort;
        [ObservableProperty]
        private string? _servicePath;
        [ObservableProperty]
        private string _serviceDetails;

        

        public ServiceNodeViewModel(string name, float x, float y, Canvas tmpParentCanvas)
        {
            NodeName = name;
            X = x;
            Y = y;

            NodeTheme = makeGradient("#b613d0", "#6524f3");

            parentCanvas = tmpParentCanvas;
        }

    }
}
