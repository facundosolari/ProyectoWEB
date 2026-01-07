using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ReglaDescuento
    {
        public int Id { get; set; }
        public int DescuentoId { get; set; }
        public Descuento? Descuento { get; set; }
        public List <Product>? Products { get; set; }
        public List <Category>? Categories { get; set; }
        public DateTime Fecha_Desde { get; set; }
        public DateTime Fecha_Hasta { get; set; }
        public bool Habilitado { get; set; } = false;
    }
}
