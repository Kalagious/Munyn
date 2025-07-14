using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Data;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using Microsoft.VisualBasic;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using static SkiaSharp.HarfBuzz.SKShaper;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Munyn.ViewModels
{
    public partial class HostNodeViewModel : ContextBase
    {

        public void Refresh()
        {
            GetNodePropertyFromName("User Count").PropertyValue = GetNodeCountOfType<UserNodeViewModel>().ToString();
            GetNodePropertyFromName("Service Count").PropertyValue = GetNodeCountOfType<ServiceNodeViewModel>().ToString();

/*            foreach (ViewModelBase node in contextNodes)
            {
                if (node.GetType() == typeof(ServiceNodeViewModel))
                    Services.Add(new ServiceListEntry(((ServiceNodeViewModel)node).NodeName));
                
            }*/

        }

        public HostNodeViewModel() : base()
        {

        }

        public void InitializeProperties()
        {
            AddNodeProperty(new Nodes.Properties.NodePropertyBasic("Os", true, true));
            AddNodeProperty(new Nodes.Properties.NodePropertyBasic("User Count", true, true, false));
            AddNodeProperty(new Nodes.Properties.NodePropertyBasic("Service Count", true, false, false));
            AddNodeProperty(new Nodes.Properties.NodePropertyBasic("Description", true, true));
        }

        public HostNodeViewModel(string name, float x, float y, ContextBase parent, Canvas tmpParentCanvas, MainViewModel mainVM)
        {
            NodeName = name;
            contextName = name;
            X = x;
            Y = y;
            parentCanvas = tmpParentCanvas;
            contextNodes = new ObservableCollection<ViewModelBase>();

            InitializeProperties();
             
            NodeTheme = makeGradient("#19c8f3", "#6551e5");
            _mainVM = mainVM;
            parentContext = parent;
        }

    }
}
