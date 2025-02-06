using Anti_Recoil_Application.Commands;
using Anti_Recoil_Application.Core.Services;
using Anti_Recoil_Application.Models;
using Anti_Recoil_Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace Anti_Recoil_Application.ViewModels.DialogViewModels
{
    public class SettingsDialogViewModel : EnterFieldDialogViewModel
    {
        private double _mainGameSensitivity;

        public SettingsDialogViewModel(Action onCloseButtonClick, Action? onSubmitButtonClick = null, Action? onResendButtonClick = null) 
            : base(onCloseButtonClick, onSubmitButtonClick, onResendButtonClick)
        {
            // Initialize Commands from the base class command base
            ResetSettingsCommand = new CommandBase(ResetSettings);
            SaveSettingsCommand = new CommandBase(SaveSettings);
            BackCommand = new CommandBase(Back);

            // fatch weapons from settings
            Microsoft.Extensions.Hosting.IHost? appHost = App.AppHost;
            if (appHost == null)
                return;
            var settings = appHost.Services.GetRequiredService<SettingsService>().Settings;
            if (settings != null)
            {
                MainGameSensitivity = settings.GameSensitivity * 100;
                Weapons = new ObservableCollection<WeaponViewModel>();
                foreach (var weapon in settings.Weapons)
                {
                    Weapons.Add(new WeaponViewModel
                    {
                        WeaponName = weapon.WeaponName,
                        Sensitivity = weapon.Sensitivity * 100
                    });
                }
            }
        }



        // Reset Settings Command
        public ICommand ResetSettingsCommand { get; }
        public ICommand SaveSettingsCommand { get; }
        public ICommand BackCommand { get; }
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


        private async void ResetSettings(object obj)
        {
            Microsoft.Extensions.Hosting.IHost? appHost = App.AppHost;
            if (appHost == null)
                return;

            var settings = appHost.Services.GetRequiredService<SettingsService>().Settings;
            if (settings != null) {

                MainGameSensitivity = 90.0f;
                // get weapons from API and set them
                var weapons = await appHost.Services.GetRequiredService<HostProviderService>().GetWeaponsAsync();

                if (weapons != null)
                {
                    // set each weapon sensitivity by weapon name
                    foreach (var weapon in weapons)
                    {
                        var weaponViewModel = Weapons.FirstOrDefault(w => w.WeaponName == weapon.WeaponName);
                        if (weaponViewModel != null)
                        {
                            weaponViewModel.Sensitivity = weapon.Sensitivity * 100;
                        }
                    }
                }
            }
        }

        private void SaveSettings(object obj)
        {
            Microsoft.Extensions.Hosting.IHost? appHost = App.AppHost;
            if (appHost == null)
                return;
            var settings = appHost.Services.GetRequiredService<SettingsService>()?.Settings;
            if (settings != null)
            {
                settings.GameSensitivity = (float)MainGameSensitivity / 100;
                // set each weapon sensitivity by weapon name
                foreach (var weapon in settings.Weapons)
                {
                    var weaponViewModel = Weapons.FirstOrDefault(w => w.WeaponName == weapon.WeaponName);
                    if (weaponViewModel != null)
                    {
                        weapon.Sensitivity = (float)weaponViewModel.Sensitivity / 100;
                    }
                }
                appHost.Services.GetRequiredService<SettingsService>()?.Save();
            }
        }
        private void Back(object obj)
        {
            Microsoft.Extensions.Hosting.IHost? appHost = App.AppHost;
            if (appHost == null)
                return; 
            appHost.Services.GetRequiredService<DialogService>()?.CloseDialog();

        }


    }
}
