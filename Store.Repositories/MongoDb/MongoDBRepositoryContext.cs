using Store.Domain.Repositories;
using Store.Repositories.EntityFramework;
using Store.Repositories.MongoDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using System.Threading;
using Store.Domain;
using MongoDB.Bson.Serialization.Conventions;
using System.Reflection;
using MongoDB.Bson;

namespace Store.Repositories.MongoDb
{
    /// <summary>
    /// Represents the MongoDB repository context.
    /// </summary>
    public class MongoDBRepositoryContext:IMongoDBRepositoryContext
    {
        

        #region 

        private readonly Guid id = Guid.NewGuid();
        private readonly IMongoDBRepositoryContextSettings settings;
        private readonly MongoServer server;
        private readonly MongoDatabase database;
        //修改成以下呢
        //private readonly IMongoDatabase database;

        private readonly object syncObj = new object();
        private readonly Dictionary<Type, MongoCollection> mongoCollections = new Dictionary<Type, MongoCollection>();
        /*ThreadLocal:http://www.csharpwin.com/csharpspace/13101r4743.shtml*/
        private readonly ThreadLocal<List<object>> localNewCollection = new ThreadLocal<List<object>>(() => new List<object>());
        private readonly ThreadLocal<List<object>> localModifiedCollection = new ThreadLocal<List<object>>(() => new List<object>());
        private readonly ThreadLocal<List<object>> localDeletedCollection = new ThreadLocal<List<object>>(() => new List<object>());
        #endregion

        #region Ctor
        public MongoDBRepositoryContext(IMongoDBRepositoryContextSettings settings)
        {
            this.settings = settings;
            this.server = new MongoServer(settings.ServerSettings);
            database = server.GetDatabase(settings.DatabaseName, settings.GetDatabaseSettings(server));

            //
            //var client = new MongoClient()

        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Clears all the registration in the repository context.
        /// </summary>
        /// <remarks>Note that this can only be called after the repository context has successfully committed.</remarks>
        protected void ClearRegistrations()
        {
            this.localNewCollection.Value.Clear();
            this.localModifiedCollection.Value.Clear();
            this.localDeletedCollection.Value.Clear();
        }
        #endregion

        #region Public Static Method
        /// <summary>
        /// Registers the MongoDB Bson serialization conventions.
        /// </summary>
        /// <param name="autoGenerateID">A <see cref="Boolean"/> value which indicates whether
        /// the ID value should be automatically generated when a new document is inserting.</param>
        /// <param name="localDateTime">A <see cref="Boolean"/> value which indicates whether
        /// the local date/time should be used when serializing/deserializing <see cref="DateTime"/> values.</param>
        public static void RegisterConventions(bool autoGenerateID = true, bool localDateTime = true)
        {
            RegisterConventions(autoGenerateID, localDateTime, null);
        }


        /// <summary>
        /// Registers the MongoDB Bson serialization conventions.
        /// </summary>
        /// <param name="autoGenerateID">A <see cref="Boolean"/> value which indicates whether
        /// the ID value should be automatically generated when a new document is inserting.</param>
        /// <param name="localDateTime">A <see cref="Boolean"/> value which indicates whether
        /// the local date/time should be used when serializing/deserializing <see cref="DateTime"/> values.</param>
        /// <param name="additionConventions">Additional conventions that needs to be registered.</param>
        public static void RegisterConventions(bool autoGenerateID, bool localDateTime, IEnumerable<IConvention> additionConventions)
        {
            var conventionPack = new ConventionPack();
            conventionPack.Add(new NamedIdMemberConvention("id", "Id", "iD", "ID"));
            if (autoGenerateID)
                conventionPack.Add(new GuidIDGeneratorConvention());
            if (localDateTime)
                conventionPack.Add(new UseLocalDateTimeConvention());
            if (additionConventions != null)
                conventionPack.AddRange(additionConventions);

            ConventionRegistry.Register("DefaultConvention", conventionPack, t => true);
        }
        #endregion

        #region IMongoDBRepositoryContext Member

        /// <summary>
        /// Gets a <see cref="IMongoDBRepositoryContextSettings"/> instance which contains the settings
        /// information used by current context.
        /// </summary>
        public IMongoDBRepositoryContextSettings Settings
        {
            get { return settings; }
        }

        /// <summary>
        /// Gets the <see cref="MongoCollection"/> instance by the given <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> object.</param>
        /// <returns>The <see cref="MongoCollection"/> instance.</returns>
        public MongoCollection GetCollectionForType(Type type)
        {
            lock (syncObj)
            {
                if (this.mongoCollections.ContainsKey(type))
                    return this.mongoCollections[type];
                else
                {
                    MongoCollection mongoColl = null;
                    if (this.settings.MapTypeToCollectionName != null)
                        mongoColl = this.database.GetCollection(settings.MapTypeToCollectionName(type));
                    else
                        mongoColl = this.database.GetCollection(type.Name);

                    this.mongoCollections.Add(type, mongoColl);
                    return mongoColl;
                }
            }

        }




        #endregion

        #region IRepositoryContext Member
        public Guid Id { get { return this.id; } }

        public void RegisterNew<TAggregateRoot>(TAggregateRoot entity) where TAggregateRoot : class, IAggregateRoot
        {
            localNewCollection.Value.Add(entity);
            Committed = false;
        }

        public void RegisterModified<TAggregateRoot>(TAggregateRoot entity)
            where TAggregateRoot : class, IAggregateRoot
        {
            if (localDeletedCollection.Value.Contains(entity))
            {
                throw new InvalidOperationException("The object Can not be registered as a modified object since it was marked as deleted.");
            }
            if (!localModifiedCollection.Value.Contains(entity) && !localNewCollection.Value.Contains(entity))
            {
                localModifiedCollection.Value.Add(entity);
            }
            Committed = false;
        }

        public void RegisterDeleted<TAggregateRoot>(TAggregateRoot entity)
            where TAggregateRoot : class, IAggregateRoot
        {
            if (localNewCollection.Value.Contains(entity))
            {
                if (localNewCollection.Value.Remove(entity))
                    return;
            }
            bool removedFromModified = localModifiedCollection.Value.Remove(entity);
            bool addedToDeleted = false;
            if (!localDeletedCollection.Value.Contains(entity))
            {
                localDeletedCollection.Value.Add(entity);
                addedToDeleted = true;
            }
            Committed = !(removedFromModified || addedToDeleted);
        }

        #endregion

        #region IUnit Of Work Member

        public bool Committed { get; protected set; }

        public void Commit()
        {
            lock (syncObj)
            {
                foreach (var newObj in this.localNewCollection.Value)
                {
                    MongoCollection coll = this.GetCollectionForType(newObj.GetType());
                    coll.Insert(newObj);
                }
                foreach (var modifiedObj in this.localModifiedCollection.Value)
                {
                    MongoCollection coll = this.GetCollectionForType(modifiedObj.GetType());
                    coll.Save(modifiedObj);
                }
                foreach (var delObj in this.localDeletedCollection.Value)
                {
                    Type objType = delObj.GetType();
                    PropertyInfo propertyInfo = objType.GetProperty("ID", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    if (propertyInfo == null)
                        throw new InvalidOperationException("Cannot delete an abject which doesn't contain an ID property.");
                    Guid id = (Guid)propertyInfo.GetValue(delObj, null);
                    MongoCollection coll = this.GetCollectionForType(objType);
                    //IMongoQuery query = Query.EQ();
                    //coll.Remove(query);
                   
                    //var filter = Builders<BsonDocument> .Filter.Eq("_id", id);
                    //coll.Remove(filter);
                    var query = new QueryDocument("_id", id);
                    coll.Remove(query);
                }
                this.ClearRegistrations();
                this.Committed = true;
            }
        }

        public void Rollback()
        {
            this.Committed = false;
        }
        #endregion



    }
}
