using Store.Domain;
using Store.Domain.Enum;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Store.Repositories.EntityFramework
{
    // 因为IQuerable中的OrderBy方法是强类型的
    // 定义排序的扩展方式，实现动态排序
    internal static class SortByExtension
    {
        #region Internal Methods

        internal static IOrderedQueryable<TAggregateRoot> SortBy<TAggregateRoot>(this IQueryable<TAggregateRoot> query, Expression<Func<TAggregateRoot, dynamic>> sortPredicate)
            where TAggregateRoot:class,IAggregateRoot
        {
            return InvokeSortBy(query, sortPredicate, SortOrder.Ascending);
        }

        internal static IOrderedQueryable<TAggregateRoot> SortByDescending<TAggregateRoot>(this IQueryable<TAggregateRoot> query, Expression<Func<TAggregateRoot, dynamic>> sortPredicate)
            where TAggregateRoot : class,IAggregateRoot
        {
            return InvokeSortBy(query, sortPredicate, SortOrder.Descending);
        }
        #endregion

        #region Private Methods

        private static IOrderedQueryable<TAggregateRoot> InvokeSortBy<TAggregateRoot>(IQueryable<TAggregateRoot> query,
            Expression<Func<TAggregateRoot, dynamic>> sortPredicate, SortOrder sortOrder)
            where TAggregateRoot : class,IAggregateRoot
        {
            var param = sortPredicate.Parameters[0];//哪一个值？
            string propertyName = null;
            Type propertyType = null;
            Expression bodyExpression = null;

            /*UnaryExpression的意思是：表示包含一元运算符的表达式（一元：说明了只有一个操作数，通过Operand属性成员即可获得其操作数）*/
            if (sortPredicate.Body is UnaryExpression)
            {
                var unaryExpression = sortPredicate.Body as UnaryExpression;
                bodyExpression = unaryExpression.Operand;
            }
            else if (sortPredicate.Body is MemberExpression)/*表示访问字段或属性*/
            {
                bodyExpression = sortPredicate.Body;
            }
            else
            {
                throw new ArgumentException(@"The body of the sort predicate expression should be 
                either UnaryExpression or MemberExpression.", "sortPredicate");

            }
            var memberExpression = (MemberExpression)bodyExpression;
            propertyName = memberExpression.Member.Name;
            if (memberExpression.Member.MemberType == MemberTypes.Property)
            {
                var propertyInfo = memberExpression.Member as PropertyInfo;
                if (propertyInfo != null) propertyType = propertyInfo.PropertyType;
            }
            else
                throw new InvalidOperationException(@"Cannot evaluate the type of property since the member expression 
                represented by the sort predicate expression does not contain a PropertyInfo object.");

            var funcType = typeof(Func<,>).MakeGenericType(typeof(TAggregateRoot), propertyType);
            var convertedExpression = Expression.Lambda(funcType, 
                Expression.Convert(Expression.Property(param, propertyName), propertyType), 
                param);

            var sortingMethods = typeof(Queryable).GetMethods(BindingFlags.Public | BindingFlags.Static);
            var sortingMethodName = GetSortingMethodName(sortOrder);
            var sortingMethod = sortingMethods.First(sm => sm.Name == sortingMethodName && sm.GetParameters().Length == 2);

            return (IOrderedQueryable<TAggregateRoot>)sortingMethod.MakeGenericMethod(typeof(TAggregateRoot), propertyType).Invoke(null, new object[] { query, convertedExpression });
        }

        private static string GetSortingMethodName(SortOrder sortOrder)
        {
            switch (sortOrder)
            {
                case SortOrder.Ascending:
                    return "OrderBy";
                case SortOrder.Descending:
                    return "OrderByDescending";
                default:
                    throw new ArgumentException("Sort Order must be specified as either Ascending or Descending.", "sortOrder");
            }
        }

        #endregion
    }
}
