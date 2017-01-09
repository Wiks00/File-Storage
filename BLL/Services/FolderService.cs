using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BLL.DTO;
using BLL.Interfaces;
using BLL.Mappers;
using DAL.Interfaces;

namespace BLL.Services
{
    public class FolderService : IFolderService
    {
        private readonly IUnitOfWork uow;
        private readonly IFolderRepository folderRepository;

        public FolderService(IUnitOfWork uow, IFolderRepository repository)
        {
            this.uow = uow;
            folderRepository = repository;
        }
        public IEnumerable<DtoFolder> GetAllFolders()
        {
            return folderRepository.GetAll().Select(item => item.ToDtoFolder());
        }

        public DtoFolder GetById(long key)
        {
            return folderRepository.GetById(key).ToDtoFolder();
        }

        public IEnumerable<DtoFolder> GetFoldersContainsTitle(string title)
        {
            return folderRepository.GetByPredicate(folder => folder.name.ToLower().Contains(title.ToLower()))
                    .Select(item => item.ToDtoFolder());
        }

        public void CreateRootFolder(long userID)
        {
            folderRepository.CreateRoot(userID);
            uow.Commit();
        }

        public void DeleteFolder(DtoFolder e)
        {
            folderRepository.Delete(e.ToOrmFolder());
            uow.Commit();
        }

        public void UpdateFolder(DtoFolder e)
        {
            folderRepository.Update(e.ToOrmFolder());
            uow.Commit();
        }

        public void UpdateFolderTitle(string newTitle, long id)
        {
            folderRepository.Update(folder => folder.id == id, folder => folder.name, newTitle);
            folderRepository.Update(folder => folder.id == id, folder => folder.dateTime, DateTime.Now);
            uow.Commit();
        }

        public void MoveFolder(DtoFolder movingFolder, DtoFolder toFolder)
        {
            folderRepository.Move(movingFolder.ToOrmFolder(), toFolder.ToOrmFolder());
            uow.Commit();
        }

        public void AddFolder(DtoFolder parent, string newFolderName)
        {
            folderRepository.Add(parent.ToOrmFolder(), newFolderName);
            uow.Commit();
        }

        public void InsertFilesIntoFolder(DtoFolder folder, params DtoFile[] files)
        {
            folderRepository.InsertFiles(folder.ToOrmFolder(), files.Select(item => item.ToOrmFile()).ToArray());
            uow.Commit();
        }

        public void MoveFilesIntoAnotherFolder(DtoFolder folder, params DtoFile[] files)
        {
            folderRepository.MoveFiles(folder.ToOrmFolder(), files.Select(item => item.ToOrmFile()).ToArray());
            uow.Commit();
        }

        public IEnumerable<DtoFolder> GetNextLevelChildNodes(DtoFolder folder)
        {
            return folderRepository.GetNextLevelChildNodes(folder.ToOrmFolder()).Select(item => item.ToDtoFolder());
        }

        public DtoFolder GetPreviousLevelParentNode(DtoFolder folder)
        {
            return folderRepository.GetPreviousLevelParentNode(folder.ToOrmFolder()).ToDtoFolder();
        }

        public IEnumerable<DtoFolder> GetNeighboringNodes(DtoFolder folder)
        {
            return folderRepository.GetNeighboringNodes(folder.ToOrmFolder()).Select(item => item.ToDtoFolder());
        }
    }
}
