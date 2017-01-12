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
using static DAL.Mappers.DalMapper;

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

        public IEnumerable<DalRole> GetAll()
        {
            return context.Set<Roles>().Select(item => item.ToDalRole());
        }

        public DalRole GetById(long key)
        {
            if (key < 0)
            {
                var error = new ArgumentOutOfRangeException(nameof(key), "parametr can't be negative");
                logger.Error(error, error.Message);
                throw error;
            }

            return context.Set<Roles>().FirstOrDefault(user => user.id == key).ToDalRole();
        }

        public IEnumerable<DalRole> GetByPredicate(Expression<Func<DalRole, bool>> func)
        {
            if (ReferenceEquals(func, null))
            {
                var error = new ArgumentNullException(nameof(func), "parametr can't be null");
                logger.Error(error, error.Message);
                throw error;
            }

            return context.Set<Roles>().Where(Convert<DalRole,Roles>(func)).Select(item => item.ToDalRole());
        }

        public void Create(DalRole entity)
        {
            if (ReferenceEquals(entity, null))
                throw new ArgumentNullException(nameof(entity), "parametr can't be null");

            context.Set<Roles>().Add(entity.ToOrmRole());
        }

        public void Delete(DalRole entity)
        {
            if (ReferenceEquals(entity, null))
            {
                var error = new ArgumentNullException(nameof(entity), "parametr can't be null");
                logger.Error(error, error.Message);
                throw error;
            }

            var possibleUser = context.Set<Roles>().Single(user => user.id == entity.ID);

            if (ReferenceEquals(possibleUser, null))
            {
                var error = new ArgumentNullException(nameof(possibleUser), "didn't find equally Role In database");
                logger.Error(error, error.Message);
                throw error;
            }

            context.Set<Roles>().Remove(possibleUser);
        }

        public void Update(DalRole entity)
        {
            if (ReferenceEquals(entity, null))
            {
                var error = new ArgumentNullException(nameof(entity), "parametr can't be null");
                logger.Error(error, error.Message);
                throw error;
             }

            var role = context.Set<Roles>().Find(entity.ID);

            role.role = entity.Role;

            context.Entry(role).State = EntityState.Modified;
        }

        public void Update<TKey>(Expression<Func<DalRole, bool>> func, Expression<Func<DalRole, TKey>> keyValue, TKey e)
        {
            if (ReferenceEquals(func, null) || ReferenceEquals(keyValue, null) || e.Equals(default(TKey)))
            {
                var error = new ArgumentException("incorrect parametr(s) value");
                logger.Error(error, error.Message);
                throw error;
            }

            MemberExpression memberExpression = (MemberExpression)keyValue.Body;
            string propName;

            if (memberExpression.Expression.GetType().ToString().Equals("System.Linq.Expressions.PropertyExpression"))
            {
                var param = (MemberExpression)memberExpression.Expression;

                propName = GetEqualProperty(typeof(DalFile).ToString(), param.Member.Name + "." + memberExpression.Member.Name);
            }
            else
            {
                propName = GetEqualProperty(typeof(DalFile).ToString(), memberExpression.Member.Name);
            }

            Type t = typeof(Roles);

            foreach (var entity in context.Set<Roles>().Where(Convert<DalRole,Roles>(func)))
            {
                MethodInfo method = t.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public).FirstOrDefault(mtd => mtd.Name.ToLower().Equals("set_" + propName));
                method?.Invoke(entity, new object[] { e });

                context.Entry(entity).State = EntityState.Modified;

            }
        }
    }
}
