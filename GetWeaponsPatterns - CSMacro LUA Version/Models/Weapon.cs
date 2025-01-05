using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetPattern.Models
{
    public class Weapon
    {
        public string WeaponName { get; set; } = string.Empty;

        public float Sensitivity { get; set; }
        public string Pattern { get; set; } = string.Empty;

    }
}
