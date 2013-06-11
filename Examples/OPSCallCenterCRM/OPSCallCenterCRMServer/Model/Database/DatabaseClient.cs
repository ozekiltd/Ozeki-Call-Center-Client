using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
using OPSCallCenterCRMAPI.Database;

namespace OPSCallCenterCRMServer.Model.Database
{
    internal class DatabaseClient<T> : IDatabaseClient<T> where T : DatabaseEntity
    {
        private MongoDatabase _repository;
        private MongoClient _mongoClient;
        private MongoServer _mongoServer;
        private string _collectionName;

        public DatabaseClient()
            : this(DatabaseConnectionProperties.Default)
        { }

        public DatabaseClient(DatabaseConnectionProperties properties)
        {
            _mongoClient = new MongoClient(properties.ConnectionString);
            _mongoServer = _mongoClient.GetServer();
            _repository = _mongoServer.GetDatabase("OPSCallCenterCRM");
            _collectionName = typeof(T).Name;
        }

        public T GetByID(string id)
        {
            return _repository.GetCollection<T>(_collectionName).FindOne(Query.EQ("_id", new ObjectId(id)));
        }

        public IEnumerable<T> GetAll()
        {
            return _repository.GetCollection<T>(_collectionName).FindAll();
        }

        public IEnumerable<T> GetAll(System.Linq.Expressions.Expression<Func<T, bool>> condition)
        {
            return _repository.GetCollection<T>(_collectionName).AsQueryable().Where(condition);
        }

        public IEnumerable<T> GetAll(List<System.Linq.Expressions.Expression<Func<T, bool>>> conditions)
        {
            var query = _repository.GetCollection<T>(_collectionName).AsQueryable<T>();

            foreach (var condition in conditions)
                query = query.Where(condition);

            return query.ToList();
        }

        public void Set(T pobject)
        {
            _repository.GetCollection<T>(_collectionName).Save(pobject);
        }

        public void Set(IEnumerable<T> pobjects)
        {
            var query = _repository.GetCollection<T>(_collectionName);
            foreach (var pobject in pobjects)
                query.Save(pobject);
        }

        public long Count()
        {
            return _repository.GetCollection<T>(_collectionName).AsQueryable().LongCount();
        }

        public void Delete(T pobject)
        {
            _repository.GetCollection<T>(_collectionName).Remove(Query.EQ("_id", new ObjectId(pobject.ID)));
        }
    }
}
