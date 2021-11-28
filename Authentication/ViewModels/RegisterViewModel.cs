namespace Authentication.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class RegisterViewModel : LoginViewModel
    {
        [Display(Name = "Şifrenizi Tekrarlayın")]
        [DataType(DataType.Password), Compare(nameof(Password), ErrorMessage = "Şifreler eşleşmiyor")]
        public string ConfirmPassword { get; set; }
    }
}
