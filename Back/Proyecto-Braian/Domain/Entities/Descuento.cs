using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Descuento
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public TipoDescuento TipoDescuento { get; set; }
        public decimal TotalDescuento { get; set; }
        public DateTime Fecha_Desde { get; set; }
        public DateTime Fecha_Hasta { get; set; }
        public bool Habilitado { get; set; } = false;
    }
}
