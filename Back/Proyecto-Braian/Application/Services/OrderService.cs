using Application.Interfaces;
using Application.Mappings;
using Application.Models.Request;
using Application.Models.Response;
using Domain.Interfaces;
using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Services
{
    public class OrderService : IOrderService
    {

        private readonly IOrderRepository _OrderRepository;
        private readonly IOrderItemService _OrderItemService;
        private readonly IProductSizeRepository _ProductSizeRepository;
        private readonly IUserRepository _UserRepository;
        private readonly IAuditLogRepository _AuditLogRepository;

        public OrderService(IOrderRepository OrderRepository, IUserRepository userRepository, IOrderItemService orderItemService,
            IProductSizeRepository productSizeRepository, IAuditLogRepository auditLogRepository)
        {
            _OrderRepository = OrderRepository;
            _UserRepository = userRepository;
            _OrderItemService = orderItemService;
            _ProductSizeRepository = productSizeRepository;
            _AuditLogRepository = auditLogRepository;
        }

        public OrderResponse? GetOrderById(int orderId, int? userId = null, bool esAdmin = false)
        {
            var order = _OrderRepository.GetOrderById(orderId);

            if (order == null) return null;

            // 🔒 Validar permisos
            if (!esAdmin && order.UserId != userId)
                return null;

            return OrderDTO.ToOrderResponse(order);
        }

        public List<OrderResponse>? GetOrdersByUserId(int userId)
        {
            var Orders = _OrderRepository.GetOrdersByUserId(userId);
            if (Orders != null)
            {
                return OrderDTO.ToOrderResponse(Orders);
            }
            return null;

        }
        public List<OrderResponse>? GetAllOrders()
        {
            var Orders = _OrderRepository.GetAllOrder();
            if (Orders != null)
            {
                return OrderDTO.ToOrderResponse(Orders);
            }
            return null;
        }

        public (List<OrderResponse> Orders, int TotalCount) GetOrdersByUserIdPaginated(
            int userId,
            int page,
            int pageSize,
            bool? tieneMensajesNoLeidos = null,
            int? estado = null,          // 🔥 sigue siendo int? para el servicio
            bool esAdmin = false,
            DateTime? fechaDesde = null,
            DateTime? fechaHasta = null,
            string sortBy = "FechaHora",
            string sortOrder = "desc")
        {
            // Convertimos int? a enum nullable solo para el repositorio
            EstadoPedido? estadoEnum = estado.HasValue ? (EstadoPedido)estado.Value : (EstadoPedido?)null;

            // Llamada al repositorio que ya maneja Include de Messages
            var (orders, totalCount) = _OrderRepository.GetOrdersByUserIdPaginated(
                userId,
                page,
                pageSize,
                tieneMensajesNoLeidos,
                esAdmin,
                estadoEnum,   // pasamos enum solo internamente
                fechaDesde,
                fechaHasta,
                sortBy,
                sortOrder
            );

            // Mapeo a DTO normal, sin DTO interno
            var response = orders.Select(OrderDTO.ToOrderResponse).ToList();

            return (response, totalCount);
        }


        public (List<OrderResponse> Orders, int TotalCount) GetOrdersByEstadoPaginated(
            EstadoPedido estadoPedido,
            int page,
            int pageSize,
            DateTime? fechaDesde = null,
            DateTime? fechaHasta = null,
            int? userId = null,
            bool? tieneMensajesNoLeidos = null,
            bool esAdmin = false,
            string sortBy = "FechaHora",
            string sortOrder = "desc")
        {
            var (orders, totalCount) = _OrderRepository.GetOrdersByEstadoPaginated(
                estadoPedido,
                page,
                pageSize,
                fechaDesde,
                fechaHasta,
                userId,
                tieneMensajesNoLeidos,   // 👈 PASAMOS LO NUEVO
                esAdmin,                 // 👈 PASAMOS EL ROL
                sortBy,
                sortOrder
            );

            var response = OrderDTO.ToOrderResponse(orders);

            return (response, totalCount);
        }
        public (List<OrderResponse> Orders, int TotalCount) GetOrdersByEstadoAndPagadoPaginated(
            EstadoPedido estadoPedido,
            int page,
            int pageSize,
            DateTime? fechaDesde = null,
            DateTime? fechaHasta = null,
            int? userId = null,
            bool? tieneMensajesNoLeidos = null,
            bool esAdmin = false,
            string sortBy = "FechaHora",
            string sortOrder = "desc")
        {
            var (orders, totalCount) = _OrderRepository.GetOrdersByEstadoPaginated(
                estadoPedido,
                page,
                pageSize,
                fechaDesde,
                fechaHasta,
                userId,
                tieneMensajesNoLeidos,   // 👈 PASAMOS LO NUEVO
                esAdmin,                 // 👈 PASAMOS EL ROL
                sortBy,
                sortOrder
            );
            var ordersPagadas = orders?.Where(o => o.Pagada).ToList()?? [];

            var response = OrderDTO.ToOrderResponse(ordersPagadas);

            return (response, totalCount);
        }
        public (bool, string) CreateOrder(OrderRequest request, int tokenUserId)
        {
            // 1️⃣ Validar usuario
            var user = _UserRepository.GetUserById(tokenUserId);
            if (user == null) return (false, "No corresponde Usuario");

            // 2️⃣ Validaciones previas de items
            foreach (var itemRequest in request.Items)
            {
                var productSize = _ProductSizeRepository.GetProductSizeById(itemRequest.ProductSizeId);
                if (productSize == null) return (false, "Talle inexistente");
                if (!productSize.Habilitado) return (false, "Talle inhabilitado");
                if (itemRequest.Cantidad <= 0) return (false, "Cantidad mayor a 0");
                if (productSize.Stock < itemRequest.Cantidad) return (false, "Cantidad no corresponde");
            }

            // 3️⃣ Crear la orden (sin total aún)
            var order = OrderDTO.ToOrderEntity(request);
            if (order == null) return (false, "Orden inexistente");

            order.UserId = tokenUserId;
            order.Total = 0;

            _OrderRepository.AddOrder(order); // genera Order.Id

            // 4️⃣ Crear OrderItems (el precio + descuento se calculan adentro)
            foreach (var itemRequest in request.Items)
            {
                var orderItemRequest = new OrderItemRequest
                {
                    OrderId = order.Id,
                    ProductSizeId = itemRequest.ProductSizeId,
                    Cantidad = itemRequest.Cantidad
                };

                _OrderItemService.CreateOrderItem(orderItemRequest, tokenUserId);
            }

            // 5️⃣ Recalcular total desde los OrderItems
            var orderWithItems = _OrderRepository.GetOrderById(order.Id);
            if (orderWithItems == null) return (false, "No existe Orden");

            orderWithItems.Total = orderWithItems.OrderItems
                .Where(oi => oi.Habilitado)
                .Sum(oi => oi.Cantidad * oi.PrecioFinalUnitario);

            // 6️⃣ Auditoría
            _AuditLogRepository.Log(
                tokenUserId,
                "CreateOrder",
                $"OrderId={order.Id}, UserId={tokenUserId}"
            );

            // 7️⃣ Guardar total final
            _OrderRepository.UpdateOrder(orderWithItems);

            return (true, "Orden creada con éxito");
        }

        public (bool, string) UpdateOrderDetalleFacturacion(int orderId, DetalleFacturacionRequest request, int tokenUserId, string userRol)
        {
            var order = _OrderRepository.GetOrderById(orderId);
            if (order == null) return (false, "Orden no encontrada");

            // Solo el dueño puede modificar sus datos
            if (order.UserId != tokenUserId && userRol != "Admin") return (false, "No corresponde Usuario");

            if (order.DetalleFacturacion == null)
            {
                order.DetalleFacturacion = new DetalleFacturacion
                {
                    OrderId = order.Id, // Relación uno a uno
                    Order = order
                };
            }

            // Actualizar la entidad existente
            DetalleFacturacionDTO.ToUpdateDetalleFacturacion(order.DetalleFacturacion, request);

            // Guardar cambios
            _OrderRepository.UpdateOrder(order);

            return (true, "Detalle de Facturacion Actualizado");
        }

        public (bool, string) CancelOrder(int id, int userId, string rolClaim)
        {
            var order = _OrderRepository.GetOrderById(id);
            if (order == null) return (false, "Orden inexistente");
            if (order.UserId != userId && rolClaim != "Admin") return (false, "No corresponde Usuario");
            if (order.EstadoPedido == EstadoPedido.Cancelado) return (false, "Pedido ya cancelado");
            if (order.Habilitado == false || order.Pagada == true) return (false, "Pedido deshabilitado o ya pagado");
            if (order.Confirmada || order.EstadoPedido == EstadoPedido.Proceso)
            {
                foreach (var item in order.OrderItems)
                {
                    var productSize = _ProductSizeRepository.GetProductSizeById(item.ProductSizeId);
                    if (productSize == null) continue;
                    productSize.Stock += item.Cantidad;
                    _ProductSizeRepository.UpdateProductSize(productSize);
                }
            }
            order.EstadoPedido = EstadoPedido.Cancelado;
            SoftDeleteOrder(id);
            _OrderRepository.UpdateOrder(order);
            return (true, "Orden cancelada con éxito");
        }
        
        public (bool, string) ConfirmOrder(int id)
        {
            var order = _OrderRepository.GetOrderById(id);
            if (order == null) return (false, "Orden no existente");
            if (order.Confirmada || order.EstadoPedido == EstadoPedido.Cancelado || order.EstadoPedido == EstadoPedido.Finalizado
                || order.Habilitado == false || order.Pagada == true) return (false, "Orden no cumple parametros");
            foreach (var item in order.OrderItems)
            {
                var productSize = _ProductSizeRepository.GetProductSizeById(item.ProductSizeId);
                if (productSize == null) return (false, "No existe Talle");
                if (productSize.Habilitado == false) return (false, "Producto Inhabilitado");
                if (productSize.Stock < item.Cantidad)
                    return (false, "Cantidades no coinciden");
                productSize.Stock -= item.Cantidad;
                _ProductSizeRepository.UpdateProductSize(productSize);
            }
            order.Confirmada = true;
            order.EstadoPedido = EstadoPedido.Proceso;
            _OrderRepository.UpdateOrder(order);
            return (true, "Orden confirmada con éxito");
        }
       

        public (bool, string) PagoOrder(int id)
        {
            var order = _OrderRepository.GetOrderById(id);
            if (order == null) return (false, "Orden inexistente");
            if (order.Confirmada == false) return (false, "Orden no confirmada");
            if (order.Habilitado == false) return (false, "Orden inhabilitada");
            if (order.EstadoPedido == EstadoPedido.Cancelado || order.EstadoPedido == EstadoPedido.Finalizado) return (false, "Orden no cumple parametros");
            if (order.Pagada == true)  return (false, "No cumple con los parametros de Orden");
            order.Pagada = true;
            _OrderRepository.UpdateOrder(order);
            return (true, "Orden pagada con éxito");
        }

        public (bool, string) FinalizeOrder(int id)
        {
            var order = _OrderRepository.GetOrderById(id);
            if (order == null) return (false, "Orden no existente");
            if (order.Confirmada == false) return (false, "Orden no confirmada");
            if (order.Habilitado == false) return (false, "Orden no habilitada");
            if (order.EstadoPedido == EstadoPedido.Cancelado || order.EstadoPedido == EstadoPedido.Finalizado) return (false, "Orden no cumple parametros");
            if (order.Pagada == false) return (false, "Orden no pagada");
            order.EstadoPedido = EstadoPedido.Finalizado;
            foreach (var orderItem in order.OrderItems)
            {
                if (orderItem.Habilitado)
                {
                    var product = orderItem.ProductSize?.Product;
                    if (product != null)
                    {
                        product.TotalVentas += orderItem.Cantidad;
                    }
                }
            }
            _OrderRepository.UpdateOrder(order);
            return (true, "Orden finalizada con éxito");
        }

        public (bool, string) SoftDeleteOrder(int id)
        {
            var entity = _OrderRepository.GetOrderById(id);
            if (entity != null)
            {
                foreach(var size in entity.OrderItems)
{
                    size.Habilitado = !size.Habilitado;
                }
                entity.EstadoPedido = EstadoPedido.Cancelado;
                _OrderRepository.SoftDeleteOrder(entity);
                return (true, "Habilitacion actualiaza de Orden con éxito");
            }
            return (false, "Orden no se puede deshabilitar");
        }
        public (bool, string) HardDeleteOrder(int id)
        {
            var entity = _OrderRepository.GetOrderById(id);
            if (entity != null)
            {
                _OrderRepository.HardDeleteOrder(entity);
                return (true, "Orden borrada con éxito");
            }
            return (false, "No se pudo borrar Orden");
        }

    }
}