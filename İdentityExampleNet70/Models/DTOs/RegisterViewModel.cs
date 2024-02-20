using System.ComponentModel.DataAnnotations;

namespace İdentityExampleNet70.Models.DTOs
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage ="Email eksik ya da hatalı")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage ="Lütfen Şifrenizi Girin")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Şifre Eşleşmedi")]
        public string ConfirmPassword { get; set; }


        [Required(ErrorMessage ="Şirket Adı Zorunlu")]
       
        public string? CompanyName { get; set; }
    }
}
