using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DulcesDuendesApp.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido.")]
        [MaxLength(256)]
        public string Email { get; set; } = string.Empty;

        [BindNever]
        [MaxLength(300)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los {1} caracteres.")]
        [RegularExpression(@"^[a-zA-ZÀ-ÿñÑ\s]{2,}$", ErrorMessage = "El nombre debe tener al menos 2 caracteres y no puede contener números ni símbolos.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(100, ErrorMessage = "El apellido no puede superar los {1} caracteres.")]
        [RegularExpression(@"^[a-zA-ZÀ-ÿñÑ\s]{2,}$", ErrorMessage = "El apellido debe tener al menos 2 caracteres y no puede contener números ni símbolos.")]
        public string Apellido { get; set; } = string.Empty;

        public string? AvatarUrl { get; set; }

        [Required]
        [Display(Name = "Rol")]
        public int RolId { get; set; }

        [ForeignKey(nameof(RolId))]
        [BindNever]
        public Rol? Rol { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; }

        [NotMapped]
        public string? Password { get; set; }
    }
}
