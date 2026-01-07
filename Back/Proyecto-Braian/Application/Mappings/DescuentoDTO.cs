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
    public static class DescuentoDTO
    {
        // CREATE
        public static Descuento ToEntity(DescuentoRequest request)
        {
            return new Descuento
            {
                Nombre = request.Nombre,
                TipoDescuento = request.TipoDescuento,
                TotalDescuento = request.TotalDescuento,
                Fecha_Desde = request.Fecha_Desde,
                Fecha_Hasta = request.Fecha_Hasta,
                Habilitado = false,
            };
        }

        // RESPONSE
        public static DescuentoResponse ToResponse(Descuento entity)
        {
            return new DescuentoResponse
            {
                Id = entity.Id,
                Nombre = entity.Nombre,
                TipoDescuento = entity.TipoDescuento,
                TotalDescuento = entity.TotalDescuento,
                Fecha_Desde = entity.Fecha_Desde,
                Fecha_Hasta = entity.Fecha_Hasta,
                Habilitado = entity.Habilitado
            };
        }

        public static List<DescuentoResponse> ToResponseList(List<Descuento> descuentos)
        {
            return descuentos.Select(ToResponse).ToList();
        }

        // UPDATE
        public static void UpdateEntity(Descuento entity, DescuentoRequest request)
        {
            entity.Nombre = request.Nombre;
            entity.TipoDescuento = request.TipoDescuento;
            entity.TotalDescuento = request.TotalDescuento;
            entity.Fecha_Desde = request.Fecha_Desde;
            entity.Fecha_Hasta = request.Fecha_Hasta;
            entity.Habilitado = request.Habilitado;
        }
    }
}
