using System;
using System.Collections.Generic;
namespace Orders.Domain.DTOModels
{
    /// <summary>
    /// OrderDetails
    /// </summary>
    public class OrderDetails
    {
        public Guid OrderID { get; set; }
        public string UserName { get; set; }
        public string ShippingAddress { get; set; }
        public List<OrderedItem> OrderItems { get; set; } = new List<OrderedItem>();
    }

    /// <summary>
    /// Item
    /// </summary>
    public class OrderedItem
    {
        public int Quantity { get; set; }
        public string ProductName { get; set; }
        public decimal ProductHeight { get; set; }
        public decimal ProductWeight { get; set; }
        public string StockKeepingUnit { get; set; }
    }
}
