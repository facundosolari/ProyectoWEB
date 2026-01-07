using Application.Models.Request;
using Application.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ICategoryService
    {
        CategoryResponse? GetCategoryById(int id);
        List<CategoryResponse>? GetAllCategories();
        (bool, string) CreateCategory(CategoryRequest request);
        (bool, string) UpdateCategory(int id, CategoryRequest request);
        bool AssignProductsToCategory(int categoryId, List<int> productIds);
        (bool, string) SoftDeleteCategory(int id);
        (bool, string) HardDeleteCategory(int id);
    }
}
