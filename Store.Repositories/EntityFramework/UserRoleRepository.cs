using System;
using System.Linq;
using Store.Domain.Model;
using Store.Domain.Repositories;

namespace Store.Repositories.EntityFramework
{
    public class UserRoleRepository : EntityFrameworkRepository<UserRole>, IUserRoleRepository
    {
        public UserRoleRepository(IRepositoryContext context)
            : base(context)
        { }

        public Role GetRoleForUser(User user)
        {
            var context = EfContext.DbContext as StoreDbContext;
            if (context != null)
            {
                var query = from role in context.Roles
                            from userRole in context.UserRoles
                            from usr in context.Users
                            where role.Id == userRole.RoleId &&
                                usr.Id == userRole.UserId &&
                                usr.Id == user.Id
                            select role;
                return query.FirstOrDefault();
            }
            throw new InvalidOperationException("The provided repository context is invalid.");
        }
    }
}
