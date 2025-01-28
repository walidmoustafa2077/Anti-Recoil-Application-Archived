using Anti_Recoil_Application.Commands;
using Anti_Recoil_Application.Services;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace Anti_Recoil_Application.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        private readonly HostProviderService _hostProviderService;
        private readonly DialogService _dialogService;

        private WeaponViewModel _selectedWeapon = new WeaponViewModel();

        public ObservableCollection<WeaponViewModel> Weapons { get; set; } = new ObservableCollection<WeaponViewModel>();

        public CommandBase ActivateWeaponCommand { get; }

        public WeaponViewModel SelectedWeapon
        {
            get => _selectedWeapon;
            set
            {
                if (SetProperty(ref _selectedWeapon, value, nameof(SelectedWeapon)) && value != null)
                {
                    // Set IsActive to true when SelectedWeapon is not null
                    // Update activation status for all weapons
                    foreach (var w in Weapons)
                    {
                        w.IsActive = w == value;
                    }
                }
            }
        }


        public HomeViewModel(DialogService dialogService, HostProviderService hostProviderService)
        {
            _dialogService = dialogService;
            _hostProviderService = hostProviderService;

            ActivateWeaponCommand = new CommandBase(ActivateWeapon);

        }

        public async void LoadWeaponsAsync()
        {
            try
            {
                var weapons = await _hostProviderService.GetWeaponsAsync();
                Weapons.Clear();
                for (int i = 0; i < weapons.Count; i++)
                {
                    var weapon = weapons[i];
                    Weapons.Add(new WeaponViewModel
                    {
                        WeaponName = weapon.WeaponName,
                        WeaponImage = $"pack://application:,,,/Resources/Images/Weapons/{weapon.WeaponName}.png",
                        Shortcut = $"F{i + 1}", // Assign shortcut keys based on the order (F1, F2, etc.)
                        IsActive = false
                    });
                }
            }
            catch (Exception ex)
            {
                // Log the error or show a dialog to the user
                _dialogService.ShowDialog(ex.Message);
            }
        }


        private void ActivateWeapon(object parameter)
        {
            if (parameter is WeaponViewModel weapon)
            {
                if (SelectedWeapon != weapon)
                {
                    SelectedWeapon = weapon;

                    // Update activation status for all weapons
                    foreach (var w in Weapons)
                    {
                        w.IsActive = w == weapon;
                    }
                }
                else if (SelectedWeapon.IsActive)
                {
                    // Deactivate the selected weapon
                    SelectedWeapon.IsActive = false;
                    SelectedWeapon = null;
                }
            }
        }
    }
}
