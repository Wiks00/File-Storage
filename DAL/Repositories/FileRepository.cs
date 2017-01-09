using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DAL.Interfaces;
using Logger;
using ORM;

namespace DAL.Repositories
{
    public class FileRepository : IFileRepository
    {
        private readonly DbContext context;
        private readonly ILogAdapter logger;

        public FileRepository(DbContext context, ILogAdapter logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public IEnumerable<Files> GetAll()
        {
            return context.Set<Files>();
        }

        public Files GetById(long key)
        {
            if (key < 0)
                throw new ArgumentOutOfRangeException(nameof(key), "parametr can't be negative");

            return context.Set<Files>().FirstOrDefault(user => user.id == key);
        }

        public IEnumerable<Files> GetByPredicate(Func<Files, bool> func)
        {
            if (ReferenceEquals(func, null))
                throw new ArgumentNullException(nameof(func), "parametr can't be null");

            return context.Set<Files>().Where(func);
        }

        public void Create(Files e)
        {
            if (ReferenceEquals(e, null))
                throw new ArgumentNullException(nameof(e), "parametr can't be null");

            context.Set<Files>().Add(e);
        }

        public void Delete(Files e)
        {
            if (ReferenceEquals(e, null))
                throw new ArgumentNullException(nameof(e), "parametr can't be null");

            var possibleUser = context.Set<Files>().Single(user => user.id == e.id);

            if (ReferenceEquals(possibleUser, null))
                throw new ArgumentNullException(nameof(possibleUser), "didn't find equally File In database");

            context.Set<Files>().Remove(possibleUser);
        }

        public void Update(Files e)
        {
            if (ReferenceEquals(e, null))
                throw new ArgumentNullException(nameof(e), "parametr can't be null");

            var entity = context.Set<Files>().Find(e.id);

            entity.Folder = e.Folder;
            entity.dateTime = e.dateTime;
            entity.folderId = e.folderId;
            entity.name = e.name;

            entity.content = e.content;
            entity.FileTypes = e.FileTypes;

            context.Entry(entity).State = EntityState.Modified;
        }

        public void Update<TKey>(Func<Files, bool> func, Expression<Func<Files, TKey>> keyValue, TKey e)
        {
            if (ReferenceEquals(func, null) || ReferenceEquals(func, null) || e.Equals(default(TKey)))
                throw new ArgumentException("incorrect parametr(s) value");

            MemberExpression memberExpression = (MemberExpression)keyValue.Body;
            string propName = memberExpression.Member.Name;

            Type t = typeof(Files);

            foreach (var entity in context.Set<Files>().Where(func))
            {
                MethodInfo method = t.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public).FirstOrDefault(mtd => mtd.Name.ToLower().Equals("set_" + propName));
                method?.Invoke(entity, new object[] { e });

                context.Entry(entity).State = EntityState.Modified;
            }  
        }
    }
}
