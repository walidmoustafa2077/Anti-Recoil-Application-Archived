using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anti_Recoil_Application.Helpers
{
    public static class PasswordHelper
    {
        public static bool IsPasswordStrong(string password)
        {
            // Minimum length of 8 characters
            const int minLength = 8;

            // Regex patterns to check for uppercase, lowercase, digit, and special character
            var hasUpperCase = new System.Text.RegularExpressions.Regex("[A-Z]");
            var hasLowerCase = new System.Text.RegularExpressions.Regex("[a-z]");
            var hasDigit = new System.Text.RegularExpressions.Regex("[0-9]");
            var hasSpecialChar = new System.Text.RegularExpressions.Regex("[!@#$%^&*(),.?\":{}|<>]");

            // Ensure the password meets the strength criteria
            return password.Length >= minLength &&
                   hasUpperCase.IsMatch(password) &&
                   hasLowerCase.IsMatch(password) &&
                   hasDigit.IsMatch(password) &&
                   hasSpecialChar.IsMatch(password);
        }

    }
}
