using System.ComponentModel.DataAnnotations;

namespace Galaxy.Security.UI.Models
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "El campo usuario es obligatorio")]
        public string UserName { get; set; } = default!;
        [Required(ErrorMessage = "El campo contraseña es obligatorio")]
        public string Password { get; set; } = default!;
    }
}
