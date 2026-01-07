using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class DescuentoRepository : IDescuentoRepository
    {
        private readonly DataBaseContext _databaseContext;

        public DescuentoRepository(DataBaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public List<Descuento> GetAllDescuentos()
        {
            return _databaseContext.Descuentos
                .ToList();
        }

        public Descuento? GetDescuentoById(int id)
        {
            return _databaseContext.Descuentos
                .FirstOrDefault(d => d.Id == id);
        }

        // 🔥 CLAVE para reglas
        public List<Descuento> GetDescuentosActivos()
        {
            var ahora = DateTime.UtcNow;

            return _databaseContext.Descuentos
                .Where(d =>
                    d.Habilitado &&
                    d.Fecha_Desde <= ahora &&
                    d.Fecha_Hasta >= ahora
                )
                .ToList();
        }

        public void AddDescuento(Descuento entity)
        {
            _databaseContext.Descuentos.Add(entity);
            _databaseContext.SaveChanges();
        }

        public void UpdateDescuento(Descuento entity)
        {
            _databaseContext.Descuentos.Update(entity);
            _databaseContext.SaveChanges();
        }

        public void SoftDeleteDescuento(Descuento entity)
        {
            entity.Habilitado = !entity.Habilitado;
            _databaseContext.SaveChanges();
        }

        public void HardDeleteDescuento(Descuento entity)
        {
            _databaseContext.Descuentos.Remove(entity);
            _databaseContext.SaveChanges();
        }
    }
}