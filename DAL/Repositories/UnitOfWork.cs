using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Interfaces;

namespace DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        public DbContext Context { get; }

        public UnitOfWork(DbContext context)
        {
            Context = context;
        }

        public void Commit()
        {
            if (!ReferenceEquals(Context,null))
            {
                Context.SaveChanges();
            }
        }

        public void Dispose()
        {
            if (!ReferenceEquals(Context, null))
            {
                Context.Dispose();
            }
        }
    }
}
