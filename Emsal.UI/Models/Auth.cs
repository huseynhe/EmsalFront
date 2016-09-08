using System.ComponentModel.DataAnnotations;

namespace Emsal.UI.Models
{
    public class AuthLogin : UserRoles
    {
        [Required(ErrorMessage = "İstifadəçi adı daxil edilməmişdir")]
        [Display(Name = "İstifadəçi adı")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Şifrə daxil edilməmişdir")]
        [Display(Name = "Şifrə")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        
        public string uid { get; set; }
        public string type { get; set; }
    }
}