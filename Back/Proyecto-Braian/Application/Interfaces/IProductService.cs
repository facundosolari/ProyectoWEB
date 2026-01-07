using Application.Models.Request;
using Application.Models.Response;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IProductService
    {

        ProductResponse? GetProductById(int id);
        List<ProductResponse>? GetAllProducts();
        (List<ProductResponse> Products, int TotalCount) GetProductsPaged(
            int page, int pageSize, string? sortBy = null,
            List<int>? categoryIds = null, List<string>? sizeIds = null,
            decimal? minPrice = null, decimal? maxPrice = null,
            bool onlyEnabled = false, string? searchName = null);
        (bool, string) CreateProduct(ProductRequest request);
        (bool, string) UpdateProduct(ProductRequest request, int id);
        (bool, string) SoftDeleteProduct(int id);
        /*
        (bool, string) HardDeleteProduct(int id);

        */
    }
}
