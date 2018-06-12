using System.Collections.Generic;
using Store.Domain.Model;

namespace Store.Domain.Repositories
{
    public interface IShoppingCartItemRepository : IRepository<ShoppingCartItem>
    {
        ShoppingCartItem FindItem(ShoppingCart shoppingCart, Product product);
    }
}