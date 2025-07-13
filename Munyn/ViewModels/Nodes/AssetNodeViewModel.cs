using Avalonia.Controls;
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

        [ObservableProperty]
        private string? _assetLocation;

        [ObservableProperty]
        private string _assetImportantInfo;

        [ObservableProperty]
        private string _assetFullInfo;


        public AssetNodeViewModel(string name, float x, float y, Canvas tmpParentCanvas)
        {
            NodeName = name;
            X = x;
            Y = y;

            NodeTheme = makeGradient("#eab900", "#fa8700");

            AddNodeProperty(new Nodes.Properties.NodePropertyBasic("Location", true, true));
            AddNodeProperty(new Nodes.Properties.NodePropertyBasic("Description", true, true));

            parentCanvas = tmpParentCanvas;
        }

    }
}
