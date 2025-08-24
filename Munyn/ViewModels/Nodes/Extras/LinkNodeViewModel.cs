using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
    public partial class LinkNodeViewModel : ContextBase
    {
        public NodeBaseViewModel targetNode;

        [ObservableProperty]
        private string _iconPath;

        [ObservableProperty]
        private string _targetName;

        [RelayCommand]
        private void GoToLinkedNode()
        {
            if (targetNode.parentContext != mainVM.currentContext)
                mainVM.EnterContext((ContextBase)targetNode);



        }

            public LinkNodeViewModel(string name, float x, float y, ContextBase parent, NodeBaseViewModel targetNodeTmp, Canvas tmpParentCanvas, MainViewModel mainVMIn)
        {
            NodeName = name;
            X = x;
            Y = y;

            targetNode = targetNodeTmp;
            parentCanvas = tmpParentCanvas;
            parentContext = parent;
            mainVM = mainVMIn;

            RefreshLink();
        }

        public void RefreshLink()
        {
            TargetName = targetNode.NodeName;
            NodeName = targetNode.NodeName + "_Link";
            IconName = targetNode.IconName;
            Icon = targetNode.Icon;
            NodeTheme = targetNode.NodeTheme;
        }
    }
}
