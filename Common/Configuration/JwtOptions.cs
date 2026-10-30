namespace DulcesDuendesApp.Common.Configuration
{
    public class JwtOptions
    {
        public const string Seccion = "Jwt";

        public string Emisor { get; set; } = string.Empty;
        public string Audiencia { get; set; } = string.Empty;
        public int MinutosExpiracion { get; set; } = 60;
        public string Clave { get; set; } = string.Empty;
    }
}
