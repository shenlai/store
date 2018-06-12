using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.IdGenerators;
using Store.Domain;

namespace Store.Repositories.MongoDb
{

    /// <summary>
    /// Represents the ID generator convention which generates a <see cref="System.Guid"/> value
    /// for ID.
    /// </summary>
    public class GuidIDGeneratorConvention:IPostProcessingConvention
    {
        #region IPostProcessingConvertion Members
        /// <summary>
        /// Post process the class map
        /// </summary>
        /// <param name="classMap"> the class map to be processed </param>
        public void PostProcess(BsonClassMap classMap)
        {
            if (typeof(IEntity).IsAssignableFrom(classMap.ClassType) && classMap.IdMemberMap != null)
            {
                classMap.IdMemberMap.SetIdGenerator(new GuidGenerator());
            }
        }

        #endregion

        #region IConvertion　Members

        /// <summary>
        /// Gets the name of the convertion
        /// </summary>
        public string Name
        {
            get { return this.GetType().Name; }
        }
        #endregion

    }
}
