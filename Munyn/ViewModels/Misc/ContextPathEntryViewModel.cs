
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;



namespace Munyn.ViewModels
{
    public partial class ContextPathEntryViewModel : ViewModelBase
    {
        
        [ObservableProperty]
        private string _contextName;

        [ObservableProperty]
        private StreamGeometry icon;

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
            Icon = context.ContextIcon;

        }
    }
}
