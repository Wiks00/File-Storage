using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BLL.DTO;


namespace BLL.Interfaces
{
    public interface IUserService 
    {
        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns>Users enumeration</returns>
        IEnumerable<DtoUser> GetAllUsers();

        /// <summary>
        /// Get user by id
        /// </summary>
        /// <param name="key">id of the user</param>
        /// <returns>User</returns>
        DtoUser GetUserById(long key);

        /// <summary>
        /// Get user by delegate
        /// </summary>
        /// <param name="func">search delegate</param>
        /// <returns>User enumeration</returns>
        IEnumerable<DtoUser> GetUserByPredicate(Expression<Func<DtoUser, bool>> func);

        /// <summary>
        /// Create new User
        /// </summary>
        /// <param name="e">inserting User</param>
        /// <returns>new User</returns>
        DtoUser CreateUser(DtoUser e);

        /// <summary>
        /// Delete User
        /// </summary>
        /// <param name="e">deleting User</param>
        void DeleteUser(DtoUser e);

        /// <summary>
        /// Update User
        /// </summary>
        /// <param name="e">updating User</param>
        void UpdateUser(DtoUser e);

        /// <summary>
        /// Update User password
        /// </summary>
        /// <param name="e">updating User</param>
        void ChangePassword(DtoUser e);

        /// <summary>
        /// Update User login
        /// </summary>
        /// <param name="newLogin">new User login</param>
        /// <param name="id">updating User id</param>
        void ChangeLogin(string newLogin, long id);

        /// <summary>
        /// Update User email
        /// </summary>
        /// <param name="newEmail">new User email</param>
        /// <param name="id">updating User id</param>
        void ChangeEmail(string newEmail, long id);

        /// <summary>
        /// Check if email is free
        /// </summary>
        /// <param name="email">ckecked email</param>
        bool IsFreeEmail(string email);

        /// <summary>
        /// Check if login is free
        /// </summary>
        /// <param name="login">ckecked login</param>
        bool IsFreeLogin(string login);
    }
}
