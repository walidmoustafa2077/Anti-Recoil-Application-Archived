using Anti_Recoil_Application.Commands;
using System.Windows.Input;

namespace Anti_Recoil_Application.ViewModels.DialogViewModels
{
    public class EnterFieldDialogViewModel : MainDialogViewModel
    {
        // Add username property
        private string _mainField = string.Empty;
        private string _secondField = string.Empty;
        private string _mainFieldWatermarkText = string.Empty;
        private string _submitButtonText = "Confirm";

        public string MainField
        {
            get => _mainField;
            set => SetProperty(ref _mainField, value, nameof(MainField));
        }

        public string SecondField
        {
            get => _secondField;
            set => SetProperty(ref _secondField, value, nameof(SecondField));
        }
        
        public string MainFieldWatermarkText
        {
            get => _mainFieldWatermarkText;
            set => SetProperty(ref _mainFieldWatermarkText, value, nameof(MainFieldWatermarkText));
        }

        public string SubmitButtonText
        {
            get => _submitButtonText;
            set => SetProperty(ref _submitButtonText, value, nameof(SubmitButtonText));
        }

        // Submit command
        public ICommand SubmitCommand { get; }

        public EnterFieldDialogViewModel(Action onButtonClick, Action onSubmitButtonClick)
            : base(onButtonClick)
        {
            SubmitCommand = new CommandBase(_ => onSubmitButtonClick?.Invoke());
        }
    }
}
