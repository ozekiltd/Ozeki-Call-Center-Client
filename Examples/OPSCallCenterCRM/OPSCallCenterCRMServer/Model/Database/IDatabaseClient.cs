using System;
using System.Collections.Generic;
using OPSCallCenterCRMAPI.Database;

namespace OPSCallCenterCRMServer.Model.Database
{
    interface IDatabaseClient<T> where T : DatabaseEntity
    {
        T GetByID(string id);

        IEnumerable<T> GetAll();

        IEnumerable<T> GetAll(System.Linq.Expressions.Expression<Func<T, bool>> condition);

        IEnumerable<T> GetAll(List<System.Linq.Expressions.Expression<Func<T, bool>>> conditions);

        void Set(T pobject);

        void Set(IEnumerable<T> pobjects);

        long Count();

        void Delete(T pobject);
    }
}
