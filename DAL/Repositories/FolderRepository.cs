using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DAL.Interfaces;
using Logger;
using ORM;

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

        public IEnumerable<Folders> GetAll()
        {
            return context.Set<Folders>();
        }

        public Folders GetById(long key)
        {
            if (key < 0)
                throw new ArgumentOutOfRangeException(nameof(key), "parametr can't be negative");

            return context.Set<Folders>().FirstOrDefault(user => user.id == key);
        }

        public IEnumerable<Folders> GetByPredicate(Func<Folders, bool> func)
        {
            if (ReferenceEquals(func, null))
                throw new ArgumentNullException(nameof(func), "parametr can't be null");

            return context.Set<Folders>().Where(func);
        }

        void IRepository<Folders>.Create(Folders e)
        {
            Create(e);
        }

        public void Delete(Folders e)
        {
            if (ReferenceEquals(e, null))
                throw new ArgumentNullException(nameof(e), "parametr can't be null");


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

        public void Update(Folders e)
        {
            if (ReferenceEquals(e, null))
                throw new ArgumentNullException(nameof(e), "parametr can't be null");

            if (ReferenceEquals(e.User, null) && e.ownerId.Equals(0))
                throw new ArgumentException("incorrect Folder object");


            var entity = context.Set<Folders>().Find(e.id);

            entity.User = e.User;
            entity.ownerId = e.ownerId;
            entity.dateTime = e.dateTime;
            entity.leftKey = e.leftKey;
            entity.rightKey = e.rightKey;
            entity.level = e.level;
            entity.name = e.name;

            entity.Files = e.Files;
            entity.UsersShared = e.UsersShared;

            context.Entry(entity).State = EntityState.Modified;
        }


        public void Update<TKey>(Func<Folders, bool> func, Expression<Func<Folders, TKey>> keyValue, TKey e)
        {
            if (ReferenceEquals(func, null) || ReferenceEquals(func, null) || e.Equals(default(TKey)))
                throw new ArgumentException("incorrect parametr(s) value");

            MemberExpression memberExpression = (MemberExpression)keyValue.Body;
            string propName = memberExpression.Member.Name;

            Type t = typeof(Folders);

            foreach (var entity in context.Set<Folders>().Where(func))
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
                throw new ArgumentException("root folder for that user already exist", nameof(userID));

            Users user = context.Set<Users>().FirstOrDefault(usr => usr.id == userID);

            if (ReferenceEquals(user,null))
                throw new ArgumentException("user with this user ID not found", nameof(userID));

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

        public void Move(Folders movingFolder, Folders toFolder)
        {
            throw new NotImplementedException();
        }

        public void Add(Folders parent, string newFolderName)
        {       
            if(ReferenceEquals(parent,null))
                throw new ArgumentNullException(nameof(parent),"folder can't be null");

            if (string.IsNullOrEmpty(newFolderName))
                throw new ArgumentNullException(nameof(newFolderName), "new folder name can't be null or empty");

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

        public void InsertFiles(Folders folder, params Files[] files)
        {
            if(ReferenceEquals(folder,null))
                throw new ArgumentNullException(nameof(folder),"folder can't be null");

            if (files.Length == 0)
                throw new ArgumentException("invalid files count",nameof(files));

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

        public void MoveFiles(Folders newFolder, params Files[] files)
        {
            if (ReferenceEquals(newFolder, null))
                throw new ArgumentNullException(nameof(newFolder), "folder can't be null");

            if (files.Length == 0)
                throw new ArgumentException("invalid files count", nameof(files));

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

        public IEnumerable<Folders> GetNextLevelChildNodes(Folders folder)
        {
            return context.Set<Folders>().Where(a => (a.leftKey >= folder.leftKey) 
                                                    && (a.rightKey <= folder.rightKey) 
                                                    && (a.level == folder.level + 1)
                                                    && (a.ownerId == folder.ownerId))
                                                    .OrderBy(a => a.leftKey);
        }

        public Folders GetPreviousLevelParentNode(Folders folder)
        {
            return context.Set<Folders>().FirstOrDefault(a => (a.leftKey <= folder.leftKey)
                                                            && (a.rightKey >= folder.rightKey) 
                                                            && (a.level == folder.level - 1)
                                                            && (a.ownerId == folder.ownerId));
        }

        public IEnumerable<Folders> GetNeighboringNodes(Folders folder)
        {
            return GetNextLevelChildNodes(GetPreviousLevelParentNode(folder));
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
