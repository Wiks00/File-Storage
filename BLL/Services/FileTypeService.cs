using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.DTO;
using BLL.Interfaces;
using BLL.Mappers;
using DAL.Interfaces;

namespace BLL.Services
{
    public class FileTypeService : IFileTypeService
    {

        private readonly IUnitOfWork uow;
        private readonly IFileTypeRepository fileTypeRepository;

        public FileTypeService(IUnitOfWork uow, IFileTypeRepository repository)
        {
            this.uow = uow;
            fileTypeRepository = repository;
        }

        public IEnumerable<DtoFileType> GetAllFileTypes()
        {
            return fileTypeRepository.GetAll().Select(item => item.ToDtoFileType());
        }

        public DtoFileType GetFileTypeById(long key)
        {
            return fileTypeRepository.GetById(key).ToDtoFileType();
        }

        public void CreateFileType(DtoFileType e)
        {
            fileTypeRepository.Create(e.ToDalFileType());
            uow.Commit();
        }

        public void DeleteFileType(DtoFileType e)
        {
            fileTypeRepository.Delete(e.ToDalFileType());
            uow.Commit();
        }

        public void UpdateFileType(DtoFileType e)
        {
            fileTypeRepository.Update(e.ToDalFileType());
            uow.Commit();
        }
    }
}
