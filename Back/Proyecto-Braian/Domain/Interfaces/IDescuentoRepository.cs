using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IDescuentoRepository
    {
        List<Descuento> GetAllDescuentos();
        Descuento? GetDescuentoById(int id);
        List<Descuento> GetDescuentosActivos();
        void AddDescuento(Descuento entity);
        void UpdateDescuento(Descuento entity);
        void SoftDeleteDescuento(Descuento entity);
        void HardDeleteDescuento(Descuento entity);
    }
}
