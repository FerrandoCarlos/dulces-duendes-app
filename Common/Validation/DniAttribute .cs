using System.ComponentModel.DataAnnotations;

namespace DulcesDuendesApp.Common.Validation
{
    public class DniAttribute : RegularExpressionAttribute
    {
        public DniAttribute() : base(@"^\d{7,8}$")
        {
            ErrorMessage = MensajesValidacion.DniInvalido;
        }
    }
}
