namespace DulcesDuendesApp.Models.Dtos
{
    public class TokenResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Tipo { get; set; } = "Bearer";
        public DateTime ExpiraUtc { get; set; }
    }
}
