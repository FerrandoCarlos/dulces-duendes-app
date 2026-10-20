using System.ComponentModel.DataAnnotations;

namespace DulcesDuendesApp.Common.Validation
{
    public class CodigoBarraAttribute : RegularExpressionAttribute
    {
        public CodigoBarraAttribute() : base(@"^\d{8}$|^\d{12,14}$")
        {
            ErrorMessage = MensajesValidacion.CodigoBarraInvalido;
        }
    }
}
