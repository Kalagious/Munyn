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
    public partial class NodeBaseViewModel : ViewModelBase
    {

        [ObservableProperty]
        private double _x;
        [ObservableProperty]
        private double _y;

        protected MainViewModel _mainVM;
        public Canvas? parentCanvas;

    }
}
