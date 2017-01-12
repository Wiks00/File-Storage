﻿using System;
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
            return folderRepository.GetByPredicate(folder => folder.Title.ToLower().Contains(title.ToLower()))
                    .Select(item => item.ToDtoFolder());
        }

        public void CreateRootFolder(long userID)
        {
            folderRepository.CreateRoot(userID);
            uow.Commit();
        }

        public void DeleteFolder(DtoFolder e)
        {
            folderRepository.Delete(e.ToDalFolder());
            uow.Commit();
        }

        public void UpdateFolder(DtoFolder e)
        {
            folderRepository.Update(e.ToDalFolder());
            uow.Commit();
        }

        public void UpdateFolderTitle(string newTitle, long id)
        {
            folderRepository.Update(folder => folder.ID == id, folder => folder.Title, newTitle);
            folderRepository.Update(folder => folder.ID == id, folder => folder.DateTime, DateTime.Now);
            uow.Commit();
        }

        public void MoveFolder(DtoFolder movingFolder, DtoFolder toFolder)
        {
            folderRepository.Move(movingFolder.ToDalFolder(), toFolder.ToDalFolder());
            uow.Commit();
        }

        public void AddFolder(DtoFolder parent, string newFolderName)
        {
            folderRepository.Add(parent.ToDalFolder(), newFolderName);
            uow.Commit();
        }

        public void InsertFilesIntoFolder(DtoFolder folder, params DtoFile[] files)
        {
            folderRepository.InsertFiles(folder.ToDalFolder(), files.Select(item => item.ToDalFile()).ToArray());
            uow.Commit();
        }

        public void MoveFilesIntoAnotherFolder(DtoFolder folder, params DtoFile[] files)
        {
            folderRepository.MoveFiles(folder.ToDalFolder(), files.Select(item => item.ToDalFile()).ToArray());
            uow.Commit();
        }

        public void ShareFolderToUsers(DtoFolder folder, params DtoUser[] users)
        {
            folderRepository.ShareFolderTo(folder.ToDalFolder(), users.Select(item => item.ToDalUser()).ToArray());
            uow.Commit();
        }

        public void RemoveAccessToFolderToUsers(DtoFolder folder, params DtoUser[] users)
        {
            folderRepository.RemoveAccessToFolder(folder.ToDalFolder(), users.Select(item => item.ToDalUser()).ToArray());
        }

        public IEnumerable<DtoFolder> GetNextLevelChildNodes(DtoFolder folder)
        {
            return folderRepository.GetNextLevelChildNodes(folder.ToDalFolder()).Select(item => item.ToDtoFolder());
        }

        public DtoFolder GetPreviousLevelParentNode(DtoFolder folder)
        {
            return folderRepository.GetPreviousLevelParentNode(folder.ToDalFolder()).ToDtoFolder();
        }

        public IEnumerable<DtoFolder> GetNeighboringNodes(DtoFolder folder)
        {
            return folderRepository.GetNeighboringNodes(folder.ToDalFolder()).Select(item => item.ToDtoFolder());
        }
    }
}
