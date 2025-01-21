using Anti_Recoil_Application.ViewModels.DialogViewModels;
using System.Windows.Controls;

namespace Anti_Recoil_Application.UserControls.Dialogs
{
    /// <summary>
    /// Interaction logic for EnterUsernameDialog.xaml
    /// </summary>
    public partial class EnterFieldDialog : UserControl
    {
        public EnterFieldDialog(MainDialogViewModel mainDialogViewModel)
        {
            DataContext = mainDialogViewModel;

            InitializeComponent();
        }


    }
}
