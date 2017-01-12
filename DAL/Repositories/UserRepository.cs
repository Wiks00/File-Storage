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
    public class UserRepository : IUserRepository
    {
        private readonly DbContext context;
        private readonly ILogAdapter logger;

        public UserRepository(DbContext context, ILogAdapter logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public IEnumerable<DalUser> GetAll()
        {
            return context.Set<Users>().Select(item => item.ToDalUser());
        }

        public DalUser GetById(long key)
        {
            if (key < 0)
            {
                var error = new ArgumentOutOfRangeException(nameof(key), "parametr can't be negative");
                logger.Error(error, error.Message);
                throw error;
            }

            return context.Set<Users>().FirstOrDefault(user => user.id == key).ToDalUser();        
        }

        public IEnumerable<DalUser> GetByPredicate(Expression<Func<DalUser, bool>> func)
        {
            if (ReferenceEquals(func, null))
            {
                var error = new ArgumentNullException(nameof(func), "parametr can't be null");
                logger.Error(error, error.Message);
                throw error;
            }

            return context.Set<Users>().Where(Convert<DalUser,Users>(func)).Select(item => item.ToDalUser());        
        }

        public void Create(DalUser entity)
        {
            if (ReferenceEquals(entity, null))
            {
                var error = new ArgumentNullException(nameof(entity), "parametr can't be null");
                logger.Error(error, error.Message);
                throw error;
            }

            context.Set<Users>().Add(entity.ToOrmUser());              
        }

        public void Delete(DalUser entity)
        {
            if (ReferenceEquals(entity, null))
            {
                var error = new ArgumentNullException(nameof(entity),"parametr can't be null");
                logger.Error(error, error.Message);
                throw error;
            }

            var possibleUser = context.Set<Users>().Single(user => user.id == entity.ID);

            if (ReferenceEquals(possibleUser, null))
            {
                var error = new ArgumentNullException(nameof(possibleUser), "didn't find equally User In database");
                logger.Error(error, error.Message);
                throw error;
            }

            context.Set<Users>().Remove(possibleUser); 
        }

        public void Update(DalUser entity)
        {
            if (ReferenceEquals(entity, null))
            {
                var error = new ArgumentNullException(nameof(entity), "parametr can't be null");
                logger.Error(error, error.Message);
                throw error;
            }

            var user = context.Set<Users>().Find(entity.ID);
            var e = entity.ToOrmUser();

            user.email = e.email;
            user.login = e.login;
            user.password = e.password;

            user.Roles = e.Roles;
            user.Folders = e.Folders;
            user.FoldersShared = e.FoldersShared;

            context.Entry(entity).State = EntityState.Modified;
        }

        public void Update<TKey>(Expression<Func<DalUser, bool>> func, Expression<Func<DalUser, TKey>> keyValue, TKey e)
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

            Type t = typeof(Users);

            foreach (var entity in context.Set<Users>().Where(Convert<DalUser,Users>(func)))
            {
                MethodInfo method = t.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public).FirstOrDefault(mtd => mtd.Name.ToLower().Equals("set_" + propName));
                method?.Invoke(entity, new object[] { e });

                context.Entry(entity).State = EntityState.Modified;
            }
        }
    }
}
