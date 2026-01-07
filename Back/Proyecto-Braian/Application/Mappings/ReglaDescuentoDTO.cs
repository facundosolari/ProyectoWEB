using Application.Models.Request;
using Application.Models.Response;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappings
{
    public static class ReglaDescuentoDTO
    {
        // CREATE
        public static ReglaDescuento ToEntity(
            ReglaDescuentoRequest request,
            List<Product>? products,
            List<Category>? categories)
        {
            return new ReglaDescuento
            {
                DescuentoId = request.DescuentoId,
                Products = products,
                Categories = categories,
                Fecha_Desde = request.Fecha_Desde,
                Fecha_Hasta = request.Fecha_Hasta,
                Habilitado = false,
            };
        }

        // RESPONSE
        public static ReglaDescuentoResponse ToResponse(ReglaDescuento entity)
        {
            return new ReglaDescuentoResponse
            {
                Id = entity.Id,
                DescuentoId = entity.DescuentoId,
                NombreDescuento = entity.Descuento.Nombre,

                Fecha_Desde = entity.Fecha_Desde,
                Fecha_Hasta = entity.Fecha_Hasta,
                Habilitado = entity.Habilitado,

                Products = entity.Products?.Select(p => new ProductResponse
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Precio = p.Precio,
                    Habilitado = p.Habilitado
                }).ToList(),

                Categories = entity.Categories?.Select(c => new CategoryResponse
                {
                    Id = c.Id,
                    Nombre = c.Nombre
                }).ToList()
            };
        }

        public static List<ReglaDescuentoResponse> ToResponseList(List<ReglaDescuento> reglas)
        {
            return reglas.Select(ToResponse).ToList();
        }

        // UPDATE
        public static void UpdateEntity(
            ReglaDescuento entity,
            ReglaDescuentoRequest request,
            List<Product>? products,
            List<Category>? categories)
        {
            entity.DescuentoId = request.DescuentoId;
            entity.Products = products;
            entity.Categories = categories;
            entity.Fecha_Desde = request.Fecha_Desde;
            entity.Fecha_Hasta = request.Fecha_Hasta;
            entity.Habilitado = request.Habilitado;
        }
    }
}
