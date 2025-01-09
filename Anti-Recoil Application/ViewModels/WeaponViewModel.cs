using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anti_Recoil_Application.ViewModels
{
    public class WeaponViewModel : ViewModelBase
    {
        private bool _isActive;

        public string WeaponName { get; set; } // Name of the weapon
        public string WeaponImage { get; set; } // Image of the weapon
        public string Shortcut { get; set; } // Shortcut for the weapon (e.g., "F3")

        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value, nameof(IsActive));
        }
    }
}
