using Application.Mappings;
using Application.Models.Request;
using Application.Models.Response;
using Application.Interfaces;
using Domain.Interfaces;

namespace Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;

        public CategoryService(
            ICategoryRepository categoryRepository,
            IProductRepository productRepository)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
        }
        public CategoryResponse? GetCategoryById(int id)
        {
            var category = _categoryRepository.GetCategoryById(id);
            if (category == null) return null;

            return CategoryDTO.ToCategoryResponse(category);
        }
        public List<CategoryResponse>? GetAllCategories()
        {
            var categories = _categoryRepository.GetAllCategories();
            return CategoryDTO.ToCategoryResponse(categories);
        }
        public (bool, string) CreateCategory(CategoryRequest request)
        {
            var entity = CategoryDTO.ToCategoryEntity(request);

            if (entity == null)
                return (false, "No se pudo crear la Categoria");

            // Si el request trae ids de productos, los asociamos
            if (request.ProductsIds != null && request.ProductsIds.Any())
            {
                var products = _productRepository.GetProductsByIds(request.ProductsIds);
                entity.Productos = products;
            }

            _categoryRepository.AddCategory(entity);
            return (true, "Categoria creada con éxito");
        }
        public (bool, string) UpdateCategory(int id, CategoryRequest request)
        {
            var category = _categoryRepository.GetCategoryById(id);
            if (category == null) return (false, "Categoria no encontrada");
            category.Nombre = request.Nombre;
            category.ParentCategoryId = request.ParentCategoryId; // <- actualizar padre

            if (request.ProductsIds != null)
            {
                var products = _productRepository.GetProductsByIds(request.ProductsIds);
                category.Productos = products;
            }

            _categoryRepository.UpdateCategory(category);
            return (true, "Categoria actualizada con éxito");
        }
        public bool AssignProductsToCategory(int categoryId, List<int> productIds)
        {
            var category = _categoryRepository.GetCategoryById(categoryId);
            if (category == null) return false;

            var products = _productRepository.GetProductsByIds(productIds);
            category.Productos = products;

            _categoryRepository.UpdateCategory(category);
            return true;
        }
        public (bool, string) SoftDeleteCategory(int id)
        {
            var entity = _categoryRepository.GetCategoryById(id);
            if (entity == null) return (false, "Categoria no encontrada");

            _categoryRepository.SoftDeleteCategory(entity);
            return (true, "Actualizacion de Habilitacion de Categoria con éxito");
        }
        public (bool, string) HardDeleteCategory(int id)
        {
            var entity = _categoryRepository.GetCategoryById(id);
            if (entity == null) return (false, "Categoria no encontrada");

            _categoryRepository.HardDeleteCategory(entity);
            return (true, "Categoria borrada con éxito");
        }
    }
}