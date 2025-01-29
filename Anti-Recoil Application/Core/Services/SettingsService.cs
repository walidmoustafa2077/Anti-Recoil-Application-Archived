using Anti_Recoil_Application.Models;
using System.IO;
using System.Text.Json;

namespace Anti_Recoil_Application.Core.Services
{
    public class SettingsService
    {
        // Path to the application directory and settings file
        private string AppDataPath = Directory.GetCurrentDirectory();
        private string ConfigFilePath => Path.Combine(AppDataPath, "settings.json");

        public Settings Settings { get; set; } = new Settings();

        // Constructor to ensure AppData directory exists
        public SettingsService()
        {
            // Ensure that the AppData directory exists
            if (!Directory.Exists(AppDataPath))
            {
                Directory.CreateDirectory(AppDataPath);
            }
        }


        // Load the settings from a JSON file
        public void Load()
        {

            // Check if the settings file exists
            if (File.Exists(ConfigFilePath))
            {
                // If the file exists, read and deserialize it
                string json = File.ReadAllText(ConfigFilePath);
                
                var settings = JsonSerializer.Deserialize<Settings>(json) ?? new Settings();
                Settings = settings;
            }
            else
            {
                // If the file doesn't exist, create a new one with default settings
                Save();  // Save the default settings to the file
            }
        }

        // Update specific settings (Token, GameSensitivity, or Weapons)
        public void UpdateSettings(string token = null, float? gameSensitivity = null, string weaponName = null, float? sensitivity = null)
        {
            // Update the token if provided
            if (token != null)
            {
                Settings.Token = token;
            }

            // Update the game sensitivity if provided
            if (gameSensitivity.HasValue)
            {
                Settings.GameSensitivity = gameSensitivity.Value;
            }

            // Update weapon sensitivity if a weapon name and sensitivity are provided
            if (weaponName != null && sensitivity.HasValue)
            {
                var weapon = Settings.Weapons.FirstOrDefault(w => w.WeaponName == weaponName);
                if (weapon != null)
                {
                    weapon.Sensitivity = sensitivity.Value;
                }
                else
                {
                    // If the weapon doesn't exist, add it
                    Settings.Weapons.Add(new Weapon
                    {
                        WeaponName = weaponName,
                        Sensitivity = sensitivity.Value
                    });
                }
            }

            // Save the updated settings to the file
            Save();
        }

        // Save the settings to a JSON file
        public void Save()
        {
            // create instance of this settings service 
            var settings = new Settings();

            // set the token
            settings.Token = Settings.Token;

            // set the game sensitivity
            settings.GameSensitivity = Settings.GameSensitivity;

            // get all weapons from the weapon list excluding pattern
            settings.Weapons = Settings.Weapons.Select(w => new Weapon
            {
                WeaponName = w.WeaponName,
                Sensitivity = w.Sensitivity
            }).ToList();

            string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ConfigFilePath, json);
        }
    }
}
