using System.ComponentModel.DataAnnotations;

namespace DulcesDuendesApp.Common.Validation
{
    public class SoloLetrasAttribute : RegularExpressionAttribute
    {
        public SoloLetrasAttribute() : base(@"^[a-zA-ZÀ-ÿñÑ\s]{2,}$")
        {
            ErrorMessage = MensajesValidacion.SoloLetras;
        }
    }
}
