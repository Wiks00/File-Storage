using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DAL.DTO;

namespace DAL.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
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
        TEntity GetById(long key);

        /// <summary>
        /// Get the table objects from database by delegate
        /// </summary>
        /// <param name="func">search delegate</param>
        /// <returns>TEntity object</returns>
        IEnumerable<TEntity> GetByPredicate(Func<TEntity, bool> func);

        /// <summary>
        /// Insert the table object into database
        /// </summary>
        /// <param name="e">inserting object</param>
        void Create(TEntity e);

        /// <summary>
        /// Delete the table object from database
        /// </summary>
        /// <param name="e">deleting object</param>
        void Delete(TEntity e);

        /// <summary>
        /// Update the table object in database
        /// </summary>
        /// <param name="e">updating object</param>
        void Update(TEntity e);

        /// <summary>
        /// Update field of the table objects from database by delegate
        /// </summary>
        /// <typeparam name="TKey">property type</typeparam>
        /// <param name="func">search delegate</param>
        /// <param name="keyValue">filed</param>
        /// <param name="e">new property value</param>
        void Update<TKey>(Func<TEntity, bool> func, Expression<Func<TEntity, TKey>> keyValue, TKey e);
    }
}
