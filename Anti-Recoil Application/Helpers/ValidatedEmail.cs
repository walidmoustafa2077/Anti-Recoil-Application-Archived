using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anti_Recoil_Application.Helpers
{
    public static class ValidatedEmail
    {
        public static bool IsValidEmail(string email)
        {
            // Define a list of allowed domains
            var allowedDomains = new List<string> { "gmail.com", "outlook.com", "icloud.com" };

            // Validate email format using a simple regex
            var emailRegex = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");

            if (string.IsNullOrEmpty(email) || !emailRegex.IsMatch(email))
            {
                return false; // Invalid email format
            }

            // Extract the domain from the email
            var emailDomain = email.Split('@')[1].ToLower();

            // Check if the domain is in the allowed list
            return allowedDomains.Contains(emailDomain);
        }

    }
}
