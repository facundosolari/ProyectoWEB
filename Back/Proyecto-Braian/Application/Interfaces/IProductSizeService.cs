using Application.Models.Request;
using Application.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IProductSizeService
    {
        ProductSizeResponse? GetProductSizeById(int id);
        List<ProductSizeResponse>? GetAllProductSizes();
        (bool, string) CreateProductSize(ProductSizeRequest request);
        (bool, string) UpdateProductSize(ProductSizeRequest request, int id);

        (bool, string) SoftDeleteProductSize(int id);
        /*
        (bool, string) HardDeleteProductSize(int id);
        */
    }
}
