﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository.IRepository
{
    public  interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll(string? IncludeProperties = null);
        T Get(Expression<Func<T,bool>> filter, string? IncludeProperties = null);

        void Add(T entity);

        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
    }
}
