using AntiRecoil_Tester.Services;
using GetPattern.Models;
using GetPattern.Services;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var patternService = new PatternService();
        var userInteractionService = new UserInteractionService();
        var weaponService = new WeaponService();

        Console.WriteLine("Welcome to the Anti-Recoil Tester!");

        // Get the current application's directory and the full path to the "Pattern" folder and file
        string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string patternFolderPath = Path.Combine(appDirectory, "Pattern");
        string patternFilePath = Path.Combine(patternFolderPath, "Pattern.txt");

        // Check if the "Pattern" folder exists, if not, create it
        if (!Directory.Exists(patternFolderPath))
        {
            Directory.CreateDirectory(patternFolderPath); // Create the directory if it doesn't exist
            Console.WriteLine("Pattern folder created.");
        }

        // Check if the "Pattern.txt" file exists
        if (!File.Exists(patternFilePath))
        {
            // Create the file if it doesn't exist by writing an empty string
            File.WriteAllText(patternFilePath, string.Empty);
            Console.WriteLine("The text file has been created. Please enter the text, save the file, and restart the program.");
            return; // Exit the current execution to let the user add content
        }

        // Read the entire content of the Pattern.txt file
        string code = File.ReadAllText(patternFilePath);

        // Define the function to search for in the code
        string functionName = "MoveMouseRelativeFractional";
        var calls = patternService.GetFunctionCallsWithParameters(code, functionName);

        // Convert the logged pattern into a single comma-separated line
        string pattern = string.Join(",", calls.Select(call =>
        {
            try
            {
                double xValue = patternService.ParseNumericValue(call.Parameters["x"]);
                double yValue = patternService.ParseNumericValue(call.Parameters["y"]);
                return $"{xValue},{yValue}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing parameters for a call: {ex.Message}");
                return string.Empty; // Return empty if parsing fails
            }
        }).Where(result => !string.IsNullOrEmpty(result))); // Ensure we skip empty results

        if (string.IsNullOrEmpty(pattern))
        {
            Console.WriteLine("No valid pattern found.");
            return;
        }

        Console.WriteLine($"Pattern: {pattern}");



        // Get weapon name and check if it already exists in the database
        string weaponName = userInteractionService.GetWeaponName();


        var existingWeapon = await weaponService.GetWeaponByNameAsync(weaponName); // Await the Task here

        if (existingWeapon != null)
        {
            Console.WriteLine($"Weapon '{weaponName}' already exists.");
            Console.WriteLine("Do you want to update it? (yes/no)");
            var updateChoice = Console.ReadLine()?.ToLower();

            if (updateChoice == "yes")
            {
                // Optionally, ask for new sensitivity
                Console.WriteLine("Enter new sensitivity (or press Enter to keep current): ");
                float newSensitivity = existingWeapon.Sensitivity; // Access the sensitivity of the existing weapon
                string sensitivityInput = Console.ReadLine();

                if (!string.IsNullOrEmpty(sensitivityInput) && float.TryParse(sensitivityInput, out var parsedSensitivity))
                {
                    newSensitivity = parsedSensitivity;
                }

                var updatedWeapon = new Weapon
                {
                    WeaponName = weaponName,
                    Sensitivity = newSensitivity,
                    Pattern = pattern
                };

                // Call update method (this method needs to be implemented in WeaponService)
                await weaponService.UpdateWeaponAsync(updatedWeapon);
            }
            else
            {
                Console.WriteLine($"Weapon '{weaponName}' was not updated.");
            }
        }
        else
        {
            // Create new weapon if it doesn't exist
            Console.WriteLine("Enter sensitivity for the new weapon (or press Enter to skip): ");
            string sensitivityInput = Console.ReadLine();
            float sensitivity = 0; // Default sensitivity

            if (!string.IsNullOrEmpty(sensitivityInput) && float.TryParse(sensitivityInput, out var parsedSensitivity))
            {
                sensitivity = parsedSensitivity;
            }

            var newWeapon = new Weapon
            {
                WeaponName = weaponName,
                Sensitivity = sensitivity,
                Pattern = pattern
            };

            // Call method to add the weapon (this method needs to be implemented in WeaponService)
            await weaponService.AddWeaponAsync(newWeapon);
        }

        Console.ReadLine();
    }
}