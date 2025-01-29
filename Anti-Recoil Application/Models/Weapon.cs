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

        public List<Vector2> ConvertPattern()
        {
            // Check if the pattern is null or empty
            if (string.IsNullOrEmpty(Pattern))
            {
                return new List<Vector2>();
            }

            // Assuming the pattern data is a flat list of coordinates like "2,4,2,4,2,4,..."
            var coordinates = new List<Vector2>();

            // Split the data by commas (e.g., 2,4,2,4,...)
            var parts = Pattern.Split(',');

            // Iterate through the parts in pairs (x,y)
            for (int i = 0; i < parts.Length; i += 2)
            {
                // Parse x and y values and create a new Vector2
                if (i + 1 < parts.Length)
                {
                    float x = float.Parse(parts[i]);
                    float y = float.Parse(parts[i + 1]);
                    coordinates.Add(new Vector2(x, y));
                }
            }

            return coordinates;
        }

    }
}
