using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Bulky.DataAccess.Repository
{
	public class Repository<T> : IRepository<T> where T : class
	{
		private readonly ApplicationDbContext _context;
		internal DbSet<T> dbset;
        public Repository(ApplicationDbContext db)
        {
            _context = db;
			this.dbset=_context.Set<T>();
			db.Products.Include(u => u.Category).Include(u=>u.CategoryId);
        }
        public void Add(T entity)
		{
			dbset.Add(entity);

		}

		public T Get(Expression<Func<T, bool>> filter, string? includeProperties = null)
		{
			IQueryable<T> query = dbset;
			query=query.Where(filter);
			if (!string.IsNullOrEmpty(includeProperties))
			{
				foreach (var includeprop in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
				{
					query = query.Include(includeprop);
				}
			}
			return query.FirstOrDefault();
		}
	
		public IEnumerable<T> GetAll(string? includeProperties=null)
		{
			IQueryable<T> query = dbset;
			if(!string.IsNullOrEmpty(includeProperties))
			{
				foreach(var includeprop in includeProperties.Split(new char[] { ',' },StringSplitOptions.RemoveEmptyEntries)) 
				{
					query=query.Include(includeprop);
				}
			}
			return query.ToList();
		}

		public void Remove(T entity)
		{
			dbset.Remove(entity);
		}

		public void RemoveRange(IEnumerable<T> entity)
		{
			dbset.RemoveRange(entity);
		}
	}
}
