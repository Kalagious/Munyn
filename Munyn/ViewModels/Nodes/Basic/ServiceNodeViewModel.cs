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
        public ServiceNodeViewModel(string name, float x, float y, Canvas tmpParentCanvas, ContextBase parent)
        {
            NodeName = name;
            X = x;
            Y = y;

            IconName = "database-fill-gear";
            NodeTheme = makeTheme("#b613d0", "#6524f3");


            AddNodeProperty(new Nodes.Properties.NodePropertyBasic("Location", true, true));
            AddNodeProperty(new Nodes.Properties.NodePropertyBasic("Version", true, true, false));
            AddNodeProperty(new Nodes.Properties.NodePropertyText { PropertyName = "Description", IsDefault = true, IsEditable = true, IsVisableOnGraphNode = true, IconColor = NodeTheme });

            parentCanvas = tmpParentCanvas;
            parentContext = parent;
        }

    }
}
