using Application.Interfaces;
using Application.Mappings;
using Application.Models.Request;
using Application.Models.Response;
using Domain.Interfaces;

namespace Application.Services
{
    public class DescuentoService : IDescuentoService
    {
        private readonly IDescuentoRepository _descuentoRepository;

        public DescuentoService(IDescuentoRepository descuentoRepository)
        {
            _descuentoRepository = descuentoRepository;
        }

        public DescuentoResponse? GetDescuentoById(int id)
        {
            var descuento = _descuentoRepository.GetDescuentoById(id);
            if (descuento == null) return null;

            return DescuentoDTO.ToResponse(descuento);
        }

        public List<DescuentoResponse> GetAllDescuentos()
        {
            var descuentos = _descuentoRepository.GetAllDescuentos();
            return DescuentoDTO.ToResponseList(descuentos);
        }

        public (bool, string) CreateDescuento(DescuentoRequest request)
        {
            var entity = DescuentoDTO.ToEntity(request);
            if (entity == null) return (false, "No se pudo crear el Descuento");

            _descuentoRepository.AddDescuento(entity);
            return (true, "Descuento creado con éxito");
        }

        public (bool, string) UpdateDescuento(int id, DescuentoRequest request)
        {
            var descuento = _descuentoRepository.GetDescuentoById(id);
            if (descuento == null) return (false, "Descuento no encontrado");

            DescuentoDTO.UpdateEntity(descuento, request);
            _descuentoRepository.UpdateDescuento(descuento);

            return (true, "Descuento actualizado con éxito");
        }

        public (bool, string) SoftDeleteDescuento(int id)
        {
            var descuento = _descuentoRepository.GetDescuentoById(id);
            if (descuento == null) return (false, "No se encontro el Descuento");

            _descuentoRepository.SoftDeleteDescuento(descuento);
            return (true, "Actualizacion de Habilitacion de Descuento con éxito");
        }

        public (bool, string) HardDeleteDescuento(int id)
        {
            var descuento = _descuentoRepository.GetDescuentoById(id);
            if (descuento == null) return (false, "No se encontro el Descuento");

            _descuentoRepository.HardDeleteDescuento(descuento);
            return (true, "Descuento borrado con éxito");
        }
    }
}