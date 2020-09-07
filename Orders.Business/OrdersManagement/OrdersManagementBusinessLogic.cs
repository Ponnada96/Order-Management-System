using Orders.Business.OrdersManagement.Interfaces;
using Orders.Domain.CustomModels;
using Orders.Domain.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using Orders.Business.Helper;
using System.Data.Entity;
using Orders.Domain.DTOModels;

namespace Orders.Business.OrdersManagement
{
    /// <summary>
    /// Orders Management BusinessLogic
    /// </summary>
    public class OrdersManagementBusinessLogic : IOrdersManagement
    {

        #region Public Methods
        /// <summary>
        /// To Get Order Details
        /// </summary>
        /// <returns></returns>
        public List<OrderDetails> GetUserOrderDetails(Guid UserID)
        {
            try
            {
                using (var dbContext = new OrderManagementEntities())
                {
                    var userIfo = ValidateUser(UserID, dbContext);
                    var userOrders = new List<Order>();

                    if (userIfo.isBuyer)
                    {
                        userOrders = dbContext.Orders.Where(x => x.UserID == userIfo.Id && x.IsDeleted == false).ToList();
                    }
                    else if (userIfo.isAdmin)
                    {
                        userOrders = dbContext.Orders.Where(x => x.IsDeleted == false).ToList();
                    }

                    var orderDetais = new List<OrderDetails>();
                    foreach (var userOrder in userOrders)
                    {
                        var orderInfo = new OrderDetails();

                        orderInfo.UserName = userIfo.isBuyer ? userIfo.UserName :
                            dbContext.Users.First(x => x.Id == userOrder.UserID).UserName;

                        orderInfo.ShippingAddress = userOrder.ShippingAddress;

                        orderInfo.OrderID = userOrder.UUID;

                        var orderdItems = SerializationHelper.xmlDeserialize<OrderedItems>(userOrder.Items);
                        foreach (var orderedItem in orderdItems.Item)
                        {
                            var item = new OrderedItem();
                            var productInfo = dbContext.Products.First(x => x.UUID == orderedItem.ProductId);
                            item.Quantity = orderedItem.Quantity;
                            item.ProductName = productInfo.Name;

                            item.ProductHeight = productInfo.Height;
                            item.ProductWeight = productInfo.Weight;
                            item.StockKeepingUnit = productInfo.SKU;
                            orderInfo.OrderItems.Add(item);
                            orderDetais.Add(orderInfo);
                        }
                    }
                    return orderDetais;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// To Create an Order
        /// </summary>
        /// <param name="OrderModel"></param>
        /// <returns></returns>
        public bool AddOrder(OrderModel OrderModel)
        {
            try
            {
                using (var dbContext = new OrderManagementEntities())
                {
                   
                    var OrderStatusParentID = 1;
                    var order = new Order();
                    var items = new List<Item>();
                    var products = new List<Product>();

                    var userInfo = ValidateUser(OrderModel.UserID, dbContext);
                    if (!userInfo.isBuyer)
                        throw new Exception("Invalid User");
                    order.UserID = userInfo.Id;

                    var orderState = GetLookupValues(OrderStatusParentID, dbContext).FirstOrDefault(x => x.UUID == OrderModel.OrderState);
                    if (orderState == null)
                        throw new Exception("Invalid Order Status");

                    order.OrderState = orderState.Id;
                    order.ShippingAddress = OrderModel.ShippingAddress;
                    order.Items = xmlSerialize(OrderModel.Items, dbContext, ref products);
                    order.UUID = Guid.NewGuid();
                    dbContext.Orders.Add(order);
                    dbContext.SaveChanges();

                    //Sending Email To The User
                    SmtpHelper.SendEmail(userInfo.Email,userInfo.UserName);
                    //Updating Available Quantity of products
                    UpdateProductsAvailableQuantity(products);

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// To Update an Order 
        /// </summary>
        /// <param name="UpdateOrderModel"></param>
        /// <returns></returns>
        public bool UpdateOrder(UpdateOrderModel UpdateOrderModel)
        {
            try
            {
                using (var dbContext = new OrderManagementEntities())
                {
                    var products = new List<Product>();
                    var OrderStatusParentID = 1;
                    var userInfo = ValidateUser(UpdateOrderModel.UserID, dbContext);
                    if (!userInfo.isBuyer)
                        throw new Exception("Invalid User");

                    var orderInfo = dbContext.Orders.FirstOrDefault(x => x.UUID == UpdateOrderModel.OrderId);
                    if (orderInfo == null)
                        throw new Exception("Order Not found or Invalid Order");

                    if (orderInfo.UserID != userInfo.Id)
                        throw new Exception("Orders are not associated with give user");

                    var orderState = GetLookupValues(OrderStatusParentID, dbContext)
                                         .FirstOrDefault(x => x.UUID == UpdateOrderModel.OrderState);
                    if (orderState == null)
                        throw new Exception("Invalid Order Status");

                    orderInfo.ShippingAddress = UpdateOrderModel.ShippingAddress;
                    orderInfo.Items = xmlSerialize(UpdateOrderModel.Items, dbContext, ref products, orderInfo);
                    orderInfo.OrderState = orderState.Id;
                    dbContext.Orders.Add(orderInfo);
                    dbContext.Entry(orderInfo).State = EntityState.Modified;
                    dbContext.SaveChanges();

                    //Updating Available Quantity of products
                    UpdateProductsAvailableQuantity(products);

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// To Delete Order
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public bool DeleteOrder(Guid OrderId)
        {
            try
            {
                using (var dbContext = new OrderManagementEntities())
                {
                    var orderInfo = dbContext.Orders.FirstOrDefault(x => x.UUID == OrderId);
                    if (orderInfo == null)
                        throw new Exception("Order Not found or Invalid Order");

                    orderInfo.IsDeleted = true;
                    dbContext.Orders.Add(orderInfo);
                    dbContext.Entry(orderInfo).State = EntityState.Modified;
                    dbContext.SaveChanges();

                    //Updating Available Quantity of products
                    UpdateCanceledProductQuantity(dbContext, orderInfo);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// UpdateCanceledProductQuantity
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="orderInfo"></param>
        private void UpdateCanceledProductQuantity(OrderManagementEntities dbContext, Order orderInfo)
        {
            try
            {
                var prevProducts = SerializationHelper.xmlDeserialize<OrderedItems>(orderInfo.Items);
                var products = new List<Product>();
                foreach (var item in prevProducts.Item)
                {
                    var productInfo = dbContext.Products.FirstOrDefault(x => x.UUID == item.ProductId);
                    if (productInfo == null)
                        throw new Exception("Invalid Product");

                    productInfo.AvailableQuantity = productInfo.AvailableQuantity + item.Quantity;
                    products.Add(productInfo);
                }
                //Updating Available Quantity of products
                UpdateProductsAvailableQuantity(products);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// To Validate User
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="Dbcontext"></param>
        /// <returns></returns>
        private User ValidateUser(Guid UserId, OrderManagementEntities Dbcontext)
        {
            try
            {
                var userRoleParentId = 7;
                var buyerRoleId = "9CF54CB4-0782-410F-BBFA-653564A23948";
                var adminRoleId = "ABA6A599-BDCF-40A2-B45B-C254DA6FBC96";

                var userInfo = Dbcontext.Users.FirstOrDefault(x => x.UUID == UserId);
                if (userInfo == null)
                    throw new Exception("User Not Found");

                var userRole = GetLookupValues(userRoleParentId, Dbcontext).
                               FirstOrDefault(x => x.Id == userInfo.UserRoleID);
                if (userRole == null)
                    throw new Exception("User Role Not Found");

                if (userRole.UUID == Guid.Parse(buyerRoleId))
                {
                    userInfo.isBuyer = true;
                }
                else if (userRole.UUID == Guid.Parse(adminRoleId))
                {
                    userInfo.isAdmin = true;
                }

                return userInfo;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// xml Serialization
        /// </summary>
        /// <param name="Items"></param>
        /// <param name="Dbcontext"></param>
        /// <returns></returns>
        private string xmlSerialize(List<Item> OrderedItems, OrderManagementEntities Dbcontext, ref List<Product> products, Order orderInfo = null)
        {
            try
            {
                var orderedItemList = new OrderedItems();
                foreach (var orderedItem in OrderedItems)
                {
                    var productInfo = Dbcontext.Products.FirstOrDefault(x => x.UUID == orderedItem.ProductId);
                    if (productInfo == null)
                        throw new Exception("Invalid Product");

                    if (orderInfo == null)
                    {
                        if (productInfo.AvailableQuantity - orderedItem.Quantity < 0)
                            throw new Exception($"The availability of selected product {productInfo.Name} are {productInfo.AvailableQuantity}");

                        productInfo.AvailableQuantity = productInfo.AvailableQuantity - orderedItem.Quantity;
                    }
                    else
                    {
                        var prevOrderedItems = SerializationHelper.xmlDeserialize<OrderedItems>(orderInfo.Items);
                        var PrevOrderedItem = prevOrderedItems.Item.FirstOrDefault(x => x.ProductId == orderedItem.ProductId);
                        if (PrevOrderedItem != null)
                        {
                            if (productInfo.AvailableQuantity + PrevOrderedItem.Quantity - orderedItem.Quantity < 0)
                                throw new Exception($"The availability of selected product {productInfo.Name} are {productInfo.AvailableQuantity + PrevOrderedItem.Quantity}");

                            var quantityChange = PrevOrderedItem.Quantity - orderedItem.Quantity;
                            productInfo.AvailableQuantity = productInfo.AvailableQuantity + quantityChange;

                        }
                    }

                    products.Add(productInfo);
                    var item = new Item
                    {
                        Quantity = orderedItem.Quantity,
                        ProductId = productInfo.UUID
                    };
                    orderedItemList.Item.Add(item);
                }
                return SerializationHelper.xmlSerialize(orderedItemList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// To Update Products Available Quantity
        /// </summary>
        /// <param name="Products"></param>
        private void UpdateProductsAvailableQuantity(List<Product> Products)
        {
            try
            {
                using (var dbContext = new OrderManagementEntities())
                {
                    foreach (var pro in Products)
                    {
                        dbContext.Products.Add(pro);
                        dbContext.Entry(pro).State = EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get LookUpValues Based on ParentID
        /// </summary>
        /// <param name="ParentId"></param>
        /// <param name="Dbcontext"></param>
        /// <returns></returns>
        private IQueryable<Lookup> GetLookupValues(int ParentId, OrderManagementEntities Dbcontext)
        {
            try
            {
                return Dbcontext.Lookups.Where(x => x.ParentId == ParentId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
