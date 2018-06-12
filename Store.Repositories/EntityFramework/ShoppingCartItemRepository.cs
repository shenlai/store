using Store.Domain.Model;
using Store.Domain.Repositories;
using Store.Domain.Specifications;

namespace Store.Repositories.EntityFramework
{                           
    public class ShoppingCartItemRepository:EntityFrameworkRepository<ShoppingCartItem>,IShoppingCartItemRepository
    {
        public ShoppingCartItemRepository(IRepositoryContext context)
            :base(context)
        { 
        }

        public ShoppingCartItem FindItem(ShoppingCart shoppingCart, Product product)
        {
            return base.GetBySpecification(Specification<ShoppingCartItem>.Eval
                (sci => sci.ShoopingCart.Id == shoppingCart.Id &&
                sci.Product.Id == product.Id), elp => elp.Product);
        }
    }
}
