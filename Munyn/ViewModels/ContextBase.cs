using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Munyn.ViewModels
{
    public partial class ContextBase : NodeBaseViewModel
    {
        public ObservableCollection<ViewModelBase>? contextNodes;
        public String? contextName;
        public ContextBase? parentContext;

        protected MainViewModel _mainVM;
        public ContextBase()
        {
            contextNodes = new ObservableCollection<ViewModelBase>();
        }
    }
}
