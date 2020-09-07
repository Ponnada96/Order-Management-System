using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Orders.Domain.CustomModels
{
    /// <summary>
    /// Order Model
    /// </summary>
    public class OrderModel
    {
        public Guid UserID { get; set; }
        public Guid OrderState { get; set; }
        public string ShippingAddress { get; set; }

        public List<Item> Items { get; set; }

    }

    [XmlRoot(ElementName = "Item")]
    public class Item
    {
        [XmlElement(ElementName = "Quantity")]
        public int Quantity { get; set; }
        [XmlElement(ElementName = "ProductId")]
        public Guid ProductId { get; set; }

        //[XmlAttribute(AttributeName = "JsonArray")]
        //public string JsonArray { get; set; }
    }

   
    [XmlRoot(ElementName = "OrderedItems")]
    public class OrderedItems
    {
        [XmlElement(ElementName = "Item")]
        public List<Item> Item { get; set; } = new List<Item>();
    }

}
