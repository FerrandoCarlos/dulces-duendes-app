using DulcesDuendesApp.Common.Configuration;
using Microsoft.Extensions.Options;
using DulcesDuendesApp.Services.Interfaces;

namespace DulcesDuendesApp.Services.Implementations
{
    public class PrecioCalculador : IPrecioCalculador
    {
        private readonly decimal _recargo;

        public PrecioCalculador(IOptionsSnapshot<NegocioOptions> negocio)
        {
            _recargo = negocio.Value.RecargoCuentaCorriente;
        }

        public decimal PrecioVenta(decimal costo, decimal ganancia) => Redondear(costo * (1 + ganancia / 100));
        public decimal PrecioCuentaCorriente(decimal costo, decimal ganancia) => Redondear(costo * (1 + (ganancia + _recargo) / 100));

        private static decimal Redondear(decimal valor) => Math.Round(valor, 2, MidpointRounding.AwayFromZero);
    }
}
