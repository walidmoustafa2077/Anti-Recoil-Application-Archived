using Anti_Recoil_Application.Core.Handlers;
using Anti_Recoil_Application.Models;
using Anti_Recoil_Application.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Numerics;
using System.Windows;
using static System.Formats.Asn1.AsnWriter;

namespace Anti_Recoil_Application.Core.Services
{
    public class CoreService
    {
        private readonly HomeViewModel _homeViewModel;
        private readonly SettingsService _settingsService;
        private readonly InputHandler _inputHandler;


        public bool IsActive
        {
            get => _homeViewModel.IsActivateChecked;
            set
            {
                _homeViewModel.IsActivateChecked = value;

                // Example: Update a UI component (PatternIndexLabel) from a non-UI thread
                Application.Current.Dispatcher.Invoke(() =>
                {
                    // Assuming you're updating a UI element like a TextBox or Label
                    // Ensure all UI component updates happen on the UI thread
                    _homeViewModel.IsActivateChecked = value;
                });
            }
        }

        public Weapon SelectedWeapon
        {
            get
            {
                var weapon = _settingsService.Settings.Weapons.FirstOrDefault(w => w.WeaponName == _homeViewModel.SelectedWeapon.WeaponName);
                if (weapon == null)
                    return new Weapon();

                return weapon;
            }
            set
            {
                var weapon = _homeViewModel.Weapons.FirstOrDefault(w => w.WeaponName == value.WeaponName);
                if (weapon == null)
                    return;
                _homeViewModel.SelectedWeapon = weapon;

                // Example: Update a UI component (PatternIndexLabel) from a non-UI thread
                Application.Current.Dispatcher.Invoke(() =>
                {
                    // Assuming you're updating a UI element like a TextBox or Label
                    // Ensure all UI component updates happen on the UI thread
                    _homeViewModel.SelectedWeapon = weapon;
                });
            }
        }

        public Weapon[] Weapons
        {
            get => _settingsService.Settings.Weapons.ToArray();
        }


        // Constants representing the fire rate of the weapon
        private int fireRate = 17;

        // Thread for handling recoil
        private Thread _recoilThread;

        // Constants representing the current pattern index
        private int _currentPatternIndex = 0;

        private CancellationTokenSource _cancellationTokenSource;

        public CoreService(HomeViewModel homeViewModel)
        {
            // get the settings service from the AppHost DI container
            Microsoft.Extensions.Hosting.IHost? appHost = App.AppHost;
            if (appHost == null)
            {
                throw new ArgumentNullException(nameof(appHost));
            }
            _settingsService = appHost.Services.GetRequiredService<SettingsService>();
            _settingsService.Load();
            _inputHandler = new InputHandler(this);
            _homeViewModel = homeViewModel;
        }

        public void InitializeWeapons(List<Weapon> weapons)
        {
            // Add Weapon Patterns to same weapon in settings
            foreach (var weapon in weapons)
            {
                var existingWeapon = _settingsService.Settings.Weapons.FirstOrDefault(w => w.WeaponName == weapon.WeaponName);
                if (existingWeapon != null)
                {
                    existingWeapon.Pattern = weapon.Pattern;
                }
                else
                {
                    weapon.Pattern = string.Empty;
                    _settingsService.Settings.Weapons.Add(weapon);
                }
            }
            _settingsService.Save();
        }

        public void UpdateLoop()
        {
            StartRecoilLoop();
        }

        public void StartRecoilLoop()
        {
            _recoilThread = new Thread(RecoilLoop);
            _recoilThread.SetApartmentState(ApartmentState.STA); // Ensure thread is STA
            _recoilThread.Start();
        }

        private void RecoilLoop()
        {
            while (true)
            {
                _inputHandler.Update();

                if (_inputHandler.Fire && IsActive && SelectedWeapon != null)
                {
                    // Blocking call here simulates the delay
                    WaitForMilliseconds(fireRate);
                    HandleRecoil();
                }
                else
                {
                    _currentPatternIndex = 0;
                }
            }
        }

        private void HandleRecoil()
        {
            Vector2[] pattern = SelectedWeapon.ConvertPattern().ToArray();
            if (_currentPatternIndex < pattern.Length)
            {
                // Get the recoil vector for the current pattern
                Vector2 recoilVector = pattern[_currentPatternIndex] * _settingsService.Settings.GameSensitivity * SelectedWeapon.Sensitivity;

                // Adjust mouse movement based on the recoil vector
                _inputHandler.MoveMouseRelativeFractional(recoilVector.X, recoilVector.Y);

                _currentPatternIndex++;
            }
        }


        private void WaitForMilliseconds(int time)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            while (stopwatch.ElapsedMilliseconds <= time)
            {
                // Do nothing, just wait

            }
            stopwatch.Stop();
        }
    }
}
