using System.Runtime.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace OPSCallCenterCRMAPI.Database
{
    [DataContract]
    public class DatabaseEntity : EventArgs
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [DataMember]
        public string ID { get; set; }
    }
}
