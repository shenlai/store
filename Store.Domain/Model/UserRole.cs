﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Domain.Model
{
    public class UserRole : AggregateRoot
    {
        public Guid UserId { get; set; }

        public Guid RoleId { get; set; }

        public static UserRole CreateUserRole(User user, Role role)
        {
            return new UserRole() { UserId = user.Id, RoleId = role.Id };
        }
    }
}
