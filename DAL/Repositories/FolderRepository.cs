﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Permissions;
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
    public class FolderRepository : IFolderRepository 
    {
        private readonly DbContext context;
        private readonly ILogAdapter logger;

        public FolderRepository(DbContext context, ILogAdapter logger)
        {
            this.context = context;
            this.logger = logger;
        }

        #region iRepository   

        public IEnumerable<DalFolder> GetAll()
        {
            return context.Set<Folders>().Select(item => item.ToDalFolder());
        }

        public DalFolder GetById(long key)
        {
            if (key < 0)
            {
                var error = new ArgumentOutOfRangeException(nameof(key), "parametr can't be negative");
                logger.Error(error, error.Message);
                throw error;
            }

            return context.Set<Folders>().FirstOrDefault(user => user.id == key).ToDalFolder();
        }

        public IEnumerable<DalFolder> GetByPredicate(Expression<Func<DalFolder, bool>> func)
        {
            if (ReferenceEquals(func, null))
            {
                var error = new ArgumentNullException(nameof(func), "parametr can't be null");
                logger.Error(error, error.Message);
                throw error;
            }

            return context.Set<Folders>().Where(Convert<DalFolder,Folders>(func)).Select(item => item.ToDalFolder());
        }

        void IRepository<DalFolder>.Create(DalFolder entity)
        {
            Create(entity.ToOrmFolder());
        }

        public void Delete(DalFolder entity)
        {
            if (ReferenceEquals(entity, null))
            {
                var error = new ArgumentNullException(nameof(entity), "parametr can't be null");
                logger.Error(error, error.Message);
                throw error;
            }

            var e = entity.ToOrmFolder();

            var possibleFolder = context.Set<Folders>().Single(user => user.id == e.id);

            if (ReferenceEquals(possibleFolder, null))
                throw new ArgumentNullException(nameof(possibleFolder), "didn't find equally Folder In database");

            long id = GetParentsNodes(possibleFolder).First().id;

            context.Set<Folders>().RemoveRange(context.Set<Folders>().Where(a => (a.leftKey >= possibleFolder.leftKey)
                                                                  && (a.rightKey <= possibleFolder.rightKey)
                                                                  && (a.ownerId == possibleFolder.ownerId)));

            int range = possibleFolder.rightKey - possibleFolder.leftKey + 1;

            foreach (Folders item in context.Set<Folders>().Where(a => (a.rightKey > possibleFolder.rightKey) 
                                                                    && (a.ownerId == possibleFolder.ownerId)))
            {
                if (item.leftKey > possibleFolder.leftKey)
                    item.leftKey = item.leftKey - range;

                item.rightKey = item.rightKey - range;

                context.Entry(item).State = EntityState.Modified;
            }

            foreach (Folders item in context.Set<Folders>().Where(a => a.ownerId == possibleFolder.ownerId))
            {
                item.id = id;
                id++;

                context.Entry(item).State = EntityState.Modified;
            }
        }

        public void Update(DalFolder entity)
        {
            if (ReferenceEquals(entity, null))
            {
                var error = new ArgumentNullException(nameof(entity), "parametr can't be null");
                logger.Error(error, error.Message);
                throw error;
            }

            var e = entity.ToOrmFolder();

            if (ReferenceEquals(e.User, null) && e.ownerId.Equals(0))
            {
                var error = new ArgumentException("incorrect Folder object");
                logger.Error(error, error.Message);
                throw error;
            }

            var folder = context.Set<Folders>().Find(e.id);

            folder.User = e.User;
            folder.ownerId = e.ownerId;
            folder.dateTime = e.dateTime;
            folder.leftKey = e.leftKey;
            folder.rightKey = e.rightKey;
            folder.level = e.level;
            folder.name = e.name;

            folder.Files = e.Files;
            folder.UsersShared = e.UsersShared;

            context.Entry(folder).State = EntityState.Modified;
        }


        public void Update<TKey>(Expression<Func<DalFolder, bool>> func, Expression<Func<DalFolder, TKey>> keyValue, TKey e)
        {
            if (ReferenceEquals(func, null) || ReferenceEquals(keyValue, null) || e.Equals(default(TKey)))
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

            Type t = typeof(Folders);

            foreach (var entity in context.Set<Folders>().Where(Convert<DalFolder,Folders>(func)))
            {
                MethodInfo method = t.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public).FirstOrDefault(mtd => mtd.Name.ToLower().Equals("set_" + propName));
                method?.Invoke(entity, new object[] { e });

                context.Entry(entity).State = EntityState.Modified;
            }
        }
        #endregion

        #region iFolderRepository 

        public void CreateRoot(long userID)
        {
            if (context.Set<Folders>().Count(a => a.ownerId == userID) > 0)
            {
                var error = new ArgumentException("root folder for that user already exist", nameof(userID));
                logger.Error(error, error.Message);
                throw error;
            }

            Users user = context.Set<Users>().FirstOrDefault(usr => usr.id == userID);

            if (ReferenceEquals(user,null))
            {
                var error = new ArgumentException("user with this user ID not found", nameof(userID));
                logger.Error(error, error.Message);
                throw error;
            }

            long id = 0;

            if (context.Set<Folders>().Count() != 0)
            {
                id = GetMaxId();
            }

            context.Set<Folders>().Add(new Folders
            {
                id = id,
                ownerId = userID,
                name = "root",
                dateTime = DateTime.Now,
                level = 0,
                leftKey = 1,
                rightKey = 2,
                Files = new HashSet<Files>(),
                User = user,
                UsersShared = new HashSet<Users>()
            });
        }

        public void Move(DalFolder movingFolder, DalFolder toFolder)
        {
            throw new NotImplementedException();
        }

        public void Add(DalFolder prnt, string newFolderName)
        {       
            if(ReferenceEquals(prnt, null))
            {
                var error = new ArgumentNullException(nameof(prnt),"folder can't be null");
                logger.Error(error, error.Message);
                throw error;
            }

            if (string.IsNullOrEmpty(newFolderName))
            {
                var error = new ArgumentNullException(nameof(newFolderName), "new folder name can't be null or empty");
                logger.Error(error, error.Message);
                throw error;
            }

            var parent = prnt.ToOrmFolder();

            int leftKey = parent.rightKey;
            int rightKey = parent.rightKey + 1;

            foreach (Folders item in context.Set<Folders>().Where(a => (a.leftKey > parent.rightKey)
                                                                    && (a.ownerId == parent.ownerId)))
            {
                item.leftKey = item.leftKey + 2;
                item.rightKey = item.rightKey + 2;

                context.Entry(item).State = EntityState.Modified;
            }

            foreach (Folders item in context.Set<Folders>().Where(a => (a.rightKey >= parent.rightKey) 
                                                   && (a.leftKey < parent.rightKey)
                                                   && (a.ownerId == parent.ownerId)))
            {
                item.rightKey = item.rightKey + 2;

                context.Entry(item).State = EntityState.Modified;
            }

            Create(new Folders
            {
                ownerId = parent.ownerId,
                name = newFolderName,
                dateTime = DateTime.Now,
                level = parent.level + 1,
                leftKey = leftKey,
                rightKey = rightKey,
                Files = new HashSet<Files>(),
                User = context.Set<Users>().FirstOrDefault(user => user.id == parent.ownerId),
                UsersShared = new HashSet<Users>()
            });
        }

        public void InsertFiles(DalFolder fldr, params DalFile[] fls)
        {
            if(ReferenceEquals(fldr, null))
            {
                var error = new ArgumentNullException(nameof(fldr),"folder can't be null");
                logger.Error(error, error.Message);
                throw error;
            }

            if (fls.Length == 0)
            {
                var error = new ArgumentOutOfRangeException(nameof(fls),"invalid files count");
                logger.Error(error, error.Message);
                throw error;
            }

            var enumerable = fls.Select(item => item.ToOrmFile());
            var folder = fldr.ToOrmFolder();

            var files = enumerable as Files[] ?? enumerable.ToArray();

            foreach (Files file in files)
            {
                folder.Files.Add(file);

                file.folderId = folder.id;
                file.Folder = folder;

                context.Entry(file).State = EntityState.Modified;
            }

            folder.dateTime = files[files.Length - 1].dateTime;

            context.Entry(folder).State = EntityState.Modified;
        }

        public void MoveFiles(DalFolder newFldr, params DalFile[] fls)
        {
            if (ReferenceEquals(newFldr, null))
            {
                var error = new ArgumentNullException(nameof(newFldr), "folder can't be null");
                logger.Error(error, error.Message);
                throw error;
            }

            if (fls.Length == 0)
            {
                var error = new ArgumentOutOfRangeException(nameof(fls), "invalid files count");
                logger.Error(error, error.Message);
                throw error;
            }

            var enumerable = fls.Select(item => item.ToOrmFile());
            var newFolder = newFldr.ToOrmFolder();

            var files = enumerable as Files[] ?? enumerable.ToArray();


            Folders oldFolder = files[0].Folder;

            foreach (Files file in files)
            {
                oldFolder.Files.Remove(file);
                newFolder.Files.Add(file);

                file.folderId = newFolder.id;
                file.Folder = newFolder;

                context.Entry(file).State = EntityState.Modified;
            }

            newFolder.dateTime = files[files.Length - 1].dateTime;

            context.Entry(oldFolder).State = EntityState.Modified;
            context.Entry(newFolder).State = EntityState.Modified;
        }

        public void ShareFolderTo(DalFolder fldr, params DalUser[] usrs)
        {
            if (ReferenceEquals(fldr, null))
            {
                var error = new ArgumentNullException(nameof(fldr), "folder can't be null");
                logger.Error(error, error.Message);
                throw error;
            }

            if (usrs.Length == 0)
            {
                var error = new ArgumentOutOfRangeException(nameof(usrs), "invalid usrs count");
                logger.Error(error, error.Message);
                throw error;
            }

            var users = usrs.Select(item => item.ToOrmUser());
            var folder = fldr.ToOrmFolder();
           
            foreach (Users user in users)
            {
                folder.UsersShared.Add(user);

                user.FoldersShared.Add(folder);

                context.Entry(user).State = EntityState.Modified;
            }

            context.Entry(folder).State = EntityState.Modified;

        }

        public void RemoveAccessToFolder(DalFolder fldr, params DalUser[] usrs)
        {
            if (ReferenceEquals(fldr, null))
            {
                var error = new ArgumentNullException(nameof(fldr), "folder can't be null");
                logger.Error(error, error.Message);
                throw error;
            }

            if (usrs.Length == 0)
            {
                var error = new ArgumentOutOfRangeException(nameof(usrs), "invalid usrs count");
                logger.Error(error, error.Message);
                throw error;
            }

            var users = usrs.Select(item => item.ToOrmUser());
            var folder = fldr.ToOrmFolder();

            foreach (Users user in users)
            {
                folder.UsersShared.Remove(user);

                user.FoldersShared.Remove(folder);

                context.Entry(user).State = EntityState.Modified;
            }

            context.Entry(folder).State = EntityState.Modified;
        }

        public IEnumerable<DalFolder> GetNextLevelChildNodes(DalFolder fldr)
        {
            if (ReferenceEquals(fldr, null))
            {
                var error = new ArgumentNullException(nameof(fldr), "folder can't be null");
                logger.Error(error, error.Message);
                throw error;
            }

            var folder = fldr.ToOrmFolder();

            return context.Set<Folders>().Where(a => (a.leftKey >= folder.leftKey) 
                                                    && (a.rightKey <= folder.rightKey) 
                                                    && (a.level == folder.level + 1)
                                                    && (a.ownerId == folder.ownerId))
                                                    .OrderBy(a => a.leftKey).Select(item => item.ToDalFolder());
        }

        public DalFolder GetPreviousLevelParentNode(DalFolder fldr)
        {
            if (ReferenceEquals(fldr, null))
            {
                var error = new ArgumentNullException(nameof(fldr), "folder can't be null");
                logger.Error(error, error.Message);
                throw error;
            }

            var folder = fldr.ToOrmFolder();

            return context.Set<Folders>().FirstOrDefault(a => (a.leftKey <= folder.leftKey)
                                                            && (a.rightKey >= folder.rightKey) 
                                                            && (a.level == folder.level - 1)
                                                            && (a.ownerId == folder.ownerId)).ToDalFolder();
        }

        public IEnumerable<DalFolder> GetNeighboringNodes(DalFolder fldr)
        {
            if (ReferenceEquals(fldr, null))
            {
                var error = new ArgumentNullException(nameof(fldr), "folder can't be null");
                logger.Error(error, error.Message);
                throw error;
            }

            return GetNextLevelChildNodes(GetPreviousLevelParentNode(fldr));
        }
        #endregion

        #region private members 

        private IEnumerable<Folders> GetParentsNodes(Folders folder)
        {
            return context.Set<Folders>().Where(a => (a.leftKey <= folder.leftKey) && (a.rightKey >= folder.rightKey)).OrderBy(a => a.leftKey);
        }

        private long GetMaxId()
        {
            long id = context.Set<Folders>().Max(a => a.id);

            return (long)Math.Round((double)(id / 10000)) * 10000 + 10000;
        }

        private void Create(Folders e)
        {
            if (ReferenceEquals(e, null))
                throw new ArgumentNullException(nameof(e), "parametr can't be null");

            if (ReferenceEquals(e.User, null) && e.ownerId.Equals(0))
                throw new ArgumentException("incorrect Folder object");

            long id = GetMaxId();

            var set = context.Set<Folders>().Where(a => a.ownerId == e.ownerId);

            if (set.Count() != 0)
            {
                id = set.Max(a => a.id) + 1;
            }

            e.id = id;

            context.Set<Folders>().Add(e);
        }
        #endregion
    }
}