using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Store.ServiceContracts;
using Store.Domain.Repositories;
using Store.Repositories.EntityFramework;
using Store.Domain.Model;
using Store.Domain.Specifications;
using Store.Domain;
using Store.ServiceContracts.ModelDTOs;
using AutoMapper;
using Store.Domain.Services;
using Store.Domain.Enum;
using System.Transactions;
using Store.Events;
using Store.Events.Bus;

namespace Store.Application.ServiceImplementations
{
    public class OrderServiceImpl : ApplicationService, IOrderService
    {
        #region

        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly IShoppingCartItemRepository _shoppingCartItemRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;
        private readonly IDomainService _domainService;
        private readonly IOrderRepository _orderRepository;
        private readonly IEventBus _eventBus;

        #endregion

        #region Ctor

        public OrderServiceImpl(IRepositoryContext context,
            IUserRepository userRepository,
            IShoppingCartRepository shoppingCartRepository,
            IProductRepository productRepository,
            IShoppingCartItemRepository shoppingCartItemRepository,
            IDomainService domainService,
            IOrderRepository orderRepository,
            IEventBus bus)
            : base(context)
        {
            _userRepository = userRepository;
            _shoppingCartRepository = shoppingCartRepository;
            _productRepository = productRepository;
            _shoppingCartItemRepository = shoppingCartItemRepository;
            _domainService = domainService;
            _orderRepository = orderRepository;
            _eventBus = bus;
        }


        #endregion

        #region IOrderService Members

        public void AddProductToCart(Guid customerId, Guid productId, int quantity)
        {
            var user = _userRepository.GetByKey(customerId);

            //备注：源代码此处 可以加载到shoppingCart.User的值，而这里不能，为什么？(已解决，仓储上下文必须单例）  
            var shoppingCart = _shoppingCartRepository.GetBySpecification(new ExpressionSpecification<ShoppingCart>(s => s.User.Id == user.Id));
            /*调试代码*/
            //var shoppingCart1 = _shoppingCartRepository.GetBySpecification(new ExpressionSpecification<ShoppingCart>(s => s.User.Id == user.Id),s=>s.User);
            //var userInfo = shoppingCart.User;   /*User 非vitrual  不支持延时加载*/
            /*以上*/
            if (shoppingCart == null)
                throw new DomainException("用户{0}不存在购物车", customerId);

            var product = _productRepository.GetByKey(productId);
            var shoppingCartItem = _shoppingCartItemRepository.FindItem(shoppingCart, product);
            if (shoppingCartItem == null)
            {
                shoppingCartItem = new ShoppingCartItem()
                {
                    Product = product,
                    ShoopingCart = shoppingCart,
                    Quantity = quantity
                };

                _shoppingCartItemRepository.Add(shoppingCartItem);
            }
            else
            {
                shoppingCartItem.UpdateQuantity(shoppingCartItem.Quantity + quantity);
                _shoppingCartItemRepository.Update(shoppingCartItem);
            }
            RepositoryContext.Commit();
        }

        public ShoppingCartDto GetShoppingCart(Guid customerId)
        {
            var user = _userRepository.GetByKey(customerId);
            var shoppingCart = _shoppingCartRepository.GetBySpecification(
                new ExpressionSpecification<ShoppingCart>(s => s.User.Id == user.Id));
            if (shoppingCart == null)
                throw new DomainException("用户{0}不存在购物车.", customerId);
            var shoppingCartItems = _shoppingCartItemRepository.GetAll(
                new ExpressionSpecification<ShoppingCartItem>(s => s.ShoopingCart.Id == shoppingCart.Id));
            var shoppingCartDto = Mapper.Map<ShoppingCart, ShoppingCartDto>(shoppingCart);
            shoppingCartDto.Items = new List<ShoppingCartItemDto>();

            if (shoppingCartItems != null && shoppingCartItems.Any())
            {
                shoppingCartItems.ToList()
                    .ForEach(s => shoppingCartDto.Items.Add(Mapper.Map<ShoppingCartItem, ShoppingCartItemDto>(s)));
                shoppingCartDto.Subtotal = shoppingCartDto.Items.Sum(p => p.ItemAmount);
            }
            return shoppingCartDto;
        }

        public int GetShoppingCartItemCount(Guid userId)
        {
            var user = _userRepository.GetByKey(userId);
            var shoppingCart = _shoppingCartRepository.GetBySpecification(new ExpressionSpecification<ShoppingCart>(s => s.User.Id == user.Id));
            if (shoppingCart == null)
                throw new InvalidOperationException("没有可用的购物车实例.");
            var shoppingCartItems =
                _shoppingCartItemRepository.GetAll(new ExpressionSpecification<ShoppingCartItem>(s => s.ShoopingCart.Id == shoppingCart.Id));
            return shoppingCartItems.Sum(s => s.Quantity);
        }

        public void UpdateShoppingCartItem(Guid shoppingCartItemId, int quantity)
        {
            var shoppingCartItem = _shoppingCartItemRepository.GetByKey(shoppingCartItemId);
            shoppingCartItem.UpdateQuantity(quantity);
            _shoppingCartItemRepository.Update(shoppingCartItem);
            RepositoryContext.Commit();
        }

        public void DeleteShoppingCartItem(Guid shoppingCartItemId)
        {
            var shoppingCartItem = _shoppingCartItemRepository.GetByKey(shoppingCartItemId);
            _shoppingCartItemRepository.Remove(shoppingCartItem);
            RepositoryContext.Commit();
        }

        public OrderDto Checkout(Guid customerId)
        {
            var user = _userRepository.GetByKey(customerId);
            var shoppingCart = _shoppingCartRepository.GetByExpression(s => s.User.Id == user.Id);
            var order = _domainService.CreateOrder(user, shoppingCart);

            var dto = Mapper.Map<Order, OrderDto>(order);
            return dto;
        }

        public OrderDto GetOrder(Guid orderId)
        {
            /*此处 饥饿加载*/
            var order = _orderRepository.GetBySpecification(new ExpressionSpecification<Order>(o => o.Id.Equals(orderId)), elp => elp.OrderItems);

            return Mapper.Map<Order, OrderDto>(order);

        }

        //获得指定用户的所有订单
        public IList<OrderDto> GetOrdersForUser(Guid userId)
        {
            var user = _userRepository.GetByKey(userId);
            var orders = _orderRepository.GetAll(new ExpressionSpecification<Order>(o => o.User.Id == userId),
                sp => sp.CreatedDate,
                SortOrder.Descending,
                elp => elp.OrderItems);
            var orderDtos = new List<OrderDto>();
            orders.ToList().ForEach(o => orderDtos.Add(Mapper.Map<Order, OrderDto>(o)));
            return orderDtos;
        }

        public IList<OrderDto> GetAllOrders()
        {
            var orders = _orderRepository.GetAll(sort => sort.CreatedDate, SortOrder.Descending);
            var orderDtos = new List<OrderDto>();
            orders
                .ToList()
                .ForEach(o => orderDtos.Add(Mapper.Map<Order, OrderDto>(o)));
            return orderDtos;
        }


        /// <summary>
        /// 发货
        /// </summary>
        /// <param name="orderId"></param>
        public void Dispatch(Guid orderId)
        {
            using (var transactionScope = new TransactionScope())
            {
                var order = _orderRepository.GetByKey(orderId);
                order.Dispatch();
                //Dispatch: 实体方法,处理逻辑主要是更新订单的状态和更新时间，
                //然后再将该事件发布到EventBus(MsmqEventBus)，EventBus(MsmqEventBus)中保存了一个队列来存放事件，发布操作的实现就是往该队列插入一个待处理的事件
                _orderRepository.Update(order);
                RepositoryContext.Commit();
                _eventBus.Commit();//Commit:对队列中的事件进行出队列操作，通过事件聚合类来获得对应事件处理器来对出队列的事件进行处理
                transactionScope.Complete();
            }
        }
        #endregion




    }
}