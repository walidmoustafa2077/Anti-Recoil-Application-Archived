using Anti_Recoil_Application.Commands;
using System.Windows.Input;

namespace Anti_Recoil_Application.ViewModels.DialogViewModels
{
    public class EnterFieldDialogViewModel : MainDialogViewModel
    {
        // Add username property
        private string _mainField;
        private string _secondField;
        private string _submitButtonText;

        public string MainField
        {
            get => _mainField;
            set => SetProperty(ref _mainField, value, nameof(_mainField));
        }

        public string SecondField
        {
            get => _secondField;
            set => SetProperty(ref _mainField, value, nameof(_secondField));
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
