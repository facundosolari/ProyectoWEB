using Application.Models.Request;
using Application.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Mappings
{
    public static class ProductDTO
    {
        public static Product ToProductEntity (ProductRequest request)
        {
            return new Product()
            {
                Nombre = request.Nombre,
                Descripcion = request.Descripcion,
                Precio = request.Precio,
                Sizes = new List<ProductSize>(),
                Fotos = request.Fotos ?? new List<string>(),
            };
        }

        public static ProductResponse ToProductResponse(
           Product product,
           (decimal monto, string? nombre) descuentoData
       )
        {
            var (descuento, nombreDescuento) = descuentoData;

            decimal porcentaje = 0;
            if (descuento > 0)
            {
                porcentaje = Math.Round((descuento / product.Precio) * 100, 0);
            }

            return new ProductResponse
            {
                Id = product.Id,
                Nombre = product.Nombre,
                Descripcion = product.Descripcion,
                Precio = product.Precio,
                PrecioConDescuento = Math.Max(product.Precio - descuento, 0),
                DescuentoPorcentaje = porcentaje > 0 ? porcentaje : (decimal?)null,
                NombreDescuento = nombreDescuento,

                Habilitado = product.Habilitado,
                Sizes = product.Sizes?.Select(s => new ProductSizeResponse
                {
                    Id = s.Id,
                    Talle = s.Talle,
                    Stock = s.Stock,
                    Habilitado = s.Habilitado,
                }).ToList(),
                Fotos = product.Fotos,
                Categories = product.Categories?.Select(c => new CategoryResponse
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                }).ToList(),
                TotalVentas = product.TotalVentas
            };
        }

        public static List<ProductResponse> ToProductResponse(
            List<Product> products,
            Func<Product, (decimal monto, string? nombre)> calcularDescuento
        )
        {
            return products.Select(p =>
            {
                var descuentoData = calcularDescuento(p);
                return ToProductResponse(p, descuentoData);
            }).ToList();
        }
        public static void ToProductUpdate (Product product, ProductRequest request)
        {
            product.Nombre = request.Nombre;
            product.Descripcion = request.Descripcion;
            product.Precio = request.Precio;
            product.Fotos = request.Fotos;

        }
    }
}
