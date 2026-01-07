using Application.Interfaces;
using Domain.Entities;
using Domain.Enum;
using Domain.Interfaces;

namespace Application.Services
{
    public class DiscountCalculatorService : IDiscountCalculatorService
    {
        private readonly IReglaDescuentoRepository _reglaRepository;

        public DiscountCalculatorService(IReglaDescuentoRepository reglaRepository)
        {
            _reglaRepository = reglaRepository;
        }

        public (decimal monto, string? nombre) CalcularMejorDescuento(Product product)
        {
            var reglas = _reglaRepository.GetReglasActivas();

            decimal mejorMonto = 0;
            string? nombreDescuento = null;

            foreach (var regla in reglas)
            {
                if (regla.Descuento == null || !regla.Descuento.Habilitado)
                    continue;

                bool aplicaPorProducto =
                    regla.Products?.Any(p => p.Id == product.Id) ?? false;

                bool aplicaPorCategoria =
                    regla.Categories != null &&
                    product.Categories != null &&
                    regla.Categories.Any(rc =>
                        product.Categories.Any(pc => pc.Id == rc.Id));

                if (!aplicaPorProducto && !aplicaPorCategoria)
                    continue;

                decimal monto = regla.Descuento.TipoDescuento switch
                {
                    TipoDescuento.Porcentaje =>
                        product.Precio * regla.Descuento.TotalDescuento / 100,

                    TipoDescuento.Monto =>
                        regla.Descuento.TotalDescuento,

                    _ => 0
                };

                monto = Math.Min(monto, product.Precio);

                if (monto > mejorMonto)
                {
                    mejorMonto = monto;
                    nombreDescuento = regla.Descuento.Nombre;
                }
            }

            return (mejorMonto, nombreDescuento);
        }
    }
}