using System.Numerics;
using System.Text.Json.Serialization;

namespace Anti_Recoil_Application.Models
{
    public class Weapon
    {
        [JsonPropertyName("weaponName")]
        public string WeaponName { get; set; } = string.Empty;

        [JsonPropertyName("sensitivity")]
        public float Sensitivity { get; set; } = 0;

        [JsonPropertyName("pattern")]
        public string Pattern { get; set; } = string.Empty;

        public static List<Vector2> ConvertPattern(string pattern)
        {
            return pattern.Split(';') // Assuming ";" separates x,y pairs
                          .Select(coord =>
                          {
                              var parts = coord.Split(',');
                              return new Vector2(float.Parse(parts[0]), float.Parse(parts[1]));
                          })
                          .ToList();
        }
    }
}
