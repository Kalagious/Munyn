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

        

        public UserNodeViewModel(string name, string role, string details, float x, float y, Canvas tmpParentCanvas)
        {
            NodeName = name;
            UserRole = role;
            UserDetails = details;
            X = x;
            Y = y;
            parentCanvas = tmpParentCanvas;
        }

    }
}
