using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace API.Data
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected DataContext DataContext { get; set; }

        public RepositoryBase(DataContext repositoryContext)
        {
            this.DataContext = repositoryContext;
        }

        public IQueryable<T> FindAll()
        {
            return this.DataContext.Set<T>()
                .AsNoTracking();
        }

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression)
        {
            return this.DataContext.Set<T>()
                .Where(expression)
                .AsNoTracking();
        }

        public void Create(T entity)
        {
            this.DataContext.Set<T>().Add(entity);
        }

        public void Update(T entity)
        {
            this.DataContext.Set<T>().Update(entity);
        }

        public void Delete(T entity)
        {
            this.DataContext.Set<T>().Remove(entity);
        }
    }
}
