﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.ServiceContracts.ModelDTOs
{
    public class CategoryDto
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        //public IEnumerable<ProductDto> Products { get; set; }
    }
}
