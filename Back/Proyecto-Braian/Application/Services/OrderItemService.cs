using Application.Interfaces;
using Application.Mappings;
using Application.Models.Request;
using Application.Models.Response;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class OrderItemService : IOrderItemService
    {

        private readonly IOrderItemRepository _OrderItemRepository;
        private readonly IOrderRepository _OrderRepository;
        private readonly IProductSizeRepository _ProductSizeRepository;
        private readonly IDiscountCalculatorService _discountCalculator;

        public OrderItemService(IOrderItemRepository OrderItemRepository, IOrderRepository orderRepository,
            IProductSizeRepository productSizeRepository, IDiscountCalculatorService discountCalculator)
        {
            _OrderItemRepository = OrderItemRepository;
            _OrderRepository = orderRepository;
            _ProductSizeRepository = productSizeRepository;
            _discountCalculator = discountCalculator;
        }

        public OrderItemResponse? GetOrderItemById(int id)
        {
            var OrderItem = _OrderItemRepository.GetOrderItemById(id);
            if (OrderItem != null)
            {
                return OrderItemDTO.ToOrderItemResponse(OrderItem);
            }
            return null;

        }
        public List<OrderItemResponse>? GetAllOrderItems()
        {
            var OrderItems = _OrderItemRepository.GetAllOrderItem();
            if (OrderItems != null)
            {
                return OrderItemDTO.ToOrderItemResponse(OrderItems);
            }
            return null;
        }

        public (bool, string) CreateOrderItem(OrderItemRequest request, int tokenUserId)
        {
            var order = _OrderRepository.GetOrderById(request.OrderId);
            if (order == null || order.UserId != tokenUserId) return (false, "Credenciales de Orden incorrectas");

            var productSize = _ProductSizeRepository.GetProductSizeById(request.ProductSizeId);
            if (productSize == null) return (false, "Talle no encontrado");
            if (request.Cantidad <= 0 || productSize.Stock < request.Cantidad) return (false, "Stock Invalido");

            var product = productSize.Product;

            var (descuento, nombre) = _discountCalculator.CalcularMejorDescuento(product);

            var precioUnitario = product.Precio;
            var precioFinal = precioUnitario - descuento;

            var orderItem = new OrderItem
            {
                OrderId = request.OrderId,
                ProductSizeId = request.ProductSizeId,
                Cantidad = request.Cantidad,

                PrecioUnitario = precioUnitario,
                DescuentoUnitario = descuento,
                PrecioFinalUnitario = precioFinal,
                NombreDescuento = nombre,

                Habilitado = true
            };

            _OrderItemRepository.AddOrderItem(orderItem);

            return (true, "OrderItem creado con éxito");
        }

        public void CreateOrderItem(OrderItem orderItem, int tokenUserId)
        {
            var order = _OrderRepository.GetOrderById(orderItem.OrderId);
            if (order == null) throw new Exception("Orden no encontrada");
            if (order.UserId != tokenUserId) throw new Exception("Usuario no autorizado");

            _OrderItemRepository.AddOrderItem(orderItem);
        }

        public (bool, string) UpdateOrderItemAdmin(OrderItemRequest request, int id)
        {
            var order = _OrderRepository.GetOrderById(request.OrderId);
            if (order == null) return (false, "Orden no encontrada");
            var productSize = _ProductSizeRepository.GetProductSizeById(request.ProductSizeId);
            if (productSize == null) return (false, "Talle no encontrado");
            if (request.Cantidad > productSize.Stock) return (false, "Stock Invalido");
            if (order.Confirmada || order.Habilitado == false)
                return (false, "Orden no habilitada");
            if (request.Cantidad <= 0) return (false, "Cantidades incorrectas");
            if (productSize.Stock < request.Cantidad) return (false, "Stock de Talles Incorrectos");
            var entity = _OrderItemRepository.GetOrderItemById(id);
            if (entity == null) return (false, "OrderItem no encontrada");
            OrderItemDTO.ToUpdateOrderItemAdmin(entity, request);
            _OrderItemRepository.UpdateOrderItem(entity);
            return (true, "OrderItem actualizada con éxito");
        }

        public (bool, string) UpdateOrderItem(OrderItemPatchRequest request, int id, int tokenUserId)
        {
            var entity = _OrderItemRepository.GetOrderItemById(id);
            if (entity == null) return (false, "OrderItem no encontrada");

            var order = _OrderRepository.GetOrderById(entity.OrderId);
            if (order == null) return (false, "Order no encontrada");

            if (order.UserId != tokenUserId) return (false, "Credenciales de Usuario no coincidentes"); // solo el dueño puede modificar
            if (order.Confirmada || order.Habilitado == false) return (false, "No se puede actualizar Order"); // no modificar orden confirmada

            if (request.Cantidad < 0) return (false, "Cantidades incorrectas");

            var productSize = _ProductSizeRepository.GetProductSizeById(entity.ProductSizeId);
            if (productSize == null) return (false, "Talle no encontrado");

            // Considerar stock disponible sumando la cantidad que ya estaba reservada
            int stockDisponible = productSize.Stock + entity.Cantidad;
            if (request.Cantidad > stockDisponible) return (false, "Cantidades de Talle incorrectas");

            if (request.Cantidad == 0)
            {
                // Si la cantidad es 0, eliminamos el item
                _OrderItemRepository.HardDeleteOrderItem(entity);
            }
            else
            {
                // Actualizamos cantidad
                OrderItemDTO.ToUpdateOrderItem(entity, request);
                _OrderItemRepository.UpdateOrderItem(entity);
            }

            // Recalcular total de la orden
            order.Total = order.OrderItems
                .Where(x => x.Habilitado)
                .Sum(x => x.Cantidad * x.ProductSize.Product.Precio);

            _OrderRepository.UpdateOrder(order);

            return (true, "OrderItem actualizada con éxito");
        }

        public (bool, string) SoftDeleteOrderItem(int id)
        {
            var entity = _OrderItemRepository.GetOrderItemById(id);
            if (entity != null)
            {
                _OrderItemRepository.SoftDeleteOrderItem(entity);
                return (true, "Habilitacion de OrderItem actualizada con éxito");
            }
            return (false, "No se pudo actualizar la Habilitacion de OrderItem");
        }
        public (bool, string) HardDeleteOrderItem(int id)
        {
            var entity = _OrderItemRepository.GetOrderItemById(id);
            if (entity != null)
            {
                _OrderItemRepository.HardDeleteOrderItem(entity);
                return (true, "OrderItem borrada con éxito");
            }
            return (false, "No se encontro OrderItem");
        }

    }
}