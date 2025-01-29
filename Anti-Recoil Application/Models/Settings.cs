namespace Anti_Recoil_Application.Models
{
    public class Settings
    {
        public string Token { get; set; } = string.Empty;

        // Properties to hold game sensitivity and weapon list
        public float GameSensitivity { get; set; } = 1.0f;

        public List<Weapon> Weapons { get; set; } = new List<Weapon>();
    }
}
