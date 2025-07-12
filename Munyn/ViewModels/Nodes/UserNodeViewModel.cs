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
    public partial class UserNodeViewModel : NodeBaseViewModel
    {

        [ObservableProperty]
        private string _UserRole;
        [ObservableProperty]
        private string? _UserPassword;
        [ObservableProperty]
        private string _UserDetails;

        

        public UserNodeViewModel(string name, float x, float y, Canvas tmpParentCanvas)
        {
            NodeName = name;
            X = x;
            Y = y;

            NodeTheme = makeGradient("#ef3f79", "#d511d5");

            parentCanvas = tmpParentCanvas;
        }

    }
}
