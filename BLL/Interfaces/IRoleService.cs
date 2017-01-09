using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BLL.DTO;

namespace BLL.Interfaces
{
    public interface IRoleService
    {
        /// <summary>
        /// Get all Roles from database
        /// </summary>
        /// <returns>Roles enumeration</returns>
        IEnumerable<DtoRole> GetAllRoles();

        /// <summary>
        /// Get Role by title
        /// </summary>
        /// <param name="func">title of the search Roles</param>
        /// <returns>Roles enumeration</returns>
        DtoRole GetRolesByTitle(string title);

        /// <summary>
        /// Create new Role
        /// </summary>
        /// <param name="e">inserting Role</param>
        void CreateRole(DtoRole e);

        /// <summary>
        /// Delete Role
        /// </summary>
        /// <param name="e">deleting Role</param>
        void DeleteRole(DtoRole e);

        /// <summary>
        /// Update Role
        /// </summary>
        /// <param name="e">updating Role</param>
        void UpdateRole(DtoRole e);
    }
}
