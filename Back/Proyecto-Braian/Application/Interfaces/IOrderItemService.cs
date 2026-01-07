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
    public interface IOrderItemService
    {
        
        OrderItemResponse? GetOrderItemById(int id);
        // List<OrderItemResponse>? GetAllOrderItems();


        (bool, string) CreateOrderItem(OrderItemRequest request, int tokenUserId);
        /*
        bool CreateOrderItem(OrderItemRequest request, int tokenUserId); 
        bool UpdateOrderItemAdmin(OrderItemRequest request, int id);
        bool UpdateOrderItem(OrderItemPatchRequest request, int id, int tokenUserId);
        bool SoftDeleteOrderItem(int id);
        bool HardDeleteOrderItem(int id);
        */
    }
}
