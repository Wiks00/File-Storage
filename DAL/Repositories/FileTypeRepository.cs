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
using DAL.Mappers;
using Logger;
using ORM;
using static DAL.Mappers.DalMapper;

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

        public IEnumerable<DalFileType> GetAll()
            => context.Set<FileTypes>().Select(item => item.ToDalFileType());

        public DalFileType GetById(long key)
        {
            if (key < 0)
            {
                var error =  new ArgumentOutOfRangeException(nameof(key), "parametr can't be negative");
                logger.Error(error,error.Message);
                throw error;
            }

            return context.Set<FileTypes>().FirstOrDefault(user => user.id == key).ToDalFileType();
        }

        public IEnumerable<DalFileType> GetByPredicate(Expression<Func<DalFileType, bool>> func)
        {
            if (ReferenceEquals(func, null))
            {
                var error = new ArgumentNullException(nameof(func), "parametr can't be null");
                logger.Error(error, error.Message);
                throw error;
            }

            return context.Set<FileTypes>().Where(Convert<DalFileType,FileTypes>(func)).Select(item => item.ToDalFileType());
        }

        public DalFileType Create(DalFileType entity)
        {
            if (ReferenceEquals(entity, null))
            {
                var error = new ArgumentNullException(nameof(entity), "parametr can't be null");
                logger.Error(error, error.Message);
                throw error;
            }

            var fileType = context.Set<FileTypes>().Add(entity.ToOrmFileType());

            return fileType.ToDalFileType();
        }

        public void Delete(DalFileType entity)
        {
            if (ReferenceEquals(entity, null))
            {
                var error = new ArgumentNullException(nameof(entity), "parametr can't be null");
                logger.Error(error, error.Message);
                throw error;
            }

            var possibleUser = context.Set<FileTypes>().Single(user => user.id == entity.ID);

            if (ReferenceEquals(possibleUser, null))
            {
                var error = new ArgumentNullException(nameof(possibleUser), "didn't find equally FileType In database");
                logger.Error(error, error.Message);
                throw error;
            }

            context.Set<FileTypes>().Remove(possibleUser);
        }

        public void Update(DalFileType entity)
        {
            if (ReferenceEquals(entity, null))
            {
                var error = new ArgumentNullException(nameof(entity), "parametr can't be null");
                logger.Error(error, error.Message);
                throw error;
            }

            var fileType = context.Set<FileTypes>().Find(entity.ID);

            fileType.format = entity.Format;
            fileType.typeName = entity.TypeName;

            context.Entry(fileType).State = EntityState.Modified;
        }

        public void Update<TKey>(Expression<Func<DalFileType, bool>> func, Expression<Func<DalFileType, TKey>> keyValue, TKey e)
        {
            if (ReferenceEquals(func, null) || ReferenceEquals(keyValue, null) || ReferenceEquals(e, null))
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

            Type t = typeof(FileTypes);

            foreach (var entity in context.Set<FileTypes>().Where(Convert<DalFileType,FileTypes>(func)))
            {
                MethodInfo method = t.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public).FirstOrDefault(mtd => mtd.Name.ToLower().Equals("set_" + propName));
                method?.Invoke(entity, new object[] { e });

                context.Entry(entity).State = EntityState.Modified;
            }         
        }
    }
}
