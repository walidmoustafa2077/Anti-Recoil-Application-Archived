using System.Text.Json.Serialization;

namespace GetPattern.Models
{
    public class Weapon
    {
        [JsonPropertyName("weaponName")]
        public string WeaponName { get; set; }

        [JsonPropertyName("sensitivity")]
        public float Sensitivity { get; set; }

        [JsonPropertyName("pattern")]
        public string Pattern { get; set; }

    }
}
