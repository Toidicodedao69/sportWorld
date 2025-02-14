using sportWorld.DataAccess.Data;
using sportWorld.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sportWorld.DataAccess.Repository
{
	// Unit of work pattern: Group multiple transactions/actions in a single operation and then save change only once.
	// If any transaction fails, it will be easier to roll back.
	public class UnitOfWork : IUnitOfWork
	{
		private ApplicationDbContext _db;
		public ICategoryRepository Category { get; private set; }
        public IProductRepository Product { get; private set; }


        public UnitOfWork(ApplicationDbContext db)
		{
			_db = db;
			Category = new CategoryRepository(_db);
            Product = new ProductRepository(_db);
        }

        public void Save()
		{
			_db.SaveChanges();
		}
	}
}
