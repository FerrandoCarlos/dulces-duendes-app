namespace DulcesDuendesApp.Common.Validation
{
    public static class MensajesValidacion
    {
        public const string Requerido = "El campo {0} es obligatorio.";
        public const string LongitudMaxima = "El campo {0} no puede superar los {1} caracteres.";
        public const string FormatoInvalido = "El formato del campo {0} no es válido.";
        public const string SoloLetras = "El campo {0} debe tener al menos 2 caracteres y no puede contener números ni símbolos.";
        public const string Seleccionar = "Seleccioná una opción en el campo {0}.";
        public const string NoNegativo = "El campo {0} no puede ser negativo.";
        public const string FueraDeRango = "El campo {0} debe estar entre {1} y {2}.";
        public const string CodigoBarraInvalido = "El código de barras debe tener 8 dígitos, o entre 12 y 14.";
        public const string DniInvalido = "El DNI debe tener 7 u 8 dígitos, sin puntos.";
    }
}
