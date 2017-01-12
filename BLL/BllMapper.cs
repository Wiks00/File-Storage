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
        public static DtoFile ToDtoFile(this DalFile file)
        {
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

        public static DtoFileType ToDtoFileType(this DalFileType fileType)
        {
            return new DtoFileType
            {
                ID = fileType.ID,
                TypeName = fileType.TypeName,
                Format = fileType.Format
            };
        }
        public static DtoUser ToDtoUser(this DalUser user)
        {
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

        public static DtoFolder ToDtoFolder(this DalFolder folder)
        {
            return new DtoFolder
            {
                ID = folder.ID,
                LeftKey = folder.LeftKey,
                RightKey = folder.RightKey,
                Level = folder.Level,
                DateTime = folder.DateTime,
                Title = folder.Title,
                Owner = folder.Owner.ToDtoUser(),
                Files = new HashSet<DtoFile>(folder.Files.Select(item => item.ToDtoFile())),
                SharedToUsers = new HashSet<DtoUser>(folder.SharedToUsers.Select(item => item.ToDtoUser()))
            };
        }

        public static DtoRole ToDtoRole(this DalRole role)
        {
            return new DtoRole
            {
                ID = role.ID,
                Role = role.Role
            };
        }
        #endregion

        #region To DAL Entity
        public static DalFile ToDalFile(this DtoFile file)
        {
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

        public static DalFileType ToDalFileType(this DtoFileType fileType)
        {
            return new DalFileType
            {
                ID = fileType.ID,
                TypeName = fileType.TypeName,
                Format = fileType.Format
            };
        }
        public static DalUser ToDalUser(this DtoUser user)
        {
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

        public static DalFolder ToDalFolder(this DtoFolder folder)
        {
            return new DalFolder
            {
                ID = folder.ID,
                LeftKey = folder.LeftKey,
                RightKey = folder.RightKey,
                Level = folder.Level,
                DateTime = folder.DateTime,
                Title = folder.Title,
                Owner = folder.Owner.ToDalUser(),
                Files = new HashSet<DalFile>(folder.Files.Select(item => item.ToDalFile())),
                SharedToUsers = new HashSet<DalUser>(folder.SharedToUsers.Select(item => item.ToDalUser()))
            };
        }

        public static DalRole ToDalRole(this DtoRole role)
        {
            return new DalRole
            {
                ID = role.ID,
                Role = role.Role
            };
        }
        #endregion

        public static Func<TTarget, bool> Convert<TSource, TTarget>(Expression<Func<TSource, bool>> root)
        {
            var visitor = new ParameterTypeVisitor<TSource, TTarget>();
            var expression = (Expression<Func<TTarget, bool>>)visitor.Visit(root);
            return expression.Compile();
        }
    }

    public class ParameterTypeVisitor<TSource, TTarget> : ExpressionVisitor
    {
        private System.Collections.ObjectModel.ReadOnlyCollection<ParameterExpression> _parameters;

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return _parameters?.FirstOrDefault(p => p.Name == node.Name) ??
                (node.Type == typeof(TSource) ? Expression.Parameter(typeof(TTarget), node.Name) : node);
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            _parameters = VisitAndConvert(node.Parameters, "VisitLambda");
            return Expression.Lambda(Visit(node.Body), _parameters);
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
