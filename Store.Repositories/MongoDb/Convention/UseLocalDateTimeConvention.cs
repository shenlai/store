using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using System;
using System.Reflection;
using Store.Domain;
using MongoDB.Bson.Serialization.Serializers;

namespace Store.Repositories.MongoDb
{
    public class UseLocalDateTimeConvention:IMemberMapConvention
    {
        #region IMemberMaoConention Members
        /// <summary>
        /// Applies the specified member map convention.
        /// </summary>
        /// <param name="memberMap">The member map convention.</param>
        public void Apply(BsonMemberMap memberMap)
        {
            //IBsonSerializationOptions  2.0以后废弃
            if (memberMap.MemberType == typeof(DateTime))
            {
                var dateTimeSerializer = new DateTimeSerializer(DateTimeKind.Local);
                memberMap.SetSerializer(dateTimeSerializer);
            }
            else if (memberMap.MemberType == typeof(DateTime?))
            {
                var dateTimeSerializer = new DateTimeSerializer(DateTimeKind.Local);
                var nullableDateTimeSerializer = new NullableSerializer<DateTime>(dateTimeSerializer);
                memberMap.SetSerializer(nullableDateTimeSerializer);
            }
        }
        #endregion

        #region IConvention Members
        public string Name
        {
            get { return this.GetType().Name; }
        }
        #endregion

    }
}
