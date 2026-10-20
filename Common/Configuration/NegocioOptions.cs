namespace DulcesDuendesApp.Common.Configuration
{
    public class NegocioOptions
    {
        public const string Seccion = "Negocio";
        public int StockBajo { get; set; } = 5;
        public decimal RecargoCuentaCorriente { get; set; } = 15;
    }
}
