using Orders.Domain.CustomModels;
using Orders.Domain.DTOModels;
using System;
using System.Collections.Generic;

namespace Orders.Business.OrdersManagement.Interfaces
{
    /// <summary>
    /// OrdersManagement Interface
    /// </summary>
    public interface IOrdersManagement
    {
        /// <summary>
        /// To Create An Order
        /// </summary>
        /// <param name="OrderModel"></param>
        /// <returns></returns>
        bool AddOrder(OrderModel OrderModel);

        /// <summary>
        /// To Update An Order
        /// </summary>
        /// <param name="UpdateOrderModel"></param>
        /// <returns></returns>
        bool UpdateOrder(UpdateOrderModel UpdateOrderModel);

        /// <summary>
        /// To Delete Order
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        bool DeleteOrder(Guid OrderId);


        /// <summary>
        /// To User Order Details
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        List<OrderDetails> GetUserOrderDetails(Guid UserID);
    }
}
