﻿using System.Collections.Generic;

namespace Store.ServiceContracts.ModelDTOs
{
    public class ProductDtoWithPagination
    {
        public Pagination Pagination { get; set; }
        public List<ProductDto> ProductDtos { get; set; }
    }
}