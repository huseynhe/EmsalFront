using System.ComponentModel.DataAnnotations;

namespace Emsal.AdminUI.Models
{
    public class AuthLogin
    {
        [Required(ErrorMessage = "İstifadəçi adı daxil edilməmişdir")]
        [Display(Name = "İstifadəçi Adı")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Şifrə daxil edilməmişdir")]
        [Display(Name = "Şifrə")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}