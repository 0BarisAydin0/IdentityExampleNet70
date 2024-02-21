using System.ComponentModel.DataAnnotations;

namespace İdentityExampleNet70.Models.DTOs
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
