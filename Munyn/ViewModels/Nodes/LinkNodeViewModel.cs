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
        public ContextBase targetContext;
        [ObservableProperty]
        private string _contextName;

        [ObservableProperty]
        private string _iconPath;

        [RelayCommand]
        private void EnterLinkedContextButton()
        {
            _mainVM.EnterContext(targetContext);
        }

        public LinkNodeViewModel(string name, float x, float y, ContextBase parent, ContextBase targetContextTmp, Canvas tmpParentCanvas, MainViewModel mainVM)
        {
            ContextName = name;
            X = x;
            Y = y;
            parentCanvas = tmpParentCanvas;
            targetContext = targetContextTmp;
            _mainVM = mainVM;
        }
    }
}
