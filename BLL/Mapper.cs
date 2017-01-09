using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.DTO;
using DAL.DTO;
using ORM;

namespace BLL.Mappers
{
    public static class Mapper
    {
        #region To Dto Entity
        public static DtoFile ToDtoFile(this Files file)
        {
            return new DtoFile
            {
                ID = file.id,
                IDFile = file.idFile,
                Data = file.content,
                DateTime = file.dateTime,
                Folder = file.Folder.ToDtoFolder(),
                Title = file.name,
                FileTypes = new HashSet<DtoFileType>(file.FileTypes.Select(item => item.ToDtoFileType()))
            };
        }

        public static DtoFileType ToDtoFileType(this FileTypes fileType)
        {
            return new DtoFileType
            {
                ID = fileType.id,
                TypeName = fileType.typeName,
                Format = fileType.format
            };
        }
        public static DtoUser ToDtoUser(this Users user)
        {
            return new DtoUser
            {
                ID = user.id,
                Email = user.email,
                Login = user.login,
                Password = user.password,
                Folders = new HashSet<DtoFolder>(user.Folders.Select(item => item.ToDtoFolder())),
                Roles = new HashSet<DtoRole>(user.Roles.Select(item => item.ToDtoRole())),
                SharedFolders = new HashSet<DtoFolder>(user.FoldersShared.Select(item => item.ToDtoFolder()))
            };
        }

        public static DtoFolder ToDtoFolder(this Folders folder)
        {
            return new DtoFolder
            {
                ID = folder.id,
                LeftKey = folder.leftKey,
                RightKey = folder.rightKey,
                Level = folder.level,
                DateTime = folder.dateTime,
                Title = folder.name,
                Owner = folder.User.ToDtoUser(),
                Files = new HashSet<DtoFile>(folder.Files.Select(item => item.ToDtoFile())),
                SharedToUsers = new HashSet<DtoUser>(folder.UsersShared.Select(item => item.ToDtoUser()))
            };
        }

        public static DtoRole ToDtoRole(this Roles role)
        {
            return new DtoRole
            {
                ID = role.id,
                Role = role.role              
            };
        }
        #endregion

        #region To Orm Entity
        public static Files ToOrmFile(this DtoFile file)
        {
            return new Files
            {
                id = file.ID,
                content = file.Data,
                dateTime = file.DateTime,
                idFile = file.IDFile,
                name = file.Title,
                folderId = file.Folder.ID,
                Folder = file.Folder.ToOrmFolder(),
                FileTypes = new HashSet<FileTypes>(file.FileTypes.Select(item => item.ToOrmFileType()))
            };
        }

        public static FileTypes ToOrmFileType(this DtoFileType fileType)
        {
            return new FileTypes
            {
                id = fileType.ID,
                typeName = fileType.TypeName,
                format = fileType.Format
                //Files = ????????????? 
            };
        }
        public static Users ToOrmUser(this DtoUser user)
        {
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

        public static Folders ToOrmFolder(this DtoFolder folder)
        {
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

        public static Roles ToOrmRole(this DtoRole role)
        {
            return new Roles
            {
                id = role.ID,
                role = role.Role
                //Users = ?????????????? 
            };
        }
        #endregion
    }
}
