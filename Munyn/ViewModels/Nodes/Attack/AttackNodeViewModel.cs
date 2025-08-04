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
    public partial class AttackNodeViewModel : NodeBaseViewModel
    {
        public AttackNodeViewModel(string name, float x, float y, Canvas tmpParentCanvas, ContextBase parent)
        {
            NodeName = name;
            X = x;
            Y = y;

            IconName = "striking-arrows";
            NodeTheme = makeTheme("#FF3333", "#DD2222");

            AddNodeProperty(new Nodes.Properties.NodePropertyCommand { PropertyName = "Command", IsDefault=true, IsEditable=true, IsVisableOnGraphNode = true });
            AddNodeProperty(new Nodes.Properties.NodePropertyText { PropertyName = "Result", IsDefault=true, IsEditable=true});
            AddNodeProperty(new Nodes.Properties.NodePropertyText { PropertyName = "Description", IsDefault = true, IsEditable = true, IsVisableOnGraphNode = true, IconColor = NodeTheme});
            parentCanvas = tmpParentCanvas;
            parentContext = parent;
        }

    }
}
