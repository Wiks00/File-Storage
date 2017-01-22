using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BLL.DTO;
using DAL.DTO;

namespace BLL.Mappers
{
    public static class BllMapper
    {
        #region To DTO Entity

        /// <summary>
        /// Convert Dal File entity to Bll entity
        /// </summary>
        /// <param name="file">dal entity</param>
        /// <returns>bll entity</returns>
        public static DtoFile ToDtoFile(this DalFile file)
        {
            if (ReferenceEquals(file, null))
                return null;

            return new DtoFile
            {
                ID = file.ID,
                Data = file.Data,
                DateTime = file.DateTime,
                Folder = file.Folder.ToDtoFolder(),
                Title = file.Title,
                FileTypes = new HashSet<DtoFileType>(file.FileTypes.Select(item => item.ToDtoFileType()))
            };
        }

        /// <summary>
        /// Convert Dal FileType entity to Bll entity
        /// </summary>
        /// <param name="fileType">dal entity</param>
        /// <returns>bll entity</returns>
        public static DtoFileType ToDtoFileType(this DalFileType fileType)
        {
            if (ReferenceEquals(fileType, null))
                return null;

            return new DtoFileType
            {
                ID = fileType.ID,
                TypeName = fileType.TypeName,
                Format = fileType.Format
            };
        }

        /// <summary>
        /// Convert Dal User entity to Bll entity
        /// </summary>
        /// <param name="user">dal entity</param>
        /// <returns>bll entity</returns>
        public static DtoUser ToDtoUser(this DalUser user)
        {
            if (ReferenceEquals(user, null))
                return null;

            return new DtoUser
            {
                ID = user.ID,
                Email = user.Email,
                Login = user.Login,
                Password = user.Password,
                Folders = new HashSet<DtoFolder>(user.Folders.Select(item => item.ToDtoFolder())),
                Roles = new HashSet<DtoRole>(user.Roles.Select(item => item.ToDtoRole())),
                SharedFolders = new HashSet<DtoFolder>(user.SharedFolders.Select(item => item.ToDtoFolder()))
            };
        }

        /// <summary>
        /// Convert Dal Folder entity to Bll entity
        /// </summary>
        /// <param name="folder">dal entity</param>
        /// <returns>bll entity</returns>
        public static DtoFolder ToDtoFolder(this DalFolder folder)
        {
            if (ReferenceEquals(folder, null))
                return null;

            return new DtoFolder
            {
                ID = folder.ID,
                LeftKey = folder.LeftKey,
                RightKey = folder.RightKey,
                Level = folder.Level,
                DateTime = folder.DateTime,
                Title = folder.Title,
                OwnerID = folder.OwnerID,
                Files = new HashSet<DtoFile>(folder.Files.Select(item => item.ToDtoFile())),
                SharedToUsers = new HashSet<long>(folder.SharedToUsers)
            };
        }

        /// <summary>
        /// Convert Dal Role entity to Bll entity
        /// </summary>
        /// <param name="role">dal entity</param>
        /// <returns>bll entity</returns>
        public static DtoRole ToDtoRole(this DalRole role)
        {
            if (ReferenceEquals(role, null))
                return null;

            return new DtoRole
            {
                ID = role.ID,
                Role = role.Role
            };
        }
        #endregion

        #region To DAL Entity

        /// <summary>
        /// Convert Bll File entity to Dal entity
        /// </summary>
        /// <param name="file">bll entity</param>
        /// <returns>dal entity</returns>
        public static DalFile ToDalFile(this DtoFile file)
        {
            if (ReferenceEquals(file, null))
                return null;

            return new DalFile
            {
                ID = file.ID,
                Data = file.Data,
                DateTime = file.DateTime,
                Title = file.Title,
                Folder = file.Folder.ToDalFolder(),
                FileTypes = new HashSet<DalFileType>(file.FileTypes.Select(item => item.ToDalFileType()))
            };
        }

        /// <summary>
        /// Convert Bll FileType entity to Dal entity
        /// </summary>
        /// <param name="fileType">bll entity</param>
        /// <returns>dal entity</returns>
        public static DalFileType ToDalFileType(this DtoFileType fileType)
        {
            if (ReferenceEquals(fileType, null))
                return null;

            return new DalFileType
            {
                ID = fileType.ID,
                TypeName = fileType.TypeName,
                Format = fileType.Format
            };
        }

        /// <summary>
        /// Convert Bll User entity to Dal entity
        /// </summary>
        /// <param name="user">bll entity</param>
        /// <returns>dal entity</returns>
        public static DalUser ToDalUser(this DtoUser user)
        {
            if (ReferenceEquals(user, null))
                return null;

            return new DalUser
            {
                ID = user.ID,
                Email = user.Email,
                Login = user.Login,
                Password = user.Password,
                Folders = new HashSet<DalFolder>(user.Folders.Select(item => item.ToDalFolder())),
                Roles = new HashSet<DalRole>(user.Roles.Select(item => item.ToDalRole())),
                SharedFolders = new HashSet<DalFolder>(user.SharedFolders.Select(item => item.ToDalFolder()))
            };
        }

        /// <summary>
        /// Convert Bll Folder entity to Dal entity
        /// </summary>
        /// <param name="folder">bll entity</param>
        /// <returns>dal entity</returns>
        public static DalFolder ToDalFolder(this DtoFolder folder)
        {
            if (ReferenceEquals(folder, null))
                return null;

            return new DalFolder
            {
                ID = folder.ID,
                LeftKey = folder.LeftKey,
                RightKey = folder.RightKey,
                Level = folder.Level,
                DateTime = folder.DateTime,
                Title = folder.Title,
                OwnerID = folder.OwnerID,
                Files = new HashSet<DalFile>(folder.Files.Select(item => item.ToDalFile())),
                SharedToUsers = new HashSet<long>(folder.SharedToUsers)
            };
        }

        /// <summary>
        /// Convert Bll Role entity to Dal entity
        /// </summary>
        /// <param name="role">bll entity</param>
        /// <returns>dal entity</returns>
        public static DalRole ToDalRole(this DtoRole role)
        {
            if (ReferenceEquals(role, null))
                return null;

            return new DalRole
            {
                ID = role.ID,
                Role = role.Role
            };
        }
        #endregion

        /// <summary>
        /// Convert function with TSource type entity to function with TTarget type entity
        /// </summary>
        /// <typeparam name="TSource">old type</typeparam>
        /// <typeparam name="TTarget">new type</typeparam>
        /// <param name="root">convertible function</param>
        /// <returns>converted function with new entity type</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Expression<Func<TTarget, bool>> Convert<TSource, TTarget>(Expression<Func<TSource, bool>> root)
        {
            if (ReferenceEquals(root, null))
                throw new ArgumentNullException(nameof(root), "function can't be null");

            var visitor = new ParameterTypeVisitor<TSource, TTarget>();
            var expression = (Expression<Func<TTarget, bool>>)visitor.Visit(root);
            return expression;
        }
    }

    public class ParameterTypeVisitor<TSource, TTarget> : ExpressionVisitor
    {
        private System.Collections.ObjectModel.ReadOnlyCollection<ParameterExpression> parameters;

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return parameters?.FirstOrDefault(p => p.Name == node.Name) ??
                (node.Type == typeof(TSource) ? Expression.Parameter(typeof(TTarget), node.Name) : node);
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            parameters = VisitAndConvert(node.Parameters, "VisitLambda");
            return Expression.Lambda(Visit(node.Body), parameters);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Member.DeclaringType == typeof(TSource))
            {
                return Expression.Property(Visit(node.Expression), node.Member.Name);
            }
            return base.VisitMember(node);
        }
    }
}
