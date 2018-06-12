using AutoMapper;
using Store.Domain.Enum;
using Store.Domain.Model;
using Store.Domain.Repositories;
using Store.Domain.Services;
using Store.ServiceContracts;
using Store.ServiceContracts.ModelDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Application.ServiceImplementations
{
    // 商品服务的实现                   
    public class ProductServiceImp : ApplicationService, IProductService
    {
        #region Private Fields
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductCategorizationRepository _productCategorizationRepository;
        private readonly IDomainService _domainService;
        #endregion

        #region Ctor
        public ProductServiceImp(IRepositoryContext context,
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            IProductCategorizationRepository productCategorizationRepository,
            IDomainService domainservice)
            : base(context)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _productCategorizationRepository = productCategorizationRepository;
            _domainService = domainservice;
        }

        #endregion

        #region IProductService Members

        /*测试方法*/
        public int sum(int n)
        {
            return n * n;
        }

        public IEnumerable<Product> TestGetProducts()
        {
            return _productRepository.GetAll();
        }


        public IEnumerable<ProductDto> GetProducts()
        {
            //return _productRepository.GetAll();
            var result = new List<ProductDto>();

            _productRepository.GetAll().ToList()
                .ForEach(p =>
                {
                    var productDto = Mapper.Map<Product, ProductDto>(p);
                    {
                        var category = _productCategorizationRepository.GetCategoryForProduct(p);
                        if (category != null)
                            productDto.Category = Mapper.Map<Category, CategoryDto>(category);
                    }
                    result.Add(productDto);
                });
            return result;
        }

        public IEnumerable<CategoryDto> GetCategories()
        {
            //return _categoryRepository.GetAll();
            var result = new List<CategoryDto>();
            _categoryRepository.GetAll().ToList().ForEach(c =>
                {
                    var categoryDto = Mapper.Map<Category, CategoryDto>(c);
                    result.Add(categoryDto);
                });
            return result;
        }

        public IEnumerable<ProductDto> GetNewProducts(int count)
        {
            //return _productRepository.GetNewProducts(count);
            var newProducts = new List<ProductDto>();
            _productRepository.GetNewProducts(count)
                .ToList()
                .ForEach
                (
                    np => newProducts.Add(Mapper.Map<Product, ProductDto>(np))
                );

            return newProducts;
        }

        public ProductDto GetProductById(Guid id)
        {
            var product = _productRepository.GetByKey(id);
            var result = Mapper.Map<Product, ProductDto>(product);
            result.Category =
                Mapper.Map<Category, CategoryDto>(_productCategorizationRepository.GetCategoryForProduct(product));
            return result;
        }

        public IEnumerable<ProductDto> GetProductsForCategory(Guid categoryId)
        {
            var result = new List<ProductDto>();

            var category = _categoryRepository.GetByKey(categoryId);
            var products = _productCategorizationRepository.GetProductsForCategory(category);
            products.ToList().ForEach(p => result.Add(Mapper.Map<Product, ProductDto>(p)));
            return result;
        }

        // 获得所有类别的契约方法
        public CategoryDto GetCategoryById(Guid id)
        {
            var category = _categoryRepository.GetByKey(id);
            var result = Mapper.Map<Category, CategoryDto>(category);

            return result;
        }

        public ProductDtoWithPagination GetProductsWithPagination(Pagination pagination)
        {
            var pagedProducts = _productRepository.GetAll(sp => sp.Name, SortOrder.Ascending, pagination.PageNumber,
                pagination.PageSize);
            pagination.TotalPages = pagedProducts.TotalPages;

            var productDtoList = new List<ProductDto>();
            pagedProducts.PageData.ToList().ForEach(p => productDtoList.Add(Mapper.Map<Product, ProductDto>(p)));
            return new ProductDtoWithPagination()
            {
                Pagination = pagination,
                ProductDtos = productDtoList
            };
        }

        public ProductDtoWithPagination GetProductsForCategoryWithPagination(Guid categoryId, Pagination pagination)
        {
            var category = _categoryRepository.GetByKey(categoryId);
            var pagedProducts = _productCategorizationRepository.GetProductsForCategoryWithPagination(category, pagination.PageNumber,
                pagination.PageSize);
            if (pagedProducts == null)
            {
                pagination.TotalPages = 0;
                return new ProductDtoWithPagination()
                {
                    Pagination = pagination,
                    ProductDtos = new List<ProductDto>()
                };
            }

            pagination.TotalPages = pagedProducts.TotalPages;
            var productDtoList = new List<ProductDto>();
            pagedProducts.PageData.ToList().ForEach(p => productDtoList.Add(Mapper.Map<Product, ProductDto>(p)));
            return new ProductDtoWithPagination()
            {

                Pagination = pagination,
                ProductDtos = productDtoList
            };
        }

        public List<ProductDto> CreateProducts(List<ProductDto> productsDtos)
        {
            return PerformCreateObjects<List<ProductDto>, ProductDto, Product>(productsDtos, _productRepository);
        }

        /// <summary>
        /// 设置产品分类
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public ProductCategorizationDto CategorizeProduct(Guid productId,Guid categoryId)
        {
            if (productId == Guid.Empty)
                throw new ArgumentNullException("productId");
            if (categoryId == Guid.Empty)
                throw new ArgumentNullException("categoryId");
            var product = _productRepository.GetByKey(productId);
            var category = _categoryRepository.GetByKey(categoryId);

            var productCategorization = _domainService.Categorize(product, category);
            return Mapper.Map<ProductCategorization, ProductCategorizationDto>(productCategorization);

        }
        #endregion

    }
}
