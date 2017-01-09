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
    public class RoleRepository : IRoleRepository
    {
        private readonly DbContext context;
        private readonly ILogAdapter logger;

        public RoleRepository(DbContext context, ILogAdapter logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public IEnumerable<Roles> GetAll()
        {
            return context.Set<Roles>();
        }

        public Roles GetById(long key)
        {
            if (key < 0)
                throw new ArgumentOutOfRangeException(nameof(key), "parametr can't be negative");

            return context.Set<Roles>().FirstOrDefault(user => user.id == key);
        }

        public IEnumerable<Roles> GetByPredicate(Func<Roles, bool> func)
        {
            if (ReferenceEquals(func, null))
                throw new ArgumentNullException(nameof(func), "parametr can't be null");

            return context.Set<Roles>().Where(func);
        }

        public void Create(Roles e)
        {
            if (ReferenceEquals(e, null))
                throw new ArgumentNullException(nameof(e), "parametr can't be null");

            context.Set<Roles>().Add(e);
        }

        public void Delete(Roles e)
        {
            if (ReferenceEquals(e, null))
                throw new ArgumentNullException(nameof(e), "parametr can't be null");

            var possibleUser = context.Set<Roles>().Single(user => user.id == e.id);

            if (ReferenceEquals(possibleUser, null))
                throw new ArgumentNullException(nameof(possibleUser), "didn't find equally Role In database");

            context.Set<Roles>().Remove(possibleUser);
        }

        public void Update(Roles e)
        {
            if (ReferenceEquals(e, null))
                throw new ArgumentNullException(nameof(e), "parametr can't be null");

            var entity = context.Set<Roles>().Find(e.id);

            entity.role = e.role;

            entity.Users = e.Users;

            context.Entry(entity).State = EntityState.Modified;
        }

        public void Update<TKey>(Func<Roles, bool> func, Expression<Func<Roles, TKey>> keyValue, TKey e)
        {
            if (ReferenceEquals(func, null) || ReferenceEquals(func, null) || e.Equals(default(TKey)))
                throw new ArgumentException("incorrect parametr(s) value");

            MemberExpression memberExpression = (MemberExpression)keyValue.Body;
            string propName = memberExpression.Member.Name;

            Type t = typeof(Roles);

            foreach (var entity in context.Set<Roles>().Where(func))
            {
                MethodInfo method = t.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public).FirstOrDefault(mtd => mtd.Name.ToLower().Equals("set_" + propName));
                method?.Invoke(entity, new object[] { e });

                context.Entry(entity).State = EntityState.Modified;

            }
        }
    }
}
