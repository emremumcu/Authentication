namespace Authentication.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class RegisterViewModel : LoginViewModel
    {
        [DataType(DataType.Password), Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}
