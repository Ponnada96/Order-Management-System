using System;

namespace Orders.Domain.CustomModels
{
    /// <summary>
    /// Update Order Model
    /// </summary>
    public class UpdateOrderModel : OrderModel
    {
        public Guid OrderId { get; set; }
        public bool IsDeleted { get; set; }
    }
}
