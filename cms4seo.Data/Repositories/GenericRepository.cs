using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace cms4seo.Data.Repositories
{
    public abstract class GenericRepository<TC, T> : IGenericRepository<T>
        where T : class
        where TC : DbContext, new()
    {
        public TC Context { get; set; } = new TC();

        public virtual IQueryable<T> GetAll()
        {
            IQueryable<T> query = Context.Set<T>();
            return query;
        }

        public IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            var query = Context.Set<T>().Where(predicate);
            return query;
        }

        public virtual void Add(T entity)
        {
            Context.Set<T>().Add(entity);
        }

        public virtual void Delete(T entity)
        {
            Context.Set<T>().Remove(entity);
        }

        public virtual void Edit(T entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Save()
        {
            Context.SaveChanges();
        }

        public T GetById(int id)
        {
            return Context.Set<T>().Find(id);
        }
    }
}