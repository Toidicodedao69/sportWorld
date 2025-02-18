using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace sportWorld.DataAccess.Repository.IRepository
{
	public interface IRepository <T> where T : class
	{
		// Generic methods to access db
		IEnumerable<T> GetAll(string? includeProperties = null);
		T Get(Expression<Func<T, bool>> filter, string? includeProperties = null); // General syntax for FirstorDefault()
		void Add(T entity);
		// void Update(T entity);  No update method because updating logic may be different between models (i.e Category vs Product)
		void Remove(T entity);
		void RemoveRange(IEnumerable<T> entities);

	}
}
