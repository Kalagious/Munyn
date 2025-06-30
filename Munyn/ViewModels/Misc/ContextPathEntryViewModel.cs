
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;



namespace Munyn.ViewModels
{
    public partial class ContextPathEntryViewModel : ViewModelBase
    {
        
        [ObservableProperty]
        private string _contextName;

        [ObservableProperty]
        private string _iconPath;

        [ObservableProperty]
        private bool _arrowVisible = true;

        public ContextBase context;
        private MainViewModel _mainVM;
        
        [RelayCommand]
        private void EnterContextButton()
        {
            
            int contextPathi = _mainVM.ContextPathList.IndexOf(this);
            int pathLength = _mainVM.ContextPathList.Count;
            for (int i = contextPathi+1; i < pathLength; i++)
                _mainVM.ContextPathList.RemoveAt(contextPathi + 1);
            

            _mainVM.EnterContext(context, false);
        }
        public ContextPathEntryViewModel(ContextBase contextTmp, MainViewModel mainVM)
        {
            context = contextTmp;
            ContextName = context.contextName;
            _mainVM = mainVM;


            if (contextTmp is HostNodeViewModel)
                IconPath = "avares://Munyn/Assets/Icons/pc-display.svg";
            else if (contextTmp is ContextBase && context.contextName == "Root")
            {
                IconPath = "avares://Munyn/Assets/Icons/sitemap-icon.svg";
                ArrowVisible = false;
            }
                
        }

    }
}
