using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DAL.DTO;

namespace DAL.Interfaces
{
    public interface IRepository<TEntity> where TEntity : IEntity
    {
        /// <summary>
        /// Get all the table objects from database
        /// </summary>
        /// <returns>TEntity objects</returns>
        IEnumerable<TEntity> GetAll();

        /// <summary>
        /// Get the table object from database by id
        /// </summary>
        /// <param name="key">id of the searching object</param>
        /// <returns>TEntity object</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        TEntity GetById(long key);

        /// <summary>
        /// Get the table objects from database by delegate
        /// </summary>
        /// <param name="func">search delegate</param>
        /// <returns>TEntity object</returns>
        /// <exception cref="ArgumentNullException"></exception>
        IEnumerable<TEntity> GetByPredicate(Expression<Func<TEntity, bool>> func);

        /// <summary>
        /// Insert the table object into database
        /// </summary>
        /// <param name="entity">inserting object</param>
        /// <returns>new TEntity object</returns>
        /// <exception cref="ArgumentNullException"></exception>
        TEntity Create(TEntity entity);

        /// <summary>
        /// Delete the table object from database
        /// </summary>
        /// <param name="entity">deleting object</param>
        /// <exception cref="ArgumentNullException"></exception>
        void Delete(TEntity entity);

        /// <summary>
        /// Update the table object in database
        /// </summary>
        /// <param name="entity">updating object</param>
        /// <exception cref="ArgumentNullException"></exception>
        void Update(TEntity entity);

        /// <summary>
        /// Update field of the table objects from database by delegate
        /// </summary>
        /// <typeparam name="TKey">property type</typeparam>
        /// <param name="func">search delegate</param>
        /// <param name="keyValue">filed</param>
        /// <param name="e">new property value</param>
        /// <exception cref="ArgumentException"></exception>
        void Update<TKey>(Expression<Func<TEntity, bool>> func, Expression<Func<TEntity, TKey>> keyValue, TKey e);
    }
}
