
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
            _mainVM.EnterContext(context);
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
            else if (contextTmp is NetworkNodeViewModel)
            {
                IconPath = "avares://Munyn/Assets/Icons/neural-network-black-icon.svg";
                ArrowVisible = true;
            }

        }

    }
}
