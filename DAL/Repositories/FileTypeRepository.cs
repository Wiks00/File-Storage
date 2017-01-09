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
    public class FileTypeRepository : IFileTypeRepository
    {
        private readonly DbContext context;
        private readonly ILogAdapter logger;

        public FileTypeRepository(DbContext context, ILogAdapter logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public IEnumerable<FileTypes> GetAll()
        {
            return context.Set<FileTypes>();
        }

        public FileTypes GetById(long key)
        {
            if (key < 0)
                throw new ArgumentOutOfRangeException(nameof(key), "parametr can't be negative");

            return context.Set<FileTypes>().FirstOrDefault(user => user.id == key);
        }

        public IEnumerable<FileTypes> GetByPredicate(Func<FileTypes, bool> func)
        {
            if (ReferenceEquals(func, null))
                throw new ArgumentNullException(nameof(func), "parametr can't be null");

            return context.Set<FileTypes>().Where(func);
        }

        public void Create(FileTypes e)
        {
            if (ReferenceEquals(e, null))
                throw new ArgumentNullException(nameof(e), "parametr can't be null");

            context.Set<FileTypes>().Add(e);
        }

        public void Delete(FileTypes e)
        {
            if (ReferenceEquals(e, null))
                throw new ArgumentNullException(nameof(e), "parametr can't be null");

            var possibleUser = context.Set<FileTypes>().Single(user => user.id == e.id);

            if (ReferenceEquals(possibleUser, null))
                throw new ArgumentNullException(nameof(possibleUser), "didn't find equally FileType In database");

            context.Set<FileTypes>().Remove(possibleUser);
        }

        public void Update(FileTypes e)
        {
            if (ReferenceEquals(e, null))
                throw new ArgumentNullException(nameof(e), "parametr can't be null");

            var entity = context.Set<FileTypes>().Find(e.id);

            entity.format = e.format;
            entity.typeName = e.typeName;

            entity.Files = e.Files;

            context.Entry(entity).State = EntityState.Modified;
        }

        public void Update<TKey>(Func<FileTypes, bool> func, Expression<Func<FileTypes, TKey>> keyValue, TKey e)
        {
            if (ReferenceEquals(func, null) || ReferenceEquals(func, null) || e.Equals(default(TKey)))
                throw new ArgumentException("incorrect parametr(s) value");

            MemberExpression memberExpression = (MemberExpression)keyValue.Body;
            string propName = memberExpression.Member.Name;

            Type t = typeof(FileTypes);

            foreach (var entity in context.Set<FileTypes>().Where(func))
            {
                MethodInfo method = t.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public).FirstOrDefault(mtd => mtd.Name.ToLower().Equals("set_" + propName));
                method?.Invoke(entity, new object[] { e });

                context.Entry(entity).State = EntityState.Modified;
            }         
        }
    }
}
