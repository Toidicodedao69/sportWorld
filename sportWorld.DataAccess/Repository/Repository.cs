using Microsoft.EntityFrameworkCore;
using sportWorld.DataAccess.Data;
using sportWorld.DataAccess.Repository.IRepository;
using System.Linq.Expressions;

namespace sportWorld.DataAccess.Repository
{
	public class Repository<T> : IRepository<T> where T : class
	{
		private readonly ApplicationDbContext _db;
		internal DbSet<T> dbSet;
		public Repository(ApplicationDbContext db)
		{
			_db = db;
			this.dbSet = _db.Set<T>(); // Access the specific model defined by T
		}
		public void Add(T entity)
		{
			dbSet.Add(entity);
		}

		public T Get(Expression<Func<T, bool>> filter, string? includeProperties = null)
		{
			IQueryable<T> query = dbSet;
			query = query.Where(filter);

			if (!string.IsNullOrEmpty(includeProperties))
			{
				foreach (var prop in includeProperties
					.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
				{
					query = query.Include(prop);
				}
			}

			return query.FirstOrDefault();
		}

		// includeProperties is the Navigation properties with format 'Property 1, Property 2, ...'
		public IEnumerable<T> GetAll(string? includeProperties = null)
		{
			IQueryable<T> query = dbSet;

			if (!string.IsNullOrEmpty(includeProperties))
			{
				foreach (var prop in includeProperties
					.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries ))
				{
					query = query.Include(prop);
				}
			}

			return query.ToList();
		}

		public void Remove(T entity)
		{
			dbSet.Remove(entity);
		}

		public void RemoveRange(IEnumerable<T> entities)
		{
			dbSet.RemoveRange(entities);
		}
	}
}
