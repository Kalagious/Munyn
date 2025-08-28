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
    public partial class AssetNodeViewModel : NodeBaseViewModel
    {

        public AssetNodeViewModel(string name, float x, float y, Canvas tmpParentCanvas, ContextBase parent)
        {
            NodeName = name;
            X = x;
            Y = y;

            IconName = "gold-icon";
            NodeTheme = makeTheme("#eab900", "#fa8700");

            AddNodeProperty(new Nodes.Properties.NodePropertyBasic("Location"));
            AddNodeProperty(new Nodes.Properties.NodePropertyText { PropertyName = "Description", IsDefault = true, IsEditable = true, IsVisableOnGraphNode = false, IconColor = NodeTheme });


            parentCanvas = tmpParentCanvas;
            parentContext = parent;
        }

    }
}
