using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sportWorld.DataAccess.Data;
using sportWorld.DataAccess.Repository.IRepository;
using sportWorld.Models;


namespace sportWorld.DataAccess.Repository
{
	public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
	{
		private ApplicationDbContext _db;
        public ShoppingCartRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

		public void Update(ShoppingCart shoppingcart)
		{
			_db.ShoppingCarts.Update(shoppingcart);
		}
	}
}
