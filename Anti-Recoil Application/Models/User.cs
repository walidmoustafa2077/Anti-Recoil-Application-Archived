using System.Text.Json.Serialization;

namespace Anti_Recoil_Application.Models
{
    public class User
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsEmailConfirmed { get; set; }
        public string Role { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; } = new DateTime(2024, 1, 1);
        public string Gender { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string LicenseType { get; set; } = string.Empty;
        public DateTime EndTrialDate { get; set; } = DateTime.Today;
    }
}
