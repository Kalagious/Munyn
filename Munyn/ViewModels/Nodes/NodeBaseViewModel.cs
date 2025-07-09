using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

        [ObservableProperty]
        private bool _editName = false;

        [RelayCommand]
        private void ToggleEditName() { EditName = !EditName; }

        public MainViewModel _mainVM;
        public Canvas? parentCanvas;

        public List<PathBaseViewModel> connectedPaths = new List<PathBaseViewModel>();


        public Action<NodeBaseViewModel, Point, PointerPressedEventArgs> OnStartConnectionDragNode { get; internal set; }
        public Action<NodeBaseViewModel, PointerReleasedEventArgs> OnClickedNode { get; internal set; }


    }


}
