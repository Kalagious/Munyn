using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
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

        public MainViewModel _mainVM;
        public Canvas? parentCanvas;

        public List<PathBaseViewModel> connectedPaths = new List<PathBaseViewModel>();


        public Action<NodeBaseViewModel, Point, PointerPressedEventArgs> OnStartConnectionDragNode { get; internal set; }
        public Action<NodeBaseViewModel, Point, PointerReleasedEventArgs> OnEndConnectionDragNode { get; internal set; }

    }


}
