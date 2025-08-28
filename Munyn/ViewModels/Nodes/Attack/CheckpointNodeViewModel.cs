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
    public partial class CheckpointNodeViewModel : NodeBaseViewModel
    {
        public CheckpointNodeViewModel(string name, float x, float y, Canvas tmpParentCanvas, ContextBase parent)
        {
            NodeName = name;
            X = x;
            Y = y;

            IconName = "start-cog";
            NodeTheme = makeTheme("#FF3333", "#BB5599");

            AddNodeProperty(new Nodes.Properties.NodePropertyCommand { PropertyName = "Command", IsDefault=true, IsEditable=true, IsVisableOnGraphNode = false });
            AddNodeProperty(new Nodes.Properties.NodePropertyText { PropertyName = "Description", IsDefault = true, IsEditable = true, IsVisableOnGraphNode = false, IconColor = NodeTheme});

            parentCanvas = tmpParentCanvas;
            parentContext = parent;
        }

    }
}
