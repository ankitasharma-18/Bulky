using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
	public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
		private readonly ApplicationDbContext _context;
		
		public CompanyRepository(ApplicationDbContext db):base(db) {
			_context = db;
		}
		
		

		public void Update(Company obj)
		{
			_context.Companies.Update(obj);
		}
	}
}
