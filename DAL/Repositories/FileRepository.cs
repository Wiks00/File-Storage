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
    public class FileRepository : IFileRepository
    {
        private readonly DbContext context;
        private readonly ILogAdapter logger;

        public FileRepository(DbContext context, ILogAdapter logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public IEnumerable<DalFile> GetAll()
            => context.Set<Files>().Select(item => item.ToDalFile());

        public DalFile GetById(long key)
        {
            if (key < 0)
            {
                var error = new ArgumentOutOfRangeException(nameof(key), "parametr can't be negative");
                logger.Error(error, error.Message);
                throw error;               
            }

            return context.Set<Files>().FirstOrDefault(user => user.id == key).ToDalFile();
        }

        public IEnumerable<DalFile> GetByPredicate(Expression<Func<DalFile, bool>> func)
        {
            if (ReferenceEquals(func, null))
            {
                var error = new ArgumentNullException(nameof(func), "parametr can't be null");
                logger.Error(error, error.Message);
                throw error;
            }

            return context.Set<Files>().Where(Convert<DalFile, Files>(func)).Select(item => item.ToDalFile());
        }

        public DalFile Create(DalFile entity)
        {
            if (ReferenceEquals(entity, null))
            {
                var error = new ArgumentNullException(nameof(entity), "parametr can't be null");
                logger.Error(error, error.Message);
                throw error;
            }
            var file = context.Set<Files>().Add(entity.ToOrmFile());

            return file.ToDalFile();
        }

        public void Delete(DalFile entity)
        {
            if (ReferenceEquals(entity, null))
            {
                var error = new ArgumentNullException(nameof(entity), "parametr can't be null");
                logger.Error(error, error.Message);
                throw error;
            }

            var possibleUser = context.Set<Files>().Single(user => user.id == entity.ID);

            if (ReferenceEquals(possibleUser, null))
            {
                var error = new ArgumentNullException(nameof(possibleUser), "didn't find equally File In database");
                logger.Error(error, error.Message);
                throw error;
            }

            context.Set<Files>().Remove(possibleUser);
        }

        public void Update(DalFile entity)
        {
            if (ReferenceEquals(entity, null))
            {
                var error = new ArgumentNullException(nameof(entity), "parametr can't be null");
                logger.Error(error, error.Message);
                throw error;
            }

            var file = context.Set<Files>().Find(entity.ID);
            var e = entity.ToOrmFile();

            file.Folder = e.Folder;
            file.dateTime = e.dateTime;
            file.folderId = e.folderId;
            file.name = e.name;

            file.content = e.content;
            file.FileTypes = e.FileTypes;

            context.Entry(file).State = EntityState.Modified;
        }

        public void Update<TKey>(Expression<Func<DalFile, bool>> func, Expression<Func<DalFile, TKey>> keyValue, TKey e)
        {
            if (ReferenceEquals(func, null) || ReferenceEquals(keyValue, null) || ReferenceEquals(e,null))
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

            Type t = typeof(Files);

            foreach (var entity in context.Set<Files>().Where(Convert<DalFile,Files>(func)))
            {
                MethodInfo method = t.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public).FirstOrDefault(mtd => mtd.Name.ToLower().Equals("set_" + propName));
                method?.Invoke(entity, new object[] { e });

                context.Entry(entity).State = EntityState.Modified;
            }  
        }
    }
}
