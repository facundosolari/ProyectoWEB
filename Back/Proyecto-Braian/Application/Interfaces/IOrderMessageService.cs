using Application.Models.Request;
using Application.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IOrderMessageService
    {
        OrderMessageResponse? GetOrderMessageById(int id);
        List<OrderMessageResponse>? GetAllOrderMessages();
        List<OrderMessageResponse>? GetAllOrderMessagesByOrderId(int orderId);
        (bool, string) CreateMessage(OrderMessageRequest request, int senderId, string senderRole);
        (bool, string) UpdateMessage(OrderMessageRequest request, int id);

        (bool, string) MarkMessagesAsRead(int orderId, string role, int userId);

        Task<int> GetUnreadCountAsync(int orderId, int userId, string userRole);
        (bool, string) SoftDeleteMessage(int id);

    }
}
