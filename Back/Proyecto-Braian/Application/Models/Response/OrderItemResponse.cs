using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Response
{
    public class OrderItemResponse
    {
        public int Id { get; set; }
        public int ProductSizeId { get; set; }
        public int ProductId { get; set; }
        public int OrderId { get; set; }

        public string NombreProducto { get; set; } = string.Empty;
        public string Talle { get; set; } = string.Empty;
        public List<string>? Fotos { get; set; }

        public int Cantidad { get; set; }

        // 💰 históricos
        public decimal PrecioUnitario { get; set; }
        public decimal DescuentoUnitario { get; set; }
        public decimal PrecioFinalUnitario { get; set; }

        // 🏷️
        public string? NombreDescuento { get; set; }

        public bool Habilitado { get; set; }
    }

}


