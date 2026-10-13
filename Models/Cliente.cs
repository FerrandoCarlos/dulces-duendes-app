using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DulcesDuendesApp.Models
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El DNI es obligatorio.")]
    [MaxLength(8)]
    public string Dni { get; set; } ? string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength(30)]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [MaxLength(30)]
    public string Apellido { get; set; } = string.Empty;

    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [MaxLength(15)]
    public string Telefono { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Direccion { get; set; } = string.Empty;

    public decimal Monto { get; set; } = 0;

    public bool Activo { get; set; } = true;
}