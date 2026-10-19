namespace DulcesDuendesApp.Services.Interfaces
{
    public interface IPrecioCalculador
    {
        decimal PrecioVenta(decimal costo, decimal ganancia);
        decimal PrecioCuentaCorriente(decimal costo, decimal ganancia);
    }
}
