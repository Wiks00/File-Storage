using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using BLL.DTO;
using BLL.Interfaces;
using Ninject;
using WebUI.Infrastructure;

namespace WebUI.Providers
{
    public class CustomRoleProvider : RoleProvider
    {

        private readonly NinjectDependencyResolver dependencyResolver = new NinjectDependencyResolver(new StandardKernel());
        private IUserService userService
            => (IUserService)dependencyResolver.GetService(typeof(IUserService));

        private IRoleService roleService
            => (IRoleService)dependencyResolver.GetService(typeof(IRoleService));

        public override bool IsUserInRole(string email, string roleName)
        {
            DtoRole userRole = roleService.GetRoleByTitle(roleName);

            DtoUser user = userService.GetUserByPredicate(usr => usr.Email.Equals(email,StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

            if (ReferenceEquals(user,null)) return false;        

            if (!ReferenceEquals(userRole,null) && user.Roles.Contains(userRole, new RoleCompare()))
            {
                return true;
            }

            return false;
        }

        public override string[] GetRolesForUser(string email)
        {
            var user = userService.GetUserByPredicate(u => u.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

            if (ReferenceEquals(user, null)) return null;

            string[] roles = new string[user.Roles.Count];

            for (int i = 0; i < user.Roles.Count ; i++)
            {
                roles[i] = user.Roles.ElementAt(i).Role;
            }

            return roles;
        }

        #region not implemented members
        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName { get; set; }
        #endregion
    }

    internal class RoleCompare : IEqualityComparer<DtoRole>
    {
        public bool Equals(DtoRole x, DtoRole y)
        {
            return x.Role.Equals(y.Role,StringComparison.InvariantCultureIgnoreCase);
        }
        public int GetHashCode(DtoRole role)
        {
            return role.Role.GetHashCode() * role.ID.GetHashCode();
        }
    }
}