using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Store.Domain.Repositories;
using Store.Domain;
using AutoMapper;

namespace Store.Application
{
    public abstract class ApplicationService
    {
        private readonly IRepositoryContext _repositoryContext;

        protected ApplicationService(IRepositoryContext context)
        {
            this._repositoryContext = context;
        }

        protected IRepositoryContext RepositoryContext
        {
            get { return this._repositoryContext; }
        }

        protected bool IsEmptyGuidString(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return true;
            var guid = new Guid(s);
            return guid == Guid.Empty;
        }

        /// <summary>
        /// 处理简单的聚合创建逻辑
        /// </summary>
        /// <typeparam name="TDtoList"></typeparam>
        /// <typeparam name="TDto"></typeparam>
        /// <typeparam name="TAggregateRoot"></typeparam>
        /// <param name="dataTransferObjects"></param>
        /// <param name="repository"></param>
        /// <param name="processDto"></param>
        /// <param name="processAggregrateRoot"></param>
        /// <returns></returns>
        protected TDtoList PerformCreateObjects<TDtoList, TDto, TAggregateRoot>(TDtoList dataTransferObjects,
            IRepository<TAggregateRoot> repository,
            Action<TDto> processDto = null,
            Action<TAggregateRoot> processAggregrateRoot = null)
            where TDtoList : List<TDto>, new()   // where T：new()指明了创建T的实例时应该使用的构造函数
            where TAggregateRoot : class,IAggregateRoot
        {
            if (dataTransferObjects == null)
                throw new ArgumentNullException("dataTransferObjects");
            if (repository == null)
                throw new ArgumentNullException("repository");
            TDtoList result = new TDtoList();
            if (dataTransferObjects.Count <= 0)
                return result;
            var ars = new List<TAggregateRoot>();

            foreach (var dto in dataTransferObjects)
            {
                if (processDto != null)
                    processDto(dto);
                var ar = Mapper.Map<TDto, TAggregateRoot>(dto);
                /*Id手动赋值*/
                ar.Id = Guid.NewGuid();
                if (processAggregrateRoot != null)
                    processAggregrateRoot(ar);
                ars.Add(ar);
                repository.Add(ar);
            }

            RepositoryContext.Commit();/**/

            ars.ForEach(ar => result.Add(Mapper.Map<TAggregateRoot, TDto>(ar)));
            return result;

        }




    }
}