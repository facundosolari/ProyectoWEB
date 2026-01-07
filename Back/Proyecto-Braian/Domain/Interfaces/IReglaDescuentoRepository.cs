using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IReglaDescuentoRepository
    {
        List<ReglaDescuento> GetAllReglaDescuentos();
        ReglaDescuento? GetReglaDescuentoById(int id);
        List<ReglaDescuento> GetReglasActivas();
        void AddReglaDescuento(ReglaDescuento entity);
        void UpdateReglaDescuento(ReglaDescuento entity);
        void SoftDeleteReglaDescuento(ReglaDescuento entity);
        void HardDeleteReglaDescuento(ReglaDescuento entity);
    }
}
