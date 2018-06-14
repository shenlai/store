
using Store.Web.UserService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Web.Common
{
    public class WebContext
    {
        public Guid UserId { get; set; }

        public UserDto user { get; set; }



    }
}
