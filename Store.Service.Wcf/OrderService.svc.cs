using Store.Infrastructure;
using Store.ServiceContracts;
using Store.ServiceContracts.ModelDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Store.Service.Wcf
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“OrderService”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 OrderService.svc 或 OrderService.svc.cs，然后开始调试。
    
    //[ServiceBehavior(InstanceContextMode=InstanceContextMode.PerSession)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class OrderService : IOrderService
    {
        private readonly IOrderService _orderServiceImp;

        public OrderService()
        {
            _orderServiceImp = ServiceLocator.Instance.GetService<IOrderService>();
        }

        public void AddProductToCart(Guid customerId, Guid productId, int quantity)
        {
            try
            {
                _orderServiceImp.AddProductToCart(customerId, productId, quantity);
            }
            catch (Exception ex)
            {
                throw new FaultException<FaultData>(FaultData.CreateFromException(ex), FaultData.CreateFaultReason(ex));
            }
        }

        public ShoppingCartDto GetShoppingCart(Guid customerId)
        {
            try
            {
                return _orderServiceImp.GetShoppingCart(customerId);
            }
            catch (Exception e)
            {
                throw new FaultException<FaultData>(FaultData.CreateFromException(e), FaultData.CreateFaultReason(e));
            }
        }

        public int GetShoppingCartItemCount(Guid userId)
        {
            try
            {
                return _orderServiceImp.GetShoppingCartItemCount(userId);
            }
            catch (Exception ex)
            {
                throw new FaultException<FaultData>(FaultData.CreateFromException(ex), FaultData.CreateFaultReason(ex));
            }
        }

        public void UpdateShoppingCartItem(Guid shoppingCartItemId, int quantity)
        {
            try
            {
                _orderServiceImp.UpdateShoppingCartItem(shoppingCartItemId, quantity);
            }
            catch (Exception ex)
            {
                throw new FaultException<FaultData>(FaultData.CreateFromException(ex), FaultData.CreateFaultReason(ex));
            }
        }

        public void DeleteShoppingCartItem(Guid shoppingCartItemId)
        {
            try
            {
                _orderServiceImp.DeleteShoppingCartItem(shoppingCartItemId);
            }
            catch (Exception ex)
            {
                throw new FaultException<FaultData>(FaultData.CreateFromException(ex), FaultData.CreateFaultReason(ex));
            }
        }

        public OrderDto Checkout(Guid customerId)
        {
            try
            {
                return _orderServiceImp.Checkout(customerId);
            }
            catch (Exception ex)
            {
                throw new FaultException<FaultData>(FaultData.CreateFromException(ex), FaultData.CreateFaultReason(ex));
            }
        }

        public OrderDto GetOrder(Guid orderId)
        {
            try
            {
                return _orderServiceImp.GetOrder(orderId);
            }
            catch (Exception ex)
            {
                throw new FaultException<FaultData>(FaultData.CreateFromException(ex), FaultData.CreateFaultReason(ex));
            }
        }

        public IList<OrderDto> GetOrdersForUser(Guid userId)
        {
            try 
            {
                return _orderServiceImp.GetOrdersForUser(userId);
            }
            catch (Exception ex)
            {
                throw new FaultException<FaultData>(FaultData.CreateFromException(ex), FaultData.CreateFaultReason(ex));
            }
        }

        public IList<OrderDto> GetAllOrders()
        {
            try
            {
                return _orderServiceImp.GetAllOrders();
            }
            catch (Exception ex)
            {
                throw new FaultException<FaultData>(FaultData.CreateFromException(ex), FaultData.CreateFaultReason(ex));
            }
        }

        public void Dispatch(Guid orderId)
        {
            try
            {
                _orderServiceImp.Dispatch(orderId);
            }
            catch (Exception ex)
            {
                throw new FaultException<FaultData>(FaultData.CreateFromException(ex), FaultData.CreateFaultReason(ex));
            }
        }
    }
}
