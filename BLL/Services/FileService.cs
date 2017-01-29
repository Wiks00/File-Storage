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
using DAL.DTO;
using static BLL.Mappers.BllMapper;

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
            => fileRepository.GetAll().Select(item => item.ToDtoFile());
        

        public DtoFile GetFileById(long key) 
            => fileRepository.GetById(key).ToDtoFile();

        public IEnumerable<DtoFile> GetFielsByPredicate(Expression<Func<DtoFile, bool>> func)
            => fileRepository.GetByPredicate(Convert<DtoFile,DalFile>(func)).Select(item => item.ToDtoFile());
        
        public DtoFile CreateFile(DtoFile e)
        {
            var file = fileRepository.Create(e.ToDalFile());
            uow.Commit();

            return file.ToDtoFile();
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
