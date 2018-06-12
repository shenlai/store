using Store.Domain.Model;
using Store.Infrastructure;
using Store.ServiceContracts;
using Store.ServiceContracts.ModelDTOs;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Store.Service.Wcf
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“ProductService”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 ProductService.svc 或 ProductService.svc.cs，然后开始调试。

    //商品WCF服务
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class ProductService : IProductService
    {

        //引用商品服务接口
        private readonly IProductService _productService;

        /********************Unity 注入*****************************************************************************************/
        // 1. Dependency 属性注入，标注[Dependency]
        // 2. InjectionConstructor  构造器注入，标注[InjectionConstructor] (在构造函数处标注)
        // 3. InjectionMethod 方法注入，标注[InjectionMethod]
        /********************Unity 注入*****************************************************************************************/
        //[InjectionConstructor]
        public ProductService()
        {
            /*获取ProductServiceImp类实例，调用其构造函数 */
            try {
                _productService = ServiceLocator.Instance.GetService<IProductService>();
            }
            catch (Exception e) 
            { }
            
        }

        /*测试方法*/
        public int sum(int n)
        {
            return _productService.sum(n);
        }
        //忽略此方法
        public IEnumerable<Product> TestGetProducts()
        {
            return _productService.TestGetProducts();
        }
        public IEnumerable<ProductDto> GetProducts()
        {
            return _productService.GetProducts();
        }

        public IEnumerable<ProductDto> GetNewProducts(int count)
        {
            return _productService.GetNewProducts(count);
        }

        public IEnumerable<CategoryDto> GetCategories()
        {
            return _productService.GetCategories();
        }

        public ProductDto GetProductById(Guid id)
        {
            return _productService.GetProductById(id);
        }


        public IEnumerable<ProductDto> GetProductsForCategory(Guid categoryId)
        {
            try
            {
                return _productService.GetProductsForCategory(categoryId);
            }
            catch (Exception ex)
            {
                throw new FaultException<FaultData>(FaultData.CreateFromException(ex), FaultData.CreateFaultReason(ex));
            }
        }

        public CategoryDto GetCategoryById(Guid id)
        {
            try
            {
                return _productService.GetCategoryById(id);
            }
            catch (Exception ex)
            {
                throw new FaultException<FaultData>(FaultData.CreateFromException(ex), FaultData.CreateFaultReason(ex));
            }
        }

        public ProductDtoWithPagination GetProductsWithPagination(Pagination pagination)
        {
            try
            {
                return _productService.GetProductsWithPagination(pagination);
            }
            catch (Exception ex)
            {
                throw new FaultException<FaultData>(FaultData.CreateFromException(ex), FaultData.CreateFaultReason(ex));
            }
        }

        public ProductDtoWithPagination GetProductsForCategoryWithPagination(Guid categoryId, Pagination pagination)
        {
            try
            {
                return _productService.GetProductsForCategoryWithPagination(categoryId, pagination);
            }
            catch (Exception ex)
            {
                throw new FaultException<FaultData>(FaultData.CreateFromException(ex), FaultData.CreateFaultReason(ex));
            }
        }

        public List<ProductDto> CreateProducts(List<ProductDto> productsDtos)
        {
            try
            {
                return _productService.CreateProducts(productsDtos);
            }
            catch(Exception ex)
            {
                throw new FaultException<FaultData>(FaultData.CreateFromException(ex), FaultData.CreateFaultReason(ex));
            }
        }

        public ProductCategorizationDto CategorizeProduct(Guid productId, Guid categoryId)
        {
            try
            {
                return _productService.CategorizeProduct(productId, categoryId);
            }
            catch (Exception ex)
            {
                throw new FaultException<FaultData>(FaultData.CreateFromException(ex), FaultData.CreateFaultReason(ex));
            }
        }
    }
}
