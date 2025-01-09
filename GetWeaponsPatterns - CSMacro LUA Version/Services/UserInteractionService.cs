using GetPattern.Models;

namespace GetPattern.Services
{
    public class UserInteractionService
    {
        public bool ConfirmPattern(string pattern)
        {
            Console.WriteLine($"Pattern: {pattern}");
            Console.Write("Is this pattern correct? (yes/no): ");
            string confirmation = Console.ReadLine()?.Trim().ToLower();
            return confirmation == "yes";
        }

        public string GetWeaponName()
        {
            Console.Write("Enter weapon name: ");
            return Console.ReadLine()?.Trim();
        }

        public void DisplayWeapon(Weapon weapon)
        {
            Console.WriteLine($"Weapon Created:\nName: {weapon.WeaponName}\nPattern: {weapon.Pattern}");
        }

        public static string GetUserText(string neededText)
        {
            Console.Write($"{neededText}: ");
            return Console.ReadLine()?.Trim();
        }
    }
}
