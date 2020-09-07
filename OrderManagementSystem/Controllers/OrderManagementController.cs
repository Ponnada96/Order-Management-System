using Orders.Business.OrdersManagement.Interfaces;
using Orders.Domain.CustomModels;
using System;
using System.Web.Http;

namespace OrderManagementSystem.Controllers
{
    /// <summary>
    /// Order Management Controller
    /// </summary>
    public class OrderManagementController : ApiController
    {
        public IOrdersManagement _iOrdersManagement;

        /// <summary>
        /// OrderManagementController
        /// </summary>
        /// <param name="IOrdersManagement"></param>
        public OrderManagementController(IOrdersManagement IOrdersManagement)
        {
            _iOrdersManagement = IOrdersManagement;
        }

        /// <summary>
        /// To Add an Order
        /// </summary>
        /// <param name="OrderModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/AddOrder")]
        public IHttpActionResult AddOrder(OrderModel OrderModel)
        {
            try
            {
                var response = _iOrdersManagement.AddOrder(OrderModel);
                return Ok(response);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// To Update Order Details
        /// </summary>
        /// <param name="UpdateOrderModel"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/UpdateOrder")]
        public IHttpActionResult UpdateOrder(UpdateOrderModel UpdateOrderModel)
        {
            try
            {
                var response = _iOrdersManagement.UpdateOrder(UpdateOrderModel);
                return Ok(response);
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
        [HttpDelete]
        [Route("api/DeleteOrder/{OrderId}")]
        public IHttpActionResult DeleteOrder(Guid OrderId)
        {
            try
            {
                var response = _iOrdersManagement.DeleteOrder(OrderId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// To Get Orders
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetOrderDetails/{UserID}")]
        public IHttpActionResult GetOrderDetails(Guid UserID)
        {
            try
            {
                var response = _iOrdersManagement.GetUserOrderDetails(UserID);
                return Ok(response);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
