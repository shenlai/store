using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization;
using Store.Domain.Model;

namespace Store.Repositories.MongoDb
{
    public static class MongoDBBootstrapper
    {
        public static void Bootstrap()
        {
            MongoDBRepositoryContext.RegisterConventions();
            BsonClassMap.RegisterClassMap<ShoppingCart>(s =>
            {
                s.AutoMap();
                s.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<Order>(s =>
            {
                s.AutoMap();
                s.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<User>(s =>
            {
                s.AutoMap();
                s.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<UserRole>(s =>
            {
                s.AutoMap();
                s.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<Category>(s =>
            {
                s.AutoMap();
                s.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<ProductCategorization>(s =>
            {
                s.AutoMap();
                s.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<Product>(s =>
            {
                s.AutoMap();
                s.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<ShoppingCartItem>(s =>
            {
                s.AutoMap();
                s.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<OrderItem>(s =>
            {
                s.AutoMap();
                s.SetIgnoreExtraElements(true);
                s.UnmapProperty<Order>(p => p.Order); // bypass circular reference.   在我们的Order对象中聚合了OrderItem，而OrderItem本身又聚合了Order，这就造成了循环引用，在序列化为MongoDB数据时会报错的，EF不会
            });



        }
    }
}
