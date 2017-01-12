using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DAL.DTO;
using Microsoft.SqlServer.Server;
using ORM;

namespace DAL.Mappers
{
    public static class DalMapper
    {
        #region To DAL Entity

        /// <summary>
        /// Convert Orm File entity to Dal entity
        /// </summary>
        /// <param name="file">orm entity</param>
        /// <returns>dal entity</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static DalFile ToDalFile(this Files file)
        {
            if(ReferenceEquals(file,null))
                throw new ArgumentNullException(nameof(file),"file can't be null");

            return new DalFile
            {
                ID = file.id,
                Data = file.content,
                DateTime = file.dateTime,
                Folder = file.Folder.ToDalFolder(),
                Title = file.name,
                FileTypes = new HashSet<DalFileType>(file.FileTypes.Select(item => item.ToDalFileType()))
            };
        }

        /// <summary>
        /// Convert Orm FileType entity to Dal entity
        /// </summary>
        /// <param name="fileType">orm entity</param>
        /// <returns>dal entity</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static DalFileType ToDalFileType(this FileTypes fileType)
        {
            if (ReferenceEquals(fileType, null))
                throw new ArgumentNullException(nameof(fileType), "fileType can't be null");

            return new DalFileType
            {
                ID = fileType.id,
                TypeName = fileType.typeName,
                Format = fileType.format
            };
        }

        /// <summary>
        /// Convert Orm User entity to Dal entity
        /// </summary>
        /// <param name="user">orm entity</param>
        /// <returns>dal entity</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static DalUser ToDalUser(this Users user)
        {
            if (ReferenceEquals(user, null))
                throw new ArgumentNullException(nameof(user), "user can't be null");

            return new DalUser
            {
                ID = user.id,
                Email = user.email,
                Login = user.login,
                Password = user.password,
                Folders = new HashSet<DalFolder>(user.Folders.Select(item => item.ToDalFolder())),
                Roles = new HashSet<DalRole>(user.Roles.Select(item => item.ToDalRole())),
                SharedFolders = new HashSet<DalFolder>(user.FoldersShared.Select(item => item.ToDalFolder()))
            };
        }

        /// <summary>
        /// Convert Orm Folder entity to Dal entity
        /// </summary>
        /// <param name="folder">orm entity</param>
        /// <returns>dal entity</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static DalFolder ToDalFolder(this Folders folder)
        {
            if (ReferenceEquals(folder, null))
                throw new ArgumentNullException(nameof(folder), "folder can't be null");

            return new DalFolder
            {
                ID = folder.id,
                LeftKey = folder.leftKey,
                RightKey = folder.rightKey,
                Level = folder.level,
                DateTime = folder.dateTime,
                Title = folder.name,
                Owner = folder.User.ToDalUser(),
                Files = new HashSet<DalFile>(folder.Files.Select(item => item.ToDalFile())),
                SharedToUsers = new HashSet<DalUser>(folder.UsersShared.Select(item => item.ToDalUser()))
            };
        }

        /// <summary>
        /// Convert Orm Role entity to Dal entity
        /// </summary>
        /// <param name="role">orm entity</param>
        /// <returns>dal entity</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static DalRole ToDalRole(this Roles role)
        {
            if (ReferenceEquals(role, null))
                throw new ArgumentNullException(nameof(role), "role can't be null");

            return new DalRole
            {
                ID = role.id,
                Role = role.role              
            };
        }
        #endregion

        #region To ORM Entity

        /// <summary>
        /// Convert Dal File entity to Orm entity
        /// </summary>
        /// <param name="file">dal entity</param>
        /// <returns>orm entity</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Files ToOrmFile(this DalFile file)
        {
            if (ReferenceEquals(file, null))
                throw new ArgumentNullException(nameof(file), "file can't be null");

            return new Files
            {
                id = file.ID,
                content = file.Data,
                dateTime = file.DateTime,
                name = file.Title,
                folderId = file.Folder.ID,
                Folder = file.Folder.ToOrmFolder(),
                FileTypes = new HashSet<FileTypes>(file.FileTypes.Select(item => item.ToOrmFileType()))
            };
        }

        /// <summary>
        /// Convert Dal FileType entity to Orm entity
        /// </summary>
        /// <param name="fileType">dal entity</param>
        /// <returns>orm entity</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static FileTypes ToOrmFileType(this DalFileType fileType)
        {
            if (ReferenceEquals(fileType, null))
                throw new ArgumentNullException(nameof(fileType), "fileType can't be null");

            return new FileTypes
            {
                id = fileType.ID,
                typeName = fileType.TypeName,
                format = fileType.Format
            };
        }

        /// <summary>
        /// Convert Dal User entity to Orm entity
        /// </summary>
        /// <param name="user">dal entity</param>
        /// <returns>orm entity</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Users ToOrmUser(this DalUser user)
        {
            if (ReferenceEquals(user, null))
                throw new ArgumentNullException(nameof(user), "user can't be null");

            return new Users
            {
                id = user.ID,
                email = user.Email,
                login = user.Login,
                password = user.Password,
                Folders = new HashSet<Folders>(user.Folders.Select(item => item.ToOrmFolder())),
                Roles = new HashSet<Roles>(user.Roles.Select(item => item.ToOrmRole())),
                FoldersShared = new HashSet<Folders>(user.SharedFolders.Select(item => item.ToOrmFolder()))
            };
        }

        /// <summary>
        /// Convert Dal Folder entity to Orm entity
        /// </summary>
        /// <param name="folder">dal entity</param>
        /// <returns>orm entity</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Folders ToOrmFolder(this DalFolder folder)
        {
            if (ReferenceEquals(folder, null))
                throw new ArgumentNullException(nameof(folder), "folder can't be null");

            return new Folders
            {
                id = folder.ID,
                leftKey = folder.LeftKey,
                rightKey = folder.RightKey,
                level = folder.Level,
                dateTime = folder.DateTime,
                name = folder.Title,
                ownerId = folder.Owner.ID,
                User = folder.Owner.ToOrmUser(),
                Files = new HashSet<Files>(folder.Files.Select(item => item.ToOrmFile())),
                UsersShared = new HashSet<Users>(folder.SharedToUsers.Select(item => item.ToOrmUser()))
            };
        }

        /// <summary>
        /// Convert Dal Folder entity to Orm entity
        /// </summary>
        /// <param name="role">dal entity</param>
        /// <returns>orm entity</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Roles ToOrmRole(this DalRole role)
        {
            if (ReferenceEquals(role, null))
                throw new ArgumentNullException(nameof(role), "role can't be null");

            return new Roles
            {
                id = role.ID,
                role = role.Role
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
        public static Func<TTarget, bool> Convert<TSource, TTarget>(Expression<Func<TSource, bool>> root)
        {
            if (ReferenceEquals(root, null))
                throw new ArgumentNullException(nameof(root), "function can't be null");

            var visitor = new ParameterTypeVisitor<TSource, TTarget>();
            var expression = (Expression<Func<TTarget, bool>>)visitor.Visit(root);
            return expression.Compile();
        }

        /// <summary>
        /// Search for equal property in orm entity 
        /// </summary>
        /// <param name="type">type of the entity</param>
        /// <param name="property">investigated property</param>
        /// <returns>equal property name</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string GetEqualProperty(string type, string property)
        {
            if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(property))
                throw new ArgumentNullException();

            switch (type.ToLower())
            {
                #region user
                case "daluser":
                    switch (property.ToLower())
                    {
                        case "id":
                            return "id";
                        case "email":
                            return "email";
                        case "login":
                            return "login";
                        case "password":
                            return "password";
                        case "folders":
                            return "folders";
                        case "roles":
                            return "roles";
                        case "sharedfolders":
                            return "foldersshared";
                    }
                    break;
                #endregion

                #region role
                case "dalrole":
                    switch (property.ToLower())
                    {
                        case "id":
                            return "id";
                        case "role":
                            return "role";
                    }
                    break;
                #endregion

                #region folder
                case "dalfolder":
                    switch (property.ToLower())
                    {
                        case "id":
                            return "id";
                        case "leftkey":
                            return "leftkey";
                        case "rightkey":
                            return "rightkey";
                        case "level":
                            return "level";
                        case "datetime":
                            return "datetime";
                        case "title":
                            return "name";
                        case "owner.id":
                            return "ownerid";
                        case "owner":
                            return "user";
                        case "files":
                            return "files";
                        case "sharedtousers":
                            return "usersshared";
                    }
                    break;
                #endregion

                #region file
                case "dalfile":
                    switch (property.ToLower())
                    {
                        case "id":
                            return "id";
                        case "data":
                            return "content";
                        case "datetime":
                            return "datetime";
                        case "title":
                            return "name";
                        case "folder.id":
                            return "folderId";
                        case "folder":
                            return "folder";
                        case "filetypes":
                            return "filetypes";

                    }
                    break;
                #endregion

                #region file type
                case "dalfiletype":
                    switch (property.ToLower())
                    {
                        case "id":
                            return "id";
                        case "typename":
                            return "typename";
                        case "format":
                            return "format";
                    }
                    break;
                    #endregion

            }

            return "undef property";
        }
    }

    internal class ParameterTypeVisitor<TSource, TTarget> : ExpressionVisitor
    {
        private ReadOnlyCollection<ParameterExpression> _parameters;

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
                if (node.Expression.GetType().ToString().Equals("System.Linq.Expressions.PropertyExpression"))
                {
                    var param = (MemberExpression)node.Expression;
                    var paramName = (ParameterExpression)param.Expression;

                    return Expression.Property(Visit(Expression.Parameter(typeof(TTarget), paramName.Name)), DalMapper.GetEqualProperty(typeof(TSource).ToString(), param.Member.Name + "." + node.Member.Name));
                }

                return Expression.Property(Visit(node.Expression), DalMapper.GetEqualProperty(typeof(TSource).ToString(), node.Member.Name));
            }

            return base.VisitMember(node);
        }
    }
}
