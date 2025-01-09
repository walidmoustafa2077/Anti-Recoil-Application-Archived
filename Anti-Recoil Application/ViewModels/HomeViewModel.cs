using Anti_Recoil_Application.Commands;
using System.Collections.ObjectModel;

namespace Anti_Recoil_Application.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        private WeaponViewModel _selectedWeapon;

        public ObservableCollection<WeaponViewModel> Weapons { get; set; }

        public CommandBase ActivateWeaponCommand { get; }

        public WeaponViewModel SelectedWeapon
        {
            get => _selectedWeapon;
            set => SetProperty(ref _selectedWeapon, value, nameof(SelectedWeapon));
        }

        public HomeViewModel()
        {
            //Weapons = new ObservableCollection<WeaponViewModel>
            //{
            //    new WeaponViewModel { WeaponName = "AK-47", WeaponImage = "pack://application:,,,/Resources/Images/Weapons/AKM.png", Shortcut = "F1", IsActive = false },
            //    new WeaponViewModel { WeaponName = "M4A1", WeaponImage = "pack://application:,,,/Resources/Images/Weapons/FSCR.png", Shortcut = "F2", IsActive = false },
            //    new WeaponViewModel { WeaponName = "AWP", WeaponImage = "pack://application:,,,/Resources/Images/Weapons/LUIS.png", Shortcut = "F3", IsActive = false },
            //    new WeaponViewModel { WeaponName = "AWP", WeaponImage = "pack://application:,,,/Resources/Images/Weapons/LUIS.png", Shortcut = "F3", IsActive = false }
            //};
            ActivateWeaponCommand = new CommandBase(ActivateWeapon);
        }

        private void ActivateWeapon(object parameter)
        {
            if (parameter is WeaponViewModel weapon)
            {
                if (SelectedWeapon != weapon)
                {
                    SelectedWeapon = weapon;
           
                    foreach (var w in Weapons)
                    {
                        w.IsActive = w == weapon;
                    }
                }
                else if (SelectedWeapon.IsActive)
                {
                    weapon.IsActive = false;
                    // If the weapon is deactivated, reset the SelectedWeapon to null
                    if (!weapon.IsActive)
                    {
                        SelectedWeapon = null;
                    }
                }
                else
                {
                    SelectedWeapon = null;

                }
            }
        }
    }
}
