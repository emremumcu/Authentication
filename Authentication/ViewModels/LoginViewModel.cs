namespace Authentication.ViewModels
{
    using Authentication.Models;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class LoginViewModel
    {
        [Display(Name = "Kullanıcı Adı")]
        [Required(ErrorMessage = "Kullanıcı Adınızı giriniz"), DataType(DataType.Text), MaxLength(55)]
        public string Username { get; set; }

        [Display(Name = "Şifre")]
        [Required(ErrorMessage = "Şifrenizi giriniz"), DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Güvenlik Kodu")]
        public CaptchaResult Captcha { get; set; }

        [Display(Name = "Beni Hatırla")]
        public Boolean RememberMe { get; set; }
    }
}
