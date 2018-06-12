using Store.Domain.Model;
using Store.Domain.Repositories;
using Store.Domain.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Domain.Services
{
    public class DomainService : IDomainService
    {
        #region Private Fields
        private readonly IRepositoryContext _repositoryContext;
        private readonly IShoppingCartItemRepository _shoppingCartItemRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IProductCategorizationRepository _productCategorizationRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        #endregion

        #region Ctor

        public DomainService(IRepositoryContext repositoryContext,
            IOrderRepository orderRepository,
            IShoppingCartItemRepository shoppingCartItemRepository,
            ICategoryRepository categoryRepository,
            IProductCategorizationRepository productCategorizationRepository,
            IProductRepository productRepository,
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IUserRoleRepository userRoleRepository)
        {
            _repositoryContext = repositoryContext;
            _orderRepository = orderRepository;
            _shoppingCartItemRepository = shoppingCartItemRepository;
            _productCategorizationRepository = productCategorizationRepository;
            _userRoleRepository = userRoleRepository;
        }
        #endregion

        #region IDomainService
        /// <summary>
        /// 创建订单，涉及到的操作有2个：1.把购物车中的项中的购物移除，2.创建一个订单。
        /// 这两个操作必须同时完成或失败。
        /// </summary>
        /// <param name="user"></param>
        /// <param name="shoppingCart"></param>
        /// <returns></returns>
        public Order CreateOrder(User user, ShoppingCart shoppingCart)
        {
            var order = new Order();
            var shoppingCartItems = _shoppingCartItemRepository.GetAll(
                new ExpressionSpecification<ShoppingCartItem>(s => s.ShoopingCart.Id == shoppingCart.Id));
            if (shoppingCartItems == null || !shoppingCartItems.Any())
                throw new InvalidOperationException("购物车中没有任何物品");
            order.OrderItems = new List<OrderItem>();
            foreach (var shoppingCartItem in shoppingCartItems)
            {
                var orderItem = shoppingCartItem.ConvertToOrderItem();
                orderItem.Order = order;
                order.OrderItems.Add(orderItem);
                _shoppingCartItemRepository.Remove(shoppingCartItem);
            }
            order.User = user;
            order.Status = OrderStatus.Paid;
            _orderRepository.Add(order);
            _repositoryContext.Commit();
            return order;
        }

        /// <summary>
        /// 设置产品分类
        /// </summary>
        /// <param name="product"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public ProductCategorization Categorize(Product product, Category category)
        {
            if (product == null)
                throw new ArgumentNullException("product");
            if (category == null)
                throw new ArgumentNullException("category");
            var prdCategorization = _productCategorizationRepository.GetBySpecification(
                Specification<ProductCategorization>.Eval(c => c.ProductId == product.Id));
            /*对比这两个方法*/
            var test = _productCategorizationRepository.GetByExpression(c => c.ProductId == product.Id);

            if (prdCategorization == null)
            {
                prdCategorization = ProductCategorization.CreateCategorization(product, category);
                _productCategorizationRepository.Add(prdCategorization);
            }
            else
            {
                prdCategorization.CategoryId = category.Id;
                _productCategorizationRepository.Update(prdCategorization);
            }

            _repositoryContext.Commit();
            return prdCategorization;
        }

        #endregion
    }
}
