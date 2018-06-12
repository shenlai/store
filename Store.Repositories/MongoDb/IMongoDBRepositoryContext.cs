﻿using Store.Domain.Repositories;
using MongoDB.Driver;
using System;

namespace Store.Repositories.MongoDb
{
    /// <summary>
    /// Represents that the implemented classes are repository contexts that use
    /// MongoDB as the storage.
    /// </summary>
    public interface IMongoDBRepositoryContext:IRepositoryContext
    {
        /// <summary>
        /// Gets a <see cref="IMongoDBRepositoryContextSettings"/> instance which contains the settings
        /// information used by current context.
        /// </summary>
        IMongoDBRepositoryContextSettings Settings { get; }


        /// <summary>
        /// Gets the <see cref="MongoCollection"/> instance by the given <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> object.</param>
        /// <returns>The <see cref="MongoCollection"/> instance.</returns>
        MongoCollection GetCollectionForType(Type type);
    }
}
