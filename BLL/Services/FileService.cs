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
    public class FileService : IFileService
    {
        private readonly IUnitOfWork uow;
        private readonly IFileRepository fileRepository;

        public FileService(IUnitOfWork uow, IFileRepository repository)
        {
            this.uow = uow;
            fileRepository = repository;
        }

        public IEnumerable<DtoFile> GetAllFiles()
        {
            return fileRepository.GetAll().Select(item => item.ToDtoFile());
        }

        public DtoFile GetFileById(long key)
        {
            return fileRepository.GetById(key).ToDtoFile();
        }

        public IEnumerable<DtoFile> GetFilesContainsTitle(string title)
        {
            return
                fileRepository.GetByPredicate(
                        file => file.Title.ToLower().Contains(title.ToLower()))
                    .Select(item => item.ToDtoFile());
        }

        public void CreateFile(DtoFile e)
        {
            fileRepository.Create(e.ToDalFile());
            uow.Commit();
        }

        public void DeleteFile(DtoFile e)
        {
            fileRepository.Delete(e.ToDalFile());
            uow.Commit();
        }

        public void UpdateFile(DtoFile e)
        {
            fileRepository.Update(e.ToDalFile());
            uow.Commit();
        }

        public void UpdateFileTitle(string newTitle, long id)
        {
            fileRepository.Update(file => file.ID == id, file => file.Title, newTitle);
            fileRepository.Update(file => file.ID == id, file => file.DateTime, DateTime.Now);
            uow.Commit();
        }
    }
}
