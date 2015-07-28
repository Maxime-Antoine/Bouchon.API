using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Bouchon.API.Db
{
    public class Repository<T> : IRepository<T> where T : class
    {
        public async Task<IEnumerable<T>> GetAll()
        {
            using (var db = new AppDbContext())
            {
                return await db.Set<T>().ToListAsync();
            }
        }

        public async Task<T> GetById(int id)
        {
            using (var db = new AppDbContext())
            {
                return await db.Set<T>().FindAsync(id);
            }
        }

        public async Task<T> Add(T entity)
        {
            T createdEntity;
            using (var db = new AppDbContext())
            {
                createdEntity = db.Set<T>().Add(entity);
                await db.SaveChangesAsync();
            }

            return createdEntity;
        }

        public async Task Update(T entity)
        {
            using (var db = new AppDbContext())
            {
                db.Set<T>().Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
        }

        public async Task Delete(int id)
        {
            using (var db = new AppDbContext())
            {
                var dbSet = db.Set<T>();
                var entity = await dbSet.FindAsync(id);
                dbSet.Remove(entity);
                await db.SaveChangesAsync();
            }
        }

        public IQueryable<T> Query()
        {
            var db = new AppDbContext();
            return db.Set<T>().AsQueryable();
        }
    }
}