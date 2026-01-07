using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class ReglaDescuentoRepository : IReglaDescuentoRepository
    {
        private readonly DataBaseContext _databaseContext;

        public ReglaDescuentoRepository(DataBaseContext dataBaseContext)
        {
            _databaseContext = dataBaseContext;
        }

        public List<ReglaDescuento> GetAllReglaDescuentos()
        {
            return _databaseContext.ReglaDescuentos
                .Include(r => r.Descuento)
                .Include(r => r.Products)
                .Include(r => r.Categories)
                .ToList();
        }

        public ReglaDescuento? GetReglaDescuentoById(int id)
        {
            return _databaseContext.ReglaDescuentos
                .Include(r => r.Descuento)
                .Include(r => r.Products)
                .Include(r => r.Categories)
                .FirstOrDefault(r => r.Id == id);
        }

        // 🔥 CLAVE para el cálculo
        public List<ReglaDescuento> GetReglasActivas()
        {
            var ahora = DateTime.UtcNow;

            return _databaseContext.ReglaDescuentos
                .Include(r => r.Descuento)
                .Include(r => r.Products)
                .Include(r => r.Categories)
                .Where(r =>
                    r.Habilitado &&
                    r.Fecha_Desde <= ahora &&
                    r.Fecha_Hasta >= ahora &&
                    r.Descuento != null &&
                    r.Descuento.Habilitado
                )
                .ToList();
        }

        public void AddReglaDescuento(ReglaDescuento entity)
        {
            _databaseContext.ReglaDescuentos.Add(entity);
            _databaseContext.SaveChanges();
        }

        public void UpdateReglaDescuento(ReglaDescuento entity)
        {
            _databaseContext.ReglaDescuentos.Update(entity);
            _databaseContext.SaveChanges();
        }

        public void SoftDeleteReglaDescuento(ReglaDescuento entity)
        {
            entity.Habilitado = !entity.Habilitado;
            _databaseContext.SaveChanges();
        }

        public void HardDeleteReglaDescuento(ReglaDescuento entity)
        {
            _databaseContext.ReglaDescuentos.Remove(entity);
            _databaseContext.SaveChanges();
        }
    }
}