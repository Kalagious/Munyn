using Avalonia.Controls;
using Avalonia.Media;
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
        private bool isLinked = false;

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


        public void LinkToNode(NodeBaseViewModel newTarget)
        {
            targetNode = newTarget;
            RefreshLink();
        }

        public void RefreshLink()
        {
            if (targetNode == null)
            {
                IsLinked = false;
                TargetName = "Unlinked";
                NodeName = "Unlinked_Link";
                return;
            }

            IsLinked = true;
            TargetName = targetNode.NodeName;
            NodeName = targetNode.NodeName + "_Link";
            NodeTheme = targetNode.NodeTheme;
        }
    }
}
