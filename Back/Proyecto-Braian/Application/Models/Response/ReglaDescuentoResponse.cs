using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Response
{
    public class ReglaDescuentoResponse
    {
        public int Id { get; set; }

        public int DescuentoId { get; set; }
        public string NombreDescuento { get; set; } = string.Empty;

        public List<ProductResponse>? Products { get; set; }
        public List<CategoryResponse>? Categories { get; set; }

        public DateTime Fecha_Desde { get; set; }
        public DateTime Fecha_Hasta { get; set; }

        public bool Habilitado { get; set; }
    }
}
