using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
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
    public partial class ServiceNodeViewModel : NodeBaseViewModel
    {
        public ServiceNodeViewModel(string name, float x, float y, Canvas tmpParentCanvas)
        {
            NodeName = name;
            X = x;
            Y = y;

            AddNodeProperty(new Nodes.Properties.NodePropertyBasic("Location", true, true));
            AddNodeProperty(new Nodes.Properties.NodePropertyBasic("Version", true, true, false));
            AddNodeProperty(new Nodes.Properties.NodePropertyBasic("Description", true, true));

            Icon = (StreamGeometry)Application.Current.Resources["database-fill-gear"];


            NodeTheme = makeGradient("#b613d0", "#6524f3");

            parentCanvas = tmpParentCanvas;
        }

    }
}
