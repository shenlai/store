using Store.Domain.Model;
using Store.Infrastructure.Caching;
using Store.ServiceContracts.ModelDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Store.ServiceContracts
{
    //商品服务契约的定义
    [ServiceContract(Namespace = "")]
    public interface IProductService
    {
        /*
         * 
         * [OperationContract]修饰的函数可以被客户端（Client）调用
         * 
         * WCF Service在发生异常时应抛出FaultException或FaultException<T>，而不应该抛出.NET Exception，出于以下两个原因：
         * （1）未处理的.NET Exception会使服务器与客户端之间的channel变为Fault状态，继而导致client proxy无法使用。
         * （2）.NET Exception只能被.NET平台理解，而FaultException与平台无关。如果想跨平台使用，需要使用FaultException。
         * 
         * [FaultContract]我们可以将FaultContract特性直接应用到契约操作上，指定错误细节类型，并将它传递给客户端。
         * 此外，如果操作抛出的异常没有包含在契约中，则以普通的FaultException形式传递给客户端。
         * 为了传递异常，服务必须抛出与错误契约所列完全相同的细节类型。
         * 例如，若要满足如下的错误契约定义：
         * [FaultContract(typeof(DivideByZeroException))]
         * 服务必须抛出FaultException<DivideByZeroException>异常。
         * 服务甚至不能抛出错误契约的细节类型的子类，因为它要求异常要满足契约的定义： 
         */
        #region Methods


        /*测试方法*/
        [OperationContract]
        int sum(int n);

        //Mongo测试
        IEnumerable<Product> TestGetProducts();


        [OperationContract]
        [FaultContract(typeof(FaultData))]
        List<ProductDto> CreateProducts(List<ProductDto> productDtos);

        //获得所有商品的契约方法
        [OperationContract]
        [FaultContract(typeof(FaultData))]
        [EntLibCache(CachingMethod.Get)]
        IEnumerable<ProductDto> GetProducts();

        // 获得新上市的商品的契约方法
        [OperationContract]
        IEnumerable<ProductDto> GetNewProducts(int count);

        // 获得所有类别的契约方法
        [OperationContract]
        IEnumerable<CategoryDto> GetCategories();

        // 根据商品Id来获得商品的契约方法
        [OperationContract]
        ProductDto GetProductById(Guid id);

        [OperationContract]
        [FaultContract(typeof(FaultData))]
        IEnumerable<ProductDto> GetProductsForCategory(Guid categoryId);

        // 获得所有类别的契约方法
        [OperationContract]
        [FaultContract(typeof(FaultData))]
        CategoryDto GetCategoryById(Guid id);


        [OperationContract]
        [FaultContract(typeof(FaultData))]
        ProductDtoWithPagination GetProductsWithPagination(Pagination pagination);

        [OperationContract]
        [FaultContract(typeof(FaultData))]
        ProductDtoWithPagination GetProductsForCategoryWithPagination(Guid categoryId, Pagination pagination);

        /// <summary>
        /// 设置商品分类
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(FaultData))]
        ProductCategorizationDto CategorizeProduct(Guid productId, Guid categoryId);

        #endregion
    }
}
