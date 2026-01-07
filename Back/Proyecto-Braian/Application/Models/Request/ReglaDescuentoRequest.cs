using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Request
{
    public class ReglaDescuentoRequest
    {
        public int DescuentoId { get; set; }

        public List<int>? ProductIds { get; set; }
        public List<int>? CategoryIds { get; set; }

        public DateTime Fecha_Desde { get; set; }
        public DateTime Fecha_Hasta { get; set; }

        public bool Habilitado { get; set; }
    }
}
