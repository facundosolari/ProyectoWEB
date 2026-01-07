using Application.Models.Request;
using Application.Models.Response;
using Domain.Entities;

namespace Application.Mappings
{
    public static class OrderItemDTO
    {
        public static OrderItem ToOrderItemEntity(OrderItemRequest request)
        {
            return new OrderItem
            {
                OrderId = request.OrderId,
                ProductSizeId = request.ProductSizeId,
                Cantidad = request.Cantidad
            };
        }

        public static OrderItemResponse ToOrderItemResponse(OrderItem orderItem)
        {
            return new OrderItemResponse
            {
                Id = orderItem.Id,
                ProductSizeId = orderItem.ProductSizeId,
                ProductId = orderItem.ProductSize.Product.Id,
                OrderId = orderItem.OrderId,

                NombreProducto = orderItem.ProductSize.Product.Nombre,
                Talle = orderItem.ProductSize.Talle,
                Fotos = orderItem.ProductSize.Product.Fotos,

                Cantidad = orderItem.Cantidad,

                PrecioUnitario = orderItem.PrecioUnitario,
                DescuentoUnitario = orderItem.DescuentoUnitario,
                PrecioFinalUnitario = orderItem.PrecioFinalUnitario,

                NombreDescuento = orderItem.NombreDescuento,
                Habilitado = orderItem.Habilitado
            };
        }

        public static List<OrderItemResponse> ToOrderItemResponse(List<OrderItem> orderItems)
        {
            return orderItems.Select(ToOrderItemResponse).ToList();
        }

        public static void ToUpdateOrderItemAdmin(OrderItem orderItem, OrderItemRequest request)
        {
            orderItem.ProductSizeId = request.ProductSizeId;
            orderItem.Cantidad = request.Cantidad;
        }

        public static void ToUpdateOrderItem(OrderItem orderItem, OrderItemPatchRequest request)
        {
            orderItem.Cantidad = request.Cantidad;
        }
    }
}