using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DAL.DTO;
using DAL.Interfaces;
using Logger;
using ORM;

namespace DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DbContext context;
        private readonly ILogAdapter logger;

        public UserRepository(DbContext context, ILogAdapter logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public IEnumerable<Users> GetAll()
        {
            return context.Set<Users>();
        }

        public Users GetById(long key)
        {
            if (key < 0)
                throw new ArgumentOutOfRangeException(nameof(key), "parametr can't be negative");

            return context.Set<Users>().FirstOrDefault(user => user.id == key);        
        }

        public IEnumerable<Users> GetByPredicate(Func<Users, bool> func)
        {
            if (ReferenceEquals(func, null))
                throw new ArgumentNullException(nameof(func), "parametr can't be null");

            return context.Set<Users>().Where(func);        
        }

        public void Create(Users e)
        {
            if (ReferenceEquals(e, null))
                throw new ArgumentNullException(nameof(e), "parametr can't be null");

            context.Set<Users>().Add(e);              
        }

        public void Delete(Users e)
        {
            if (ReferenceEquals(e, null))
                throw new ArgumentNullException(nameof(e),"parametr can't be null");

            var possibleUser = context.Set<Users>().Single(user => user.id == e.id);

            if (ReferenceEquals(possibleUser, null))
                throw new ArgumentNullException(nameof(possibleUser), "didn't find equally User In database");

            context.Set<Users>().Remove(possibleUser); 
        }

        public void Update(Users e)
        {
            if (ReferenceEquals(e, null))
                throw new ArgumentNullException(nameof(e), "parametr can't be null");

            var entity = context.Set<Users>().Find(e.id);

            entity.email = e.email;
            entity.login = e.login;
            entity.password = e.password;

            entity.Roles = e.Roles;
            entity.Folders = e.Folders;
            entity.FoldersShared = e.FoldersShared;

            context.Entry(entity).State = EntityState.Modified;
        }

        public void Update<TKey>(Func<Users, bool> func, Expression<Func<Users, TKey>> keyValue, TKey e)
        {
            if (ReferenceEquals(func, null) || ReferenceEquals(func, null) || e.Equals(default(TKey)))
                throw new ArgumentException("incorrect parametr(s) value");

            MemberExpression memberExpression = (MemberExpression)keyValue.Body;
            string propName = memberExpression.Member.Name;

            Type t = typeof(Users);

            foreach (var entity in context.Set<Users>().Where(func))
            {
                MethodInfo method = t.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public).FirstOrDefault(mtd => mtd.Name.ToLower().Equals("set_" + propName));
                method?.Invoke(entity, new object[] { e });

                context.Entry(entity).State = EntityState.Modified;
            }
        }
    }
}
