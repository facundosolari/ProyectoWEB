using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int ProductSizeId { get; set; }
        public int OrderId { get; set; }

        public ProductSize ProductSize { get; set; } = null!;
        public Order Order { get; set; } = null!;

        public int Cantidad { get; set; }

        // 💰 valores históricos
        public decimal PrecioUnitario { get; set; }
        public decimal DescuentoUnitario { get; set; }
        public decimal PrecioFinalUnitario { get; set; }

        // 🏷️ trazabilidad
        public string? NombreDescuento { get; set; }

        public bool Habilitado { get; set; } = true;
    }
}
