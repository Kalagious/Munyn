using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Munyn.ViewModels;

namespace Munyn.ViewModels
{
    public partial class ContextBase : NodeBaseViewModel
    {
        public ObservableCollection<ViewModelBase>? contextNodes;
        public String? contextName;
        public ContextBase? parentContext;
        

        [RelayCommand]
        private void EnterContextButton()
        {
            contextName = NodeName;
            _mainVM.EnterContext(this);
        }



        public ContextBase()
        {
            contextNodes = new ObservableCollection<ViewModelBase>();
            IsContext = true;  
        }

        public int GetNodeCountOfType<T>()
        {
            int count = 0;
            for (int i = 0; i < contextNodes.Count; i++)
            {
                if (contextNodes.ElementAt(i).GetType() == typeof(T)) count++;
            }
            return count;
        }
    }
}
