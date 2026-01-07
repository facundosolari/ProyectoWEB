using Application.Interfaces;
using Application.Mappings;
using Application.Models.Request;
using Application.Models.Response;
using Domain.Interfaces;
using System.Transactions;

namespace Application.Services
{
    public class ReglaDescuentoService : IReglaDescuentoService
    {
        private readonly IReglaDescuentoRepository _reglaRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IDescuentoRepository _descuentoRepository;

        public ReglaDescuentoService(
            IReglaDescuentoRepository reglaRepository,
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            IDescuentoRepository descuentoRepository)
        {
            _reglaRepository = reglaRepository;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _descuentoRepository = descuentoRepository;
        }

        public ReglaDescuentoResponse? GetById(int id)
        {
            var regla = _reglaRepository.GetReglaDescuentoById(id);
            if (regla == null) return null;

            return ReglaDescuentoDTO.ToResponse(regla);
        }

        public List<ReglaDescuentoResponse> GetAll()
        {
            var reglas = _reglaRepository.GetAllReglaDescuentos();
            return ReglaDescuentoDTO.ToResponseList(reglas);
        }

        public (bool, string) Create(ReglaDescuentoRequest request)
        {
            var products = request.ProductIds != null
                ? _productRepository.GetProductsByIds(request.ProductIds)
                : null;

            var categories = request.CategoryIds != null
                ? _categoryRepository.GetCategoriesByIds(request.CategoryIds)
                : null;

            var entity = ReglaDescuentoDTO.ToEntity(request, products, categories);

            _reglaRepository.AddReglaDescuento(entity);
            return (true, "Regla creada con éxito");
        }

        public (bool, string) Update(int id, ReglaDescuentoRequest request)
        {
            var regla = _reglaRepository.GetReglaDescuentoById(id);
            if (regla == null)
                return (false, "Regla inexistente");

            // 🔹 Validación y actualización de productos
            if (request.ProductIds?.Any() == true)
            {
                var products = _productRepository.GetProductsByIds(request.ProductIds);

                if (products.Count != request.ProductIds.Count)
                    return (false, "Uno o más productos no existen");

                regla.Products = products;
            }

            // 🔹 Validación y actualización de categorías
            if (request.CategoryIds?.Any() == true)
            {
                var categories = _categoryRepository.GetCategoriesByIds(request.CategoryIds);

                if (categories.Count != request.CategoryIds.Count)
                    return (false, "Una o más categorías no existen");

                regla.Categories = categories;
            }

            // 🔹 Validaciones obligatorias
            if (request.DescuentoId <= 0)
                return (false, "Descuento inválido");
            var descuento = _descuentoRepository.GetDescuentoById(request.DescuentoId);
            if (descuento == null)
                return (false, "Descuento inexistente");

            if (request.Fecha_Desde > request.Fecha_Hasta)
                return (false, "La fecha desde no puede ser mayor que la fecha hasta");

            if (request.Fecha_Desde < descuento.Fecha_Desde ||
                request.Fecha_Hasta > descuento.Fecha_Hasta)
            {
                return (false, "Las fechas de la regla deben estar dentro del rango del descuento");
            }
            // 🔹 Asignaciones finales
            regla.DescuentoId = request.DescuentoId;
            regla.Fecha_Desde = request.Fecha_Desde;
            regla.Fecha_Hasta = request.Fecha_Hasta;
            regla.Habilitado = request.Habilitado;

            _reglaRepository.UpdateReglaDescuento(regla);

            return (true, "Regla de descuento actualizada con éxito");
        }

        public (bool, string) SoftDelete(int id)
        {
            var regla = _reglaRepository.GetReglaDescuentoById(id);
            if (regla == null) return (false, "Regla no existente");

            _reglaRepository.SoftDeleteReglaDescuento(regla);
            return (true, "Habilitacion cambiada con éxito");
        }

        public (bool, string) HardDelete(int id)
        {
            var regla = _reglaRepository.GetReglaDescuentoById(id);
            if (regla == null) return (false, "Regla no existente");

            _reglaRepository.HardDeleteReglaDescuento(regla);
            return (true, "Regla borrada con éxito");
        }
    }
}