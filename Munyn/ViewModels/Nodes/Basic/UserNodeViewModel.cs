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
    public partial class UserNodeViewModel : NodeBaseViewModel
    {

        public UserNodeViewModel(string name, float x, float y, Canvas tmpParentCanvas)
        {
            NodeName = name;
            X = x;
            Y = y;

            AddNodeProperty(new Nodes.Properties.NodePropertyBasic("Groups", true, true));
            AddNodeProperty(new Nodes.Properties.NodePropertyBasic("Password", true, true));
            AddNodeProperty(new Nodes.Properties.NodePropertyBasic("Description", true, true));

            IconName = "person-bounding-box";
            NodeTheme = makeTheme("#ef3f79", "#d511d5");


            parentCanvas = tmpParentCanvas;
        }

    }
}
