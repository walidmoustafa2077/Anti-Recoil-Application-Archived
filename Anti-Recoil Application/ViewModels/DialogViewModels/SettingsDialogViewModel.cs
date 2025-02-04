using Anti_Recoil_Application.Commands;
using Anti_Recoil_Application.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace Anti_Recoil_Application.ViewModels.DialogViewModels
{
    public class SettingsDialogViewModel : EnterFieldDialogViewModel
    {
        private double _mainGameSensitivity;
        public SettingsDialogViewModel(Action onCloseButtonClick, Action? onSubmitButtonClick = null, Action? onOptionalButtonClick = null,
            ObservableCollection<WeaponViewModel>? weapons = null)
            : base(onCloseButtonClick, onSubmitButtonClick, onOptionalButtonClick)
        {
            ResetSettingsCommand = new CommandBase(_ => onOptionalButtonClick?.Invoke());

            if (weapons != null)
            {
                Weapons = weapons;
            }
        }

        // Reset Settings Command
        public ICommand ResetSettingsCommand { get; }
        public ObservableCollection<WeaponViewModel> Weapons { get; set; }

        public double MainGameSensitivity
        {
            get { return _mainGameSensitivity; }
            set
            {
                if (_mainGameSensitivity != value)
                {
                    _mainGameSensitivity = value;
                    OnPropertyChanged(nameof(MainGameSensitivity)); // Notify UI of changes
                }
            }
        }



    
    }
}
