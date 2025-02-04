namespace Anti_Recoil_Application.ViewModels
{
    public class WeaponViewModel : ViewModelBase
    {
        private bool _isActive;
        private double _sensitivity;

        public string WeaponName { get; set; } // Name of the weapon
        public string WeaponImage { get; set; } // Image of the weapon
        public string Shortcut { get; set; } // Shortcut for the weapon (e.g., "F3")

        public double Sensitivity
        {
            get { return _sensitivity; }
            set
            {
                if (_sensitivity != value)
                {
                    _sensitivity = value;
                    OnPropertyChanged(nameof(Sensitivity));
                }
            }
        }
        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value, nameof(IsActive));
        }
    }
}
